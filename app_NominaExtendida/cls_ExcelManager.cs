using app_NominaExtendida.Modelos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NPOI.HSSF.Util;
using NPOI.OOXML.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace app_NominaExtendida
{
    public class cls_ExcelManager
    {
        #region Variables   
        private string sPeriodo = string.Empty;
        private HashSet<string> hsTotalesPercepciones = new HashSet<string>();
        private HashSet<string> hsTotalesDeducciones = new HashSet<string>();
        private HashSet<string> hsTotalesOtrosPagos = new HashSet<string>();
        private HashSet<string> hsTotalesIncapacidades = new HashSet<string>();

        private static XSSFWorkbook _workbook;
        private static List<XSSFCellStyle> _styles;


        private static readonly ConcurrentDictionary<string, PropertyInfo> _propertiesCache = new ConcurrentDictionary<string, PropertyInfo>();
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        private IMongoCollection<mdl_NAcumuladosMBDB> _NAcumulados { get; set; }


        private cls_MongoDBConnection _connection;

        #endregion

        public cls_ExcelManager(string sRFC)
        {

            _connection = new cls_MongoDBConnection(sRFC);

        }

        #region Propierties Load
        public void CargarDataTable(DataTable dt)
        {
            #region Columnas estaticas que representan hojas del documento Excel
            dt.Columns.Add("RFColaborador", typeof(string));
            dt.Columns.Add("NombreColaborador", typeof(string));
            dt.Columns.Add("NumeroSeguridadSocial", typeof(string));
            dt.Columns.Add("NumeroColaborador", typeof(string));
            dt.Columns.Add("RegistroPatronal", typeof(string));
            //Nomina
            dt.Columns.Add("TotalPercepciones", typeof(decimal));
            dt.Columns.Add("TotalDeducciones", typeof(decimal));
            dt.Columns.Add("TotalOtrosPagos", typeof(decimal));
            dt.Columns.Add("NetoTotal", typeof(decimal));
            //Percepciones
            dt.Columns.Add("TotalSueldos", typeof(decimal));
            dt.Columns.Add("TotalSeparacionIndemnizacion", typeof(decimal));
            dt.Columns.Add("TotalJubilacionPensionRetiro", typeof(decimal));
            dt.Columns.Add("TotalGravado", typeof(decimal));
            dt.Columns.Add("TotalExento", typeof(decimal));
            //AccionesOTitulos
            dt.Columns.Add("ValorMercado", typeof(decimal));
            dt.Columns.Add("PrecioOtorgarse", typeof(decimal));
            //HorasExtra
            dt.Columns.Add("HorasExtra", typeof(int));
            dt.Columns.Add("Dias", typeof(int));
            dt.Columns.Add("ImportePagado", typeof(decimal));
            //JubilacionPensionRetiro
            dt.Columns.Add("TotalUnaExhibicion", typeof(decimal));
            dt.Columns.Add("TotalParcialidad", typeof(decimal));
            dt.Columns.Add("MontoDiario", typeof(decimal));
            dt.Columns.Add("IngresoAcumulableJPR", typeof(decimal));
            dt.Columns.Add("IngresoNoAcumulableJPR", typeof(decimal));
            //SeparacionIndemnización
            dt.Columns.Add("TotalPagado", typeof(decimal));
            dt.Columns.Add("NumAniosServicio", typeof(int));
            dt.Columns.Add("UltimoSueldoMensOrd", typeof(decimal));
            dt.Columns.Add("IngresoAcumulableSI", typeof(decimal));
            dt.Columns.Add("IngresoNoAcumulableSI", typeof(decimal));
            //Deducciones
            dt.Columns.Add("TotalOtrasDeducciones", typeof(decimal));
            dt.Columns.Add("TotalImpuestosRetenidos", typeof(decimal));
            //OtrosPagos
            dt.Columns.Add("ImportePagadoTotal", typeof(decimal));
            dt.Columns.Add("SubsidioCausadoTotal", typeof(decimal));
            //SubsidioAlEmpleo
            dt.Columns.Add("SubsidioCausado", typeof(decimal));
            //CompensacionSaldoAFavor
            dt.Columns.Add("SaldoAFavor", typeof(decimal));
            dt.Columns.Add("RemanenteSalFav", typeof(decimal));
            //Incapacidad
            dt.Columns.Add("DiasIncapacidadTotal", typeof(int));
            dt.Columns.Add("ImporteTotal", typeof(decimal));
            dt.Columns.Add("TipoIncapacidad", typeof(string));
            #endregion
        }
        private (XSSFWorkbook, List<XSSFCellStyle>) IniciarStyles(XSSFWorkbook _workbook, List<XSSFCellStyle> _styles)
        {
            _styles = new List<XSSFCellStyle>();

            #region Estilos para la celdas
            IFont boldFont = _workbook.CreateFont();
            IFont boldItalic = _workbook.CreateFont();

            boldItalic.IsItalic = true;
            boldFont.IsBold = true;



            //Estilos para celdas con texto - 0
            XSSFCellStyle CeldasTextoCentro = (XSSFCellStyle)_workbook.CreateCellStyle();
            CeldasTextoCentro.DataFormat = _workbook.CreateDataFormat().GetFormat("#####0");
            CeldasTextoCentro.LeftBorderColor = HSSFColor.Aqua.Index;
            CeldasTextoCentro.RightBorderColor = HSSFColor.Aqua.Index;
            CeldasTextoCentro.BorderLeft = BorderStyle.Thin;
            CeldasTextoCentro.BorderRight = BorderStyle.Thin;
            CeldasTextoCentro.VerticalAlignment = VerticalAlignment.Center;
            CeldasTextoCentro.Alignment = HorizontalAlignment.Center;
            CeldasTextoCentro.SetFont(boldFont);  /*[0]*/

            _styles.Add(CeldasTextoCentro);

            //Estilos para celdas con texto orientadas a la izquiera - 1
            XSSFCellStyle CeldasTextoIzq = (XSSFCellStyle)_workbook.CreateCellStyle();
            CeldasTextoIzq.DataFormat = _workbook.CreateDataFormat().GetFormat("#####0");
            CeldasTextoIzq.LeftBorderColor = HSSFColor.Aqua.Index;
            CeldasTextoIzq.RightBorderColor = HSSFColor.Aqua.Index;
            CeldasTextoIzq.BorderLeft = BorderStyle.Thin;
            CeldasTextoIzq.BorderRight = BorderStyle.Thin;
            CeldasTextoIzq.VerticalAlignment = VerticalAlignment.Center;
            CeldasTextoIzq.Alignment = HorizontalAlignment.Left;

            _styles.Add(CeldasTextoIzq); /*[1]*/

            XSSFCellStyle CeldasNumericas = (XSSFCellStyle)_workbook.CreateCellStyle();
            CeldasNumericas.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0");
            CeldasNumericas.LeftBorderColor = HSSFColor.Aqua.Index;
            CeldasNumericas.RightBorderColor = HSSFColor.Aqua.Index;
            CeldasNumericas.BorderLeft = BorderStyle.Thin;
            CeldasNumericas.BorderRight = BorderStyle.Thin;
            CeldasNumericas.VerticalAlignment = VerticalAlignment.Center;
            CeldasNumericas.Alignment = HorizontalAlignment.Right;

            _styles.Add(CeldasNumericas); /*[2]*/

            //Estilos para celdas numericas con 0.00 centradas - 3 
            XSSFCellStyle CeldasNumericasPunto = (XSSFCellStyle)_workbook.CreateCellStyle();
            CeldasNumericasPunto.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0.00");
            CeldasNumericasPunto.LeftBorderColor = HSSFColor.Aqua.Index;
            CeldasNumericasPunto.RightBorderColor = HSSFColor.Aqua.Index;
            CeldasNumericasPunto.BorderLeft = BorderStyle.Thin;
            CeldasNumericasPunto.BorderRight = BorderStyle.Thin;
            CeldasNumericasPunto.VerticalAlignment = VerticalAlignment.Center;
            CeldasNumericasPunto.Alignment = HorizontalAlignment.Center;

            _styles.Add(CeldasNumericasPunto); /*[3]*/

            //Estilos para celdas numericas con 0.00 derecha - 4
            XSSFCellStyle CeldasConPesos = (XSSFCellStyle)_workbook.CreateCellStyle();
            CeldasConPesos.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0.00");
            CeldasConPesos.LeftBorderColor = HSSFColor.Aqua.Index;
            CeldasConPesos.RightBorderColor = HSSFColor.Aqua.Index;
            CeldasConPesos.BorderLeft = BorderStyle.Thin;
            CeldasConPesos.BorderRight = BorderStyle.Thin;

            CeldasConPesos.VerticalAlignment = VerticalAlignment.Center;
            CeldasConPesos.Alignment = HorizontalAlignment.Right;
            _styles.Add(CeldasConPesos); /*[4]*/


            /*Estilos sin color*/
            XSSFCellStyle CeldasSinColor = (XSSFCellStyle)_workbook.CreateCellStyle();
            boldFont.IsItalic = true;
            CeldasSinColor.DataFormat = _workbook.CreateDataFormat().GetFormat("#####0");
            CeldasSinColor.VerticalAlignment = VerticalAlignment.Distributed;
            CeldasSinColor.Alignment = HorizontalAlignment.Left;
            CeldasSinColor.SetFont(boldFont);  /*[5]*/
            _styles.Add(CeldasSinColor);

            #endregion


            #region Estilos para las Cabeceras para los conceptos (Percepción, Deducción, Otro Pago, Incapacidad)


            /*Fondo Azul*/
            XSSFCellStyle BordeFondoAzul = (XSSFCellStyle)_workbook.CreateCellStyle();
            var colorAzul = new XSSFColor(new byte[] { 218, 238, 243 }, new DefaultIndexedColorMap());
            BordeFondoAzul.SetFillForegroundColor(colorAzul);
            BordeFondoAzul.FillPattern = FillPattern.SolidForeground;
            BordeFondoAzul.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0");
            BordeFondoAzul.BottomBorderColor = HSSFColor.Aqua.Index;
            BordeFondoAzul.LeftBorderColor = HSSFColor.Aqua.Index;
            BordeFondoAzul.RightBorderColor = HSSFColor.Aqua.Index;
            BordeFondoAzul.BorderLeft = BorderStyle.Thin;
            BordeFondoAzul.BorderRight = BorderStyle.Thin;
            BordeFondoAzul.BorderBottom = BorderStyle.Thin;
            BordeFondoAzul.Alignment = HorizontalAlignment.Center;
            BordeFondoAzul.VerticalAlignment = VerticalAlignment.Center;
            //BordeFondoAzul.TopBorderColor = HSSFColor.Aqua.Index;
            //BordeFondoAzul.BorderTop = BorderStyle.Thin;
            BordeFondoAzul.SetFont(boldFont);
            BordeFondoAzul.WrapText = true;
            _styles.Add(BordeFondoAzul); //[6]

            /*Fondo Amarillo*/
            XSSFCellStyle BordeFondoAmarillo = (XSSFCellStyle)_workbook.CreateCellStyle();
            var colorAmarillo = new XSSFColor(new byte[] { 253, 253, 211 }, new DefaultIndexedColorMap());
            BordeFondoAmarillo.SetFillForegroundColor(colorAmarillo);
            BordeFondoAmarillo.FillPattern = FillPattern.SolidForeground;
            BordeFondoAmarillo.SetFont(boldItalic);
            _styles.Add(BordeFondoAmarillo); //[7]

            /*Fondo Rojo*/
            XSSFCellStyle BordeFondoRojo = (XSSFCellStyle)_workbook.CreateCellStyle();
            var colorRojo = new XSSFColor(new byte[3] { 251, 163, 197 }, new DefaultIndexedColorMap());
            BordeFondoRojo.SetFillForegroundColor(colorRojo);
            BordeFondoRojo.FillPattern = FillPattern.SolidForeground;
            BordeFondoRojo.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0");
            BordeFondoRojo.BottomBorderColor = HSSFColor.Red.Index;
            BordeFondoRojo.LeftBorderColor = HSSFColor.Red.Index;
            BordeFondoRojo.RightBorderColor = HSSFColor.Red.Index;
            BordeFondoRojo.BorderLeft = BorderStyle.Thin;
            BordeFondoRojo.BorderRight = BorderStyle.Thin;
            BordeFondoRojo.BorderBottom = BorderStyle.Thin;
            BordeFondoRojo.Alignment = HorizontalAlignment.Center;
            BordeFondoRojo.VerticalAlignment = VerticalAlignment.Center;
            //BordeFondoRojo.TopBorderColor = HSSFColor.DarkRed.Index;
            //BordeFondoRojo.BorderTop = BorderStyle.Thin;
            BordeFondoRojo.SetFont(boldFont);
            BordeFondoRojo.WrapText = true;
            _styles.Add(BordeFondoRojo); //[8]


            /*Fonde Verde*/
            XSSFCellStyle BordeFondoVerde = (XSSFCellStyle)_workbook.CreateCellStyle();
            var colorVerde = new XSSFColor(new byte[3] { 178, 244, 200 }, new DefaultIndexedColorMap());
            BordeFondoVerde.SetFillForegroundColor(colorVerde);
            BordeFondoVerde.FillPattern = FillPattern.SolidForeground;
            BordeFondoVerde.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0");
            BordeFondoVerde.BottomBorderColor = HSSFColor.LightGreen.Index;
            BordeFondoVerde.LeftBorderColor = HSSFColor.LightGreen.Index;
            BordeFondoVerde.RightBorderColor = HSSFColor.LightGreen.Index;
            BordeFondoVerde.BorderLeft = BorderStyle.Thin;
            BordeFondoVerde.BorderRight = BorderStyle.Thin;
            BordeFondoVerde.BorderBottom = BorderStyle.Thin;
            BordeFondoVerde.Alignment = HorizontalAlignment.Center;
            BordeFondoVerde.VerticalAlignment = VerticalAlignment.Center;
            //BordeFondoVerde.TopBorderColor = HSSFColor.DarkGreen.Index;
            //BordeFondoVerde.BorderTop = BorderStyle.Thin;
            BordeFondoVerde.SetFont(boldFont);
            BordeFondoVerde.WrapText = true;
            _styles.Add(BordeFondoVerde); //[9]


            /*Fondo Morado*/
            XSSFCellStyle BordeFondoMorado = (XSSFCellStyle)_workbook.CreateCellStyle();
            var colorMorado = new XSSFColor(new byte[3] { 186, 178, 248 }, new DefaultIndexedColorMap());
            BordeFondoMorado.SetFillForegroundColor(colorMorado);
            BordeFondoMorado.FillPattern = FillPattern.SolidForeground;
            BordeFondoMorado.DataFormat = _workbook.CreateDataFormat().GetFormat("###,##0");
            BordeFondoMorado.BottomBorderColor = HSSFColor.DarkTeal.Index;
            BordeFondoMorado.LeftBorderColor = HSSFColor.DarkTeal.Index;
            BordeFondoMorado.RightBorderColor = HSSFColor.DarkTeal.Index;
            BordeFondoMorado.BorderLeft = BorderStyle.Thin;
            BordeFondoMorado.BorderRight = BorderStyle.Thin;
            BordeFondoMorado.BorderBottom = BorderStyle.Thin;
            BordeFondoMorado.Alignment = HorizontalAlignment.Center;
            BordeFondoMorado.VerticalAlignment = VerticalAlignment.Center;
            //BordeFondoMorado.TopBorderColor = HSSFColor.DarkTeal.Index;
            //BordeFondoMorado.BorderTop = BorderStyle.Thin;
            BordeFondoMorado.SetFont(boldFont);
            BordeFondoMorado.WrapText = true;
            _styles.Add(BordeFondoMorado); //[10]

            /**/
            XSSFCellStyle EstiloConAjuste = (XSSFCellStyle)_workbook.CreateCellStyle();
            EstiloConAjuste.WrapText = true; // Habilita ajuste de texto
            EstiloConAjuste.Alignment = HorizontalAlignment.Center;
            EstiloConAjuste.BottomBorderColor = HSSFColor.Aqua.Index;
            EstiloConAjuste.LeftBorderColor = HSSFColor.Aqua.Index;
            EstiloConAjuste.RightBorderColor = HSSFColor.Aqua.Index;
            EstiloConAjuste.BorderBottom = BorderStyle.Thin;
            EstiloConAjuste.BorderRight = BorderStyle.Thin;
            EstiloConAjuste.BorderLeft = BorderStyle.Thin;
            _styles.Add(EstiloConAjuste); //[11]

            #endregion



            return (_workbook, _styles);
        }

        #endregion


        public async Task<string> ExcelProcessor(int iMesFinal, int iPeriodo)
        {
            string sB64 = string.Empty;
            string response = string.Empty;

            if (iMesFinal <= 1 && iMesFinal >= 13)
            {
                Console.WriteLine("Mes ingresado incorrecto, no entra en el parameto permitido");
                return sB64;
            }

            _NAcumulados = await _connection.GetCollection<mdl_NAcumuladosMBDB>($"MBDB_NAcumulados_{iPeriodo}");



         

            var diccionarioDataTables = new Dictionary<string, DataTable>();

            for (int iMes = 1; iMes <= iMesFinal; iMes++)
            {
                DataTable dtMes = await GenerarDataTableParaMes(iMes);
                diccionarioDataTables.Add($"Mes_{iMes:D2}", dtMes);
            }
            DataTable dtAnual = await GenerarDataTableParaMes(0);
            diccionarioDataTables.Add($"Mes_00", dtAnual);
            sB64 = LlenarExcel(diccionarioDataTables);


            //sB64 = await GenerarCaracteristicasReporteOptimized(iMes);
            //await GenerarReporteConPipeline(iMes, iPeriodo);
            return sB64;
        }

        private async Task<DataTable> GenerarDataTableParaMes(int iMes)
        {
            // 1. Definir el Pipeline de Agregación
            var pipeline = new BsonDocument[]
            {
          // Etapa $project: Remodela el documento. Extrae el valor del mes específico de los arreglos.
          new BsonDocument("$project", new BsonDocument
          {
              { "_id", 0 },
              { "RFColaborador", "$em_rfcolaborador" },
              { "NombreColaborador", "$em_nombre" },
              { "NumeroSeguridadSocial", "$em_imss" },
              { "NumeroColaborador", "$em_numero" },
              { "RegistroPatronal", "$em_regpat" },
              // Extraer el valor del mes 'iMes' del arreglo correspondiente
              { "TotalPercepciones", new BsonDocument("$arrayElemAt", new BsonArray {
                  "$Nomina.TotalPercepciones", iMes }) },
              { "TotalDeducciones", new BsonDocument("$arrayElemAt", new BsonArray {
                  "$Nomina.TotalDeducciones", iMes }) },
              { "TotalOtrosPagos", new BsonDocument("$arrayElemAt", new BsonArray {
                  "$Nomina.TotalOtrosPagos", iMes }) },
              { "NetoTotal", new BsonDocument("$arrayElemAt", new BsonArray {
                  "$Nomina.NetoTotal", iMes }) },
              // ... Agrega aquí todos los demás campos fijos de la misma manera

              // Transforma los arreglos de conceptos dinámicos en un formato clave-valor
              {
                  "ConceptosDinamicos", new BsonDocument("$concatArrays", new BsonArray
                  {
                      // Percepciones (Gravado y Exento)
                      new BsonDocument("$map", new BsonDocument
                      {
                          { "input", new BsonDocument("$ifNull", new BsonArray {
                              "$Nomina.Percepciones.Percepcion", new BsonArray() }) },
                          { "as", "p" },
                          { "in", new BsonDocument
                          {
                              { "k", new BsonDocument("$concat", new BsonArray {
                                  "$$p.Clave", "-", "$$p.Concepto", "(GRAVADO)" }) },
                              { "v", new BsonDocument("$arrayElemAt", new BsonArray 
                              {
                                  "$$p.Gravado", iMes }) 
                              }
                          }
                          }
                      }),
                      new BsonDocument("$map", new BsonDocument
                      {
                          { "input", new BsonDocument("$ifNull", new BsonArray {
                              "$Nomina.Percepciones.Percepcion", new BsonArray() }) },
                          { "as", "p" },
                          { "in", new BsonDocument
                          {
                                  { "k", new BsonDocument("$concat", new BsonArray 
                                  {
                                      "$$p.Clave", "-", "$$p.Concepto", "(EXENTO)" }) 
                              },
                                  { "v", new BsonDocument("$arrayElemAt", new BsonArray {
                                      "$$p.Exento", iMes }) 
                              }
                          }
                          }
                      }),
                      // Deducciones
                      new BsonDocument("$map", new BsonDocument
                      {
                          { "input", new BsonDocument("$ifNull", new BsonArray {
                              "$Nomina.Deducciones.Deduccion", new BsonArray() }) },
                          { "as", "d" },
                          { "in", new BsonDocument
                              {
                                  { "k", new BsonDocument("$concat", new BsonArray {
                                      "$$d.Clave", "-", "$$d.Concepto", "(IMPORTE)" }) },
                                  { "v", new BsonDocument("$arrayElemAt", new BsonArray {
                                      "$$d.Importe", iMes }) }
                              }
                          }
                      }),
                      // ... Agrega aquí los maps para OtrosPagos e Incapacidades de forma similar
                  })
              }
          }),
          // Etapa $match: Filtra los documentos que no tienen movimientos en ese mes.
          new BsonDocument("$match", new BsonDocument
          {
              { "NetoTotal", new BsonDocument("$gt", 0) }
          })
            };

            // 2. Ejecutar el Pipeline
            var resultados = await _NAcumulados.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // 3. Construir el DataTable a partir de los resultados del pipeline
            var dt = new DataTable($"Mes_{iMes:D2}");
            var columnasDinamicas = new HashSet<string>();

            // Cargar columnas estáticas primero
            CargarDataTable(dt);

            foreach (var doc in resultados)
            {
                var row = dt.NewRow();

                // Llenar columnas estáticas
                foreach (DataColumn col in dt.Columns)
                {
                    if (doc.Contains(col.ColumnName))
                    {
                        var value = doc[col.ColumnName];
                        // Convertir BsonDecimal128 a decimal
                        if (value.BsonType == BsonType.Decimal128)
                        {
                            row[col.ColumnName] = value.AsDecimal128;
                        }
                        else
                        {
                            row[col.ColumnName] = BsonTypeMapper.MapToDotNetValue(value);
                        }
                    }
                }

                // Llenar columnas dinámicas
                var conceptos = doc["ConceptosDinamicos"].AsBsonArray;
                foreach (var concepto in conceptos)
                {
                    var k = concepto["k"].AsString;
                    var v = concepto["v"];

                    if (v.AsDecimal > 0) // Solo procesar si hay un valor
                    {
                        // Si la columna no existe en el DataTable, la agregamos
                        if (!columnasDinamicas.Contains(k))
                        {
                            dt.Columns.Add(k, typeof(decimal));
                            columnasDinamicas.Add(k);
                        }
                        row[k] = v.AsDecimal128;
                    }
                }
                dt.Rows.Add(row);
            }

            return dt;
        }




        private async Task<string> GenerarCaracteristicasReporteOptimized(int iMes)
        {
            string sB64 = string.Empty;
            var tasks = new List<Task>();

            try
            {
  

                /*  CICLO  #1
                 * Consulta para obtener las claves y conceptos de cada Tipo
                 * Add percepcion,deduccion,otropago,incapacidad) dt.colums para cada mes  */
                //var diccionarioDataTables = await CreateNecessaryDatatables(iMes);
                var diccionarioDataTables =new Dictionary<string, DataTable>();

                /*  CICLO  #8 
                 *  Se consulta 5000 registros por operacion hasta que se obtenga todos los registros*/
                var registros = await _NAcumulados.Find(_ => true)
                    .Project<BsonDocument>(Builders<mdl_NAcumuladosMBDB>.Projection
                    .Exclude(r => r._id)
                    .Include(r => r.sRFColaborador)
                    .Include(r => r.sNombreColaborador)
                    .Include(r => r.sIMSS)
                    .Include(r => r.sNumeroColaborador)
                    .Include(r => r.sRegPat)
                    .Include(r => r.Nomina))
                    .ToListAsync();






                /*  CICLO  #9 
                 *  Creacion de las filas para la dt correspondiente 
                 *  Cacheo de dynamic reflections */
                var rowsConcurrent = new ConcurrentBag<DataRow>();
                foreach (var registro in registros)
                {
                    var localRows = new List<DataRow>();

                    for (int i = 0; i <= iMes; i++)
                    {
                        string sMes = i.ToString("D2");
                        DataTable dtMes = diccionarioDataTables[$"Mes_{sMes}"];
                        DataRow row = dtMes.NewRow();
                        //var prop = cachePropierties[sMes];

                        if (registro != null)
                        {
                            row["RFC"] = registro.Contains("em_rfColaborador") && registro["em_rfColaborador"] != BsonNull.Value
                                ? registro["em_rfColaborador"].AsString
                                : "-";


                            row["Nombre"] = registro.Contains("em_nombre") && registro["em_nombre"] != BsonNull.Value
                                ? registro["em_nombre"].AsString
                                : "-";

                            row["NumeroSeguridadSocial"] = registro.Contains("em_imss") && registro["em_imss"] != BsonNull.Value
                                ? registro["em_imss"].AsString
                                : "-";



                            row["NumeroEmpleado"] = registro.Contains("em_numero") && registro["em_numero"] != BsonNull.Value
                                ? registro["em_numero"].AsString
                                : "-";


                            row["RegistroPatronal"] = registro.Contains("em_regpat") && registro["em_regpat"] != BsonNull.Value
                                ? registro["em_regpat"].AsString
                                : "-";

                        }


                        if (registro.Contains("Nomina"))
                        {
                            var nominaBson = registro["Nomina"].AsBsonDocument;

                            var NNomina = BsonSerializer.Deserialize<mdl_NominaMBDB>(nominaBson);

                            if (NNomina != null)
                            {

                                var fTotalPercepciones = NNomina.afTotalPercepciones[i];
                                var fTotalDeducciones = NNomina.afTotalDeducciones[i];
                                var fTotalOtrosPagos = NNomina.afTotalOtrosPagos[i];
                                var fNetoTotal = NNomina.afNetoTotal[i];


                                if (fTotalPercepciones.Equals(0.00m) && fTotalDeducciones.Equals(0.00m) && fTotalOtrosPagos.Equals(0.00m) && fNetoTotal.Equals(0.00m))
                                    continue;

                                row["TotalPercepciones"] = fTotalPercepciones;
                                row["TotalDeducciones"] = fTotalDeducciones;
                                row["TotalOtrosPagos"] = fTotalOtrosPagos;
                                row["NetoTotal"] = fNetoTotal;


                            }

                            if (NNomina.oPercepciones != null)
                            {
                                var NPercepciones = NNomina.oPercepciones;

                                row["TotalSueldos"] = NPercepciones.afTotalSueldos[i];
                                row["TotalSeparacionIndemnizacion"] = NPercepciones.afTotalSeparacionIndemnizacion[i];
                                row["TotalJubilacionPensionRetiro"] = NPercepciones.afTotalSeparacionIndemnizacion[i];
                                row["TotalGravado"] = NPercepciones.afTotalGravado[i];
                                row["TotalExento"] = NPercepciones.afTotalExento[i];


                                if (NNomina.oPercepciones.lsPercepcion != null && NNomina.oPercepciones.lsPercepcion.Count > 0)
                                {
                                    foreach (var NPercepcion in NNomina.oPercepciones.lsPercepcion)
                                    {
                                        string sConceptoGravado = $"{NPercepcion.sClave}-{NPercepcion.sConcepto}(GRAVADO)";
                                        string sConceptoExento = $"{NPercepcion.sClave}-{NPercepcion.sConcepto}(EXENTO)";
                                        row[sConceptoGravado] = NPercepcion.afGravado[i];

                                        row[sConceptoExento] = NPercepcion.afExento[i];
                                    }
                                }

                                if (NPercepciones.oAccionesOTitulos != null)
                                {

                                    row["ValorMercado"] = NPercepciones.oAccionesOTitulos.afValorMercado[i];
                                    row["PrecioOtorgarse"] = NPercepciones.oAccionesOTitulos.afPrecioAlOtorgarse[i];
                                }


                                if (NPercepciones.oSeparacionIndemnizacion != null)
                                {
                                    row["TotalPagado"] = NPercepciones.oSeparacionIndemnizacion.afTotalPagado[i];
                                    row["NumAniosServicio"] = NPercepciones.oSeparacionIndemnizacion.aiNumAniosServicio[i];
                                    row["UltimoSueldoMensOrd"] = NPercepciones.oSeparacionIndemnizacion.afUltimoSueldoMensOrd[i];
                                    row["IngresoAcumulableSI"] = NPercepciones.oSeparacionIndemnizacion.afIngresoAcumulable[i];
                                    row["IngresoNoAcumulableSI"] = NPercepciones.oSeparacionIndemnizacion.afIngresoNoAcumulable[i];
                                }

                                if (NPercepciones.oJubilacionPensionRetiro != null)
                                {

                                    row["TotalUnaExhibicion"] = NPercepciones.oJubilacionPensionRetiro.afTotalUnaExhibicion[i];
                                    row["TotalParcialidad"] = NPercepciones.oJubilacionPensionRetiro.afTotalParcialidad[i];
                                    row["MontoDiario"] = NPercepciones.oJubilacionPensionRetiro.afMontoDiario[i];
                                    row["IngresoAcumulableJPR"] = NPercepciones.oJubilacionPensionRetiro.afIngresoAcumulable[i];
                                    row["IngresoNoAcumulableJPR"] = NPercepciones.oJubilacionPensionRetiro.afIngresoNoAcumulable[i];
                                }
                            }

                            if (NNomina.oDeducciones != null)
                            {
                                mdl_DeduccionesMBDB NDeducciones = NNomina.oDeducciones;
                                row["TotalOtrasDeducciones"] = NDeducciones.afTotalOtrasDeducciones[i];
                                row["TotalImpuestosRetenidos"] = NDeducciones.afTotalImpuestosRetenidos[i];

                                if (NDeducciones.lsDeduccion != null && NDeducciones.lsDeduccion.Count > 0)
                                {
                                    foreach (var NDeduccion in NNomina.oDeducciones.lsDeduccion)
                                    {
                                        string sConceptoImporte = $"{NDeduccion.sClave}-{NDeduccion.sConcepto}(IMPORTE)";
                                        row[sConceptoImporte] = NDeduccion.afImporte[i];
                                    }
                                }
                            }

                            if (NNomina.oOtrosPagos != null)
                            {

                                row["ImportePagadoTotal"] = NNomina.oOtrosPagos.afImporteTotal[i];
                                row["SubsidioCausadoTotal"] = NNomina.oOtrosPagos.afSubsidioCausadoTotal[i];

                                if (NNomina.oOtrosPagos.lsOtroPago != null && NNomina.oOtrosPagos.lsOtroPago.Count > 0)
                                {
                                    foreach (var NOtroPago in NNomina.oOtrosPagos.lsOtroPago)
                                    {
                                        string sConceptoImporte = $"{NOtroPago.sClave}-{NOtroPago.sConcepto}(IMPORTE)";
                                        row[sConceptoImporte] = NOtroPago.afImporte[i];
                                    }
                                }


                                if (NNomina.oOtrosPagos.oSubsidioAlEmpleo != null)
                                    row["SubsidioCausado"] = NNomina.oOtrosPagos.oSubsidioAlEmpleo.afSubsidioCausado[i];


                                if (NNomina.oOtrosPagos.oCompensacionSaldoAFavor != null)
                                {

                                    row["SaldoAFavor"] = NNomina.oOtrosPagos.oCompensacionSaldoAFavor.afSaldoAFavor[i];
                                    row["RemanenteSalFav"] = NNomina.oOtrosPagos.oCompensacionSaldoAFavor.afRemanenteSalFav[i];


                                }
                            }

                            if (NNomina.oIncapacidades != null)
                            {

                                row["DiasIncapacidadTotal"] = NNomina.oIncapacidades.aiDiasIncapacidadTotal[i];
                                row["ImporteTotal"] = NNomina.oIncapacidades.afImporteTotal[i];
                                row["TipoIncapacidad"] = NNomina.oIncapacidades.lsIncapacidad.Select(x => x.sTipoIncapacidad).FirstOrDefault();

                                if (NNomina.oIncapacidades.lsIncapacidad != null && NNomina.oIncapacidades.lsIncapacidad.Count > 0)
                                {
                                    foreach (var NIncapacidad in NNomina.oIncapacidades.lsIncapacidad)
                                    {
                                        string sColumImporteI = $"{NIncapacidad.sTipoIncapacidad}(IMPORTE)";
                                        string sColumnaDiasIncapacidad = $"{NIncapacidad.sTipoIncapacidad}(DIAS INCAPACIDAD)";

                                        row[sColumImporteI] = NIncapacidad.afImporte[i];

                                        row[sColumnaDiasIncapacidad] = NIncapacidad.aiDiasIncapacidad[i];

                                    }
                                }
                            }
                        }
                        localRows.Add(row);
                    }

                    lock (rowsConcurrent)
                    {
                        foreach (var row in localRows)
                        {
                            rowsConcurrent.Add(row);
                        }
                    }
                }//);

                /*  CICLO  #10 
                 *   Adicion de filas para su dt correspondiente*/
                foreach (var row in rowsConcurrent)
                {
                    var dtMes = diccionarioDataTables[row.Table.TableName];
                    dtMes.Rows.Add(row);
                }

                /*  CICLO  #11
                *   Borrado de columnas sin información XMES
                */
                foreach (var dt in diccionarioDataTables.Values)
                {

                    List<string> lsColumnasRemover = new List<string>();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn column = dt.Columns[i];
                        if (i <= 38)
                            continue;

                        bool bEmpty = false;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row[column] != DBNull.Value && !string.IsNullOrEmpty(row[column]?.ToString()))
                            {
                                bEmpty = true;
                                break;
                            }
                        }

                        if (!bEmpty)
                            lsColumnasRemover.Add(column.ColumnName);
                    }

                    foreach (string nombreColumna in lsColumnasRemover)
                    {
                        if (dt.Columns.Contains(nombreColumna))
                            dt.Columns.Remove(nombreColumna);
                    }
                }

                sB64 = LlenarExcel(diccionarioDataTables);
            }
            catch (Exception ex)
            {
                string sLog = $"Mes que fallo el proceso: {iMes.ToString("D2")}" +
                     $"\n {ex}";
                Console.WriteLine(sLog);
            }
            return sB64;

        }

        //private async Task<Dictionary<string, DataTable>> CreateNecessaryDatatables(int iMes)
        //{
        //    var diccionarioDataTables = new Dictionary<string, DataTable>();
        //    try
        //    {
        //        var registrosConcepto = await _NAcumulados.Find(_ => true)
        //             .Project(r => new
        //             {
        //                 Percepciones = (r.Nomina.oPercepciones != null && r.Nomina.oPercepciones.lsPercepcion != null)
        //                 ? r.Nomina.oPercepciones.lsPercepcion.Select(p => new ConceptoDto
        //                 {
        //                     Concepto = p.sConcepto,
        //                     Clave = p.sClave
        //                 }).ToList() : new List<ConceptoDto>(),


        //                 Deducciones = (r.Nomina.oDeducciones != null && r.Nomina.oDeducciones.lsDeduccion != null)
        //                 ? r.Nomina.oDeducciones.lsDeduccion.Select(d => new ConceptoDto
        //                 {
        //                     Concepto = d.sConcepto,
        //                     Clave = d.sClave
        //                 }).ToList() : new List<ConceptoDto>(),

        //                 OtrosPagos = (r.Nomina.oOtrosPagos != null && r.Nomina.oOtrosPagos.lsOtroPago != null)
        //                  ? r.Nomina.oOtrosPagos.lsOtroPago.Select(o => new ConceptoDto
        //                  {
        //                      Concepto = o.sConcepto,
        //                      Clave = o.sClave,
        //                  }).ToList() : new List<ConceptoDto>(),


        //                 Incapacidades = (r.Nomina.oIncapacidades != null && r.Nomina.oIncapacidades.lsIncapacidad != null)
        //                 ? r.Nomina.oIncapacidades.lsIncapacidad.Select(i => new ConceptoDto
        //                 {
        //                     Clave = i.sTipoIncapacidad,

        //                 }).ToList() : new List<ConceptoDto>()
        //             })
        //             .ToListAsync();

        //        if (registrosConcepto != null)
        //        {
        //            string sMesOut = string.Empty;
        //            /*  CICLO  #3 
        //             *  Creacion Columnas Dinamicas */
        //            for (int i = 0; i <= iMes; i++)
        //            {
        //                sMesOut = i.ToString("D2");
        //                string sMes = i.ToString("D2");

        //                DataTable dtMes = new DataTable($"Mes_{sMes}");
        //                CargarDataTable(dtMes); // Agregar columnas comunes
        //                diccionarioDataTables.Add($"Mes_{sMes}", dtMes);

        //                var columnasExistenes = new HashSet<string>(dtMes.Columns.Cast<DataColumn>().Select(c => c.ColumnName));

        //                /*  CICLO  #4
        //                 *  Creacion Columnas Dinamicas Percepciones */
        //                for (int k = 0; k < registrosConcepto.Count; k++)
        //                {
        //                    int iTotalConceptos = registrosConcepto.Count;
        //                    var registro = registrosConcepto[k];
        //                    if (registro != null)
        //                    {
        //                        dtMes = diccionarioDataTables[$"Mes_{sMes}"];

        //                        for (int j = 0; j < registro.Percepciones.Count; j++)
        //                        {
        //                            var NPercepcion = registro.Percepciones[j];
        //                            string sConceptoGravado = $"{NPercepcion.Clave}-{NPercepcion.Concepto}(GRAVADO)";
        //                            string sConceptoExento = $"{NPercepcion.Clave}-{NPercepcion.Concepto}(EXENTO)";
        //                            if (!columnasExistenes.Contains(sConceptoGravado))
        //                            {
        //                                dtMes.Columns.Add(sConceptoGravado, typeof(decimal));
        //                                columnasExistenes.Add(sConceptoGravado);
        //                                hsTotalesPercepciones.Add(sConceptoGravado);
        //                            }

        //                            if (!columnasExistenes.Contains(sConceptoExento))
        //                            {
        //                                dtMes.Columns.Add(sConceptoExento, typeof(decimal));
        //                                columnasExistenes.Add(sConceptoExento);
        //                                hsTotalesPercepciones.Add(sConceptoExento);
        //                            }
        //                        }
        //                    }
        //                }

        //                /*  CICLO  #5 
        //                 *  Creacion Columnas Dinamicas Dedudcciones */
        //                for (int k = 0; k < registrosConcepto.Count; k++)
        //                {
        //                    int iTotalConceptos = registrosConcepto.Count;
        //                    var registro = registrosConcepto[k];
        //                    if (registro != null)
        //                    {
        //                        dtMes = diccionarioDataTables[$"Mes_{sMes}"];

        //                        for (int j = 0; j < registro.Deducciones.Count; j++)
        //                        {
        //                            var NDeduccion = registro.Deducciones[j];

        //                            if (NDeduccion != null)
        //                            {
        //                                string sConceptoImporte = $"{NDeduccion.Clave}-{NDeduccion.Concepto}(IMPORTE)";

        //                                if (!columnasExistenes.Contains(sConceptoImporte))
        //                                {
        //                                    dtMes.Columns.Add(sConceptoImporte, typeof(decimal));
        //                                    columnasExistenes.Add(sConceptoImporte);
        //                                    hsTotalesDeducciones.Add(sConceptoImporte);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                /*  CICLO  #6 
        //                 *  Creacion Columnas Dinamicas OtrosPagos */
        //                for (int k = 0; k < registrosConcepto.Count; k++)
        //                {
        //                    var registro = registrosConcepto[k];
        //                    if (registro != null)
        //                    {
        //                        dtMes = diccionarioDataTables[$"Mes_{sMes}"];

        //                        if (registro.OtrosPagos != null)
        //                        {
        //                            for (int j = 0; j < registro.OtrosPagos.Count; j++)
        //                            {
        //                                var NOtrosPagos = registro.OtrosPagos[j];

        //                                if (NOtrosPagos != null)
        //                                {
        //                                    string sConceptoImporte = $"{NOtrosPagos.Clave}-{NOtrosPagos.Concepto}(IMPORTE)";

        //                                    if (!columnasExistenes.Contains(sConceptoImporte))
        //                                    {
        //                                        dtMes.Columns.Add(sConceptoImporte, typeof(decimal));
        //                                        columnasExistenes.Add(sConceptoImporte);
        //                                        hsTotalesOtrosPagos.Add(sConceptoImporte);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                /*  CICLO  #7 
        //                 *  Creacion Columnas Dinamicas Incapacidades */
        //                for (int k = 0; k < registrosConcepto.Count; k++)
        //                {
        //                    int iTotalConceptos = registrosConcepto.Count;
        //                    var registro = registrosConcepto[k];
        //                    if (registro != null)
        //                    {
        //                        dtMes = diccionarioDataTables[$"Mes_{sMes}"];
        //                        if (registro.Incapacidades != null)
        //                        {
        //                            for (int j = 0; j < registro.Incapacidades.Count; j++)
        //                            {
        //                                var NIncapacidades = registro.Incapacidades[j];

        //                                if (NIncapacidades != null)
        //                                {
        //                                    string sConceptoImporte = $"{NIncapacidades.Clave}(IMPORTE)";
        //                                    string sConceptoDiasIncapacidad = $"{NIncapacidades.Clave}(DIAS INCAPACIDAD)";

        //                                    if (!columnasExistenes.Contains(sConceptoImporte))
        //                                    {
        //                                        dtMes.Columns.Add(sConceptoImporte, typeof(decimal));
        //                                        columnasExistenes.Add(sConceptoImporte);
        //                                        hsTotalesIncapacidades.Add(sConceptoImporte);
        //                                    }

        //                                    if (!columnasExistenes.Contains(sConceptoDiasIncapacidad))
        //                                    {
        //                                        dtMes.Columns.Add(sConceptoDiasIncapacidad, typeof(decimal));
        //                                        columnasExistenes.Add(sConceptoDiasIncapacidad);
        //                                        hsTotalesIncapacidades.Add(sConceptoDiasIncapacidad);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string sLog = $"Mes que fallo el proceso: {iMes.ToString("D2")}" +
        //        $"\n {ex}";
        //        Console.WriteLine(sLog);
        //    }
        //    return diccionarioDataTables;
        //}

        private static string sPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private string LlenarExcel(Dictionary<string, DataTable> dicReporte)
        {

            string sB64 = string.Empty;
            byte[] ayFormato = System.IO.File.ReadAllBytes(sPath + "\\Formatos\\Formato.xlsx");
            int row = 7;
            int iHoja = 0;
            int iMesID = 0;

            DataTable dtAnual = new DataTable();
            try
            {
                using (Stream stream = new MemoryStream(ayFormato))
                {
                    _workbook = new XSSFWorkbook(stream);
                    (_workbook, _styles) = IniciarStyles(_workbook, _styles);

                    //ISheet oSheet = null;
                    IRow oRow = null;
                    IRow oHeaderRow = null;
                    NPOI.SS.UserModel.ICell oCell = null;
                    NPOI.SS.UserModel.ICell oHeadersCells = null;

                    foreach (var dt in dicReporte.Values)
                    {
                        #region NumColumnaConceptos
                        //Variables necesarias para realizar Merged a las columnas (Percepciones,Deducciones,OtrosPagos,Incapacidades)

                        int iColumnasPercepcionesXMes = dt.Columns
                            .Cast<DataColumn>()
                            .Count(col => hsTotalesPercepciones.Contains(col.ColumnName));

                        int iColumnasDeduccionesXMes = dt.Columns
                            .Cast<DataColumn>()
                            .Count(col => hsTotalesDeducciones.Contains(col.ColumnName));

                        int iColumnasOtrosPagosXMes = dt.Columns
                            .Cast<DataColumn>()
                            .Count(col => hsTotalesOtrosPagos.Contains(col.ColumnName));


                        int iColumnasIncapacidadesXMes = dt.Columns
                            .Cast<DataColumn>()
                            .Count(col => hsTotalesIncapacidades.Contains(col.ColumnName));
                        #endregion
                        string tableName = dt.TableName; // Por ej: "Mes_01", "Mes_00"
                        int numeroMes = int.Parse(tableName.Split('_')[1]);

                        ISheet oSheet;
                        if (numeroMes == 0)
                        {
                            // Es el acumulado anual ("Mes_00"), va en la hoja 13 (índice 12)
                            oSheet = _workbook.GetSheetAt(12);
                        }
                        else
                        {
                            // Es un mes normal. Enero (mes 1) va en la hoja 1 (índice 0).
                            // Febrero (mes 2) va en la hoja 2 (índice 1), etc.
                            oSheet = _workbook.GetSheetAt(numeroMes - 1);
                        }
                        //oSheet = dt.TableName == "Mes_00" ? _workbook.GetSheetAt(12) : _workbook.GetSheetAt(iHoja + 1);
                        oHeaderRow = oSheet.GetRow(6) ?? oSheet.CreateRow(6);
                        IRow headerConceptoRow = oSheet.GetRow(5) ?? oSheet.CreateRow(5);

                        IRow DetallesRow = oSheet.GetRow(1) ?? oSheet.CreateRow(1);
                        ICell periodoCell = DetallesRow.GetCell(7) ?? DetallesRow.CreateCell(7);
                        periodoCell.SetCellValue($"Periodo: {sPeriodo}");
                        periodoCell.CellStyle = _styles[5];
                        periodoCell.CellStyle = _styles[7];

                        var celdaFecha = DetallesRow.GetCell(8) ?? DetallesRow.CreateCell(8);
                        celdaFecha.SetCellValue($"Fecha Emisión: {DateTime.Now:dd/MM/yyyyy}");
                        celdaFecha.CellStyle = _styles[5];
                        celdaFecha.CellStyle = _styles[7];



                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            oRow = oSheet.GetRow(row + i) ?? oSheet.CreateRow(row + i);
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (j < 38)
                                {
                                    oCell = oRow.GetCell(j) ?? oRow.CreateCell(j);
                                    object cellValue = dt.Rows[i][j];
                                    if (cellValue == null || cellValue == DBNull.Value || string.IsNullOrWhiteSpace(cellValue.ToString()))
                                    {
                                        if (j == 15 && j == 16)
                                            continue;
                                        else // Asignar 0 si está vacío
                                            cellValue = 0.00m;
                                    }
                                    Type dataType = cellValue.GetType();
                                    if (dataType == typeof(string))
                                    {
                                        // Estilo [1]
                                        oCell.SetCellValue(cellValue.ToString());
                                        oCell.CellStyle = _styles[1];
                                    }
                                    else if (dataType == typeof(decimal))
                                    {
                                        // Estilo [4]
                                        oCell.SetCellValue(Convert.ToDouble(cellValue));
                                        oCell.CellStyle = _styles[4];
                                    }
                                    else if (dataType == typeof(int))
                                    {
                                        // Estilo [2]
                                        oCell.SetCellValue(Convert.ToInt32(cellValue));
                                        oCell.CellStyle = _styles[2];
                                    }
                                }
                                else
                                {
                                    string columnName = dt.Columns[j].ColumnName;
                                    oHeadersCells = oHeaderRow.GetCell(j) ?? oHeaderRow.CreateCell(j);
                                    if (string.IsNullOrEmpty(oHeadersCells.StringCellValue))
                                    {
                                        oHeadersCells.SetCellValue(columnName);
                                        oHeadersCells.CellStyle = _styles[0];
                                        oHeadersCells.CellStyle = _styles[11];
                                    }

                                    oCell = oRow.GetCell(j) ?? oRow.CreateCell(j);
                                    object cellValue = dt.Rows[i][j];

                                    if (cellValue == null || cellValue == DBNull.Value || string.IsNullOrWhiteSpace(cellValue.ToString()))
                                        cellValue = 0.00m; // Asignar 0 si está vacío

                                    Type dataType = cellValue.GetType();
                                    if (dataType == typeof(string))
                                    {
                                        // Estilo [1]
                                        oCell.SetCellValue(cellValue.ToString());
                                        oCell.CellStyle = _styles[1];
                                    }
                                    else if (dataType == typeof(decimal))
                                    {
                                        // Estilo [4]
                                        oCell.SetCellValue(Convert.ToDouble(cellValue));
                                        oCell.CellStyle = _styles[4];
                                    }
                                    else if (dataType == typeof(int))
                                    {
                                        // Estilo [2]
                                        oCell.SetCellValue(Convert.ToInt32(cellValue));
                                        oCell.CellStyle = _styles[2];
                                    }
                                }
                            }
                        }

                        #region MergedColumns
                        /*Percepciones*/
                        if (iColumnasPercepcionesXMes > 0)
                        {
                            int iRangePercepcion = 39 + iColumnasPercepcionesXMes;

                            ICell headerPercepcionCell = headerConceptoRow.GetCell(39) ?? headerConceptoRow.CreateCell(39);
                            headerPercepcionCell.SetCellValue("Percepción");

                            CellRangeAddress regionPercepcion = new CellRangeAddress(5, 5, 39, iRangePercepcion - 1);
                            oSheet.AddMergedRegion(regionPercepcion);
                            headerPercepcionCell.CellStyle = _styles[6];

                            for (int col = regionPercepcion.FirstColumn; col < regionPercepcion.LastColumn + 1; col++)
                            {
                                ICell styleCell = headerConceptoRow.GetCell(col) ?? headerConceptoRow.CreateCell(col);
                                styleCell.CellStyle = _styles[0];
                                styleCell.CellStyle = _styles[6];
                            }
                        }

                        /*Deducciones*/
                        if (iColumnasDeduccionesXMes > 0)
                        {
                            int iRangeDeduccion = 39 + iColumnasPercepcionesXMes;

                            ICell headerDeduccionCell = headerConceptoRow.GetCell(iRangeDeduccion) ?? headerConceptoRow.CreateCell(iRangeDeduccion);
                            headerDeduccionCell.SetCellValue("Deducción");

                            CellRangeAddress regionDeduccion = new CellRangeAddress(5, 5, iRangeDeduccion, iRangeDeduccion + iColumnasDeduccionesXMes - 1);
                            oSheet.AddMergedRegion(regionDeduccion);

                            for (int col = regionDeduccion.FirstColumn; col < regionDeduccion.LastColumn + 1; col++)
                            {
                                ICell styleCell = headerConceptoRow.GetCell(col) ?? headerConceptoRow.CreateCell(col);
                                styleCell.CellStyle = _styles[0];
                                styleCell.CellStyle = _styles[8];
                            }
                        }

                        /*OtrosPagos*/
                        if (iColumnasOtrosPagosXMes > 0 && iColumnasOtrosPagosXMes > 1)
                        {
                            int iFirstColumOtroPago = 39 + iColumnasPercepcionesXMes + iColumnasDeduccionesXMes;

                            ICell headerOtrosPagosCell = headerConceptoRow.GetCell(iFirstColumOtroPago) ?? headerConceptoRow.CreateCell(iFirstColumOtroPago);
                            headerOtrosPagosCell.SetCellValue("Otros Pagos");

                            CellRangeAddress regionOtrosPagos = new CellRangeAddress(5, 5, iFirstColumOtroPago, iFirstColumOtroPago + iColumnasOtrosPagosXMes - 1);
                            oSheet.AddMergedRegion(regionOtrosPagos);

                            for (int col = regionOtrosPagos.FirstColumn; col < regionOtrosPagos.LastColumn + 1; col++)
                            {
                                ICell styleCell = headerConceptoRow.GetCell(col) ?? headerConceptoRow.CreateCell(col);
                                styleCell.CellStyle = _styles[0];
                                styleCell.CellStyle = _styles[9];
                            }
                        }
                        else
                        {
                            int iFirstColumOtroPago = 39 + iColumnasPercepcionesXMes + iColumnasDeduccionesXMes;

                            ICell headerOtrosPagosCell = headerConceptoRow.GetCell(iFirstColumOtroPago) ?? headerConceptoRow.CreateCell(iFirstColumOtroPago);
                            headerOtrosPagosCell.SetCellValue("Otros Pagos");
                            headerOtrosPagosCell.CellStyle = _styles[0];
                            headerOtrosPagosCell.CellStyle = _styles[9];

                        }

                        /*Incapacidades*/
                        if (iColumnasIncapacidadesXMes > 0 && iColumnasIncapacidadesXMes > 1)
                        {
                            int iFirstColumIncapacidad = 39 + iColumnasPercepcionesXMes + iColumnasDeduccionesXMes + iColumnasOtrosPagosXMes;

                            ICell headerIncapacidadCell = headerConceptoRow.GetCell(iFirstColumIncapacidad) ?? headerConceptoRow.CreateCell(iFirstColumIncapacidad);
                            headerIncapacidadCell.SetCellValue("Incapacidad");

                            CellRangeAddress regionIncapacidades = new CellRangeAddress(5, 5, iFirstColumIncapacidad, iFirstColumIncapacidad + iColumnasIncapacidadesXMes - 1);
                            oSheet.AddMergedRegion(regionIncapacidades);

                            for (int col = regionIncapacidades.FirstColumn; col < regionIncapacidades.LastColumn + 1; col++)
                            {
                                ICell styleCell = headerConceptoRow.GetCell(col) ?? headerConceptoRow.CreateCell(col);
                                styleCell.CellStyle = _styles[0];
                                styleCell.CellStyle = _styles[10];
                            }

                        }
                        else
                        {
                            int iFirstColumIncapacidad = 39 + iColumnasPercepcionesXMes + iColumnasDeduccionesXMes + iColumnasOtrosPagosXMes;

                            ICell headerIncapacidadCell = headerConceptoRow.GetCell(iFirstColumIncapacidad) ?? headerConceptoRow.CreateCell(iFirstColumIncapacidad);
                            headerIncapacidadCell.SetCellValue("Incapacidad");
                            headerIncapacidadCell.CellStyle = _styles[0];
                            headerIncapacidadCell.CellStyle = _styles[10];
                        }

                        /*Se calcula el largo de la columna */
                        for (int col = 0; col < dt.Columns.Count; col++)
                        {
                            oSheet.SetColumnWidth(col, CalculateColumnWidth(oSheet, col));
                        }

                        iHoja++;
                        iMesID = iHoja;
                    }
                    #endregion
                }

                MemoryStream ms = new MemoryStream();
                _workbook.Write(ms);
                ayFormato = ms.ToArray();
                string rutaEjecutable = AppDomain.CurrentDomain.BaseDirectory;
                string rutaArchivo = Path.Combine(rutaEjecutable, "PruebaReporteNominaAnual_v2.xlsx");
                File.WriteAllBytes(rutaArchivo, ayFormato); 
                sB64 = Convert.ToBase64String(ayFormato);
            }
            catch (Exception ex)
            {
                string sMes = iMesID.ToString("D2");
                Console.WriteLine($"Mes donde ocurrio el fallo: {sMes} " +
                    $"\n {ex}");
            }
            return sB64;
        }


        private static int CalculateColumnWidth(ISheet sheet, int column)
        {
            int maxWidth = 0;
            for (int rowNum = 0; rowNum <= sheet.LastRowNum; rowNum++)
            {
                var row = sheet.GetRow(rowNum);
                if (row?.GetCell(column) != null)
                {
                    var cell = row.GetCell(column);
                    var contentLength = Encoding.UTF8.GetByteCount(cell.ToString());
                    maxWidth = Math.Max(maxWidth, contentLength);
                }
            }
            return (maxWidth + 2) * 240;
        }

        public class ConceptoDto
        {
            public string Concepto { get; set; }
            public string Clave { get; set; }
        }


    }
}
