using app_NominaExtendida.Modelos;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace app_NominaExtendida
{
    internal class cls_BarridoXMLPeriodo
    {
        #region Variables


        #endregion



        public async Task BarridoPeriodoNominaEmpresas()
        {
            var lsCollections = await ObtenerNombreDatabases();
            foreach (var nameCollection in lsCollections)
            {
                try
                {
                    var oMongoConnection = new cls_MongoDBConnection(nameCollection);
                    var procesoHistoricoCollection = oMongoConnection._database.GetCollection<mdl_NProcesoHistorico>("ProcesoHistorico");

                    var solicitanteCollection = await oMongoConnection.GetCollection<BsonDocument>("Solicitante");

                    if (solicitanteCollection == null) continue;

                    var filter = new BsonDocument("sol_RFC", nameCollection);
                    var documento = await solicitanteCollection.Find(filter).FirstOrDefaultAsync();

                    if (documento == null) continue;

                    var inicioNomina = documento["sol_YearInicial"].ToInt32();

                    for (int anioActual = inicioNomina; anioActual <= DateTime.UtcNow.Year; anioActual++)
                    {
                        var filterBarrido = Builders<mdl_NProcesoHistorico>.Filter.And(
                            Builders<mdl_NProcesoHistorico>.Filter.Eq(x => x.iPeriodo, anioActual),
                            Builders<mdl_NProcesoHistorico>.Filter.Eq(x => x.sRFCEmpresa, nameCollection)
                        );
                        var registroExistente = await procesoHistoricoCollection.Find(filterBarrido).FirstOrDefaultAsync();

                        // LÓGICA DE DECISIÓN BASADA EN ESTADO
                        if (registroExistente != null && registroExistente.sEstado == "ProcesamientoCompleto")
                        {
                            cls_Tools.EscribeLog(nameCollection, $"[{nameCollection}] Año {anioActual} ya está completamente procesado. Omitiendo.");
                            continue;
                        }
                        // FASE 1: PREPARAR ENTORNO (si es necesario)
                        if (registroExistente == null)
                        {
                            cls_Tools.EscribeLog(nameCollection, $"[{nameCollection}] Preparando entorno para el año {anioActual}...");
                            string sNameCollection = $"MBDB_NAcumulados_{anioActual}";

                            if (!oMongoConnection.VerificarColeccion(sNameCollection))
                            {
                                await oMongoConnection._database.CreateCollectionAsync(sNameCollection);
                                var nacum = await oMongoConnection.GetCollection<mdl_NAcumuladosMBDB>(sNameCollection);
                                CreacionIndices(nacum);
                            }

                            // MARCAMOS QUE EL ENTORNO ESTÁ LISTO
                            var registroNuevo = new mdl_NProcesoHistorico
                            {
                                iPeriodo = anioActual,
                                sRFCEmpresa = nameCollection,
                                sEstado = "EntornoCreado", // Estado intermedio
                                dtFechaProceso = DateTime.UtcNow
                            };
                            await procesoHistoricoCollection.InsertOneAsync(registroNuevo);
                            cls_Tools.EscribeLog(nameCollection, $"[{nameCollection}] Entorno para {anioActual} creado y registrado.");
                        }

                        // FASE 2: PROCESAR DATOS
                        cls_Tools.EscribeLog(nameCollection, $"[{nameCollection}] Iniciando procesamiento de datos para el año {anioActual}...");

                        // Determina hasta qué mes procesar
                        int mesFinal = (anioActual == DateTime.UtcNow.Year) ? DateTime.Now.Month : 12;

                        var lsDataNomina = await ObtenerXML(nameCollection, anioActual, mesFinal);

                        if (lsDataNomina != null && lsDataNomina.Any(d => d.lsyXMLs.Any()))
                        {

                            foreach (var nom in lsDataNomina)
                            {
                                if (nom != null && nom.lsyXMLs.Any())
                                    // 
                                    await ProcesarXML(nom);
                            }


                        }

                        // FASE 3: MARCAR COMO COMPLETADO
                        // Usamos un 'Update' para cambiar el estado del registro que ya creamos.
                        var updateDef = Builders<mdl_NProcesoHistorico>.Update
                            .Set(x => x.sEstado, "ProcesamientoCompleto")
                            .Set(x => x.dtFechaProceso, DateTime.UtcNow);

                        await procesoHistoricoCollection.UpdateOneAsync(filterBarrido, updateDef, new UpdateOptions { IsUpsert = true });
                        cls_Tools.EscribeLog(nameCollection, $"[{nameCollection}] Año {anioActual} marcado como 'ProcesamientoCompleto'.");

                    }
                }
                catch (Exception ex)
                {
                    string sLog = $" Se origino una exception al momento de crear el barrido de la información para la empresa {nameCollection}.\n " +
                        $"{ex}\n" +
                        "Clase: cls_BarridoXMLPeriodo" +
                        $"Método: {nameof(BarridoPeriodoNominaEmpresas)}";
                }
            }
        }



        public async Task<byte[]> RecuperarXML(string sRFC, string sUUID, string sVersion)
        {
            byte[] ayXML = new byte[0];
            try
            {
                var oMongoConnection = new cls_MongoDBConnection(sRFC);
                var omcCollection = await oMongoConnection.GetCollection<BsonDocument>("Nomina");
                var projection = Builders<BsonDocument>.Projection.Include("Transaccion.XML");

                var filter = Builders<BsonDocument>.Filter.Eq("_id", sUUID);
                var doc = await omcCollection.Find(filter).Project(projection).FirstOrDefaultAsync();

                if (doc == null)
                {
                    filter = Builders<BsonDocument>.Filter.Eq("_id", sUUID.ToLower());
                    doc = await omcCollection.Find(filter).Project(projection).FirstOrDefaultAsync();
                }

                if (doc != null && doc.Contains("Transaccion") && doc["Transaccion"].AsBsonDocument.Contains("XML"))
                {
                    string sXMLZIP = doc["Transaccion"]["XML"].AsString;
                    if (!string.IsNullOrEmpty(sXMLZIP))
                    {
                        byte[] ayXMLZip = Convert.FromBase64String(sXMLZIP);
                        return cls_Tools.DescomprimirZip(ayXMLZip);
                    }
                }

                cls_Tools.EscribeLog(sRFC, $"XML no encontrado para UUID: {sUUID}, Versión: {sVersion}");

            }
            catch (Exception ex)
            {
                cls_Tools.EscribeLog(sRFC, $"Error en RecuperarXML para UUID {sUUID}: {ex.Message}");

            }
            return ayXML;
        }


        static void CreacionIndices(IMongoCollection<mdl_NAcumuladosMBDB> collection)
        {
            var verifyindexes = collection.Indexes.List().ToList();
            var existIndexes = verifyindexes.Select(index => index["key"].AsBsonDocument).ToList();

            var requiredFields = new List<string>
            {
                "em_mesrfColaborador",
                "em_rfColaborador",
                "em_imss",
                "em_regpat",
                "Nomina.Percepciones.Percepcion.Tipo",
                "Nomina.Deducciones.Deduccion.Tipo",
                "Nomina.OtrosPagos.OtroPago.Tipo",
                "Nomina.Incapacidades.Incapacidad.Tipo",
                "Nomina.Percepciones.Percepcion.Clave",
                "Nomina.Deducciones.Deduccion.Clave",
                "Nomina.OtrosPagos.OtroPago.Clave",
                "Nomina.Incapacidades.Incapacidad.Clave"
            };

            var missingIndexes = requiredFields.Where(field => !existIndexes.Any(key => key.Contains(field))).ToList();

            if (missingIndexes.Any())
            {
                var indexKeysDefinition = Builders<mdl_NAcumuladosMBDB>.IndexKeys;
                var indexModel = missingIndexes.Select(field => new CreateIndexModel<mdl_NAcumuladosMBDB>(
                    indexKeysDefinition.Ascending(field)
                    )).ToList();
                collection.Indexes.CreateMany(indexModel);
            }
        }

        private async Task<List<string>> ObtenerNombreDatabases()
        {
            List<string> lsNameSquemas = new List<string>();
            var oMongoConnection = new cls_MongoDBConnection();
            try
            {
                List<BsonDocument> lsSchemas = await oMongoConnection._client.ListDatabases().ToListAsync();
                for (int i = 0; i < lsSchemas.Count; i++)
                {
                    string s = lsSchemas[i].ToString();
                    string sWebRFCs = @"([A-Z&Ñ]{3,4})(\d{6})([A-Z0-9]{3}[_]{1}(MBD))";
                    Match resultWebRFCs = Regex.Match(s, sWebRFCs);
                    while (resultWebRFCs.Success)
                    {
                        lsNameSquemas.Add(resultWebRFCs.ToString().Replace("_MBD", ""));
                        break;
                    }
                }
            }
            catch (MongoException ex)
            {
                string sLog = $" Se origino una exception al momento de obtener el nombre de las bases de datos de MBDB.\n " +
                    $"{ex}\n" +
                    "Clase: cls_BarridoXMLPeriodo" +
                    $"Método: {nameof(ObtenerNombreDatabases)}";
                cls_Tools.EscribeLog("", sLog);

            }
            catch (Exception ex)
            {
                string sLog = $" Se origino una exception al momento de crear el barrido de la información para las empresas.\n " +
                    $"{ex}\n" +
                    "Clase: cls_BarridoXMLPeriodo" +
                    $"Método: {nameof(ObtenerNombreDatabases)}";
                cls_Tools.EscribeLog("", sLog);


            }
            return lsNameSquemas;
        }

        private async Task<List<mdl_DataReporteExtAnualNomina>> ObtenerXML(string sRFC, int iPeriodo, int iMes)
        {
            List<mdl_DataReporteExtAnualNomina> lsDataNomina = new List<mdl_DataReporteExtAnualNomina>();
            try
            {
                for (int i = 1; i <= iMes; i++)
                {
                    var oDataNomina = await EnviaInformacionREXMLMensual(sRFC, iPeriodo, i);
                    oDataNomina.sRFC = sRFC;
                    lsDataNomina.Add(oDataNomina);
                }
            }
            catch (Exception ex)
            {
                string Log = $" Mes donde se origina el problema {iMes} \n " +
                  $"{ex}\n" +
                  $"Método: {nameof(ObtenerXML)}" +
                  $"Periodo: {iPeriodo}";

                cls_Tools.EscribeLog(sRFC, Log);
            }

            return lsDataNomina;
        }

        public async Task<mdl_DataReporteExtAnualNomina> EnviaInformacionREXMLMensual(string sRFC, int iYear, int iMes)
        {
            mdl_DataReporteExtAnualNomina oProReporteExtendidoAnual = new mdl_DataReporteExtAnualNomina();
            List<string> lsUuids = new List<string>();
            try
            {
                #region Rango por Fecha de Emisión
                DateTime dFechaInicial = new DateTime();
                DateTime dFechaFinal = new DateTime();
                dFechaInicial = new DateTime(iYear, iMes, 1);
                if (iMes == 12)
                    dFechaFinal = new DateTime(iYear + 1, 1, 1).AddDays(-1);
                else
                    dFechaFinal = new DateTime(iYear, iMes + 1, 1).AddDays(-1);

                dFechaInicial = Convert.ToDateTime(dFechaInicial.ToString("yyyy-MM-ddT00:00:00"));
                dFechaFinal = Convert.ToDateTime(dFechaFinal.ToString("yyyy-MM-ddT23:59:59"));
                dFechaInicial = dFechaInicial.AddHours(dFechaInicial.Hour);
                dFechaFinal = dFechaFinal.AddHours(dFechaFinal.Hour);
                #endregion

                #region Conexión

                var oMongoConnection = new cls_MongoDBConnection(sRFC);
                IMongoCollection<BsonDocument> omcCollection = await oMongoConnection.GetCollection<BsonDocument>("NominaMeta");
                #endregion

                #region Filtro
                FilterDefinition<BsonDocument> oQuerysNE = Builders<BsonDocument>.Filter.And(
                                Builders<BsonDocument>.Filter.Gte("met_FechaEmision", dFechaInicial),
                                Builders<BsonDocument>.Filter.Lte("met_FechaEmision", dFechaFinal),
                                Builders<BsonDocument>.Filter.Eq("met_Estatus", 1));

                var vProjectionNE = Builders<BsonDocument>.Projection
                                .Include("_id")
                                .Include("met_VersionCFDI");

                var queryNE = omcCollection.Find(oQuerysNE).Project<BsonDocument>(vProjectionNE).ToList();

                foreach (var slt in queryNE)
                    lsUuids.Add(string.Format("{0}|{1}", slt["_id"], slt["met_VersionCFDI"]));
                #endregion


                #region Descarga de XML
                List<byte[]> lyArchXMLs = new List<byte[]>();
                if (lsUuids != null && lsUuids.Count > 0)
                {
                    int v = 0;

                    for (int i = 0; i < lsUuids.Count; i++)
                    {
                        v++;
                        byte[] ayXML = await RecuperarXML(sRFC, lsUuids[i].Split('|')[0], lsUuids[i].Split('|')[1]);
                        lyArchXMLs.Add(ayXML);
                    }
                }

                oProReporteExtendidoAnual.sRFC = sRFC;
                oProReporteExtendidoAnual.lsyXMLs = lyArchXMLs;
                oProReporteExtendidoAnual.sPeriodo = iYear.ToString();
                oProReporteExtendidoAnual.sMesXML = iMes.ToString();
                #endregion

     
            }
            catch (Exception ex)
            {
                string sLog = $" Se origino una exception al momento de enviar los parametros para recuperar los XML.\n " +
                  $"{ex}\n" +
                  "Clase: cls_BarridoXMLPeriodo" +
                  $"Método: {nameof(EnviaInformacionREXMLMensual)}" +
                  $"Paramétros: " +
                  $"    Periodo: {iYear}" +
                  $"    Mes: {iMes}";
                cls_Tools.EscribeLog(sRFC, sLog);
         
            }
            return oProReporteExtendidoAnual;
        }

        private async Task ProcesarXML(mdl_DataReporteExtAnualNomina oDataNomina)
        {
            var oProcesarXML = new cls_XMLProceso(oDataNomina.sRFC);
            await oProcesarXML.ProcesarXMLOptimizado(oDataNomina);
        }
    }
    public class mdl_NProcesoHistorico
    {
        public ObjectId _id { get; set; }
        public int iPeriodo { get; set; }
        public string sRFCEmpresa { get; set; }
        public string sEstado { get; set; } // Por ejemplo: "EntornoCreado", "ProcesamientoCompleto"
        public DateTime dtFechaProceso { get; set; }

        public mdl_NProcesoHistorico()
        {
            sRFCEmpresa = string.Empty;
            sEstado = string.Empty;
            //dtFechaProceso = DateTime.MinValue;
        }
    }
}
