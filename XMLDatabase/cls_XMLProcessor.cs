using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XMLDatabase
{
    
    internal class cls_XMLProcessor
    {
        #region Collections
        private IMongoCollection<mdl_NDetallePago> _NDetallePagoCollection { get; set; } = null!;
        private IMongoCollection<mdl_NAcumulados> _NAcumuladosCollection { get; set; } = null!;
        #endregion

        #region XML Data
        private static XmlSerializer? oCFDIserializer33 = new XmlSerializer(typeof(dll_Cfdv33.Comprobante));
        private static XmlSerializer? oCFDIserializer40 = new XmlSerializer(typeof(dll_Cfdv40.Comprobante));

        private static dll_Cfdv33.Comprobante? oCFDIv33 = new dll_Cfdv33.Comprobante();
        private static dll_Cfdv40.Comprobante? oCFDIv40 = new dll_Cfdv40.Comprobante();
        //private static List<DataTable> lsDT = new List<DataTable>();
        #endregion

        public cls_XMLProcessor()
        {
            var mongoConnector = new cls_DatabaseConnector();
            _NAcumuladosCollection = mongoConnector.GetCollection<mdl_NAcumulados>(mongoConnector.sNAcumuladosCollection);
            _NDetallePagoCollection = mongoConnector.GetCollection<mdl_NDetallePago>(mongoConnector.sNDetallePagoCollection);
        }

        public void XMLCatcher(mdl_XMLData oData)
        {
            string response = string.Empty;


            if (oData != null)
                ClasificarNomina(oData).GetAwaiter().GetResult();
            else
                Console.WriteLine("El objeto esta vacío o no contiene información alguna.");
        }

        private async Task ClasificarNomina(mdl_XMLData oNominaData)
        {
            string sUUID = string.Empty;
            string sMes = oNominaData.sMesXML;
            var dicNominas = new Dictionary<string, mdl_NAcumulados>();

            try
            {
                if (oNominaData.lsyXMLs != null && oNominaData.lsyXMLs.Count > 0)
                {
                    List<byte[]> lsyXML = oNominaData.lsyXMLs;
                    oNominaData = null;

                    foreach (var xmlBytes in lsyXML)
                    {
                        using (var ms = new MemoryStream(xmlBytes))
                        {
                            var xmlDocument = new XmlDocument();
                            xmlDocument.Load(ms);
                            XmlNodeList xmlComplemento = xmlDocument.GetElementsByTagName("cfdi:Complemento");
                            XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");
                            XmlNodeList oListaTimbreFiscalDigital = ((XmlElement)xmlComplemento[0]).GetElementsByTagName("tfd:TimbreFiscalDigital");

                            if (xmlComprobante.Count > 0)
                            {
                                #region XML UUID 
                                if (oListaTimbreFiscalDigital.Count > 0)
                                {
                                    foreach (XmlElement oNodo in oListaTimbreFiscalDigital)
                                    {
                                        sUUID = oNodo.GetAttribute("UUID") == null ? "-" : oNodo.GetAttribute("UUID");

                                    }
                                }
                                #endregion

                                var xmlElement = (XmlElement)xmlComprobante[0];
                                string sVersionCFDI = xmlElement.GetAttribute("Version") ?? xmlElement.GetAttribute("version");

                                if (sVersionCFDI == "3.3")
                                {
                                    ms.Position = 0;
                                    oCFDIv33 = (dll_Cfdv33.Comprobante)oCFDIserializer33.Deserialize(ms);

                                    if (oCFDIv33 != null)
                                    {
                                        foreach (var oComplement in oCFDIv33.Complemento)
                                        {

                                            foreach (var oInsideComplement in oComplement.Any)
                                            {

                                                if (oInsideComplement.Name.Contains("Nomina"))
                                                {
                                                    string sFechaN12A = "01-02-2020";
                                                    string sFechaN12B = "30-06-2023";

                                                    var dtFechaNomina = oCFDIv33.Fecha;
                                                    DateTime dtFechaN12A = DateTime.Parse(sFechaN12A);
                                                    DateTime dtFechaN12B = DateTime.Parse(sFechaN12B);

                                                    Type? Nomina12AB = dtFechaNomina < dtFechaN12A ? typeof(dll_nom12a.Nomina) :
                                                         (dtFechaNomina > dtFechaN12A && dtFechaNomina < dtFechaN12B) ? typeof(dll_nom12b.Nomina) : null;

                                                    if (Nomina12AB != null)
                                                    {
                                                        XmlSerializer oComplementSerializer = new XmlSerializer(Nomina12AB);
                                                        var oData = new mdl_NAcumulados();
                                                        using (var readerComplement = new StringReader(oInsideComplement.OuterXml))
                                                        {
                                                            var oNomina = oComplementSerializer.Deserialize(readerComplement);
                                                            if (dtFechaNomina < dtFechaN12A)
                                                            {
                                                                oData = await ObtenerNomina33(xmlDocument, (dll_nom12a.Nomina)oNomina, null, sMes);
                                                            }
                                                            else if (dtFechaNomina > dtFechaN12A && dtFechaNomina < dtFechaN12B)
                                                            {
                                                                oData = await ObtenerNomina33(xmlDocument, null, (dll_nom12b.Nomina)oNomina, sMes);

                                                            }

                                                            // ✅ Acumular datos para evitar duplicidad de registros en la DB
                                                            if (!dicNominas.TryGetValue(oData.sRFC, out var existente))
                                                            {
                                                                oData.lsUUID.Add(sUUID);
                                                                dicNominas[oData.sRFC] = oData;
                                                            }
                                                            else
                                                            {

                                                                existente.sRFC = oData.sRFC;
                                                                existente.sNombre = oData.sNombre;
                                                                existente.sIMSS = oData.sIMSS;
                                                                existente.sNumero = oData.sNumero;
                                                                existente.sRegPat = oData.sRegPat;

                                                                if (oData.Nomina != null)
                                                                {
                                                                    existente.Nomina = AcumularNomina(existente.Nomina, oData.Nomina, sMes);
                                                                    if (oData.Nomina.oPercepciones != null)
                                                                    {
                                                                        existente.Nomina.oPercepciones = AcumularPercepciones(existente.Nomina.oPercepciones, oData.Nomina.oPercepciones, sMes);

                                                                        if (oData.Nomina.oPercepciones.oAccionesOTitulos != null)
                                                                            existente.Nomina.oPercepciones.oAccionesOTitulos = AcumularAccionesOTitulos(existente.Nomina.oPercepciones.oAccionesOTitulos, oData.Nomina.oPercepciones.oAccionesOTitulos, sMes);

                                                                        if (oData.Nomina.oPercepciones.oHorasExtra != null)
                                                                            existente.Nomina.oPercepciones.oHorasExtra = AcumularHorasExtra(existente.Nomina.oPercepciones.oHorasExtra, oData.Nomina.oPercepciones.oHorasExtra, sMes);

                                                                        if (oData.Nomina.oPercepciones.oSeparacionIndemnizacion != null)

                                                                            existente.Nomina.oPercepciones.oSeparacionIndemnizacion = AcumularSeparacionIndemnizacion(existente.Nomina.oPercepciones.oSeparacionIndemnizacion, oData.Nomina.oPercepciones.oSeparacionIndemnizacion, sMes);


                                                                        if (oData.Nomina.oPercepciones.oJubilacionPensionRetiro != null)
                                                                            existente.Nomina.oPercepciones.oJubilacionPensionRetiro = AcumularJubilacionPensionRetiro(existente.Nomina.oPercepciones.oJubilacionPensionRetiro, oData.Nomina.oPercepciones.oJubilacionPensionRetiro, sMes);
                                                                    }

                                                                    if (oData.Nomina.oDeducciones != null)
                                                                        existente.Nomina.oDeducciones = AcumularDeducciones(existente.Nomina.oDeducciones, oData.Nomina.oDeducciones);

                                                                    if (oData.Nomina.oOtrosPagos != null)
                                                                    {
                                                                        existente.Nomina.oOtrosPagos = AcumularOtrosPagos(existente.Nomina.oOtrosPagos, oData.Nomina.oOtrosPagos);
                                                                        if (oData.Nomina.oOtrosPagos.oCompensacionSaldoAFavor != null)
                                                                            existente.Nomina.oOtrosPagos.oCompensacionSaldoAFavor = AcumularCompensacionSaldoAFavor(existente.Nomina.oOtrosPagos.oCompensacionSaldoAFavor, oData.Nomina.oOtrosPagos.oCompensacionSaldoAFavor, sMes);

                                                                        if (oData.Nomina.oOtrosPagos.oSubsidioAlEmpleo != null)
                                                                            existente.Nomina.oOtrosPagos.oSubsidioAlEmpleo = AcumularSubsidioAlEmpleo(existente.Nomina.oOtrosPagos.oSubsidioAlEmpleo, oData.Nomina.oOtrosPagos.oSubsidioAlEmpleo, sMes);

                                                                    }

                                                                    if (oData.Nomina.oIncapacidades != null)
                                                                        existente.Nomina.oIncapacidades = AcumularIncapacidades(existente.Nomina.oIncapacidades, oData.Nomina.oIncapacidades);

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            var keys = dicNominas.Keys.ToList();
                                            foreach (var key in keys)
                                            {
                                                if (dicNominas.TryGetValue(key, out var item))
                                                {
                                                    await VerificarRegistro(item, sMes, sUUID);
                                                    dicNominas.Remove(key);
                                                }
                                            }

                                        }
                                    }
                                }
                                else if (sVersionCFDI == "4.0")
                                {
                                    ms.Position = 0;
                                    oCFDIv40 = (dll_Cfdv40.Comprobante)oCFDIserializer40.Deserialize(ms);
                                    if (oCFDIv40 != null)
                                    {
                                        foreach (var oComplemento in oCFDIv40.Complemento.Any)
                                        {

                                            if (oComplemento.Name.Contains("Nomina"))
                                            {
                                                XmlSerializer oSerializer = new XmlSerializer(typeof(dll_nom12c.Nomina));
                                                //mdl_NAcumulados oData = new mdl_NAcumulados();
                                                using (var readerComplement = new StringReader(oComplemento.OuterXml))
                                                {
                                                    var oNomina12C = (dll_nom12c.Nomina)oSerializer.Deserialize(readerComplement);
                                                    var oData = await ObtenerNomina40(xmlDocument, oNomina12C, sMes);

                                                    if (!dicNominas.TryGetValue(oData.sRFC, out var existente))
                                                    {
                                                        oData.lsUUID.Add(sUUID);
                                                        dicNominas[oData.sRFC] = oData;
                                                    }
                                                    else
                                                    {
                                                        // ✅ Acumular datos para evitar duplicidad de registros en la DB
                                                        existente.sRFC = oData.sRFC;
                                                        existente.sNombre = oData.sNombre;
                                                        existente.sIMSS = oData.sIMSS;
                                                        existente.sNumero = oData.sNumero;
                                                        existente.sRegPat = oData.sRegPat;

                                                        if (oData.Nomina != null)
                                                        {
                                                            existente.Nomina = AcumularNomina(existente.Nomina, oData.Nomina, sMes);
                                                            if (oData.Nomina.oPercepciones != null)
                                                            {
                                                                existente.Nomina.oPercepciones = AcumularPercepciones(existente.Nomina.oPercepciones, oData.Nomina.oPercepciones, sMes);

                                                                if (oData.Nomina.oPercepciones.oAccionesOTitulos != null)
                                                                    existente.Nomina.oPercepciones.oAccionesOTitulos = AcumularAccionesOTitulos(existente.Nomina.oPercepciones.oAccionesOTitulos, oData.Nomina.oPercepciones.oAccionesOTitulos, sMes);

                                                                if (oData.Nomina.oPercepciones.oHorasExtra != null)
                                                                    existente.Nomina.oPercepciones.oHorasExtra = AcumularHorasExtra(existente.Nomina.oPercepciones.oHorasExtra, oData.Nomina.oPercepciones.oHorasExtra, sMes);

                                                                if (oData.Nomina.oPercepciones.oSeparacionIndemnizacion != null)

                                                                    existente.Nomina.oPercepciones.oSeparacionIndemnizacion = AcumularSeparacionIndemnizacion(existente.Nomina.oPercepciones.oSeparacionIndemnizacion, oData.Nomina.oPercepciones.oSeparacionIndemnizacion, sMes);


                                                                if (oData.Nomina.oPercepciones.oJubilacionPensionRetiro != null)
                                                                    existente.Nomina.oPercepciones.oJubilacionPensionRetiro = AcumularJubilacionPensionRetiro(existente.Nomina.oPercepciones.oJubilacionPensionRetiro, oData.Nomina.oPercepciones.oJubilacionPensionRetiro, sMes);
                                                            }

                                                            if (oData.Nomina.oDeducciones != null)
                                                                existente.Nomina.oDeducciones = AcumularDeducciones(existente.Nomina.oDeducciones, oData.Nomina.oDeducciones);

                                                            if (oData.Nomina.oOtrosPagos != null)
                                                            {
                                                                existente.Nomina.oOtrosPagos = AcumularOtrosPagos(existente.Nomina.oOtrosPagos, oData.Nomina.oOtrosPagos);
                                                                if (oData.Nomina.oOtrosPagos.oCompensacionSaldoAFavor != null)
                                                                    existente.Nomina.oOtrosPagos.oCompensacionSaldoAFavor = AcumularCompensacionSaldoAFavor(existente.Nomina.oOtrosPagos.oCompensacionSaldoAFavor, oData.Nomina.oOtrosPagos.oCompensacionSaldoAFavor, sMes);

                                                                if (oData.Nomina.oOtrosPagos.oSubsidioAlEmpleo != null)
                                                                    existente.Nomina.oOtrosPagos.oSubsidioAlEmpleo = AcumularSubsidioAlEmpleo(existente.Nomina.oOtrosPagos.oSubsidioAlEmpleo, oData.Nomina.oOtrosPagos.oSubsidioAlEmpleo, sMes);

                                                            }

                                                            if (oData.Nomina.oIncapacidades != null)
                                                                existente.Nomina.oIncapacidades = AcumularIncapacidades(existente.Nomina.oIncapacidades, oData.Nomina.oIncapacidades);

                                                        }
                                                    }
                                                }

                                            }

                                        }
                                        var keys = dicNominas.Keys.ToList();
                                        foreach (var key in keys)
                                        {
                                            if (dicNominas.TryGetValue(key, out var item))
                                            {
                                                await VerificarRegistro(item, sMes, sUUID);
                                                dicNominas.Remove(key);
                                            }
                                        }
                                    }
                                }
                                else
                                    Console.WriteLine("No se puedo verificar la version del CFDI." + sUUID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }


        #region Engranajes

        private mdl_NominaMBDB AcumularNomina(mdl_NominaMBDB oExistente, mdl_NominaMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                oExistente = oExistente ?? new mdl_NominaMBDB();

                oExistente.fTotalPercepciones00 += oNueva.fTotalPercepciones00;
                oExistente.fTotalDeducciones00 += oNueva.fTotalDeducciones00;
                oExistente.fTotalOtrosPagos00 += oNueva.fTotalOtrosPagos00;



                string propTotalPercepciones = $"fTotalPercepciones{sMes:D2}";
                string propTotalDeducciones = $"fTotalDeducciones{sMes:D2}";
                string propTotalOtrosPagos = $"fTotalOtrosPagos{sMes:D2}";


                var typeNom = typeof(mdl_NominaMBDB);

                var propertyTotalPercepciones = typeNom.GetProperty(propTotalPercepciones);
                var propertyTotalDeducciones = typeNom.GetProperty(propTotalDeducciones);
                var propertyTotalOtrosPagos = typeNom.GetProperty(propTotalOtrosPagos);



                if (propertyTotalPercepciones != null)
                {
                    var TotalPercepcionesNew = Convert.ToDecimal(propertyTotalPercepciones.GetValue(oNueva));
                    var TotalPercepcionesExist = Convert.ToDecimal(propertyTotalPercepciones.GetValue(oExistente));

                    var fTotalPercepciones = TotalPercepcionesNew += TotalPercepcionesExist;
                    propertyTotalPercepciones.SetValue(oExistente, fTotalPercepciones);
                }

                if (propertyTotalDeducciones != null)
                {
                    var TotalDeduccionesNew = Convert.ToDecimal(propertyTotalDeducciones.GetValue(oNueva));
                    var TotalDeduccionesExist = Convert.ToDecimal(propertyTotalDeducciones.GetValue(oExistente));

                    var fTotalDeducciones = TotalDeduccionesNew += TotalDeduccionesExist;

                    propertyTotalDeducciones.SetValue(oExistente, fTotalDeducciones);

                }

                if (propertyTotalOtrosPagos != null)
                {
                    var TotalOtrosPagosNew = Convert.ToDecimal(propertyTotalOtrosPagos.GetValue(oNueva));
                    var TotalOtrosPagosExist = Convert.ToDecimal(propertyTotalOtrosPagos.GetValue(oExistente));

                    var fTotalOtrosPagos = TotalOtrosPagosNew += TotalOtrosPagosExist;

                    propertyTotalOtrosPagos.SetValue(oExistente, fTotalOtrosPagos);

                }
            }
            return oExistente;
        }

        private PercepcionesMBDB AcumularPercepciones(PercepcionesMBDB oExistente, PercepcionesMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {

                oExistente = oExistente ?? new PercepcionesMBDB();
                oExistente.fTotalExento00 += oNueva.fTotalExento00;
                oExistente.fTotalGravado00 += oNueva.fTotalGravado00;
                oExistente.fTotalJubilacionPensionRetiro00 += oNueva.fTotalJubilacionPensionRetiro00;
                oExistente.fTotalSeparacionIndemnizacion00 += oNueva.fTotalSeparacionIndemnizacion00;
                oExistente.fTotalSueldos00 += oNueva.fTotalSueldos00;

                string propTotalExento = $"fTotalExento{sMes:D2}";
                string propTotalGravado = $"fTotalGravado{sMes:D2}";
                string propTotalJubilacionPensionRetiro = $"fTotalJubilacionPensionRetiro{sMes:D2}";
                string propTotalSeparacionIndemnizacion = $"fTotalSeparacionIndemnizacion{sMes:D2}";
                string propTotalSueldos = $"fTotalSueldos{sMes:D2}";

                var typePercep = typeof(PercepcionesMBDB);
                var propertyTotalExento = typePercep.GetProperty(propTotalExento);
                var propertyTotalGravado = typePercep.GetProperty(propTotalGravado);
                var propertyTotalJPR = typePercep.GetProperty(propTotalJubilacionPensionRetiro);
                var propertyTotalSI = typePercep.GetProperty(propTotalSeparacionIndemnizacion);
                var propertyTotalSueldos = typePercep.GetProperty(propTotalSueldos);

                if (propertyTotalExento != null)
                {

                    var TotalExentoNew = Convert.ToDecimal(propertyTotalExento.GetValue(oNueva));
                    var TotalExentoExist = Convert.ToDecimal(propertyTotalExento.GetValue(oExistente));

                    var fTotalExento = TotalExentoNew += TotalExentoExist;
                    propertyTotalExento.SetValue(oExistente, fTotalExento);
                }

                if (propertyTotalGravado != null)
                {
                    var TotalGravadoNew = Convert.ToDecimal(propertyTotalGravado.GetValue(oNueva));
                    var TotalGravadoExist = Convert.ToDecimal(propertyTotalGravado.GetValue(oExistente));

                    var fTotalGravado = TotalGravadoNew += TotalGravadoExist;
                    propertyTotalGravado.SetValue(oExistente, fTotalGravado);
                }

                if (propertyTotalJPR != null)
                {
                    var TotalJPRNew = Convert.ToDecimal(propertyTotalJPR.GetValue(oNueva));
                    var TotalJPRExist = Convert.ToDecimal(propertyTotalJPR.GetValue(oExistente));
                    var fTotalJPR = TotalJPRNew += TotalJPRExist;
                    propertyTotalJPR.SetValue(oExistente, fTotalJPR);

                }

                if (propertyTotalSI != null)
                {
                    var TotalSINew = Convert.ToDecimal(propertyTotalSI.GetValue(oNueva));
                    var TotalSIExist = Convert.ToDecimal(propertyTotalSI.GetValue(oExistente));
                    var fTotalSI = TotalSINew += TotalSIExist;

                    propertyTotalSI.SetValue(oExistente, fTotalSI);

                }

                if (propertyTotalSueldos != null)
                {
                    var TotalSueldosNew = Convert.ToDecimal(propertyTotalSueldos.GetValue(oNueva));
                    var TotalSueldosExist = Convert.ToDecimal(propertyTotalSueldos.GetValue(oExistente));
                    var fTotalSueldos = TotalSueldosNew += TotalSueldosExist;
                    propertyTotalSueldos.SetValue(oExistente, fTotalSueldos);

                }


                if (oNueva.lsPercepcion != null && oNueva.lsPercepcion.Count > 0)
                    oExistente.lsPercepcion.AddRange(oNueva.lsPercepcion);

            }
            return oExistente;
        }

        private AccionesOTitulosMBDB AcumularAccionesOTitulos(AccionesOTitulosMBDB oExistente, AccionesOTitulosMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                var accionesOTitulosNew = oNueva;
                if (oExistente != null)
                {
                    var accionesOTitulosExist = oExistente;

                    oExistente.fValorMercado00 = oNueva.fValorMercado00;
                    oExistente.fPrecioAlOtorgarse00 = oNueva.fPrecioAlOtorgarse00;


                    string propValorMercado = $"fValorMercado{sMes:D2}";
                    string propPrecioAlOtorgarse = $"fPrecioAlOtorgarse{sMes:D2}";


                    var typeAOT = typeof(AccionesOTitulosMBDB);
                    var propertyValorMercado = typeAOT.GetProperty(propValorMercado);
                    var propertyPrecioAlOtorgarse = typeAOT.GetProperty(propPrecioAlOtorgarse);


                    if (propertyValorMercado != null)
                    {
                        var VaorMercadoNew = Convert.ToDecimal(propertyValorMercado.GetValue(oNueva));
                        var ValorMercadoExist = Convert.ToDecimal(propertyValorMercado.GetValue(oExistente));
                        var fValorMercado = VaorMercadoNew += ValorMercadoExist;

                        propertyValorMercado.SetValue(oExistente, fValorMercado);

                    }

                    if (propertyPrecioAlOtorgarse != null)
                    {

                        var PAONew = Convert.ToDecimal(propertyPrecioAlOtorgarse.GetValue(oNueva));
                        var PAOExist = Convert.ToDecimal(propertyPrecioAlOtorgarse.GetValue(oExistente));
                        var fPAO = PAONew += PAOExist;


                        propertyPrecioAlOtorgarse.SetValue(oExistente, fPAO);

                    }
                }
                else
                    oExistente = oNueva;
            }
            return oExistente;
        }

        private HorasExtraMBDB AcumularHorasExtra(HorasExtraMBDB oExistente, HorasExtraMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                if (oExistente != null)
                {
                    oExistente.iDias00 += oNueva.iDias00;
                    oExistente.iHorasExtra00 += oNueva.iHorasExtra00;
                    oExistente.fImportePagado00 += oNueva.fImportePagado00;


                    string propDias = $"iDias{sMes:D2}";
                    string propHorasExtra = $"iHorasExtra{sMes:D2}";
                    string propImportePagado = $"fImportePagado{sMes:D2}";


                    var typeHorasExtra = typeof(HorasExtraMBDB);
                    var propertyDias = typeHorasExtra.GetProperty(propDias);
                    var propertyHorasExtra = typeHorasExtra.GetProperty(propHorasExtra);
                    var propertyImportePagado = typeHorasExtra.GetProperty(propImportePagado);


                    if (propertyDias != null)
                    {
                        var DiasNew = Convert.ToDecimal(propertyDias.GetValue(oNueva));
                        var DiasExist = Convert.ToDecimal(propertyDias.GetValue(oExistente));
                        var iDias = DiasNew += DiasExist;

                        propertyDias.SetValue(oExistente, iDias);
                    }

                    if (propertyHorasExtra != null)
                    {
                        var HorasExtraNew = Convert.ToDecimal(propertyHorasExtra.GetValue(oNueva));
                        var HorasExtraExist = Convert.ToDecimal(propertyHorasExtra.GetValue(oExistente));
                        var fHorasExtra = HorasExtraNew += HorasExtraExist;

                        propertyDias.SetValue(oExistente, fHorasExtra);
                    }

                    if (propertyImportePagado != null)
                    {
                        var ImportePagadoNew = Convert.ToDecimal(propertyImportePagado.GetValue(oNueva));
                        var ImportePagadoExist = Convert.ToDecimal(propertyImportePagado.GetValue(oExistente));
                        var fImportePagado = ImportePagadoNew += ImportePagadoExist;

                        propertyDias.SetValue(oExistente, fImportePagado);
                    }


                }
                else
                    oExistente = oNueva;
            }
            return oExistente;
        }

        private JubilacionPensionRetiroMBDB AcumularJubilacionPensionRetiro(JubilacionPensionRetiroMBDB oExistente, JubilacionPensionRetiroMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                if (oExistente != null)
                {
                    oExistente.fTotalUnaExhibicion00 = oNueva.fTotalUnaExhibicion00;
                    oExistente.fTotalParcialidad00 = oNueva.fTotalParcialidad00;
                    oExistente.fMontoDiario00 = oNueva.fMontoDiario00;
                    oExistente.fIngresoAcumulable00 = oNueva.fIngresoAcumulable00;
                    oExistente.fIngresoNoAcumulable00 = oNueva.fIngresoNoAcumulable00;


                    string propTotalUnaExhibicion = $"fTotalUnaExhibicion{sMes:D2}";
                    string propTotalParcialidad = $"fTotalParcialidad{sMes:D2}";
                    string propMontoDiario = $"fMontoDiario{sMes:D2}";
                    string propIngresoAcumulableJPR = $"fIngresoAcumulable{sMes:D2}";
                    string propIngresoNoAcumulableJPR = $"fIngresoNoAcumulable{sMes:D2}";

                    var typeJPR = typeof(JubilacionPensionRetiroMBDB);
                    var propertyTotalUnaExhibicion = typeJPR.GetProperty(propTotalUnaExhibicion);
                    var propertyTotalParcialidad = typeJPR.GetProperty(propTotalParcialidad);
                    var propertyMontoDiario = typeJPR.GetProperty(propMontoDiario);
                    var propertyIngresoAcumulableJPR = typeJPR.GetProperty(propIngresoAcumulableJPR);
                    var propertyIngresoNoAcumulableJPR = typeJPR.GetProperty(propIngresoNoAcumulableJPR);

                    if (propertyTotalUnaExhibicion != null)
                    {

                        var TotalUnaExhibicionNew = Convert.ToDecimal(propertyTotalUnaExhibicion.GetValue(oNueva));
                        var TotalUnaExhibiconExist = Convert.ToDecimal(propertyTotalUnaExhibicion.GetValue(oExistente));

                        var fTotalExento = TotalUnaExhibiconExist += TotalUnaExhibicionNew;
                        propertyTotalUnaExhibicion.SetValue(oExistente, fTotalExento);
                    }

                    if (propertyTotalParcialidad != null)
                    {
                        var TotalGravadoNew = Convert.ToDecimal(propertyTotalParcialidad.GetValue(oNueva));
                        var TotalGravadoExist = Convert.ToDecimal(propertyTotalParcialidad.GetValue(oExistente));

                        var fTotalGravado = TotalGravadoNew += TotalGravadoExist;
                        propertyTotalParcialidad.SetValue(oExistente, fTotalGravado);
                    }

                    if (propertyMontoDiario != null)
                    {
                        var TotalMontoDiarioJPRNew = Convert.ToDecimal(propertyMontoDiario.GetValue(oNueva));
                        var TotalMontoDiarioJPRExist = Convert.ToDecimal(propertyMontoDiario.GetValue(oExistente));
                        var fTotalMontoDiarioJPR = TotalMontoDiarioJPRNew += TotalMontoDiarioJPRExist;
                        propertyMontoDiario.SetValue(oExistente, fTotalMontoDiarioJPR);

                    }

                    if (propertyIngresoAcumulableJPR != null)
                    {
                        var TotalIngresoAcumulableJPRNew = Convert.ToDecimal(propertyIngresoAcumulableJPR.GetValue(oNueva));
                        var TotalIngresoAcumulableJPRExist = Convert.ToDecimal(propertyIngresoAcumulableJPR.GetValue(oExistente));
                        var fTotalIngresoAcumulableJPR = TotalIngresoAcumulableJPRNew += TotalIngresoAcumulableJPRExist;


                        propertyIngresoAcumulableJPR.SetValue(oExistente, fTotalIngresoAcumulableJPR);

                    }

                    if (propertyIngresoNoAcumulableJPR != null)
                    {
                        var TotalIngresoNoAcumulableJPRNew = Convert.ToDecimal(propertyIngresoNoAcumulableJPR.GetValue(oNueva));
                        var TotalIngresoNoAcumulableJPRExist = Convert.ToDecimal(propertyIngresoNoAcumulableJPR.GetValue(oExistente));
                        var fTotalIngresoNoAcumulableJPR = TotalIngresoNoAcumulableJPRNew += TotalIngresoNoAcumulableJPRExist;


                        propertyIngresoNoAcumulableJPR.SetValue(oExistente, fTotalIngresoNoAcumulableJPR);

                    }
                }
                else
                    oExistente = oNueva;
            }
            return oExistente;
        }

        private SeparacionIndemnizacionMBDB AcumularSeparacionIndemnizacion(SeparacionIndemnizacionMBDB oExistente, SeparacionIndemnizacionMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                if (oExistente != null)
                {
                    oExistente.fTotalPagado00 = oNueva.fTotalPagado00;
                    oExistente.iNumAniosServicio00 = oNueva.iNumAniosServicio00;
                    oExistente.fUltimoSueldoMensOrd00 = oNueva.fUltimoSueldoMensOrd00;
                    oExistente.fIngresoAcumulable00 = oNueva.fIngresoAcumulable00;
                    oExistente.fIngresoNoAcumulable00 = oNueva.fIngresoNoAcumulable00;

                    string propTotalPagado = $"fTotalPagado{sMes:D2}";
                    string propNumAniosServicio = $"iNumAniosServicio{sMes:D2}";
                    string propUltimoSueldoMensOrd = $"fUltimoSueldoMensOrd{sMes:D2}";
                    string propIngresoAcumulableSI = $"fIngresoAcumulable{sMes:D2}";
                    string propIngresoNoAcumulableSI = $"fIngresoNoAcumulable{sMes:D2}";

                    var typeSI = typeof(SeparacionIndemnizacionMBDB);
                    var propertyTotalPagado = typeSI.GetProperty(propTotalPagado);
                    var propertyNumAniosServicio = typeSI.GetProperty(propNumAniosServicio);
                    var propertyUltimoSueldoMensOrd = typeSI.GetProperty(propUltimoSueldoMensOrd);
                    var propertyIngresoAcumulableSI = typeSI.GetProperty(propIngresoAcumulableSI);
                    var propertyIngresoNoAcumulableSI = typeSI.GetProperty(propIngresoNoAcumulableSI);

                    if (propertyTotalPagado != null)
                    {

                        var TotalExentoNew = Convert.ToDecimal(propertyTotalPagado.GetValue(oNueva));
                        var TotalExentoExist = Convert.ToDecimal(propertyTotalPagado.GetValue(oExistente));

                        var fTotalExento = TotalExentoNew += TotalExentoExist;
                        propertyTotalPagado.SetValue(oExistente, fTotalExento);
                    }

                    if (propertyNumAniosServicio != null)
                    {
                        var NumAniosServicioNew = Convert.ToDecimal(propertyNumAniosServicio.GetValue(oNueva));
                        var NumAniosServicioExist = Convert.ToDecimal(propertyNumAniosServicio.GetValue(oExistente));

                        var iNumAniosServicio = NumAniosServicioNew += NumAniosServicioExist;
                        propertyNumAniosServicio.SetValue(oExistente, iNumAniosServicio);
                    }

                    if (propertyUltimoSueldoMensOrd != null)
                    {
                        var TotalUltimosSueldosMensOrdNew = Convert.ToDecimal(propertyUltimoSueldoMensOrd.GetValue(oNueva));
                        var TotalUltimosSueldosMensOrdExist = Convert.ToDecimal(propertyUltimoSueldoMensOrd.GetValue(oExistente));
                        var fTotalUltimoSueldoMesOrd = TotalUltimosSueldosMensOrdNew += TotalUltimosSueldosMensOrdExist;
                        propertyUltimoSueldoMensOrd.SetValue(oExistente, fTotalUltimoSueldoMesOrd);

                    }

                    if (propertyIngresoAcumulableSI != null)
                    {
                        var TotalIngresoAcumulableNew = Convert.ToDecimal(propertyIngresoAcumulableSI.GetValue(oNueva));
                        var TotalIngresoAcumulableExist = Convert.ToDecimal(propertyIngresoAcumulableSI.GetValue(oExistente));
                        var fIngresoAcumulableSI = TotalIngresoAcumulableNew += TotalIngresoAcumulableExist;

                        propertyIngresoAcumulableSI.SetValue(oExistente, fIngresoAcumulableSI);

                    }

                    if (propertyIngresoNoAcumulableSI != null)
                    {
                        var TotalIngresoNoAcumulableNew = Convert.ToDecimal(propertyIngresoNoAcumulableSI.GetValue(oNueva));
                        var TotalIngresoNoAcumulableExist = Convert.ToDecimal(propertyIngresoNoAcumulableSI.GetValue(oExistente));
                        var fTotalIngresoNoAcumulableSI = TotalIngresoNoAcumulableNew += TotalIngresoNoAcumulableExist;
                        propertyIngresoNoAcumulableSI.SetValue(oExistente, fTotalIngresoNoAcumulableSI);

                    }


                }
                else
                    oExistente = oNueva;
            }
            return oExistente;
        }

        private DeduccionesMBDB AcumularDeducciones(DeduccionesMBDB oExistente, DeduccionesMBDB oNueva)
        {
            if (oNueva != null)
            {
                oExistente = oExistente ?? new DeduccionesMBDB();
                oExistente.TotalOtrasDeducciones00 += oNueva.TotalOtrasDeducciones00;
                oExistente.TotalImpuestosRetenidos00 += oNueva.TotalImpuestosRetenidos00;

                if (oNueva.lsDeduccion != null && oNueva.lsDeduccion.Count > 0)
                    oExistente.lsDeduccion.AddRange(oNueva.lsDeduccion);
            }
            return oExistente;
        }

        private OtrosPagosMBDB AcumularOtrosPagos(OtrosPagosMBDB oExistente, OtrosPagosMBDB oNueva)
        {
            if (oNueva != null)
            {
                oExistente = oExistente ?? new OtrosPagosMBDB();
                oExistente.fImporteTotal00 += oNueva.fImporteTotal00;
                oExistente.fSubsidioCausadoTotal00 += oNueva.fSubsidioCausadoTotal00;

                if (oNueva.lsOtroPago != null && oNueva.lsOtroPago.Count > 0)
                    oExistente.lsOtroPago.AddRange(oNueva.lsOtroPago);

                if (oNueva.oCompensacionSaldoAFavor != null)
                {
                    if (oExistente.oCompensacionSaldoAFavor != null)
                    {
                        oExistente.oCompensacionSaldoAFavor.fSaldoAFavor00 += oNueva.oCompensacionSaldoAFavor.fSaldoAFavor00;
                        oExistente.oCompensacionSaldoAFavor.fRemanenteSalFav00 += oNueva.oCompensacionSaldoAFavor.fRemanenteSalFav00;

                    }
                    else
                        oExistente.oCompensacionSaldoAFavor = oNueva.oCompensacionSaldoAFavor;
                }


            }
            return oExistente;
        }

        private SubsidioAlEmpleoMBDB AcumularSubsidioAlEmpleo(SubsidioAlEmpleoMBDB oExistente, SubsidioAlEmpleoMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                if (oExistente != null)
                {
                    oExistente.fSubsidioCausado00 = oNueva.fSubsidioCausado00;

                    string propSubsidioCausado = $"fSubsidioCausado{sMes}";

                    var typeSAE = typeof(SubsidioAlEmpleoMBDB);
                    var propertySubsidioCausado = typeSAE.GetProperty(propSubsidioCausado);

                    if (propertySubsidioCausado != null)
                    {

                        var SubsidioCausado = Convert.ToDecimal(propertySubsidioCausado.GetValue(oNueva));
                        var SubsidioCausadoExist = Convert.ToDecimal(propertySubsidioCausado.GetValue(oExistente));

                        var fSubsidioCausado = SubsidioCausado += SubsidioCausadoExist;
                        propertySubsidioCausado.SetValue(oExistente, fSubsidioCausado);
                    }

                }
                else
                    oExistente = oNueva;
            }
            return oExistente;
        }

        private CompensacionSaldoAFavorMBDB AcumularCompensacionSaldoAFavor(CompensacionSaldoAFavorMBDB oExistente, CompensacionSaldoAFavorMBDB oNueva, string sMes)
        {
            if (oNueva != null)
            {
                if (oExistente != null)
                {
                    oExistente.fRemanenteSalFav00 = oNueva.fRemanenteSalFav00;
                    oExistente.fSaldoAFavor00 = oNueva.fSaldoAFavor00;


                    string propRemanenteSalFav = $"fRemanenteSalFav{sMes}";
                    string propSaldoAFavor = $"fSaldoAFavor{sMes}";

                    var typeCSAF = typeof(CompensacionSaldoAFavorMBDB);
                    var propertyRemanenteSalFav = typeCSAF.GetProperty(propRemanenteSalFav);
                    var propertySaldoFavor = typeCSAF.GetProperty(propSaldoAFavor);

                    if (propertyRemanenteSalFav != null)
                    {
                        var fRemanenteSaldoAFavNew = Convert.ToDecimal(propertyRemanenteSalFav.GetValue(oNueva));
                        var fRemanenteSaldoAFavExist = Convert.ToDecimal(propertyRemanenteSalFav.GetValue(oExistente));

                        var fRemanenteSalFav = fRemanenteSaldoAFavNew += fRemanenteSaldoAFavExist;
                        propertyRemanenteSalFav.SetValue(oExistente, fRemanenteSalFav);
                    }

                    if (propertySaldoFavor != null)
                    {
                        var fSaldoAFavorNew = Convert.ToDecimal(propertySaldoFavor.GetValue(oNueva));
                        var fSaldoAFavorExist = Convert.ToDecimal(propertySaldoFavor.GetValue(oExistente));

                        var fSaldoAFavor = fSaldoAFavorNew += fSaldoAFavorExist;
                        propertyRemanenteSalFav.SetValue(oExistente, fSaldoAFavor);
                    }


                }
                else
                    oExistente = oNueva;
            }
            return oExistente;
        }

        private IncapacidadesMBDB AcumularIncapacidades(IncapacidadesMBDB oExistente, IncapacidadesMBDB oNueva)
        {
            if (oNueva != null)
            {
                oExistente = oExistente ?? new IncapacidadesMBDB();
                oExistente.fImporteTotal00 += oNueva.fImporteTotal00;
                oExistente.iDiasIncapacidadTotal00 += oNueva.iDiasIncapacidadTotal00;

                if (oNueva.lsIncapacidad != null && oNueva.lsIncapacidad.Count > 0)
                    oExistente.lsIncapacidad.AddRange(oNueva.lsIncapacidad);

            }
            return oExistente;
        }


        #endregion

        #region Procesamiento del XML
        private async Task<mdl_NAcumulados> ObtenerNomina33(XmlDocument xmlDocument, dll_nom12a.Nomina oNomina12A = null, dll_nom12b.Nomina oNomina12B = null, string sMes = null)
        {
            string sUUID = string.Empty;
            cls_NodesRecolector oRecolector = new cls_NodesRecolector();
            mdl_NAcumulados oData = new mdl_NAcumulados();
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
                XmlNodeList oListaTimbreFiscalDigital = ((XmlElement)xmlComplemento[0]).GetElementsByTagName("tfd:TimbreFiscalDigital");
                #endregion




                if (xmlComprobante.Count > 0)
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
                        oData.sRFC = element.GetAttribute("Rfc") == "" ? "-" : element.GetAttribute("Rfc");
                        oData.sNombre = element.GetAttribute("Nombre") == "" ? "-" : element.GetAttribute("Nombre");

                    }
                    #endregion


                    #region Nodo: TimbreFiscalDigital
                    if (oListaTimbreFiscalDigital.Count > 0)
                    {
                        foreach (XmlElement oNodo in oListaTimbreFiscalDigital)
                        {
                            sUUID = oNodo.GetAttribute("UUID") == null ? "-" : oNodo.GetAttribute("UUID");
                        }
                    }
                    #endregion

                    /*oNomina12B*/
                    if (oNomina12B != null && oNomina12A == null)
                    {

                        if (oNomina12B.Receptor != null)
                        {
                            oData.sIMSS = oNomina12B.Receptor.NumSeguridadSocial == "" ? "-" : oNomina12B.Receptor.NumSeguridadSocial;

                            oData.sNumero = oNomina12B.Receptor.NumEmpleado == "" ? string.Empty : oNomina12B.Receptor.NumEmpleado;
                        }

                        if (oNomina12B.Emisor != null)
                        {
                            oData.sRegPat = oNomina12B.Emisor.RegistroPatronal == null ? "-" : oNomina12B.Emisor.RegistroPatronal;
                        }

                        // Inicialización segura
                        #region Tareas
                        Task<PercepcionesMBDB> taskPercepciones = Task.FromResult<PercepcionesMBDB>(null);
                        Task<DeduccionesMBDB> taskDeducciones = Task.FromResult<DeduccionesMBDB>(null);
                        Task<OtrosPagosMBDB> taskOtrosPagos = Task.FromResult<OtrosPagosMBDB>(null);
                        Task<IncapacidadesMBDB> taskIncapacidades = Task.FromResult<IncapacidadesMBDB>(null);
                        #endregion

                        /*Nómina*/
                        var taskNomina = oRecolector.ObtenerData12B(oNomina12B, sMes, fNetoTotal);
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
                        oData.Nomina.oPercepciones = percepciones;
                        oData.Nomina.oDeducciones = deducciones;
                        oData.Nomina.oOtrosPagos = otrosPagos;
                        oData.Nomina.oIncapacidades = incapacidades;
                        //oData.Nomina.fNetoTotal = fNetoTotal;

                    }

                    /*NOMINA12A*/
                    else if (oNomina12A != null && oNomina12B == null)
                    {
                        if (oNomina12A.Receptor != null)
                        {
                            oData.sIMSS = oNomina12A.Receptor.NumSeguridadSocial == "" ? "-" : oNomina12A.Receptor.NumSeguridadSocial;

                            oData.sNumero = oNomina12A.Receptor.NumEmpleado == "" ? string.Empty : oNomina12A.Receptor.NumEmpleado;
                        }

                        if (oNomina12A.Emisor != null)
                        {
                            oData.sRegPat = oNomina12A.Emisor.RegistroPatronal == null ? "-" : oNomina12A.Emisor.RegistroPatronal;
                        }

                        // Inicialización segura
                        #region Tareas
                        Task<PercepcionesMBDB> taskPercepciones = Task.FromResult<PercepcionesMBDB>(null);
                        Task<DeduccionesMBDB> taskDeducciones = Task.FromResult<DeduccionesMBDB>(null);
                        Task<OtrosPagosMBDB> taskOtrosPagos = Task.FromResult<OtrosPagosMBDB>(null);
                        Task<IncapacidadesMBDB> taskIncapacidades = Task.FromResult<IncapacidadesMBDB>(null);
                        #endregion

                        /*Nómina*/
                        var taskNomina = oRecolector.ObtenerData12A(oNomina12A, sMes, fNetoTotal);
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
                        oData.Nomina.oPercepciones = percepciones;
                        oData.Nomina.oDeducciones = deducciones;
                        oData.Nomina.oOtrosPagos = otrosPagos;
                        oData.Nomina.oIncapacidades = incapacidades;
                        //oData.Nomina.fNetoTotal = fNetoTotal;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return oData;
        }


        private async Task<mdl_NAcumulados> ObtenerNomina40(XmlDocument xmlDocument, dll_nom12c.Nomina oNomina, string sMes)
        {
            #region Modelos
            cls_NodesRecolector oRecolector = new cls_NodesRecolector();
            mdl_NAcumulados oData = new mdl_NAcumulados();
            XmlNodeList xmlReceptor = xmlDocument.GetElementsByTagName("cfdi:Receptor");
            XmlNodeList xmlComprobante = xmlDocument.GetElementsByTagName("cfdi:Comprobante");
            var tasks = new List<Task>();
            StringBuilder sb = new StringBuilder();
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
                    oData.sRFC = element.GetAttribute("Rfc") == "" ? "-" : element.GetAttribute("Rfc");
                    oData.sNombre = element.GetAttribute("Nombre") == "" ? "-" : element.GetAttribute("Nombre");

                }
                #endregion

                if (oNomina != null)
                {
                    if (oNomina.Receptor != null)
                    {
                        oData.sIMSS = oNomina.Receptor.NumSeguridadSocial == "" ? "-" : oNomina.Receptor.NumSeguridadSocial;

                        oData.sNumero = oNomina.Receptor.NumEmpleado == "" ? string.Empty : oNomina.Receptor.NumEmpleado;
                    }

                    if (oNomina.Emisor != null)
                    {
                        oData.sRegPat = oNomina.Emisor.RegistroPatronal == null ? "-" : oNomina.Emisor.RegistroPatronal;
                    }

                    // Inicialización segura
                    #region Tareas
                    Task<PercepcionesMBDB> taskPercepciones = Task.FromResult<PercepcionesMBDB>(null);
                    Task<DeduccionesMBDB> taskDeducciones = Task.FromResult<DeduccionesMBDB>(null);
                    Task<OtrosPagosMBDB> taskOtrosPagos = Task.FromResult<OtrosPagosMBDB>(null);
                    Task<IncapacidadesMBDB> taskIncapacidades = Task.FromResult<IncapacidadesMBDB>(null);
                    #endregion

                    /*Nómina*/
                    var taskNomina = oRecolector.ObtenerDataNomina(oNomina, sMes, fNetoTotal);
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
                    oData.Nomina.oPercepciones = percepciones;
                    oData.Nomina.oDeducciones = deducciones;
                    oData.Nomina.oOtrosPagos = otrosPagos;
                    oData.Nomina.oIncapacidades = incapacidades;
                    //oData.Nomina.fNetoTotal = fNetoTotal;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return oData;


        }

        #endregion

        #region MongoDB
        private async Task VerificarRegistro(mdl_NAcumulados oData, string sMes, string sUUID)
        {
            try
            {
                var filterRFC = Builders<mdl_NAcumulados>.Filter.Eq(r => r.sRFC, oData.sRFC);
                var resultRFC = await _NAcumuladosCollection.Find(filterRFC).FirstOrDefaultAsync();

                if (resultRFC != null)
                {
                    // Si el UUID ya existe en la lista, no hace nada
                    if (resultRFC.lsUUID.Contains(sUUID))
                        return;

                    // Si el RFC existe pero el UUID no, actualiza agregando el UUID
                    var update = Builders<mdl_NAcumulados>.Update.Push("UUIDs", sUUID);
                    await _NAcumuladosCollection.UpdateOneAsync(filterRFC, update);

                    if (oData.Nomina != null)
                        await ActualizarNomina(oData.Nomina, sMes, filterRFC, resultRFC);
                }
                else
                {
                    // Si no existe ni el RFC ni el UUID, inserta un nuevo registro
                    await _NAcumuladosCollection.InsertOneAsync(oData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task ActualizarNomina(mdl_NominaMBDB oNomina, string sMes, FilterDefinition<mdl_NAcumulados> filterRFC, mdl_NAcumulados result)
        {
            //var tasks = new List<Task>();

            var updateDefinitionInc = new List<UpdateDefinition<mdl_NAcumulados>>();
            var updateDefinitionSet = new List<UpdateDefinition<mdl_NAcumulados>>();
            UpdateDefinition<mdl_NAcumulados> combinenedUpdateInc = null;
            UpdateDefinition<mdl_NAcumulados> combinenedUpdateSet = null;


            var arrayFilters = new List<ArrayFilterDefinition>();
            //var updateDefinitions = new List<UpdateDefinition<mdl_NAcumulados>>();

            try
            {
                if (oNomina != null)
                {
                    var updateNomina = Builders<mdl_NAcumulados>.Update
                                  .Inc("Nomina.TotalPercepciones00", (double)oNomina.fTotalPercepciones00)
                                  .Inc("Nomina.TotalDeducciones00", (double)oNomina.fTotalDeducciones00)
                                  .Inc("Nomina.TotalOtrosPagos00", (double)oNomina.fTotalOtrosPagos00)
                                  .Inc("Nomina.NetoTotal00", (double)oNomina.fNetoTotal00)
                                  .Inc($"Nomina.TotalPercepciones{sMes:D2}", (double)oNomina.fTotalPercepciones00)
                                  .Inc($"Nomina.TotalDeducciones{sMes:D2}", (double)oNomina.fTotalDeducciones00)
                                  .Inc($"Nomina.TotalOtrosPagos{sMes:D2}", (double)oNomina.fTotalOtrosPagos00)
                                  .Inc($"Nomina.NetoTotal{sMes:D2}", (double)oNomina.fNetoTotal00);
                    updateDefinitionInc.Add(updateNomina);


                    #region Tipos/Claves

                    var lsClavesPercepcion = result.Nomina.oPercepciones?.lsPercepcion
                        .Where(p => p != null)
                        .Select(p => p.sClave)
                        .ToList() ?? new List<string>();

                    var lsClavesDeduccion = result.Nomina.oDeducciones?.lsDeduccion != null
                        ? result.Nomina.oDeducciones.lsDeduccion
                        .Where(d => d != null)
                        .Select(d => d.sClave)
                        .ToList() : new List<string>();

                    var lsClavesOtrosPagos = result.Nomina.oOtrosPagos?.lsOtroPago != null
                        ? result.Nomina.oOtrosPagos.lsOtroPago
                        .Where(o => o != null)
                        .Select(o => o.sClave)
                        .ToList() : new List<string>();

                    var lsTipoIncapacidades = result.Nomina.oIncapacidades?.lsIncapacidad != null
                        ? result.Nomina.oIncapacidades.lsIncapacidad
                        .Where(i => i != null)
                        .Select(i => i.sTipoIncapacidad)
                        .ToList() : new List<string>();

                    #endregion

                    #region Percepciones
                    if (oNomina.oPercepciones != null)
                    {
                        if (result.Nomina.oPercepciones == null)
                        {
                            var updatePerception = Builders<mdl_NAcumulados>.Update.Set(
                                "Nomina.Percepciones", new PercepcionesMBDB
                                {
                                    fTotalExento00 = 0.00m,
                                    fTotalGravado00 = 0.00m,
                                    fTotalJubilacionPensionRetiro00 = 0.00m,
                                    fTotalSeparacionIndemnizacion00 = 0.00m,
                                    fTotalSueldos00 = 0,
                                    lsPercepcion = new List<PercepcionMBDB>(),
                                    oAccionesOTitulos = new AccionesOTitulosMBDB(),
                                    oHorasExtra = new HorasExtraMBDB(),
                                    oJubilacionPensionRetiro = new JubilacionPensionRetiroMBDB(),
                                    oSeparacionIndemnizacion = new SeparacionIndemnizacionMBDB()
                                });
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, updatePerception);



                        }

                        var (updatePercepcionInc, updatePercepcionSet, arrayPercepcionesFilter, nuevosElementosP) =
                            ActualizarPercepciones(oNomina.oPercepciones, lsClavesPercepcion, filterRFC, sMes, result);

                        if (updatePercepcionSet.Any())
                        {
                            updateDefinitionSet.AddRange(updatePercepcionSet);
                            /*mmmm interesant*/
                            combinenedUpdateSet = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionSet);

                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateSet);

                            updateDefinitionSet.Clear();
                            combinenedUpdateSet = null;
                        }


                        if (updatePercepcionInc.Any())
                        {
                            var updateOptions = new UpdateOptions { ArrayFilters = arrayPercepcionesFilter, IsUpsert = true };
                            updateDefinitionInc.AddRange(updatePercepcionInc);
                            combinenedUpdateInc = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionInc);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateInc, updateOptions);
                            combinenedUpdateInc = null;
                            updateDefinitionInc.Clear();
                        }



                        if (nuevosElementosP.Any())
                        {
                            var pushUpdate = Builders<mdl_NAcumulados>.Update.PushEach("Nomina.Percepciones.Percepcion", nuevosElementosP);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, pushUpdate);
                        }


                    }

                    #endregion

                    if (oNomina.oDeducciones != null)
                    {
                        if (result.Nomina.oDeducciones == null)
                        {
                            var updateDeduccion = Builders<mdl_NAcumulados>.Update.Set(
                                "Nomina.Deducciones", new DeduccionesMBDB
                                {
                                    TotalImpuestosRetenidos00 = 0.00m,
                                    TotalOtrasDeducciones00 = 0.00m,
                                    lsDeduccion = new List<DeduccionMBDB>()
                                });
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, updateDeduccion);
                        }

                        #region Deducciones
                        var (updateDeduccionnInc, updateDeduccionSet, arrayDeduccionoesFilter, nuevosElementosD) =
                           ActualizarDeducciones(oNomina.oDeducciones, sMes, lsClavesDeduccion, filterRFC);

                        if (updateDeduccionSet.Any())
                        {
                            updateDefinitionSet.AddRange(updateDeduccionSet);
                            /*mmmm interesant*/
                            combinenedUpdateSet = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionSet);

                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateSet);
                            updateDefinitionSet.Clear();
                            combinenedUpdateSet = null;
                        }


                        if (updateDeduccionnInc.Any())
                        {
                            var updateOptions = new UpdateOptions { ArrayFilters = arrayDeduccionoesFilter, IsUpsert = true };
                            updateDefinitionInc.AddRange(updateDeduccionnInc);
                            combinenedUpdateInc = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionInc);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateInc, updateOptions);
                            combinenedUpdateInc = null;
                            updateDefinitionInc.Clear();
                        }



                        if (nuevosElementosD.Any())
                        {
                            var pushUpdate = Builders<mdl_NAcumulados>.Update.PushEach("Nomina.Deducciones.Deduccion", nuevosElementosD);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, pushUpdate);
                        }

                        #endregion
                    }

                    if (oNomina.oOtrosPagos != null)
                    {
                        if (result.Nomina.oOtrosPagos == null)
                        {
                            var updateOtrosPagos = Builders<mdl_NAcumulados>.Update.Set(
                                "Nomina.OtrosPagos", new OtrosPagosMBDB
                                {
                                    fImporteTotal00 = 0.00m,
                                    fSubsidioCausadoTotal00 = 0.00m,

                                    lsOtroPago = new List<OtroPagoMBDB>(),
                                    oCompensacionSaldoAFavor = new CompensacionSaldoAFavorMBDB(),
                                    oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB()
                                });
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, updateOtrosPagos);
                        }

                        #region OtrosPagos
                        var (updateOtrosPagosInc, updateOtrosPagosSet, arrayOtrosPagosFilter, nuevosElementosOP) =
                            ActualizarOtrosPagos(oNomina.oOtrosPagos, lsClavesOtrosPagos, sMes, filterRFC);

                        if (updateOtrosPagosSet.Any())
                        {
                            updateDefinitionSet.AddRange(updateOtrosPagosSet);
                            /*mmmm interesant*/
                            combinenedUpdateSet = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionSet);

                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateSet);
                            updateDefinitionSet.Clear();
                            combinenedUpdateSet = null;
                        }


                        if (updateOtrosPagosInc.Any())
                        {
                            var updateOptions = new UpdateOptions { ArrayFilters = arrayOtrosPagosFilter, IsUpsert = true };
                            updateDefinitionInc.AddRange(updateOtrosPagosInc);
                            combinenedUpdateInc = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionInc);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateInc, updateOptions);
                            combinenedUpdateInc = null;
                            updateDefinitionInc.Clear();
                        }



                        if (nuevosElementosOP.Any())
                        {
                            var pushUpdate = Builders<mdl_NAcumulados>.Update.PushEach("Nomina.OtrosPagos.OtroPago", nuevosElementosOP);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, pushUpdate);
                        }



                        #endregion
                    }

                    if (oNomina.oIncapacidades != null)
                    {
                        if (result.Nomina.oIncapacidades == null)
                        {
                            var updateIncapacidades = Builders<mdl_NAcumulados>.Update.Set(
                                "Nomina.Incapacidades", new IncapacidadesMBDB
                                {
                                    fImporteTotal00 = 0.00m,
                                    iDiasIncapacidadTotal00 = 0,
                                    lsIncapacidad = new List<IncapacidadMBDB>()

                                });
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, updateIncapacidades);
                        }
                        #region Incapacidades
                        var (updateIncapacidadesInc, updateIncapacidadesSet, arrayIncapacidadesFilter, nuevosElementosI) =
                            ActualizarIncapacidades(oNomina.oIncapacidades, lsTipoIncapacidades, sMes, filterRFC);

                        if (updateIncapacidadesSet.Any())
                        {
                            updateDefinitionSet.AddRange(updateIncapacidadesSet);
                            /*mmmm interesant*/
                            combinenedUpdateSet = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionSet);

                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateSet);
                            updateDefinitionSet.Clear();
                            combinenedUpdateSet = null;
                        }


                        if (updateIncapacidadesInc.Any())
                        {
                            var updateOptions = new UpdateOptions { ArrayFilters = arrayIncapacidadesFilter, IsUpsert = true };
                            updateDefinitionInc.AddRange(updateIncapacidadesInc);
                            combinenedUpdateInc = Builders<mdl_NAcumulados>.Update.Combine(updateDefinitionInc);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, combinenedUpdateInc, updateOptions);
                            combinenedUpdateInc = null;
                            updateDefinitionInc.Clear();
                        }



                        if (nuevosElementosI.Any())
                        {
                            var pushUpdate = Builders<mdl_NAcumulados>.Update.PushEach("Nomina.Incapacidades.Incapacidad", nuevosElementosI);
                            await _NAcumuladosCollection.UpdateOneAsync(filterRFC, pushUpdate);
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        private (List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionInc, List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionSet, List<ArrayFilterDefinition> arrayFiltersPercepctions, List<PercepcionMBDB> nuevosElementosP)
           ActualizarPercepciones(PercepcionesMBDB oPercepciones, List<string> lsTiposPercepcion, FilterDefinition<mdl_NAcumulados> filterRFC, string sMes, mdl_NAcumulados result)
        {
            var updateDefinitionInc = new List<UpdateDefinition<mdl_NAcumulados>>();
            var updateDefinitionSet = new List<UpdateDefinition<mdl_NAcumulados>>();

            var arrayFiltersPercepctions = new List<ArrayFilterDefinition>();

            var nuevosConceptosP = new List<PercepcionMBDB>();

            var hsTiposProcesados = new HashSet<string>();
            try
            {
                if (oPercepciones == null)
                    return (updateDefinitionInc, updateDefinitionSet, arrayFiltersPercepctions, nuevosConceptosP);

                /*Percepciones*/
                var updatePercepciones = Builders<mdl_NAcumulados>.Update
                    .Inc("Nomina.Percepciones.TotalSueldos00", (double)oPercepciones.fTotalSueldos00)
                    .Inc("Nomina.Percepciones.TotalSeparacionIndemnizacion00", (double)oPercepciones.fTotalSeparacionIndemnizacion00)
                    .Inc("Nomina.Percepciones.TotalJubilacionPensionRetiro00", (double)oPercepciones.fTotalJubilacionPensionRetiro00)
                    .Inc("Nomina.Percepciones.TotalGravado00", (double)oPercepciones.fTotalGravado00)
                    .Inc("Nomina.Percepciones.TotalExento00", (double)oPercepciones.fTotalExento00)
                    .Inc($"Nomina.Percepciones.TotalSueldos{sMes:D2}", (double)oPercepciones.fTotalSueldos00)
                    .Inc($"Nomina.Percepciones.TotalSeparacionIndemnizacion{sMes:D2}", (double)oPercepciones.fTotalSeparacionIndemnizacion00)
                    .Inc($"Nomina.Percepciones.TotalJubilacionPensionRetiro{sMes:D2}", (double)oPercepciones.fTotalJubilacionPensionRetiro00)
                    .Inc($"Nomina.Percepciones.TotalGravado{sMes:D2}", (double)oPercepciones.fTotalGravado00)
                    .Inc($"Nomina.Percepciones.TotalExento{sMes:D2}", (double)oPercepciones.fTotalExento00);
                updateDefinitionInc.Add(updatePercepciones);


                if (oPercepciones.lsPercepcion != null && oPercepciones.lsPercepcion.Count > 0)
                {
                    foreach (var percepcion in oPercepciones.lsPercepcion)
                    {
                        // Si el tipo de percepción ya existe en la base de datos (lsTiposPercepcion), se actualiza
                        if (lsTiposPercepcion.Contains(percepcion.sClave))
                        {
                            // Construimos un identificador único para la consulta y los filtros
                            string sFiltro = $"elemP{percepcion.sClave.Replace("_", "")}";

                            // Si no hemos procesado aún este tipo, agregamos el filtro de array
                            if (!hsTiposProcesados.Contains(percepcion.sClave))
                            {
                                arrayFiltersPercepctions.Add(
                                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                        new BsonDocument(sFiltro + ".Clave", percepcion.sClave)
                                    )
                                );
                                hsTiposProcesados.Add(percepcion.sClave);
                            }


                            // Se añade la actualización para el elemento ya existente (con el filtro)
                            updateDefinitionInc.Add(Builders<mdl_NAcumulados>.Update.Combine(
                                Builders<mdl_NAcumulados>.Update
                                .Set($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Tipo", percepcion.sTipo)
                                .Set($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Concepto", percepcion.sConcepto)
                                .Set($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Clave", percepcion.sClave)
                                .Inc($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Gravado00", (double)percepcion.fGravado00)
                                .Inc($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Exento00", (double)percepcion.fExento00)
                                .Inc($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Gravado{sMes:D2}", (double)percepcion.fGravado00)
                                .Inc($"Nomina.Percepciones.Percepcion.$[{sFiltro}].Exento{sMes:D2}", (double)percepcion.fExento00)
                                ));


                        }
                        else
                        {
                            nuevosConceptosP.Add(percepcion);
                        }

                    }
                }

                if (oPercepciones.oAccionesOTitulos != null)
                {

                    if (result.Nomina.oPercepciones.oAccionesOTitulos == null)
                    {
                        var structureAccionesOTitulos = Builders<mdl_NAcumulados>
                            .Update
                            .Set("Nomina.Percepciones.AccionesOTitulos",
                            new AccionesOTitulosMBDB
                            {
                                fPrecioAlOtorgarse00 = 0.00m,
                                fValorMercado00 = 0.00m,
                                fPrecioAlOtorgarse01 = 0.00m,
                                fValorMercado01 = 0.00m,
                                fPrecioAlOtorgarse02 = 0.00m,
                                fValorMercado02 = 0.00m,
                                fPrecioAlOtorgarse03 = 0.00m,
                                fValorMercado03 = 0.00m,
                                fPrecioAlOtorgarse04 = 0.00m,
                                fValorMercado04 = 0.00m,
                                fPrecioAlOtorgarse05 = 0.00m,
                                fValorMercado05 = 0.00m,
                                fPrecioAlOtorgarse06 = 0.00m,
                                fValorMercado06 = 0.00m,
                                fPrecioAlOtorgarse07 = 0.00m,
                                fValorMercado07 = 0.00m,

                                fPrecioAlOtorgarse08 = 0.00m,
                                fValorMercado08 = 0.00m,
                                fPrecioAlOtorgarse09 = 0.00m,
                                fValorMercado09 = 0.00m,
                                fPrecioAlOtorgarse10 = 0.00m,
                                fValorMercado11 = 0.00m,
                                fPrecioAlOtorgarse11 = 0.00m,
                                fValorMercado12 = 0.00m,
                                fPrecioAlOtorgarse12 = 0.00m
                            });

                        // Aplicar la inicialización
                        _NAcumuladosCollection.UpdateOne(filterRFC, structureAccionesOTitulos);


                        var updateAccionesOTitulos = Builders<mdl_NAcumulados>.Update
                            .Set("Nomina.Percepciones.AccionesOTitulos.ValorMercado00", oPercepciones.oAccionesOTitulos.fValorMercado00)
                            .Set("Nomina.Percepciones.AccionesOTitulos.PrecioOtorgarse00", oPercepciones.oAccionesOTitulos.fPrecioAlOtorgarse00)
                            .Set($"Nomina.Percepciones.AccionesOTitulos.ValorMercado{sMes:D2}", oPercepciones.oAccionesOTitulos.fValorMercado00)
                            .Set($"Nomina.Percepciones.AccionesOTitulos.PrecioOtorgarse{sMes:D2}", oPercepciones.oAccionesOTitulos.fPrecioAlOtorgarse00);
                        updateDefinitionSet.Add(updateAccionesOTitulos);
                    }
                    else
                    {
                        var updateAccionesOTitulos = Builders<mdl_NAcumulados>.Update
                       .Inc("Nomina.Percepciones.AccionesOTitulos.ValorMercado00", (double)oPercepciones.oAccionesOTitulos.fValorMercado00)
                       .Inc("Nomina.Percepciones.AccionesOTitulos.PrecioAlOtorgarse00", (double)oPercepciones.oAccionesOTitulos.fPrecioAlOtorgarse00)
                       .Inc($"Nomina.Percepciones.AccionesOTitulos.ValorMercado{sMes:D2}", (double)oPercepciones.oAccionesOTitulos.fValorMercado00)
                       .Inc($"Nomina.Percepciones.AccionesOTitulos.PrecioAlOtorgarse{sMes:D2}", (double)oPercepciones.oAccionesOTitulos.fPrecioAlOtorgarse00);
                        updateDefinitionInc.Add(updateAccionesOTitulos);
                    }


                }

                if (oPercepciones.oHorasExtra != null)
                {

                    if (result.Nomina.oPercepciones.oHorasExtra == null)
                    {
                        var structureHorasExtra = Builders<mdl_NAcumulados>
                            .Update
                            .Set("Nomina.Percepciones.HorasExtra",
                            new HorasExtraMBDB
                            {
                                iDias00 = 0,
                                iHorasExtra00 = 0,
                                fImportePagado00 = 0.00m,


                                iDias01 = 0,
                                iDias02 = 0,
                                iDias03 = 0,
                                iDias04 = 0,
                                iDias05 = 0,
                                iDias06 = 0,
                                iDias07 = 0,
                                iDias08 = 0,
                                iDias09 = 0,
                                iDias10 = 0,
                                iDias11 = 0,
                                iDias12 = 0,

                                iHorasExtra01 = 0,
                                iHorasExtra02 = 0,
                                iHorasExtra03 = 0,
                                iHorasExtra04 = 0,
                                iHorasExtra05 = 0,
                                iHorasExtra06 = 0,
                                iHorasExtra07 = 0,
                                iHorasExtra08 = 0,
                                iHorasExtra09 = 0,
                                iHorasExtra10 = 0,
                                iHorasExtra11 = 0,
                                iHorasExtra12 = 0,

                                fImportePagado01 = 0.00m,
                                fImportePagado02 = 0.00m,
                                fImportePagado03 = 0.00m,
                                fImportePagado04 = 0.00m,
                                fImportePagado05 = 0.00m,
                                fImportePagado06 = 0.00m,
                                fImportePagado07 = 0.00m,
                                fImportePagado08 = 0.00m,
                                fImportePagado09 = 0.00m,
                                fImportePagado10 = 0.00m,
                                fImportePagado11 = 0.00m,
                                fImportePagado12 = 0.00m,

                                sTipoHora01 = string.Empty,
                                sTipoHora02 = string.Empty,
                                sTipoHora03 = string.Empty,
                                sTipoHora04 = string.Empty,
                                sTipoHora05 = string.Empty,
                                sTipoHora06 = string.Empty,
                                sTipoHora07 = string.Empty,
                                sTipoHora08 = string.Empty,
                                sTipoHora09 = string.Empty,
                                sTipoHora10 = string.Empty,
                                sTipoHora11 = string.Empty,
                                sTipoHora12 = string.Empty
                            });

                        // Aplicar la inicialización
                        _NAcumuladosCollection.UpdateOne(filterRFC, structureHorasExtra);


                        var updateHorasExtra = Builders<mdl_NAcumulados>.Update.Combine(
                            Builders<mdl_NAcumulados>.Update
                             .Inc("Nomina.Percepciones.HorasExtra.ImportePagado00", (double)oPercepciones.oHorasExtra.fImportePagado00)
                                     .Inc("Nomina.Percepciones.HorasExtra.HorasExtra00", oPercepciones.oHorasExtra.iHorasExtra00)
                                     .Inc("Nomina.Percepciones.HorasExtra.Dias00", oPercepciones.oHorasExtra.iDias00)
                                     .Inc($"Nomina.Percepciones.HorasExtra.ImportePagado{sMes:D2}", (double)oPercepciones.oHorasExtra.fImportePagado00)
                                     .Inc($"Nomina.Percepciones.HorasExtra.HorasExtra{sMes:D2}", oPercepciones.oHorasExtra.iHorasExtra00)
                                     .Set($"Nomina.Percepciones.HorasExtra.TipoHora{sMes:D2}", oPercepciones.oHorasExtra.sTipoHora01)
                                     .Inc($"Nomina.Percepciones.HorasExtra.Dias{sMes:D2}", oPercepciones.oHorasExtra.iDias00));


                        string propTipoHoras = $"sTipoHoras{sMes:D2}";


                        updateDefinitionSet.Add(updateHorasExtra);
                    }
                    else
                    {
                        var updateHorasExtraInc = Builders<mdl_NAcumulados>.Update
                                     .Inc("Nomina.Percepciones.HorasExtra.ImportePagado00", (double)oPercepciones.oHorasExtra.fImportePagado00)
                                     .Inc("Nomina.Percepciones.HorasExtra.HorasExtra00", oPercepciones.oHorasExtra.iHorasExtra00)
                                     .Inc("Nomina.Percepciones.HorasExtra.Dias00", oPercepciones.oHorasExtra.iDias00)
                                     .Inc($"Nomina.Percepciones.HorasExtra.ImportePagado{sMes:D2}", (double)oPercepciones.oHorasExtra.fImportePagado00)
                                     .Inc($"Nomina.Percepciones.HorasExtra.HorasExtra{sMes:D2}", oPercepciones.oHorasExtra.iHorasExtra00)
                                     .Inc($"Nomina.Percepciones.HorasExtra.Dias{sMes:D2}", oPercepciones.oHorasExtra.iDias00);
                        updateDefinitionInc.Add(updateHorasExtraInc);

                        var updateHorasExtraSet = Builders<mdl_NAcumulados>.Update
                            .Set($"Nomina.Percepciones.HorasExtra.TipoHora{sMes:D2}", oPercepciones.oHorasExtra.sTipoHora01);

                        updateDefinitionSet.Add(updateHorasExtraSet);

                    }


                }

                if (oPercepciones.oSeparacionIndemnizacion != null)
                {
                    if (result.Nomina.oPercepciones.oSeparacionIndemnizacion == null)
                    {
                        var structureSI = Builders<mdl_NAcumulados>
                            .Update
                            .Set("Nomina.Percepciones.SeparacionIndemnizacion",
                            new SeparacionIndemnizacionMBDB
                            {
                                fTotalPagado00 = 0.00m,
                                fTotalPagado01 = 0.00m,
                                fTotalPagado02 = 0.00m,
                                fTotalPagado03 = 0.00m,
                                fTotalPagado04 = 0.00m,
                                fTotalPagado05 = 0.00m,
                                fTotalPagado06 = 0.00m,
                                fTotalPagado07 = 0.00m,
                                fTotalPagado08 = 0.00m,
                                fTotalPagado09 = 0.00m,
                                fTotalPagado10 = 0.00m,
                                fTotalPagado11 = 0.00m,
                                fTotalPagado12 = 0.00m,

                                iNumAniosServicio00 = 0,
                                iNumAniosServicio01 = 0,
                                iNumAniosServicio02 = 0,
                                iNumAniosServicio03 = 0,
                                iNumAniosServicio04 = 0,
                                iNumAniosServicio05 = 0,
                                iNumAniosServicio06 = 0,
                                iNumAniosServicio07 = 0,
                                iNumAniosServicio08 = 0,
                                iNumAniosServicio09 = 0,
                                iNumAniosServicio10 = 0,
                                iNumAniosServicio11 = 0,
                                iNumAniosServicio12 = 0,

                                fUltimoSueldoMensOrd00 = 0.00m,
                                fUltimoSueldoMensOrd01 = 0.00m,
                                fUltimoSueldoMensOrd02 = 0.00m,
                                fUltimoSueldoMensOrd03 = 0.00m,
                                fUltimoSueldoMensOrd04 = 0.00m,
                                fUltimoSueldoMensOrd05 = 0.00m,
                                fUltimoSueldoMensOrd06 = 0.00m,
                                fUltimoSueldoMensOrd07 = 0.00m,
                                fUltimoSueldoMensOrd08 = 0.00m,
                                fUltimoSueldoMensOrd09 = 0.00m,
                                fUltimoSueldoMensOrd10 = 0.00m,
                                fUltimoSueldoMensOrd11 = 0.00m,
                                fUltimoSueldoMensOrd12 = 0.00m,

                                fIngresoAcumulable00 = 0.00m,
                                fIngresoAcumulable01 = 0.00m,
                                fIngresoAcumulable02 = 0.00m,
                                fIngresoAcumulable03 = 0.00m,
                                fIngresoAcumulable04 = 0.00m,
                                fIngresoAcumulable05 = 0.00m,
                                fIngresoAcumulable06 = 0.00m,
                                fIngresoAcumulable07 = 0.00m,
                                fIngresoAcumulable08 = 0.00m,
                                fIngresoAcumulable09 = 0.00m,
                                fIngresoAcumulable10 = 0.00m,
                                fIngresoAcumulable11 = 0.00m,
                                fIngresoAcumulable12 = 0.00m,

                                fIngresoNoAcumulable00 = 0.00m,
                                fIngresoNoAcumulable01 = 0.00m,
                                fIngresoNoAcumulable02 = 0.00m,
                                fIngresoNoAcumulable03 = 0.00m,
                                fIngresoNoAcumulable04 = 0.00m,
                                fIngresoNoAcumulable05 = 0.00m,
                                fIngresoNoAcumulable06 = 0.00m,
                                fIngresoNoAcumulable07 = 0.00m,
                                fIngresoNoAcumulable08 = 0.00m,
                                fIngresoNoAcumulable09 = 0.00m,
                                fIngresoNoAcumulable10 = 0.00m,
                                fIngresoNoAcumulable11 = 0.00m,
                                fIngresoNoAcumulable12 = 0.00m
                            });
                        _NAcumuladosCollection.UpdateOne(filterRFC, structureSI);


                        var updateSI = Builders<mdl_NAcumulados>.Update
                       .Set("Nomina.Percepciones.SeparacionIndemnizacion.NumAniosServicio00", oPercepciones.oSeparacionIndemnizacion.iNumAniosServicio00)
                       .Set("Nomina.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd00", (double)oPercepciones.oSeparacionIndemnizacion.fUltimoSueldoMensOrd00)
                       .Set("Nomina.Percepciones.SeparacionIndemnizacion.TotalPagado00", (double)oPercepciones.oSeparacionIndemnizacion.fTotalPagado00)
                       .Set("Nomina.Percepciones.SeparacionIndemnizacion.IngresoAcumulable00", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoAcumulable00)
                       .Set("Nomina.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable00", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoNoAcumulable00)
                       .Set($"Nomina.Percepciones.SeparacionIndemnizacion.NumAniosServicio{sMes:D2}", oPercepciones.oSeparacionIndemnizacion.iNumAniosServicio00)
                       .Set($"Nomina.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fUltimoSueldoMensOrd00)
                       .Set($"Nomina.Percepciones.SeparacionIndemnizacion.TotalPagado{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fTotalPagado00)
                       .Set($"Nomina.Percepciones.SeparacionIndemnizacion.IngresoAcumulable{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoAcumulable00)
                       .Set($"Nomina.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoNoAcumulable00);

                        updateDefinitionSet.Add(updateSI);
                    }
                    else
                    {
                        var updateSI = Builders<mdl_NAcumulados>.Update
                       .Inc("Nomina.Percepciones.SeparacionIndemnizacion.NumAniosServicio00", oPercepciones.oSeparacionIndemnizacion.iNumAniosServicio00)
                       .Inc("Nomina.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd00", (double)oPercepciones.oSeparacionIndemnizacion.fUltimoSueldoMensOrd00)
                       .Inc("Nomina.Percepciones.SeparacionIndemnizacion.TotalPagado00", (double)oPercepciones.oSeparacionIndemnizacion.fTotalPagado00)
                       .Inc("Nomina.Percepciones.SeparacionIndemnizacion.IngresoAcumulable00", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoAcumulable00)
                       .Inc("Nomina.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable00", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoNoAcumulable00)
                       .Inc($"Nomina.Percepciones.SeparacionIndemnizacion.NumAniosServicio{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.iNumAniosServicio00)
                       .Inc($"Nomina.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fUltimoSueldoMensOrd00)
                       .Inc($"Nomina.Percepciones.SeparacionIndemnizacion.TotalPagado{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fTotalPagado00)
                       .Inc($"Nomina.Percepciones.SeparacionIndemnizacion.IngresoAcumulable{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoAcumulable00)
                       .Inc($"Nomina.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable{sMes:D2}", (double)oPercepciones.oSeparacionIndemnizacion.fIngresoNoAcumulable00);

                        updateDefinitionInc.Add(updateSI);
                    }

                }

                if (oPercepciones.oJubilacionPensionRetiro != null)
                {
                    if (result.Nomina.oPercepciones.oJubilacionPensionRetiro == null)
                    {
                        var structureJPR = Builders<mdl_NAcumulados>
                            .Update
                            .Set("Nomina.Percepciones.JubilacionPensionRetiro",
                            new JubilacionPensionRetiroMBDB
                            {
                                fTotalUnaExhibicion00 = 0.00m,
                                fTotalUnaExhibicion01 = 0.00m,
                                fTotalUnaExhibicion02 = 0.00m,
                                fTotalUnaExhibicion03 = 0.00m,
                                fTotalUnaExhibicion04 = 0.00m,
                                fTotalUnaExhibicion05 = 0.00m,
                                fTotalUnaExhibicion06 = 0.00m,
                                fTotalUnaExhibicion07 = 0.00m,
                                fTotalUnaExhibicion08 = 0.00m,
                                fTotalUnaExhibicion09 = 0.00m,
                                fTotalUnaExhibicion10 = 0.00m,
                                fTotalUnaExhibicion11 = 0.00m,
                                fTotalUnaExhibicion12 = 0.00m,

                                fTotalParcialidad00 = 0.00m,
                                fTotalParcialidad01 = 0.00m,
                                fTotalParcialidad02 = 0.00m,
                                fTotalParcialidad03 = 0.00m,
                                fTotalParcialidad04 = 0.00m,
                                fTotalParcialidad05 = 0.00m,
                                fTotalParcialidad06 = 0.00m,
                                fTotalParcialidad07 = 0.00m,
                                fTotalParcialidad08 = 0.00m,
                                fTotalParcialidad09 = 0.00m,
                                fTotalParcialidad10 = 0.00m,
                                fTotalParcialidad11 = 0.00m,
                                fTotalParcialidad12 = 0.00m,

                                fMontoDiario00 = 0.00m,
                                fMontoDiario01 = 0.00m,
                                fMontoDiario02 = 0.00m,
                                fMontoDiario03 = 0.00m,
                                fMontoDiario04 = 0.00m,
                                fMontoDiario05 = 0.00m,
                                fMontoDiario06 = 0.00m,
                                fMontoDiario07 = 0.00m,
                                fMontoDiario08 = 0.00m,
                                fMontoDiario09 = 0.00m,
                                fMontoDiario10 = 0.00m,
                                fMontoDiario11 = 0.00m,
                                fMontoDiario12 = 0.00m,

                                fIngresoAcumulable00 = 0.00m,
                                fIngresoAcumulable01 = 0.00m,
                                fIngresoAcumulable02 = 0.00m,
                                fIngresoAcumulable03 = 0.00m,
                                fIngresoAcumulable04 = 0.00m,
                                fIngresoAcumulable05 = 0.00m,
                                fIngresoAcumulable06 = 0.00m,
                                fIngresoAcumulable07 = 0.00m,
                                fIngresoAcumulable08 = 0.00m,
                                fIngresoAcumulable09 = 0.00m,
                                fIngresoAcumulable10 = 0.00m,
                                fIngresoAcumulable11 = 0.00m,
                                fIngresoAcumulable12 = 0.00m,

                                fIngresoNoAcumulable00 = 0.00m,
                                fIngresoNoAcumulable01 = 0.00m,
                                fIngresoNoAcumulable02 = 0.00m,
                                fIngresoNoAcumulable03 = 0.00m,
                                fIngresoNoAcumulable04 = 0.00m,
                                fIngresoNoAcumulable05 = 0.00m,
                                fIngresoNoAcumulable06 = 0.00m,
                                fIngresoNoAcumulable07 = 0.00m,
                                fIngresoNoAcumulable08 = 0.00m,
                                fIngresoNoAcumulable09 = 0.00m,
                                fIngresoNoAcumulable10 = 0.00m,
                                fIngresoNoAcumulable11 = 0.00m,
                                fIngresoNoAcumulable12 = 0.00m
                            });
                        _NAcumuladosCollection.UpdateOne(filterRFC, structureJPR);


                        var updateJPR = Builders<mdl_NAcumulados>.Update
                          .Set("Nomina.Percepciones.JubilacionPensionRetiro.MontoDiario00", (double)oPercepciones.oJubilacionPensionRetiro.fMontoDiario00)
                          .Set("Nomina.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion00", (double)oPercepciones.oJubilacionPensionRetiro.fTotalUnaExhibicion00)
                          .Set("Nomina.Percepciones.JubilacionPensionRetiro.TotalParcialidad00", (double)oPercepciones.oJubilacionPensionRetiro.fTotalParcialidad00)
                          .Set("Nomina.Percepciones.JubilacionPensionRetiro.IngresoAcumulable00", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoAcumulable00)
                          .Set("Nomina.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable00", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoNoAcumulable00)

                          .Set($"Nomina.Percepciones.JubilacionPensionRetiro.MontoDiario{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fMontoDiario00)
                          .Set($"Nomina.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fTotalUnaExhibicion00)
                          .Set($"Nomina.Percepciones.JubilacionPensionRetiro.TotalParcialidad{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fTotalParcialidad00)
                          .Set($"Nomina.Percepciones.JubilacionPensionRetiro.IngresoAcumulable{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoAcumulable00)
                          .Set($"Nomina.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoNoAcumulable00);

                        updateDefinitionSet.Add(updateJPR);
                    }
                    else
                    {
                        var updateJPR = Builders<mdl_NAcumulados>.Update
                             .Inc("Nomina.Percepciones.JubilacionPensionRetiro.MontoDiario00", (double)oPercepciones.oJubilacionPensionRetiro.fMontoDiario00)
                             .Inc("Nomina.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion00", (double)oPercepciones.oJubilacionPensionRetiro.fTotalUnaExhibicion00)
                             .Inc("Nomina.Percepciones.JubilacionPensionRetiro.TotalParcialidad00", (double)oPercepciones.oJubilacionPensionRetiro.fTotalParcialidad00)
                             .Inc("Nomina.Percepciones.JubilacionPensionRetiro.IngresoAcumulable00", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoAcumulable00)
                             .Inc("Nomina.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable00", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoNoAcumulable00)

                             .Inc($"Nomina.Percepciones.JubilacionPensionRetiro.MontoDiario{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fMontoDiario00)
                             .Inc($"Nomina.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fTotalUnaExhibicion00)
                             .Inc($"Nomina.Percepciones.JubilacionPensionRetiro.TotalParcialidad{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fTotalParcialidad00)
                             .Inc($"Nomina.Percepciones.JubilacionPensionRetiro.IngresoAcumulable{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoAcumulable00)
                             .Inc($"Nomina.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable{sMes:D2}", (double)oPercepciones.oJubilacionPensionRetiro.fIngresoNoAcumulable00);

                        updateDefinitionInc.Add(updateJPR);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return (updateDefinitionInc, updateDefinitionSet, arrayFiltersPercepctions, nuevosConceptosP);
        }

        private (List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionInc, List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionSet, List<ArrayFilterDefinition> arrayFiltersDeducciones, List<DeduccionMBDB> nuevosElementosD)
            ActualizarDeducciones(DeduccionesMBDB oDeducciones, string sMes, List<string> lsClavesDeduccion, FilterDefinition<mdl_NAcumulados> filterRFC)
        {
            var updateDefinitionInc = new List<UpdateDefinition<mdl_NAcumulados>>();
            var updateDefinitionSet = new List<UpdateDefinition<mdl_NAcumulados>>();
            var arrayFiltersDeducciones = new List<ArrayFilterDefinition>();
            var nuevosElementosD = new List<DeduccionMBDB>();
            var hsTiposProcesadas = new HashSet<string>();
            if (oDeducciones == null)
                return (updateDefinitionInc, updateDefinitionSet, arrayFiltersDeducciones, nuevosElementosD);

            try
            {
                /*Deducciones*/
                var updateDedducciones = Builders<mdl_NAcumulados>.Update
                     .Inc("Nomina.Deducciones.TotalOtrasDeducciones00", (double)oDeducciones.TotalOtrasDeducciones00)
                     .Inc("Nomina.Deducciones.TotalImpuestosRetenidos00", (double)oDeducciones.TotalImpuestosRetenidos00)
                     .Inc($"Nomina.Deducciones.TotalOtrasDeducciones{sMes:D2}", (double)oDeducciones.TotalOtrasDeducciones00)
                     .Inc($"Nomina.Deducciones.TotalImpuestosRetenidos{sMes:D2}", (double)oDeducciones.TotalImpuestosRetenidos00);
                updateDefinitionInc.Add(updateDedducciones);

                if (oDeducciones.lsDeduccion != null && oDeducciones.lsDeduccion.Count > 0)
                {


                    foreach (var deduccion in oDeducciones.lsDeduccion)
                    {
                        if (lsClavesDeduccion.Contains(deduccion.sClave))
                        {
                            string sFiltro = $"elemD{deduccion.sClave.Replace("_", "")}";
                            if (!hsTiposProcesadas.Contains(deduccion.sClave))
                            {
                                arrayFiltersDeducciones.Add(new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                new BsonDocument(sFiltro + ".Clave", deduccion.sClave)));

                                hsTiposProcesadas.Add(deduccion.sClave);
                            }

                            updateDefinitionInc.Add(Builders<mdl_NAcumulados>.Update.Combine(
                              Builders<mdl_NAcumulados>.Update
                              .Set($"Nomina.Deducciones.Deduccion.$[{sFiltro}].Tipo", deduccion.sTipo)
                              .Set($"Nomina.Deducciones.Deduccion.$[{sFiltro}].Concepto", deduccion.sConcepto)
                              .Set($"Nomina.Deducciones.Deduccion.$[{sFiltro}].Clave", deduccion.sClave)
                              .Inc($"Nomina.Deducciones.Deduccion.$[{sFiltro}].Importe00", (double)deduccion.fImporte00)
                              .Inc($"Nomina.Deducciones.Deduccion.$[{sFiltro}].Importe{sMes:D2}", (double)deduccion.fImporte00)));

                        }
                        else
                            nuevosElementosD.Add(deduccion);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return (updateDefinitionInc, updateDefinitionSet, arrayFiltersDeducciones, nuevosElementosD);
        }

        private (List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionInc, List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionSet, List<ArrayFilterDefinition> arrayFiltersOtrosPagos, List<OtroPagoMBDB> nuevosElementosOP)
          ActualizarOtrosPagos(OtrosPagosMBDB oOtrosPagos, List<string> lsTipoOtrosPagos, string sMes, FilterDefinition<mdl_NAcumulados> filterRFC)
        {
            var updateDefinitionInc = new List<UpdateDefinition<mdl_NAcumulados>>();
            var updateDefinitionSet = new List<UpdateDefinition<mdl_NAcumulados>>();

            var arrayFiltersOtrosPagos = new List<ArrayFilterDefinition>();

            var nuevosElementosOP = new List<OtroPagoMBDB>();

            var hsTiposProcesados = new HashSet<string>();
            if (oOtrosPagos == null)
                return (updateDefinitionInc, updateDefinitionSet, arrayFiltersOtrosPagos, nuevosElementosOP);

            try
            {
                var updateOtrosPagos = Builders<mdl_NAcumulados>.Update
                    .Inc("Nomina.OtrosPagos.ImporteTotal00", (double)oOtrosPagos.fImporteTotal00)
                    .Inc("Nomina.OtrosPagos.SubsidioCausadoTotal00", (double)oOtrosPagos.fSubsidioCausadoTotal00)
                    .Inc($"Nomina.OtrosPagos.ImporteTotal{sMes:D2}", (double)oOtrosPagos.fImporteTotal00)
                    .Inc($"Nomina.OtrosPagos.SubsidioCausadoTotal{sMes:D2}", (double)oOtrosPagos.fSubsidioCausadoTotal00);
                updateDefinitionInc.Add(updateOtrosPagos);

                if (oOtrosPagos.lsOtroPago != null && oOtrosPagos.lsOtroPago.Count > 0)
                {

                    foreach (var otrosPagos in oOtrosPagos.lsOtroPago)
                    {
                        if (lsTipoOtrosPagos.Contains(otrosPagos.sClave))
                        {
                            string sFiltro = $"elemOP{otrosPagos.sClave.Replace("_", "")}";
                            if (!hsTiposProcesados.Contains(otrosPagos.sClave))
                            {
                                arrayFiltersOtrosPagos.Add(new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                    new BsonDocument(sFiltro + ".Clave", otrosPagos.sClave)));

                                hsTiposProcesados.Add(otrosPagos.sClave);
                            }


                            updateDefinitionInc.Add(Builders<mdl_NAcumulados>.Update.Combine(
                             Builders<mdl_NAcumulados>.Update
                             .Set($"Nomina.OtrosPagos.OtroPago.$[{sFiltro}].Tipo", otrosPagos.sTipo)
                             .Set($"Nomina.OtrosPagos.OtroPago.$[{sFiltro}].Concepto", otrosPagos.sConcepto)
                             .Set($"Nomina.OtrosPagos.OtroPago.$[{sFiltro}].Clave", otrosPagos.sClave)
                             .Inc($"Nomina.OtrosPagos.OtroPago.$[{sFiltro}].Importe00", (double)otrosPagos.fImporte00)
                             .Inc($"Nomina.OtrosPagos.OtroPago.$[{sFiltro}].Importe{sMes:D2}", (double)otrosPagos.fImporte00)));


                        }
                        else
                            nuevosElementosOP.Add(otrosPagos);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return (updateDefinitionInc, updateDefinitionSet, arrayFiltersOtrosPagos, nuevosElementosOP);
        }


        private (List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionInc, List<UpdateDefinition<mdl_NAcumulados>> updateDefinitionSet, List<ArrayFilterDefinition> arrayFiltersIncapacidades, List<IncapacidadMBDB> nuevosElementosI)
            ActualizarIncapacidades(IncapacidadesMBDB oIncapacidades, List<string> lsTiposIncapacidades, string sMes, FilterDefinition<mdl_NAcumulados> filterRFC)
        {
            var updateDefinitionInc = new List<UpdateDefinition<mdl_NAcumulados>>();
            var updateDefinitionSet = new List<UpdateDefinition<mdl_NAcumulados>>();

            var arrayFiltersIncapacidades = new List<ArrayFilterDefinition>();
            var nuevosElementosI = new List<IncapacidadMBDB>();

            var hsTiposProcesados = new HashSet<string>();
            try
            {
                if (oIncapacidades == null)
                    return (updateDefinitionInc, updateDefinitionSet, arrayFiltersIncapacidades, nuevosElementosI);




                var updateIncapacidades = Builders<mdl_NAcumulados>.Update
                    .Inc("Nomina.Incapacidades.DiasIncapacidadTotal00", oIncapacidades.iDiasIncapacidadTotal00)
                    .Inc("Nomina.Incapacidades.ImporteTotal00", (double)oIncapacidades.fImporteTotal00)
                    .Inc($"Nomina.Incapacidades.DiasIncapacidadTotal{sMes:D2}", oIncapacidades.iDiasIncapacidadTotal00)
                    .Inc($"Nomina.Incapacidades.ImporteTotal{sMes:D2}", (double)oIncapacidades.fImporteTotal00);

                updateDefinitionInc.Add(updateIncapacidades);

                if (oIncapacidades.lsIncapacidad != null && oIncapacidades.lsIncapacidad.Count > 0)
                {
                    var clavesProcesadas = new HashSet<string>();
                    foreach (var incapacidad in oIncapacidades.lsIncapacidad)
                    {
                        if (lsTiposIncapacidades.Contains(incapacidad.sTipoIncapacidad))
                        {
                            string sFiltro = $"elemI{incapacidad.sTipoIncapacidad.Replace("_", "")}";
                            /*Se agrega el update junto con la definición del filtro*/
                            if (!clavesProcesadas.Contains(incapacidad.sTipoIncapacidad))
                            {
                                arrayFiltersIncapacidades.Add(new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                    new BsonDocument(sFiltro + ".Tipo", incapacidad.sTipoIncapacidad)));
                                clavesProcesadas.Add(incapacidad.sTipoIncapacidad);
                            }


                            updateDefinitionInc.Add(Builders<mdl_NAcumulados>.Update.Combine(
                             Builders<mdl_NAcumulados>.Update
                             .Set($"Nomina.Incapacidades.Incapacidad.$[{sFiltro}].Tipo", incapacidad.sTipoIncapacidad)
                             .Inc($"Nomina.Incapacidades.Incapacidad.$[{sFiltro}].DiasIncapacidad00", incapacidad.iDiasIncapacidad00)
                             .Inc($"Nomina.Incapacidades.Incapacidad.$[{sFiltro}].Importe00", (double)incapacidad.fImporte00)
                             .Inc($"Nomina.Incapacidades.Incapacidad.$[{sFiltro}].DiasIncapacidad{sMes:D2}", incapacidad.iDiasIncapacidad00)
                             .Inc($"Nomina.Incapacidades.Incapacidad.$[{sFiltro}].Importe{sMes:D2}", (double)incapacidad.fImporte00)));

                        }

                        else
                            nuevosElementosI.Add(incapacidad);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return (updateDefinitionInc, updateDefinitionSet, arrayFiltersIncapacidades, nuevosElementosI);
        }

        #endregion
    }
}
