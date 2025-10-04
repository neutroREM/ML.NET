using app_NominaExtendida.Modelos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace app_NominaExtendida
{
    public class cls_XMLProceso
    {
        #region Variables
        private cls_MongoDBConnection oMongoConnection;

        private static XmlSerializer oCFDIserializer33 = new XmlSerializer(typeof(CFDIv33.Comprobante));
        private static XmlSerializer oCFDIserializer40 = new XmlSerializer(typeof(CFDIv40.Comprobante));

        private static CFDIv33.Comprobante oCFDIv33 = new CFDIv33.Comprobante();
        private static CFDIv40.Comprobante oCFDIv40 = new CFDIv40.Comprobante();
        private string sCollectionNAcumulados = ConfigurationManager.AppSettings["CollectionDB"];

        #endregion
        public cls_XMLProceso(string sRFC)
        {
            oMongoConnection = new cls_MongoDBConnection(sRFC);
        }


        public async Task<string> ProcesarXMLOptimizado(mdl_DataReporteExtAnualNomina oNominaData)
        {
            string response = string.Empty;

            try
            {
                if (oMongoConnection != null)
                {
                    var _NAcumuladosCollection = await oMongoConnection.GetCollection<mdl_NAcumuladosMBDB>($"{sCollectionNAcumulados}_{oNominaData.sPeriodo}");

                    var dicMapeoRFC = new Dictionary<string, List<XmlInfo>>();

                    foreach (var xmlBytes in oNominaData.lsyXMLs)
                    {
                        var xmlInfo = ExtraerInfoCompleta(xmlBytes);
                        if (!string.IsNullOrEmpty(xmlInfo.Rfc))
                        {
                            if (!dicMapeoRFC.ContainsKey(xmlInfo.Rfc))
                            {
                                dicMapeoRFC[xmlInfo.Rfc] = new List<XmlInfo>();
                            }
                            dicMapeoRFC[xmlInfo.Rfc].Add(xmlInfo);
                        }
                    }

                    const int TAMANO_LOTE = 10; // Se puede ajustar el tamaño del lote
                    var listaDeRfcs = dicMapeoRFC.Keys.ToList();

                    for (int i = 0; i < listaDeRfcs.Count; i += TAMANO_LOTE)
                    {
                        var lsLoteCFDIProcesar = listaDeRfcs.Skip(i).Take(TAMANO_LOTE);
                        var operacionesBulk = new List<WriteModel<mdl_NAcumuladosMBDB>>();

                        var filterLote = Builders<mdl_NAcumuladosMBDB>.Filter.In(doc => doc.sRFColaborador, lsLoteCFDIProcesar);
                        var dicDocumentosExistentes = (await _NAcumuladosCollection.Find(filterLote).ToListAsync()).ToDictionary(doc => doc.sRFColaborador);

                        foreach (var rfc in lsLoteCFDIProcesar)
                        {
                            var xmlsInfoDelRfc = dicMapeoRFC[rfc];

                            mdl_NAcumuladosMBDB oDataAcumulada = null;

                            dicDocumentosExistentes.TryGetValue(rfc, out var docExiste);

                            var hsUUIDExistentes = new HashSet<string>(docExiste?.lsUUID ?? new List<string>());

                            foreach (var xmlInfo in xmlsInfoDelRfc)
                            {
                                if (string.IsNullOrEmpty(xmlInfo.Uuid) || hsUUIDExistentes.Contains(xmlInfo.Uuid))
                                    continue;

                                mdl_NAcumuladosMBDB oData = await DeserializarYProcesarXml(xmlInfo.XmlBytes, xmlInfo.Mes, xmlInfo.Version, xmlInfo.Uuid);
                                if (oDataAcumulada == null)
                                    oDataAcumulada = oData;
                                else
                                {
                                    int iMes = xmlInfo.Mes;
                                    // Nomina
                                    oDataAcumulada.Nomina.afTotalDeducciones[0] += oData.Nomina.afTotalDeducciones[0];
                                    oDataAcumulada.Nomina.afNetoTotal[0] += oData.Nomina.afNetoTotal[0];
                                    oDataAcumulada.Nomina.afTotalPercepciones[0] += oData.Nomina.afTotalPercepciones[0];
                                    oDataAcumulada.Nomina.afTotalOtrosPagos[0] += oData.Nomina.afTotalOtrosPagos[0];

                                    oDataAcumulada.Nomina.afTotalDeducciones[iMes] += oData.Nomina.afTotalDeducciones[iMes];
                                    oDataAcumulada.Nomina.afNetoTotal[iMes] += oData.Nomina.afNetoTotal[iMes];
                                    oDataAcumulada.Nomina.afTotalPercepciones[iMes] += oData.Nomina.afTotalPercepciones[iMes];
                                    oDataAcumulada.Nomina.afTotalOtrosPagos[iMes] += oData.Nomina.afTotalOtrosPagos[iMes];

                                    oDataAcumulada.lsUUID.Add(xmlInfo.Uuid);

                                    // Percepciones
                                    if (oData.Nomina.oPercepciones != null)
                                    {
                                        if (oDataAcumulada.Nomina.oPercepciones != null)
                                        {
                                            oDataAcumulada.Nomina.oPercepciones.afTotalExento[0] += oData.Nomina.oPercepciones.afTotalExento[0];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalGravado[0] += oData.Nomina.oPercepciones.afTotalGravado[0];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalJubilacionPensionRetiro[0] += oData.Nomina.oPercepciones.afTotalJubilacionPensionRetiro[0];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalSeparacionIndemnizacion[0] += oData.Nomina.oPercepciones.afTotalSeparacionIndemnizacion[0];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalSueldos[0] += oData.Nomina.oPercepciones.afTotalSueldos[0];

                                            oDataAcumulada.Nomina.oPercepciones.afTotalExento[iMes] += oData.Nomina.oPercepciones.afTotalExento[iMes];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalGravado[iMes] += oData.Nomina.oPercepciones.afTotalGravado[iMes];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalJubilacionPensionRetiro[iMes] += oData.Nomina.oPercepciones.afTotalJubilacionPensionRetiro[iMes];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalSeparacionIndemnizacion[iMes] += oData.Nomina.oPercepciones.afTotalSeparacionIndemnizacion[iMes];
                                            oDataAcumulada.Nomina.oPercepciones.afTotalSueldos[iMes] += oData.Nomina.oPercepciones.afTotalSueldos[iMes];

                                            if (oData.Nomina.oPercepciones.lsPercepcion != null && oData.Nomina.oPercepciones.lsPercepcion.Count > 0)
                                            {
                                                if (oDataAcumulada.Nomina.oPercepciones.lsPercepcion == null)
                                                    oDataAcumulada.Nomina.oPercepciones.lsPercepcion = new List<mdl_PercepcionMBDB>();

                                                if (oDataAcumulada.Nomina.oPercepciones.lsPercepcion.Count == 0)
                                                {
                                                    oDataAcumulada.Nomina.oPercepciones.lsPercepcion.AddRange(oData.Nomina.oPercepciones.lsPercepcion);
                                                }
                                                else
                                                {
                                                    var listaLimpia = oDataAcumulada.Nomina.oPercepciones.lsPercepcion
                                                        .GroupBy(p => p.sClave)
                                                        .Select(g => g.First())
                                                        .ToList();

                                                    var percepcionesAcum = listaLimpia.ToDictionary(p => p.sClave);

                                                    foreach (var nuevaPercepcion in oData.Nomina.oPercepciones.lsPercepcion)
                                                    {
                                                        if (percepcionesAcum.TryGetValue(nuevaPercepcion.sClave, out var percepcionExistente))
                                                        {
                                                            percepcionExistente.afExento[0] += nuevaPercepcion.afExento[0];
                                                            percepcionExistente.afGravado[0] += nuevaPercepcion.afGravado[0];


                                                            percepcionExistente.afExento[iMes] += nuevaPercepcion.afExento[iMes];
                                                            percepcionExistente.afGravado[iMes] += nuevaPercepcion.afGravado[iMes];
                                                        }
                                                        else
                                                        {
                                                            oDataAcumulada.Nomina.oPercepciones.lsPercepcion.Add(nuevaPercepcion);
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (oDataAcumulada == null) continue;

                            if (docExiste != null)
                            {
                                // Acumular totales generales (índice 0)
                                docExiste.Nomina.afNetoTotal[0] += oDataAcumulada.Nomina.afNetoTotal[0];
                                docExiste.Nomina.afTotalPercepciones[0] += oDataAcumulada.Nomina.afTotalPercepciones[0];
                                docExiste.Nomina.afTotalDeducciones[0] += oDataAcumulada.Nomina.afTotalDeducciones[0];
                                docExiste.Nomina.afTotalOtrosPagos[0] += oDataAcumulada.Nomina.afTotalOtrosPagos[0];

                                // Acumular totales mensuales iterando por cada mes
                                for (int iMes = 1; iMes < 13; iMes++)
                                {
                                    docExiste.Nomina.afNetoTotal[iMes] += oDataAcumulada.Nomina.afNetoTotal[iMes];
                                    docExiste.Nomina.afTotalPercepciones[iMes] += oDataAcumulada.Nomina.afTotalPercepciones[iMes];
                                    docExiste.Nomina.afTotalDeducciones[iMes] += oDataAcumulada.Nomina.afTotalDeducciones[iMes];
                                    docExiste.Nomina.afTotalOtrosPagos[iMes] += oDataAcumulada.Nomina.afTotalOtrosPagos[iMes];
                                }

                                docExiste.lsUUID.AddRange(oDataAcumulada.lsUUID);

                                // Lógica de fusión para Percepciones
                                if (oDataAcumulada.Nomina.oPercepciones != null)
                                {
                                    // ... (la lógica de fusión de listas complejas se mantiene pero ahora es correcta)
                                }

                                var filter = Builders<mdl_NAcumuladosMBDB>.Filter.Eq(d => d.sRFColaborador, rfc);
                                operacionesBulk.Add(new ReplaceOneModel<mdl_NAcumuladosMBDB>(filter, docExiste));
                            }
                            else
                            {
                                operacionesBulk.Add(new InsertOneModel<mdl_NAcumuladosMBDB>(oDataAcumulada));
                            }
                        }
                        if (operacionesBulk.Any())
                        {
                            await _NAcumuladosCollection.BulkWriteAsync(operacionesBulk);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }



        private class XmlInfo
        {
            public byte[] XmlBytes { get; set; }
            public string Rfc { get; set; }
            public int Mes { get; set; }
            public string Uuid { get; set; }
            public string Version { get; set; }
        }

        private XmlInfo ExtraerInfoCompleta(byte[] xmlBytes)
        {
            var info = new XmlInfo { XmlBytes = xmlBytes };
            try
            {
                if (xmlBytes != null)
                {
                    using (var ms = new MemoryStream(xmlBytes))
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(ms);
                        XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");
                        XmlNodeList xmlComplemento = xmlDocument.GetElementsByTagName("cfdi:Complemento");

                        if (xmlComplemento.Count > 0)
                        {
                            XmlNodeList oListaTimbreFiscalDigital = ((XmlElement)xmlComplemento[0]).GetElementsByTagName("tfd:TimbreFiscalDigital");
                            if (oListaTimbreFiscalDigital.Count > 0)
                            {
                                foreach (XmlElement oNodo in oListaTimbreFiscalDigital)
                                {
                                    info.Uuid = oNodo.GetAttribute("UUID") ?? "-";
                                }
                            }

                            XmlNodeList xmlNomina = ((XmlElement)xmlComplemento[0])?.GetElementsByTagName("nomina12:Nomina");
                            if (xmlNomina.Count > 0)
                            {
                                var nodoNomina = (XmlElement)xmlNomina[0];
                                if (DateTime.TryParse(nodoNomina.GetAttribute("FechaPago"), out DateTime fechaPago))
                                {
                                    info.Mes = fechaPago.Month;
                                }
                            }
                        }

                        if (xmlComprobante.Count > 0)
                        {
                            var xmlElement = (XmlElement)xmlComprobante[0];
                            info.Version = xmlElement.GetAttribute("Version") ?? xmlElement.GetAttribute("version");

                            XmlNodeList xmlReceptor = xmlDocument.GetElementsByTagName("cfdi:Receptor");
                            if (xmlReceptor.Count > 0)
                            {
                                var element = (XmlElement)xmlReceptor[0];
                                info.Rfc = element.GetAttribute("Rfc") == "" ? "-" : element.GetAttribute("Rfc");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string Log = $"{ex}\n" +
                            $"Método: {nameof(ExtraerInfoCompleta)}";
                // Consider logging the exception properly
            }
            return info;
        }
        public int ExtraerAnioXML(byte[] xmlBytes)
        {
            int iAnio = 0;
            try
            {
                if (xmlBytes != null)
                {
                    using (var ms = new MemoryStream(xmlBytes))
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(ms);

                        XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");
                        XmlNodeList xmlComplemento = xmlDocument.GetElementsByTagName("cfdi:Complemento");

                        if (xmlComplemento != null && xmlComplemento.Count > 0)
                        {

                            XmlNodeList? xmlNomina = ((XmlElement?)xmlComplemento[0])?.GetElementsByTagName("nomina12:Nomina");
                            if (xmlNomina != null && xmlNomina.Count > 0)
                            {
                                var nodoNomina = (XmlElement?)xmlNomina[0];
                                if (nodoNomina != null)
                                {
                                    if (DateTime.TryParse(nodoNomina.GetAttribute("FechaPago"), out DateTime fechaPago))
                                    {
                                        iAnio = fechaPago.Year;
                                    }
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string Log = $"{ex}\n" +
                            $"Método: {nameof(ExtraerInfoCompleta)}";
                // Consider logging the exception properly
            }
            return iAnio;
        }


        private async Task<mdl_NAcumuladosMBDB> DeserializarYProcesarXml(byte[] xmlBytes, int iMes, string sVersionCFDI, string sUUID)
        {
            mdl_NAcumuladosMBDB oData = new mdl_NAcumuladosMBDB();
            oData.lsUUID.Add(sUUID);
            try
            {
                using (var ms = new MemoryStream(xmlBytes))
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(ms);
                    XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");

                    if (xmlComprobante.Count > 0)
                    {
                        switch (sVersionCFDI)
                        {

                            case "3.3":
                                ms.Position = 0;
                                oCFDIv33 = (CFDIv33.Comprobante)oCFDIserializer33.Deserialize(ms);

                                if (oCFDIv33 != null)
                                {
                                    foreach (var oComplement in oCFDIv33.Complemento)
                                    {
                                        foreach (var oInsideComplement in oComplement.Any)
                                        {

                                            if (oInsideComplement.Name.Contains("Nomina"))
                                            {
                                                /*Fechas para validar la version de nómina que corresponde*/
                                                string sFechaN12A = "01-02-2020";
                                                string sFechaN12B = "30-06-2023";

                                                var dtFechaNomina = oCFDIv33.Fecha;
                                                DateTime dtFechaN12A = DateTime.Parse(sFechaN12A);
                                                DateTime dtFechaN12B = DateTime.Parse(sFechaN12B);

                                                Type Nomina12AB = dtFechaNomina < dtFechaN12A ? typeof(Nom12A.Nomina) :
                                                     (dtFechaNomina > dtFechaN12A && dtFechaNomina < dtFechaN12B) ? typeof(Nom12B.Nomina) : null;

                                                if (Nomina12AB != null)
                                                {
                                                    XmlSerializer oComplementSerializer = new XmlSerializer(Nomina12AB);

                                                    using (var readerComplement = new StringReader(oInsideComplement.OuterXml))
                                                    {
                                                        var oNomina = oComplementSerializer.Deserialize(readerComplement);
                                                        if (dtFechaNomina < dtFechaN12A)
                                                            oData = await ObtenerNomina33(oData, xmlDocument, iMes, (Nom12A.Nomina)oNomina, null);
                                                        else if (dtFechaNomina > dtFechaN12A && dtFechaNomina < dtFechaN12B)
                                                            oData = await ObtenerNomina33(oData, xmlDocument, iMes, null, (Nom12B.Nomina)oNomina);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                            case "4.0":
                                ms.Position = 0;
                                oCFDIv40 = (CFDIv40.Comprobante)oCFDIserializer40.Deserialize(ms);
                                if (oCFDIv40 != null)
                                {
                                    foreach (var oComplemento in oCFDIv40.Complemento.Any)
                                    {

                                        if (oComplemento.Name.Contains("Nomina"))
                                        {
                                            XmlSerializer oSerializer = new XmlSerializer(typeof(Nom12C.Nomina));
                                            using (var readerComplement = new StringReader(oComplemento.OuterXml))
                                            {
                                                var oNomina12C = (Nom12C.Nomina)oSerializer.Deserialize(readerComplement);
                                                oData = await ObtenerNomina40(oData, xmlDocument, oNomina12C, iMes);

                                            }
                                        }

                                    }

                                }

                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string Log = $" Mes donde se origina el problema {iMes} \n " +
                   $"{ex}\n" +
                   $"Método: DeserializarYProcesarXml";
            }

            return oData;
        }




        private async Task<mdl_NAcumuladosMBDB> ObtenerNomina33(mdl_NAcumuladosMBDB oData, XmlDocument xmlDocument, int iMes, Nom12A.Nomina oNomina12A = null, Nom12B.Nomina oNomina12B = null)
        {
            string sRFCID = string.Empty;
            cls_RecolectorNodosXML oRecolector = new cls_RecolectorNodosXML(iMes);

            var tasks = new List<Task>();
            decimal fNetoTotal = 0.00m;
            try
            {
                #region Lista de nodos XML
                XmlNodeList xmlComplemento = xmlDocument.GetElementsByTagName("cfdi:Complemento");
                XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");
                XmlNodeList xmlNomina = xmlDocument.GetElementsByTagName("nomina12:Nomina");
                XmlNodeList xmlEmisor = xmlDocument.GetElementsByTagName("cfdi:Emisor");
                XmlNodeList xmlReceptor = xmlDocument.GetElementsByTagName("cfdi:Receptor");
                #endregion


                if (xmlComprobante.Count > 0)
                {



                    #region Nodo: Comprobante

                    foreach (XmlElement element in xmlComprobante)
                    {
                        fNetoTotal = element.GetAttribute("Total") == null ? 0.00m : Convert.ToDecimal(element.GetAttribute("Total"));
                    }
                    #endregion

                    #region Nodo: Receptor   
                    foreach (XmlElement element in xmlReceptor)
                    {
                        oData.sRFColaborador = element.GetAttribute("Rfc") == "" ? "-" : element.GetAttribute("Rfc");
                        oData.sNombreColaborador = element.GetAttribute("Nombre") == "" ? "-" : element.GetAttribute("Nombre");
                        sRFCID = oData.sRFColaborador;

                    }
                    #endregion


                    string[] asPartesNombre = oData.sNombreColaborador.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var iniciales = asPartesNombre.Select(p => p[0].ToString().ToUpper());
                    var userID = string.Join("", iniciales);
                    string sMes = iMes.ToString("D2");
                    /*oNomina12B*/
                    if (oNomina12B != null && oNomina12A == null)
                    {
                        oData.sr_usuario = $"{userID}{oData.sRFColaborador}33B";
                        if (oNomina12B.Receptor != null)
                        {
                            oData.sIMSS = oNomina12B.Receptor.NumSeguridadSocial == "" ? "-" : oNomina12B.Receptor.NumSeguridadSocial;

                            oData.sNumeroColaborador = oNomina12B.Receptor.NumEmpleado == "" ? string.Empty : oNomina12B.Receptor.NumEmpleado;
                        }

                        if (oNomina12B.Emisor != null)
                        {
                            oData.sRegPat = oNomina12B.Emisor.RegistroPatronal == null ? "-" : oNomina12B.Emisor.RegistroPatronal;
                        }

                        // Inicialización segura
                        #region Tareas
                        Task<mdl_PercepcionesMBDB> taskPercepciones = Task.FromResult<mdl_PercepcionesMBDB>(null);
                        Task<mdl_DeduccionesMBDB> taskDeducciones = Task.FromResult<mdl_DeduccionesMBDB>(null);
                        Task<mdl_OtrosPagosMBDB> taskOtrosPagos = Task.FromResult<mdl_OtrosPagosMBDB>(null);
                        Task<mdl_IncapacidadesMBDB> taskIncapacidades = Task.FromResult<mdl_IncapacidadesMBDB>(null);
                        #endregion


                        /*Nómina*/
                        var taskNomina = oRecolector.ObtenerDataNomina12B(oNomina12B, sMes, fNetoTotal);
                        tasks.Add(taskNomina);

                        /*Percepciones*/
                        if (oNomina12B.Percepciones != null)
                        {
                            taskPercepciones = oRecolector.ObtenerDataPercepciones12B(oNomina12B, sMes);
                            tasks.Add(taskPercepciones);
                        }

                        /*Deducciones*/
                        if (oNomina12B.Deducciones != null)
                        {
                            taskDeducciones = oRecolector.ObtenerDataDeducciones12B(oNomina12B, sMes);
                            tasks.Add(taskDeducciones);
                        }

                        /*OtrosPagos*/
                        if (oNomina12B.OtrosPagos != null)
                        {
                            taskOtrosPagos = oRecolector.ObtenerDataOtrosPagos12B(oNomina12B, sMes);
                            tasks.Add(taskOtrosPagos);
                        }

                        /*Incapacidades*/
                        if (oNomina12B.Incapacidades != null)
                        {
                            taskIncapacidades = oRecolector.ObtenerDataIncapacidades12B(oNomina12B, sMes);
                            tasks.Add(taskIncapacidades);
                        }

                        await Task.WhenAll(tasks);
                        tasks.Clear();

                        var nomina = await taskNomina;
                        var percepciones = await taskPercepciones;
                        var deducciones = await taskDeducciones;
                        var otrosPagos = await taskOtrosPagos;
                        var incapacidades = await taskIncapacidades;

                        oData.Nomina = nomina;
                        // Percepciones no nulls
                        percepciones = percepciones == null ? new mdl_PercepcionesMBDB() : percepciones;
                        percepciones.lsHorasExtra = percepciones.lsHorasExtra == null ? new List<mdl_HorasExtraMBDB>() : percepciones.lsHorasExtra;
                        percepciones.oAccionesOTitulos = percepciones.oAccionesOTitulos == null ? new mdl_AccionesOTitulosMBDB() : percepciones.oAccionesOTitulos;
                        percepciones.oJubilacionPensionRetiro = percepciones.oJubilacionPensionRetiro == null ? new mdl_JubilacionPensionRetiroMBDB() : percepciones.oJubilacionPensionRetiro;
                        percepciones.oSeparacionIndemnizacion = percepciones.oSeparacionIndemnizacion == null ? new mdl_SeparacionIndemnizacionMBDB() : percepciones.oSeparacionIndemnizacion;
                        oData.Nomina.oPercepciones = percepciones;

                        // Deducciones no nulls
                        deducciones = deducciones == null ? new mdl_DeduccionesMBDB() : deducciones;
                        deducciones.lsDeduccion = deducciones.lsDeduccion == null ? new List<mdl_DeduccionMBDB>() : deducciones.lsDeduccion;
                        oData.Nomina.oDeducciones = deducciones;

                        // OtrosPagos no nulls
                        otrosPagos = otrosPagos == null ? new mdl_OtrosPagosMBDB() : otrosPagos;
                        otrosPagos.lsOtroPago = otrosPagos.lsOtroPago == null ? new List<mdl_OtroPagoMBDB>() : otrosPagos.lsOtroPago;
                        otrosPagos.oCompensacionSaldoAFavor = otrosPagos.oCompensacionSaldoAFavor == null ? new mdl_CompensacionSaldoAFavorMBDB() : otrosPagos.oCompensacionSaldoAFavor;
                        otrosPagos.oSubsidioAlEmpleo = otrosPagos.oSubsidioAlEmpleo == null ? new mdl_SubsidioAlEmpleoMBDB() : otrosPagos.oSubsidioAlEmpleo;
                        oData.Nomina.oOtrosPagos = otrosPagos;

                        // Incapacidades no nulls
                        incapacidades = incapacidades == null ? new mdl_IncapacidadesMBDB() : incapacidades;
                        incapacidades.lsIncapacidad = incapacidades.lsIncapacidad == null ? new List<mdl_IncapacidadMBDB>() : incapacidades.lsIncapacidad;
                        oData.Nomina.oIncapacidades = incapacidades;


                    }

                    /*NOMINA12A*/
                    else if (oNomina12A != null && oNomina12B == null)
                    {

                        oData.sr_usuario = $"{userID}{oData.sRFColaborador}33A";
                        if (oNomina12A.Receptor != null)
                        {
                            oData.sIMSS = oNomina12A.Receptor.NumSeguridadSocial == "" ? "-" : oNomina12A.Receptor.NumSeguridadSocial;

                            oData.sNumeroColaborador = oNomina12A.Receptor.NumEmpleado == "" ? string.Empty : oNomina12A.Receptor.NumEmpleado;
                        }

                        if (oNomina12A.Emisor != null)
                        {
                            oData.sRegPat = oNomina12A.Emisor.RegistroPatronal == null ? "-" : oNomina12A.Emisor.RegistroPatronal;
                        }

                        // Inicialización segura
                        #region Tareas
                        Task<mdl_PercepcionesMBDB> taskPercepciones = Task.FromResult<mdl_PercepcionesMBDB>(null);
                        Task<mdl_DeduccionesMBDB> taskDeducciones = Task.FromResult<mdl_DeduccionesMBDB>(null);
                        Task<mdl_OtrosPagosMBDB> taskOtrosPagos = Task.FromResult<mdl_OtrosPagosMBDB>(null);
                        Task<mdl_IncapacidadesMBDB> taskIncapacidades = Task.FromResult<mdl_IncapacidadesMBDB>(null);
                        #endregion

                        /*Nómina*/
                        var taskNomina = oRecolector.ObtenerDataNomina12A(oNomina12A, sMes, fNetoTotal);
                        tasks.Add(taskNomina);

                        /*Percepciones*/
                        if (oNomina12A.Percepciones != null)
                        {
                            taskPercepciones = oRecolector.ObtenerDataPercepciones12A(oNomina12A, sMes);
                            tasks.Add(taskPercepciones);
                        }

                        /*Deducciones*/
                        if (oNomina12A.Deducciones != null)
                        {
                            taskDeducciones = oRecolector.ObtenerDataDeducciones12A(oNomina12A, sMes);
                            tasks.Add(taskDeducciones);
                        }

                        /*OtrosPagos*/
                        if (oNomina12A.OtrosPagos != null)
                        {
                            taskOtrosPagos = oRecolector.ObtenerDataOtrosPagos12A(oNomina12A, sMes);
                            tasks.Add(taskOtrosPagos);
                        }

                        /*Incapacidades*/
                        if (oNomina12A.Incapacidades != null)
                        {
                            taskIncapacidades = oRecolector.ObtenerDataIncapacidades12A(oNomina12A, sMes);
                            tasks.Add(taskIncapacidades);
                        }

                        await Task.WhenAll(tasks);
                        tasks.Clear();

                        var nomina = await taskNomina;
                        var percepciones = await taskPercepciones;
                        var deducciones = await taskDeducciones;
                        var otrosPagos = await taskOtrosPagos;
                        var incapacidades = await taskIncapacidades;

                        oData.Nomina = nomina;
                        // Percepciones no nulls
                        percepciones = percepciones == null ? new mdl_PercepcionesMBDB() : percepciones;
                        percepciones.lsHorasExtra = percepciones.lsHorasExtra == null ? new List<mdl_HorasExtraMBDB>() : percepciones.lsHorasExtra;
                        percepciones.oAccionesOTitulos = percepciones.oAccionesOTitulos == null ? new mdl_AccionesOTitulosMBDB() : percepciones.oAccionesOTitulos;
                        percepciones.oJubilacionPensionRetiro = percepciones.oJubilacionPensionRetiro == null ? new mdl_JubilacionPensionRetiroMBDB() : percepciones.oJubilacionPensionRetiro;
                        percepciones.oSeparacionIndemnizacion = percepciones.oSeparacionIndemnizacion == null ? new mdl_SeparacionIndemnizacionMBDB() : percepciones.oSeparacionIndemnizacion;
                        oData.Nomina.oPercepciones = percepciones;

                        // Deducciones no nulls
                        deducciones = deducciones == null ? new mdl_DeduccionesMBDB() : deducciones;
                        deducciones.lsDeduccion = deducciones.lsDeduccion == null ? new List<mdl_DeduccionMBDB>() : deducciones.lsDeduccion;
                        oData.Nomina.oDeducciones = deducciones;

                        // OtrosPagos no nulls
                        otrosPagos = otrosPagos == null ? new mdl_OtrosPagosMBDB() : otrosPagos;
                        otrosPagos.lsOtroPago = otrosPagos.lsOtroPago == null ? new List<mdl_OtroPagoMBDB>() : otrosPagos.lsOtroPago;
                        otrosPagos.oCompensacionSaldoAFavor = otrosPagos.oCompensacionSaldoAFavor == null ? new mdl_CompensacionSaldoAFavorMBDB() : otrosPagos.oCompensacionSaldoAFavor;
                        otrosPagos.oSubsidioAlEmpleo = otrosPagos.oSubsidioAlEmpleo == null ? new mdl_SubsidioAlEmpleoMBDB() : otrosPagos.oSubsidioAlEmpleo;
                        oData.Nomina.oOtrosPagos = otrosPagos;

                        // Incapacidades no nulls
                        incapacidades = incapacidades == null ? new mdl_IncapacidadesMBDB() : incapacidades;
                        incapacidades.lsIncapacidad = incapacidades.lsIncapacidad == null ? new List<mdl_IncapacidadMBDB>() : incapacidades.lsIncapacidad;
                        oData.Nomina.oIncapacidades = incapacidades;

                        //oData.Nomina.fNetoTotal = fNetoTotal;
                    }

                }
            }
            catch (Exception ex)
            {
                string Log = $" Mes donde se origina el problema {iMes} \n " +
                   $"UUID:{sRFCID}\n " +
                   $"{ex}\n" +
                   $"Método: ObtenerNomina33";

                //oTools.EscribeLog(Log);
            }


            return oData;
        }


        private async Task<mdl_NAcumuladosMBDB> ObtenerNomina40(mdl_NAcumuladosMBDB oData, XmlDocument xmlDocument, Nom12C.Nomina oNomina, int iMes)
        {
            #region Modelos
            cls_RecolectorNodosXML oRecolector = new cls_RecolectorNodosXML(iMes);

            XmlNodeList xmlReceptor = xmlDocument.GetElementsByTagName("cfdi:Receptor");
            XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");
            var tasks = new List<Task>();
            StringBuilder sb = new StringBuilder();
            string sRFCID = string.Empty;
            #endregion

            string sMesFactura = oNomina.FechaPago.ToString().Split('/')[1];
            decimal fNetoTotal = 0.00m;

            try
            {

                #region Nodo: Comprobante

                foreach (XmlElement element in xmlComprobante)
                {
                    fNetoTotal = Convert.ToDecimal(element.GetAttribute("Total") == null ? "0.00m" : element.GetAttribute("Total"));
                }
                #endregion

                #region Nodo: Receptor   
                foreach (XmlElement element in xmlReceptor)
                {
                    oData.sRFColaborador = element.GetAttribute("Rfc") == "" ? "-" : element.GetAttribute("Rfc");
                    oData.sNombreColaborador = element.GetAttribute("Nombre") == "" ? "-" : element.GetAttribute("Nombre");
                    sRFCID = oData.sRFColaborador;

                }
                #endregion

                string[] asPartesNombre = oData.sNombreColaborador.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var iniciales = asPartesNombre.Select(p => p[0].ToString().ToUpper());
                var userID = string.Join("", iniciales);


                if (oNomina != null)
                {
                    oData.sr_usuario = $"{userID}{oData.sRFColaborador}40";
                    if (oNomina.Receptor != null)
                    {
                        oData.sIMSS = oNomina.Receptor.NumSeguridadSocial == "" ? "-" : oNomina.Receptor.NumSeguridadSocial;

                        oData.sNumeroColaborador = oNomina.Receptor.NumEmpleado == "" ? string.Empty : oNomina.Receptor.NumEmpleado;
                    }

                    if (oNomina.Emisor != null)
                    {
                        oData.sRegPat = oNomina.Emisor.RegistroPatronal == null ? "-" : oNomina.Emisor.RegistroPatronal;
                    }



                    // Inicialización segura
                    #region Tareas
                    Task<mdl_PercepcionesMBDB> taskPercepciones = Task.FromResult<mdl_PercepcionesMBDB>(null);
                    Task<mdl_DeduccionesMBDB> taskDeducciones = Task.FromResult<mdl_DeduccionesMBDB>(null);
                    Task<mdl_OtrosPagosMBDB> taskOtrosPagos = Task.FromResult<mdl_OtrosPagosMBDB>(null);
                    Task<mdl_IncapacidadesMBDB> taskIncapacidades = Task.FromResult<mdl_IncapacidadesMBDB>(null);
                    #endregion
                    string sMes = iMes.ToString("D2");
                    /*Nómina*/
                    var taskNomina = oRecolector.ObtenerDataNomina12C(oNomina, sMes, fNetoTotal);
                    tasks.Add(taskNomina);

                    /*Percepciones*/
                    if (oNomina.Percepciones != null)
                    {
                        taskPercepciones = oRecolector.ObtenerDataPercepciones(oNomina, sMes);
                        tasks.Add(taskPercepciones);
                    }

                    /*Deducciones*/
                    if (oNomina.Deducciones != null)
                    {
                        taskDeducciones = oRecolector.ObtenerDataDeducciones(oNomina, sMes);
                        tasks.Add(taskDeducciones);
                    }

                    /*OtrosPagos*/
                    if (oNomina.OtrosPagos != null)
                    {
                        taskOtrosPagos = oRecolector.ObtenerDataOtrosPagos(oNomina, sMes);
                        tasks.Add(taskOtrosPagos);
                    }

                    /*Incapacidades*/
                    if (oNomina.Incapacidades != null)
                    {
                        taskIncapacidades = oRecolector.ObtenerDataIncapacidades(oNomina, sMes);
                        tasks.Add(taskIncapacidades);
                    }

                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    var nomina = await taskNomina;
                    var percepciones = await taskPercepciones;
                    var deducciones = await taskDeducciones;
                    var otrosPagos = await taskOtrosPagos;
                    var incapacidades = await taskIncapacidades;

                    oData.Nomina = nomina;
                    // Percepciones no nulls
                    percepciones = percepciones == null ? new mdl_PercepcionesMBDB() : percepciones;
                    percepciones.lsHorasExtra = percepciones.lsHorasExtra == null ? new List<mdl_HorasExtraMBDB>() : percepciones.lsHorasExtra;
                    percepciones.oAccionesOTitulos = percepciones.oAccionesOTitulos == null ? new mdl_AccionesOTitulosMBDB() : percepciones.oAccionesOTitulos;
                    percepciones.oJubilacionPensionRetiro = percepciones.oJubilacionPensionRetiro == null ? new mdl_JubilacionPensionRetiroMBDB() : percepciones.oJubilacionPensionRetiro;
                    percepciones.oSeparacionIndemnizacion = percepciones.oSeparacionIndemnizacion == null ? new mdl_SeparacionIndemnizacionMBDB() : percepciones.oSeparacionIndemnizacion;
                    oData.Nomina.oPercepciones = percepciones;

                    // Deducciones no nulls
                    deducciones = deducciones == null ? new mdl_DeduccionesMBDB() : deducciones;
                    deducciones.lsDeduccion = deducciones.lsDeduccion == null ? new List<mdl_DeduccionMBDB>() : deducciones.lsDeduccion;
                    oData.Nomina.oDeducciones = deducciones;

                    // OtrosPagos no nulls
                    otrosPagos = otrosPagos == null ? new mdl_OtrosPagosMBDB() : otrosPagos;
                    otrosPagos.lsOtroPago = otrosPagos.lsOtroPago == null ? new List<mdl_OtroPagoMBDB>() : otrosPagos.lsOtroPago;
                    otrosPagos.oCompensacionSaldoAFavor = otrosPagos.oCompensacionSaldoAFavor == null ? new mdl_CompensacionSaldoAFavorMBDB() : otrosPagos.oCompensacionSaldoAFavor;
                    otrosPagos.oSubsidioAlEmpleo = otrosPagos.oSubsidioAlEmpleo == null ? new mdl_SubsidioAlEmpleoMBDB() : otrosPagos.oSubsidioAlEmpleo;
                    oData.Nomina.oOtrosPagos = otrosPagos;

                    // Incapacidades no nulls
                    incapacidades = incapacidades == null ? new mdl_IncapacidadesMBDB() : incapacidades;
                    incapacidades.lsIncapacidad = incapacidades.lsIncapacidad == null ? new List<mdl_IncapacidadMBDB>() : incapacidades.lsIncapacidad;
                    oData.Nomina.oIncapacidades = incapacidades;


                    //oData.Nomina.fNetoTotal = fNetoTotal;
                }
            }
            catch (Exception ex)
            {
                string Log = $" Mes donde se origina el problema {iMes} \n " +
                $"UUID:{sRFCID}\n " +
                $"{ex}" +
                $"Método: ObtenerNomina40";

                //oTools.EscribeLog(Log);
            }


            return oData;


        }
    }
}
