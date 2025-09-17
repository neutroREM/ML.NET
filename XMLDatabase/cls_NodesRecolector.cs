using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLDatabase
{
    internal class cls_NodesRecolector
    {
        #region Metodos
        public Task<mdl_NominaMBDB> ObtenerDataNomina(dll_nom12c.Nomina oNominaXML12C, string sMes, decimal fNetoTotal)
        {
            mdl_NominaMBDB oNomina = new mdl_NominaMBDB();
            oNomina.fNetoTotal00 = fNetoTotal;
            oNomina.fTotalPercepciones00 = oNominaXML12C != null ? oNominaXML12C.TotalPercepciones : 0.00m;
            oNomina.fTotalDeducciones00 = oNominaXML12C != null ? oNominaXML12C.TotalDeducciones : 0.00m;
            oNomina.fTotalOtrosPagos00 = oNominaXML12C != null ? oNominaXML12C.TotalOtrosPagos : 0.00m;

            string propTotalPercepciones = $"fTotalPercepciones{sMes:D2}";
            string propTotalDeducciones = $"fTotalDeducciones{sMes:D2}";
            string propTotalOtrosPagos = $"fTotalOtrosPagos{sMes:D2}";
            string propNetoTotal = $"fNetoTotal{sMes:D2}";

            var type = typeof(mdl_NominaMBDB);
            var propertyTotalPercepciones = type.GetProperty(propTotalPercepciones);
            var propertyTotalDeducciones = type.GetProperty(propTotalDeducciones);
            var propertyTotalOtrosPagos = type.GetProperty(propTotalOtrosPagos);
            var propertyNetoTotal = type.GetProperty(propNetoTotal);


            if (propertyTotalPercepciones != null)
                propertyTotalPercepciones.SetValue(oNomina, oNominaXML12C != null ? oNominaXML12C.TotalPercepciones : 0.00m);

            if (propertyTotalDeducciones != null)
                propertyTotalDeducciones.SetValue(oNomina, oNominaXML12C != null ? oNominaXML12C.TotalDeducciones : 0.00m);

            if (propertyTotalOtrosPagos != null)
                propertyTotalOtrosPagos.SetValue(oNomina, oNominaXML12C != null ? oNominaXML12C.TotalOtrosPagos : 0.00m);

            if (propertyNetoTotal != null)
                propertyNetoTotal.SetValue(oNomina, fNetoTotal);



            return Task.FromResult(oNomina);
        }

        public Task<PercepcionesMBDB> ObtenerDataPercepciones(dll_nom12c.Nomina oNominaXML12C, string sMes)
        {
            PercepcionesMBDB oPercepcion = null;
            List<PercepcionMBDB> lsPercepcion = new List<PercepcionMBDB>();
            List<HorasExtraMBDB> lsHorasExtra = new List<HorasExtraMBDB>();
            /*Preguntar si esta bien validar varias veces el mismo objeto*/
            if (oNominaXML12C.Percepciones != null)
            {
                PercepcionesMBDB oDataPercepciones = new PercepcionesMBDB
                {
                    fTotalSueldos00 = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSueldos : 0.00m,
                    fTotalGravado00 = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalGravado : 0.00m,
                    fTotalExento00 = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalExento : 0.00m,
                    fTotalJubilacionPensionRetiro00 = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalJubilacionPensionRetiro : 0.00m,
                    fTotalSeparacionIndemnizacion00 = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSeparacionIndemnizacion : 0.00m
                };


                string propTotalSueldos = $"fTotalSueldos{sMes:D2}";
                string propTotalGravado = $"fTotalGravado{sMes:D2}";
                string propTotalExento = $"fTotalExento{sMes:D2}";
                string propJubilacionPensionRetiro = $"fTotalJubilacionPensionRetiro{sMes:D2}";
                string propTotalSeparacionIndemnizacion = $"fTotalSeparacionIndemnizacion{sMes:D2}";

                var typePs = typeof(PercepcionesMBDB);
                var propertyTotalSueldos = typePs.GetProperty(propTotalSueldos);
                var propertyTotalGravado = typePs.GetProperty(propTotalGravado);
                var propertyTotalExento = typePs.GetProperty(propTotalExento);
                var propertyTotalJPR = typePs.GetProperty(propJubilacionPensionRetiro);
                var propertyTotalSI = typePs.GetProperty(propTotalSeparacionIndemnizacion);


                if (propertyTotalSueldos != null)
                    propertyTotalSueldos.SetValue(oDataPercepciones, oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSueldos : 0.00m);

                if (propertyTotalGravado != null)
                    propertyTotalGravado.SetValue(oDataPercepciones, oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalGravado : 0.00m);

                if (propertyTotalExento != null)
                    propertyTotalExento.SetValue(oDataPercepciones, oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalExento : 0.00m);

                if (propertyTotalJPR != null)
                    propertyTotalJPR.SetValue(oDataPercepciones, oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalJubilacionPensionRetiro : 0.00m);


                if (propertyTotalSI != null)
                    propertyTotalSI.SetValue(oDataPercepciones, oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSeparacionIndemnizacion : 0.00m);



                if (oNominaXML12C.Percepciones.Percepcion != null && oNominaXML12C.Percepciones.Percepcion.Length > 0)
                {
                    foreach (var concepto in oNominaXML12C.Percepciones.Percepcion)
                    {
                        var NPercepcion = new PercepcionMBDB
                        {
                            sTipo = concepto.TipoPercepcion,
                            sClave = concepto.Clave,
                            sConcepto = concepto.Concepto,
                            fGravado00 = concepto.ImporteGravado,
                            fExento00 = concepto.ImporteExento
                        };

                        string propGravado = $"fGravado{sMes:D2}";
                        string propExento = $"fExento{sMes:D2}";


                        var typeP = typeof(PercepcionMBDB);

                        var propertyGravado = typeP.GetProperty(propGravado);
                        var propertyExento = typeP.GetProperty(propExento);


                        if (propertyGravado != null)
                        {
                            propertyGravado.SetValue(NPercepcion, concepto.ImporteGravado);
                        }

                        if (propertyExento != null)
                        {
                            propertyExento.SetValue(NPercepcion, concepto.ImporteExento);
                        }


                        if (concepto.AccionesOTitulos != null)
                        {
                            var NAccionesOTitulos = new AccionesOTitulosMBDB
                            {
                                fValorMercado00 = concepto.AccionesOTitulos.ValorMercado,
                                fPrecioAlOtorgarse00 = concepto.AccionesOTitulos.PrecioAlOtorgarse
                            };
                            string propValorMercado = $"fValorMercado{sMes:D2}";
                            string propPrecioAlOtorgarse = $"fPrecioAlOtorgarse{sMes:D2}";


                            var typeAOT = typeof(AccionesOTitulosMBDB);

                            var propertyValorMercado = typeAOT.GetProperty(propGravado);
                            var propertyPrecioAlOtorgarse = typeAOT.GetProperty(propExento);


                            if (propertyValorMercado != null)
                            {
                                propertyGravado.SetValue(NAccionesOTitulos, concepto.AccionesOTitulos.ValorMercado);
                            }

                            if (propertyPrecioAlOtorgarse != null)
                            {
                                propertyPrecioAlOtorgarse.SetValue(NAccionesOTitulos, concepto.AccionesOTitulos.PrecioAlOtorgarse);
                            }
                            oDataPercepciones.oAccionesOTitulos = NAccionesOTitulos;
                        }
                        else
                            oDataPercepciones.oAccionesOTitulos = null;

                        if (concepto.HorasExtra != null && concepto.HorasExtra.Length > 0)
                        {
                            //HorasExtraMBDB NHorasExtra = null;



                            foreach (var horaextra in concepto.HorasExtra)
                            {

                                var NHorasExtra = new HorasExtraMBDB
                                {
                                    sTipoHoraExtra = horaextra.TipoHoras,
                                };

                                string propDias = $"iDias{sMes:D2}";
                                string propHorasExtra = $"iHorasExtra{sMes:D2}";
                                string propImportePagado = $"fImportePagado{sMes:D2}";
                                string propTipoHora = $"sTipoHora{sMes:D2}";



                                var typeHorasExtra = typeof(HoraExtraMBDB);

                                var propertyDias = typeHorasExtra.GetProperty(propDias);
                                var propertyHorasExtra = typeHorasExtra.GetProperty(propHorasExtra);
                                var propertyImportePagado = typeHorasExtra.GetProperty(propImportePagado);
                                var propertyTipoHora = typeHorasExtra.GetProperty(propTipoHora);


                                if (propertyDias != null)
                                {
                                    propertyDias.SetValue(NHorasExtra.oHoraExtra, horaextra.Dias);
                                }

                                if (propertyHorasExtra != null)
                                {
                                    propertyHorasExtra.SetValue(NHorasExtra.oHoraExtra, horaextra.HorasExtra);
                                }

                                if (propertyImportePagado != null)
                                {
                                    propertyImportePagado.SetValue(NHorasExtra.oHoraExtra, horaextra.ImportePagado);
                                }

                                if (propertyTipoHora != null)
                                {
                                    propertyTipoHora.SetValue(NHorasExtra.oHoraExtra, horaextra.TipoHoras);
                                }


                                lsHorasExtra.Add(NHorasExtra);

                            }

                        }
                        else
                            oDataPercepciones.lsHorasExtra = lsHorasExtra;

                        lsPercepcion.Add(NPercepcion);
                    }

                    oDataPercepciones.lsPercepcion = lsPercepcion;

                    if (oNominaXML12C.Percepciones.JubilacionPensionRetiro != null)
                    {
                        var NJubilacionPensionRetiro = new JubilacionPensionRetiroMBDB
                        {
                            fTotalUnaExhibicion00 = oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion,
                            fTotalParcialidad00 = oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalParcialidad,
                            fMontoDiario00 = oNominaXML12C.Percepciones.JubilacionPensionRetiro.MontoDiario,
                            fIngresoAcumulable00 = oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoAcumulable,
                            fIngresoNoAcumulable00 = oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable
                        };

                        string propTotalUnaExhibicion = $"fTotalUnaExhibicion{sMes:D2}";
                        string propTotalParcialidad = $"fTotalParcialidad{sMes:D2}";
                        string propDiario = $"fMontoDiario{sMes:D2}";
                        string propIngresoAcumulable = $"fIngresoAcumulable{sMes:D2}";
                        string propIngresoNoAcumulable = $"fIngresoNoAcumulable{sMes:D2}";


                        var typeJPR = typeof(JubilacionPensionRetiroMBDB);

                        var propertyUnaExhibicion = typeJPR.GetProperty(propTotalUnaExhibicion);
                        var propertyTotalParcialidad = typeJPR.GetProperty(propTotalParcialidad);
                        var propertyDiario = typeJPR.GetProperty(propDiario);
                        var propertyIngresoAcumulable = typeJPR.GetProperty(propIngresoAcumulable);
                        var propertyIngresoNoAcumulable = typeJPR.GetProperty(propIngresoNoAcumulable);


                        if (propertyUnaExhibicion != null)
                        {
                            propertyUnaExhibicion.SetValue(NJubilacionPensionRetiro, oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion);
                        }

                        if (propertyTotalParcialidad != null)
                        {
                            propertyTotalParcialidad.SetValue(NJubilacionPensionRetiro, oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalParcialidad);
                        }

                        if (propertyDiario != null)
                        {
                            propertyDiario.SetValue(NJubilacionPensionRetiro, oNominaXML12C.Percepciones.JubilacionPensionRetiro.MontoDiario);
                        }

                        if (propertyIngresoAcumulable != null)
                        {
                            propertyIngresoAcumulable.SetValue(NJubilacionPensionRetiro, oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoAcumulable);
                        }
                        if (propertyIngresoNoAcumulable != null)
                        {
                            propertyIngresoNoAcumulable.SetValue(NJubilacionPensionRetiro, oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable);
                        }

                        oDataPercepciones.oJubilacionPensionRetiro = NJubilacionPensionRetiro;
                    }
                    else
                        oDataPercepciones.oJubilacionPensionRetiro = null;

                    if (oNominaXML12C.Percepciones.SeparacionIndemnizacion != null)
                    {
                        var NSeparacionIndemnizacion = new SeparacionIndemnizacionMBDB
                        {
                            iNumAniosServicio00 = oNominaXML12C.Percepciones.SeparacionIndemnizacion.NumAñosServicio,
                            fTotalPagado00 = oNominaXML12C.Percepciones.SeparacionIndemnizacion.TotalPagado,
                            fIngresoAcumulable00 = oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoAcumulable,
                            fIngresoNoAcumulable00 = oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable,
                            fUltimoSueldoMensOrd00 = oNominaXML12C.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd,
                        };

                        string propNumServicio = $"iNumAniosServicio{sMes:D2}";
                        string propTotalPagado = $"fTotalPagado{sMes:D2}";
                        string propIngresoAcumulable = $"fIngresoAcumulable{sMes:D2}";
                        string propIngresoNoAcumulable = $"fIngresoNoAcumulable{sMes:D2}";
                        string propUltimoSueldoMesOrd = $"fUltimoSueldoMensOrd{sMes:D2}";


                        var typeJPR = typeof(SeparacionIndemnizacionMBDB);

                        var propertyNumServicio = typeJPR.GetProperty(propNumServicio);
                        var propertyTotalPagado = typeJPR.GetProperty(propTotalPagado);
                        var propertyUltimoSueldoMesOrd = typeJPR.GetProperty(propUltimoSueldoMesOrd);
                        var propertyIngresoAcumulable = typeJPR.GetProperty(propIngresoAcumulable);
                        var propertyIngresoNoAcumulable = typeJPR.GetProperty(propIngresoNoAcumulable);


                        if (propertyNumServicio != null)
                        {
                            propertyNumServicio.SetValue(NSeparacionIndemnizacion, oNominaXML12C.Percepciones.SeparacionIndemnizacion.NumAñosServicio);
                        }

                        if (propertyTotalPagado != null)
                        {
                            propertyTotalPagado.SetValue(NSeparacionIndemnizacion, oNominaXML12C.Percepciones.SeparacionIndemnizacion.TotalPagado);
                        }

                        if (propertyUltimoSueldoMesOrd != null)
                        {
                            propertyUltimoSueldoMesOrd.SetValue(NSeparacionIndemnizacion, oNominaXML12C.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd);
                        }

                        if (propertyIngresoAcumulable != null)
                        {
                            propertyIngresoAcumulable.SetValue(NSeparacionIndemnizacion, oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoAcumulable);
                        }
                        if (propertyIngresoNoAcumulable != null)
                        {
                            propertyIngresoNoAcumulable.SetValue(NSeparacionIndemnizacion, oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable);
                        }
                        oDataPercepciones.oSeparacionIndemnizacion = NSeparacionIndemnizacion;
                    }
                    else
                        oDataPercepciones.oSeparacionIndemnizacion = null;

                    oPercepcion = oDataPercepciones;
                    oDataPercepciones = null;
                }
            }


            return Task.FromResult(oPercepcion);
        }

        public Task<DeduccionesMBDB> ObtenerDataDeducciones(dll_nom12c.Nomina oNominaXML12C, string sMes)
        {
            DeduccionesMBDB oDeducciones = null;
            List<DeduccionMBDB> lsDeducciones = new List<DeduccionMBDB>();
            if (oNominaXML12C.Deducciones != null)
            {

                DeduccionesMBDB oDeduccion = new DeduccionesMBDB
                {

                    TotalOtrasDeducciones00 = oNominaXML12C.Deducciones.TotalOtrasDeducciones,
                    TotalImpuestosRetenidos00 = oNominaXML12C.Deducciones.TotalImpuestosRetenidos
                };

                string propOtrasDeducciones = $"TotalOtrasDeducciones{sMes:D2}";
                string propImpuestosRetenidos = $"TotalImpuestosRetenidos{sMes:D2}";


                var typeDs = typeof(DeduccionesMBDB);
                var propertyOtrasDeducciones = typeDs.GetProperty(propOtrasDeducciones);
                var propertyImpuestosRetenidos = typeDs.GetProperty(propImpuestosRetenidos);


                if (propertyOtrasDeducciones != null)
                    propertyOtrasDeducciones.SetValue(oDeduccion, oNominaXML12C != null ? oNominaXML12C.Deducciones.TotalOtrasDeducciones : 0.00m);

                if (propertyImpuestosRetenidos != null)
                    propertyImpuestosRetenidos.SetValue(oDeduccion, oNominaXML12C != null ? oNominaXML12C.Deducciones.TotalImpuestosRetenidos : 0.00m);



                if (oNominaXML12C.Deducciones.Deduccion != null && oNominaXML12C.Deducciones.Deduccion.Length > 0)
                {
                    foreach (var deduccion in oNominaXML12C.Deducciones.Deduccion)
                    {
                        var NDeduccion = new DeduccionMBDB
                        {
                            sTipo = deduccion.TipoDeduccion,
                            sClave = deduccion.Clave,
                            sConcepto = deduccion.Concepto,
                            fImporte00 = deduccion.Importe
                        };

                        string propImporte = $"fImporte{sMes:D2}";


                        var type = typeof(DeduccionMBDB);

                        var propertyImporte = type.GetProperty(propImporte);


                        if (propertyImporte != null)
                        {
                            propertyImporte.SetValue(NDeduccion, deduccion.Importe);
                        }
                        lsDeducciones.Add(NDeduccion);
                    }
                    oDeduccion.lsDeduccion = lsDeducciones;
                }
                else
                    oDeduccion.lsDeduccion = null;



                oDeducciones = oDeduccion;
                oDeduccion = null;

            }

            return Task.FromResult(oDeducciones);
        }

        public Task<OtrosPagosMBDB> ObtenerDataOtrosPagos(dll_nom12c.Nomina oNominaXML12C, string sMes)
        {
            var oOtrosPagos = new OtrosPagosMBDB();
            SubsidioAlEmpleoMBDB oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB();
            CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor = null;
            List<OtroPagoMBDB> lsOtroPago = new List<OtroPagoMBDB>();
            decimal fImporteTotal = 0.00m;
            decimal fSubsidioCausadoTotal = 0.00m;

            if (oNominaXML12C.OtrosPagos != null)
            {


                foreach (var otroPago in oNominaXML12C.OtrosPagos)
                {
                    fImporteTotal += otroPago.Importe;
                    fSubsidioCausadoTotal += otroPago.SubsidioAlEmpleo != null ? otroPago.SubsidioAlEmpleo.SubsidioCausado : 0.00m;
                    var NOtroPago = new OtroPagoMBDB
                    {
                        sTipo = otroPago.TipoOtroPago.ToString(),
                        sClave = otroPago.Clave.ToString(),
                        sConcepto = otroPago.Concepto.ToString(),
                        fImporte00 = otroPago.Importe

                    };

                    string propImporte = $"fImporte{sMes:D2}";
                    var type = typeof(OtroPagoMBDB);

                    var propertyImporte = type.GetProperty(propImporte);

                    if (propertyImporte != null)
                        propertyImporte.SetValue(NOtroPago, otroPago.Importe);
                    lsOtroPago.Add(NOtroPago);



                    if (otroPago.CompensacionSaldosAFavor != null)
                    {
                        oCompensacionSaldoAFavor = new CompensacionSaldoAFavorMBDB
                        {
                            fRemanenteSalFav00 = otroPago.CompensacionSaldosAFavor.RemanenteSalFav,
                            fSaldoAFavor00 = otroPago.CompensacionSaldosAFavor.SaldoAFavor,

                        };

                        string propRemanenteSalFav = $"fRemanenteSalFav{sMes:D2}";
                        string propfSaldoAFavor = $"fSaldoAFavor{sMes:D2}";
                        var typeCSAF = typeof(CompensacionSaldoAFavorMBDB);

                        var propertyRemanente = typeCSAF.GetProperty(propRemanenteSalFav);
                        var propertySaldoAFavor = typeCSAF.GetProperty(propfSaldoAFavor);

                        if (propertyRemanente != null)
                            propertyRemanente.SetValue(oCompensacionSaldoAFavor, otroPago.CompensacionSaldosAFavor.RemanenteSalFav);

                        if (propertySaldoAFavor != null)
                            propertyRemanente.SetValue(oCompensacionSaldoAFavor, otroPago.CompensacionSaldosAFavor.SaldoAFavor);

                    }
                    else
                        oCompensacionSaldoAFavor = null;

                    if (otroPago.SubsidioAlEmpleo != null)
                    {
                        oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB
                        {
                            fSubsidioCausado00 = otroPago.SubsidioAlEmpleo.SubsidioCausado
                        };

                        string propSubsidioCausado = $"fSubsidioCausado{sMes:D2}";



                        var typeSAE = typeof(SubsidioAlEmpleoMBDB);

                        var propertySubsidioCausado = typeSAE.GetProperty(propSubsidioCausado);


                        if (propertySubsidioCausado != null)
                            propertySubsidioCausado.SetValue(oSubsidioAlEmpleo, otroPago.SubsidioAlEmpleo.SubsidioCausado);

                    }


                }

                var oDataOtrosPagos = new OtrosPagosMBDB
                {
                    lsOtroPago = lsOtroPago,
                    fImporteTotal00 = fImporteTotal,
                    fSubsidioCausadoTotal00 = fSubsidioCausadoTotal,
                    oCompensacionSaldoAFavor = oCompensacionSaldoAFavor,
                    oSubsidioAlEmpleo = oSubsidioAlEmpleo
                };



                string propSubsidioCausadoTotal = $"fSubsidioCausadoTotal{sMes:D2}";
                string propImporteTotal = $"fImporteTotal{sMes:D2}";


                var typeOP = typeof(OtrosPagosMBDB);
                var propertySubsidioCausadoTotal = typeOP.GetProperty(propSubsidioCausadoTotal);
                var propertyImporteTotal = typeOP.GetProperty(propImporteTotal);


                if (propertySubsidioCausadoTotal != null)
                    propertySubsidioCausadoTotal.SetValue(oDataOtrosPagos, oNominaXML12C.OtrosPagos != null ? fSubsidioCausadoTotal : 0.00m);

                if (propertyImporteTotal != null)
                    propertyImporteTotal.SetValue(oDataOtrosPagos, oNominaXML12C.OtrosPagos != null ? fImporteTotal : 0.00m);


                oOtrosPagos = oDataOtrosPagos;
                oDataOtrosPagos = null;
            }
            else
            { oOtrosPagos = null; }

            return Task.FromResult(oOtrosPagos);

        }

        public Task<IncapacidadesMBDB> ObtenerDataIncapacidades(dll_nom12c.Nomina oNominaXML12C, string sMes)
        {

            IncapacidadesMBDB oIncapacidades = null;
            List<IncapacidadMBDB> lsIncapacidades = new List<IncapacidadMBDB>();
            decimal fImporteTotal = 0.00m;
            int iDiasIncapacidadTotal = 0;



            if (oNominaXML12C.Incapacidades != null)
            {
                foreach (var incapacidad in oNominaXML12C.Incapacidades)
                {
                    var NIncapacidad = new IncapacidadMBDB
                    {
                        sTipoIncapacidad = incapacidad.TipoIncapacidad,
                        iDiasIncapacidad00 = incapacidad.DiasIncapacidad,
                        fImporte00 = incapacidad.ImporteMonetario
                    };
                    fImporteTotal += incapacidad.ImporteMonetario;
                    iDiasIncapacidadTotal += incapacidad.DiasIncapacidad;

                    string propDiasIncapacidad = $"iDiasIncapacidad{sMes:D2}";
                    string propfIImporte = $"fImporte{sMes:D2}";
                    var type = typeof(IncapacidadMBDB);

                    var propertyDias = type.GetProperty(propDiasIncapacidad);
                    var propertyImporte = type.GetProperty(propfIImporte);

                    if (propertyDias != null)
                        propertyDias.SetValue(NIncapacidad, incapacidad.DiasIncapacidad);

                    if (propertyImporte != null)
                        propertyImporte.SetValue(NIncapacidad, incapacidad.ImporteMonetario);

                    lsIncapacidades.Add(NIncapacidad);
                }
                oIncapacidades = new IncapacidadesMBDB
                {
                    lsIncapacidad = lsIncapacidades,
                    fImporteTotal00 = fImporteTotal,
                    iDiasIncapacidadTotal00 = iDiasIncapacidadTotal
                };


                string propDiasIncapacidadTotal = $"iDiasIncapacidad{sMes:D2}";
                string propImporteTotal = $"fImporteTotal{sMes:D2}";


                var typeI = typeof(IncapacidadesMBDB);

                var propertyDiasIncapacidadTotal = typeI.GetProperty(propDiasIncapacidadTotal);
                var propertyImporteTotal = typeI.GetProperty(propImporteTotal);


                if (propertyDiasIncapacidadTotal != null)
                    propertyDiasIncapacidadTotal.SetValue(oIncapacidades, oNominaXML12C.OtrosPagos != null ? iDiasIncapacidadTotal : 0);

                if (propertyImporteTotal != null)
                    propertyImporteTotal.SetValue(oIncapacidades, oNominaXML12C.OtrosPagos != null ? fImporteTotal : 0.00m);



            }
            else
                oIncapacidades = null;


            return Task.FromResult(oIncapacidades);

        }
        #endregion

        #region Metodos dll_nom12b.Nomina 
        public Task<mdl_NominaMBDB> ObtenerData12B(dll_nom12b.Nomina oNominaXML12B, string sMes, decimal fNetoTotal)
        {
            mdl_NominaMBDB oNomina = new mdl_NominaMBDB();
            oNomina.fNetoTotal00 = fNetoTotal;
            oNomina.fTotalPercepciones00 = oNominaXML12B != null ? oNominaXML12B.TotalPercepciones : 0.00m;
            oNomina.fTotalDeducciones00 = oNominaXML12B != null ? oNominaXML12B.TotalDeducciones : 0.00m;
            oNomina.fTotalOtrosPagos00 = oNominaXML12B != null ? oNominaXML12B.TotalOtrosPagos : 0.00m;

            string propTotalPercepciones = $"fTotalPercepciones{sMes:D2}";
            string propTotalDeducciones = $"fTotalDeducciones{sMes:D2}";
            string propTotalOtrosPagos = $"fTotalOtrosPagos{sMes:D2}";
            string propNetoTotal = $"fNetoTotal{sMes:D2}";

            var type = typeof(mdl_NominaMBDB);
            var propertyTotalPercepciones = type.GetProperty(propTotalPercepciones);
            var propertyTotalDeducciones = type.GetProperty(propTotalDeducciones);
            var propertyTotalOtrosPagos = type.GetProperty(propTotalOtrosPagos);
            var propertyNetoTotal = type.GetProperty(propNetoTotal);


            if (propertyTotalPercepciones != null)
                propertyTotalPercepciones.SetValue(oNomina, oNominaXML12B != null ? oNominaXML12B.TotalPercepciones : 0.00m);

            if (propertyTotalDeducciones != null)
                propertyTotalDeducciones.SetValue(oNomina, oNominaXML12B != null ? oNominaXML12B.TotalDeducciones : 0.00m);

            if (propertyTotalOtrosPagos != null)
                propertyTotalOtrosPagos.SetValue(oNomina, oNominaXML12B != null ? oNominaXML12B.TotalOtrosPagos : 0.00m);

            if (propertyNetoTotal != null)
                propertyNetoTotal.SetValue(oNomina, fNetoTotal);



            return Task.FromResult(oNomina);
        }

        public Task<PercepcionesMBDB> ObtenerDataPercepciones12B(dll_nom12b.Nomina oNominaXML12B, string sMes)
        {
            PercepcionesMBDB oPercepcion = null;
            List<PercepcionMBDB> lsPercepcion = new List<PercepcionMBDB>();
            List<HorasExtraMBDB> lsHorasExtra = new List<HorasExtraMBDB>();
            /*Preguntar si esta bien validar varias veces el mismo objeto*/
            if (oNominaXML12B.Percepciones != null)
            {
                PercepcionesMBDB oDataPercepciones = new PercepcionesMBDB
                {
                    fTotalSueldos00 = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSueldos : 0.00m,
                    fTotalGravado00 = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalGravado : 0.00m,
                    fTotalExento00 = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalExento : 0.00m,
                    fTotalJubilacionPensionRetiro00 = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalJubilacionPensionRetiro : 0.00m,
                    fTotalSeparacionIndemnizacion00 = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSeparacionIndemnizacion : 0.00m
                };


                string propTotalSueldos = $"fTotalSueldos{sMes:D2}";
                string propTotalGravado = $"fTotalGravado{sMes:D2}";
                string propTotalExento = $"fTotalExento{sMes:D2}";
                string propJubilacionPensionRetiro = $"fTotalJubilacionPensionRetiro{sMes:D2}";
                string propTotalSeparacionIndemnizacion = $"fTotalSeparacionIndemnizacion{sMes:D2}";

                var typePs = typeof(PercepcionesMBDB);
                var propertyTotalSueldos = typePs.GetProperty(propTotalSueldos);
                var propertyTotalGravado = typePs.GetProperty(propTotalGravado);
                var propertyTotalExento = typePs.GetProperty(propTotalExento);
                var propertyTotalJPR = typePs.GetProperty(propJubilacionPensionRetiro);
                var propertyTotalSI = typePs.GetProperty(propTotalSeparacionIndemnizacion);


                if (propertyTotalSueldos != null)
                    propertyTotalSueldos.SetValue(oDataPercepciones, oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSueldos : 0.00m);

                if (propertyTotalGravado != null)
                    propertyTotalGravado.SetValue(oDataPercepciones, oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalGravado : 0.00m);

                if (propertyTotalExento != null)
                    propertyTotalExento.SetValue(oDataPercepciones, oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalExento : 0.00m);

                if (propertyTotalJPR != null)
                    propertyTotalJPR.SetValue(oDataPercepciones, oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalJubilacionPensionRetiro : 0.00m);


                if (propertyTotalSI != null)
                    propertyTotalSI.SetValue(oDataPercepciones, oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSeparacionIndemnizacion : 0.00m);



                if (oNominaXML12B.Percepciones.Percepcion != null && oNominaXML12B.Percepciones.Percepcion.Length > 0)
                {
                    foreach (var concepto in oNominaXML12B.Percepciones.Percepcion)
                    {
                        var NPercepcion = new PercepcionMBDB
                        {
                            sTipo = concepto.TipoPercepcion,
                            sClave = concepto.Clave,
                            sConcepto = concepto.Concepto,
                            fGravado00 = concepto.ImporteGravado,
                            fExento00 = concepto.ImporteExento
                        };

                        string propGravado = $"fGravado{sMes:D2}";
                        string propExento = $"fExento{sMes:D2}";


                        var typeP = typeof(PercepcionMBDB);

                        var propertyGravado = typeP.GetProperty(propGravado);
                        var propertyExento = typeP.GetProperty(propExento);


                        if (propertyGravado != null)
                        {
                            propertyGravado.SetValue(NPercepcion, concepto.ImporteGravado);
                        }

                        if (propertyExento != null)
                        {
                            propertyExento.SetValue(NPercepcion, concepto.ImporteExento);
                        }


                        if (concepto.AccionesOTitulos != null)
                        {
                            var NAccionesOTitulos = new AccionesOTitulosMBDB
                            {
                                fValorMercado00 = concepto.AccionesOTitulos.ValorMercado,
                                fPrecioAlOtorgarse00 = concepto.AccionesOTitulos.PrecioAlOtorgarse
                            };
                            string propValorMercado = $"fValorMercado{sMes:D2}";
                            string propPrecioAlOtorgarse = $"fPrecioAlOtorgarse{sMes:D2}";


                            var typeAOT = typeof(AccionesOTitulosMBDB);

                            var propertyValorMercado = typeAOT.GetProperty(propGravado);
                            var propertyPrecioAlOtorgarse = typeAOT.GetProperty(propExento);


                            if (propertyValorMercado != null)
                            {
                                propertyGravado.SetValue(NAccionesOTitulos, concepto.AccionesOTitulos.ValorMercado);
                            }

                            if (propertyPrecioAlOtorgarse != null)
                            {
                                propertyPrecioAlOtorgarse.SetValue(NAccionesOTitulos, concepto.AccionesOTitulos.PrecioAlOtorgarse);
                            }
                            oDataPercepciones.oAccionesOTitulos = NAccionesOTitulos;
                        }
                        else
                            oDataPercepciones.oAccionesOTitulos = null;

                        if (concepto.HorasExtra != null && concepto.HorasExtra.Length > 0)
                        {
                            //HorasExtraMBDB NHorasExtra = null;



                            foreach (var horaextra in concepto.HorasExtra)
                            {

                                var NHorasExtra = new HorasExtraMBDB
                                {
                                    sTipoHoraExtra = horaextra.TipoHoras,
                                };

                                string propDias = $"iDias{sMes:D2}";
                                string propHorasExtra = $"iHorasExtra{sMes:D2}";
                                string propImportePagado = $"fImportePagado{sMes:D2}";
                                string propTipoHora = $"sTipoHora{sMes:D2}";



                                var typeHorasExtra = typeof(HoraExtraMBDB);

                                var propertyDias = typeHorasExtra.GetProperty(propDias);
                                var propertyHorasExtra = typeHorasExtra.GetProperty(propHorasExtra);
                                var propertyImportePagado = typeHorasExtra.GetProperty(propImportePagado);
                                var propertyTipoHora = typeHorasExtra.GetProperty(propTipoHora);


                                if (propertyDias != null)
                                {
                                    propertyDias.SetValue(NHorasExtra.oHoraExtra, horaextra.Dias);
                                }

                                if (propertyHorasExtra != null)
                                {
                                    propertyHorasExtra.SetValue(NHorasExtra.oHoraExtra, horaextra.HorasExtra);
                                }

                                if (propertyImportePagado != null)
                                {
                                    propertyImportePagado.SetValue(NHorasExtra.oHoraExtra, horaextra.ImportePagado);
                                }

                                if (propertyTipoHora != null)
                                {
                                    propertyTipoHora.SetValue(NHorasExtra.oHoraExtra, horaextra.TipoHoras);
                                }


                                lsHorasExtra.Add(NHorasExtra);

                            }

                        }
                        else
                            oDataPercepciones.lsHorasExtra = lsHorasExtra;

                        lsPercepcion.Add(NPercepcion);
                    }

                    oDataPercepciones.lsPercepcion = lsPercepcion;

                    if (oNominaXML12B.Percepciones.JubilacionPensionRetiro != null)
                    {
                        var NJubilacionPensionRetiro = new JubilacionPensionRetiroMBDB
                        {
                            fTotalUnaExhibicion00 = oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion,
                            fTotalParcialidad00 = oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalParcialidad,
                            fMontoDiario00 = oNominaXML12B.Percepciones.JubilacionPensionRetiro.MontoDiario,
                            fIngresoAcumulable00 = oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoAcumulable,
                            fIngresoNoAcumulable00 = oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable
                        };

                        string propTotalUnaExhibicion = $"fTotalUnaExhibicion{sMes:D2}";
                        string propTotalParcialidad = $"fTotalParcialidad{sMes:D2}";
                        string propDiario = $"fMontoDiario{sMes:D2}";
                        string propIngresoAcumulable = $"fIngresoAcumulable{sMes:D2}";
                        string propIngresoNoAcumulable = $"fIngresoNoAcumulable{sMes:D2}";


                        var typeJPR = typeof(JubilacionPensionRetiroMBDB);

                        var propertyUnaExhibicion = typeJPR.GetProperty(propTotalUnaExhibicion);
                        var propertyTotalParcialidad = typeJPR.GetProperty(propTotalParcialidad);
                        var propertyDiario = typeJPR.GetProperty(propDiario);
                        var propertyIngresoAcumulable = typeJPR.GetProperty(propIngresoAcumulable);
                        var propertyIngresoNoAcumulable = typeJPR.GetProperty(propIngresoNoAcumulable);


                        if (propertyUnaExhibicion != null)
                        {
                            propertyUnaExhibicion.SetValue(NJubilacionPensionRetiro, oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion);
                        }

                        if (propertyTotalParcialidad != null)
                        {
                            propertyTotalParcialidad.SetValue(NJubilacionPensionRetiro, oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalParcialidad);
                        }

                        if (propertyDiario != null)
                        {
                            propertyDiario.SetValue(NJubilacionPensionRetiro, oNominaXML12B.Percepciones.JubilacionPensionRetiro.MontoDiario);
                        }

                        if (propertyIngresoAcumulable != null)
                        {
                            propertyIngresoAcumulable.SetValue(NJubilacionPensionRetiro, oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoAcumulable);
                        }
                        if (propertyIngresoNoAcumulable != null)
                        {
                            propertyIngresoNoAcumulable.SetValue(NJubilacionPensionRetiro, oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable);
                        }

                        oDataPercepciones.oJubilacionPensionRetiro = NJubilacionPensionRetiro;
                    }
                    else
                        oDataPercepciones.oJubilacionPensionRetiro = null;

                    if (oNominaXML12B.Percepciones.SeparacionIndemnizacion != null)
                    {
                        var NSeparacionIndemnizacion = new SeparacionIndemnizacionMBDB
                        {
                            iNumAniosServicio00 = oNominaXML12B.Percepciones.SeparacionIndemnizacion.NumAñosServicio,
                            fTotalPagado00 = oNominaXML12B.Percepciones.SeparacionIndemnizacion.TotalPagado,
                            fIngresoAcumulable00 = oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoAcumulable,
                            fIngresoNoAcumulable00 = oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable,
                            fUltimoSueldoMensOrd00 = oNominaXML12B.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd,
                        };

                        string propNumServicio = $"iNumAniosServicio{sMes:D2}";
                        string propTotalPagado = $"fTotalPagado{sMes:D2}";
                        string propIngresoAcumulable = $"fIngresoAcumulable{sMes:D2}";
                        string propIngresoNoAcumulable = $"fIngresoNoAcumulable{sMes:D2}";
                        string propUltimoSueldoMesOrd = $"fUltimoSueldoMensOrd{sMes:D2}";


                        var typeJPR = typeof(SeparacionIndemnizacionMBDB);

                        var propertyNumServicio = typeJPR.GetProperty(propNumServicio);
                        var propertyTotalPagado = typeJPR.GetProperty(propTotalPagado);
                        var propertyUltimoSueldoMesOrd = typeJPR.GetProperty(propUltimoSueldoMesOrd);
                        var propertyIngresoAcumulable = typeJPR.GetProperty(propIngresoAcumulable);
                        var propertyIngresoNoAcumulable = typeJPR.GetProperty(propIngresoNoAcumulable);


                        if (propertyNumServicio != null)
                        {
                            propertyNumServicio.SetValue(NSeparacionIndemnizacion, oNominaXML12B.Percepciones.SeparacionIndemnizacion.NumAñosServicio);
                        }

                        if (propertyTotalPagado != null)
                        {
                            propertyTotalPagado.SetValue(NSeparacionIndemnizacion, oNominaXML12B.Percepciones.SeparacionIndemnizacion.TotalPagado);
                        }

                        if (propertyUltimoSueldoMesOrd != null)
                        {
                            propertyUltimoSueldoMesOrd.SetValue(NSeparacionIndemnizacion, oNominaXML12B.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd);
                        }

                        if (propertyIngresoAcumulable != null)
                        {
                            propertyIngresoAcumulable.SetValue(NSeparacionIndemnizacion, oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoAcumulable);
                        }
                        if (propertyIngresoNoAcumulable != null)
                        {
                            propertyIngresoNoAcumulable.SetValue(NSeparacionIndemnizacion, oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable);
                        }
                        oDataPercepciones.oSeparacionIndemnizacion = NSeparacionIndemnizacion;
                    }
                    else
                        oDataPercepciones.oSeparacionIndemnizacion = null;

                    oPercepcion = oDataPercepciones;
                    oDataPercepciones = null;
                }
            }


            return Task.FromResult(oPercepcion);
        }

        public Task<DeduccionesMBDB> ObtenerDataDeducciones12B(dll_nom12b.Nomina oNominaXML12B, string sMes)
        {
            DeduccionesMBDB oDeducciones = null;
            List<DeduccionMBDB> lsDeducciones = new List<DeduccionMBDB>();
            if (oNominaXML12B.Deducciones != null)
            {

                DeduccionesMBDB oDeduccion = new DeduccionesMBDB
                {

                    TotalOtrasDeducciones00 = oNominaXML12B.Deducciones.TotalOtrasDeducciones,
                    TotalImpuestosRetenidos00 = oNominaXML12B.Deducciones.TotalImpuestosRetenidos
                };

                string propOtrasDeducciones = $"TotalOtrasDeducciones{sMes:D2}";
                string propImpuestosRetenidos = $"TotalImpuestosRetenidos{sMes:D2}";


                var typeDs = typeof(DeduccionesMBDB);
                var propertyOtrasDeducciones = typeDs.GetProperty(propOtrasDeducciones);
                var propertyImpuestosRetenidos = typeDs.GetProperty(propImpuestosRetenidos);


                if (propertyOtrasDeducciones != null)
                    propertyOtrasDeducciones.SetValue(oDeduccion, oNominaXML12B != null ? oNominaXML12B.Deducciones.TotalOtrasDeducciones : 0.00m);

                if (propertyImpuestosRetenidos != null)
                    propertyImpuestosRetenidos.SetValue(oDeduccion, oNominaXML12B != null ? oNominaXML12B.Deducciones.TotalImpuestosRetenidos : 0.00m);



                if (oNominaXML12B.Deducciones.Deduccion != null && oNominaXML12B.Deducciones.Deduccion.Length > 0)
                {
                    foreach (var deduccion in oNominaXML12B.Deducciones.Deduccion)
                    {
                        var NDeduccion = new DeduccionMBDB
                        {
                            sTipo = deduccion.TipoDeduccion,
                            sClave = deduccion.Clave,
                            sConcepto = deduccion.Concepto,
                            fImporte00 = deduccion.Importe
                        };

                        string propImporte = $"fImporte{sMes:D2}";


                        var type = typeof(DeduccionMBDB);

                        var propertyImporte = type.GetProperty(propImporte);


                        if (propertyImporte != null)
                        {
                            propertyImporte.SetValue(NDeduccion, deduccion.Importe);
                        }
                        lsDeducciones.Add(NDeduccion);
                    }
                    oDeduccion.lsDeduccion = lsDeducciones;
                }
                else
                    oDeduccion.lsDeduccion = null;



                oDeducciones = oDeduccion;
                oDeduccion = null;

            }

            return Task.FromResult(oDeducciones);
        }

        public Task<OtrosPagosMBDB> ObtenerDataOtrosPagos12B(dll_nom12b.Nomina oNominaXML12B, string sMes)
        {
            var oOtrosPagos = new OtrosPagosMBDB();
            SubsidioAlEmpleoMBDB oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB();
            CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor = null;
            List<OtroPagoMBDB> lsOtroPago = new List<OtroPagoMBDB>();
            decimal fImporteTotal = 0.00m;
            decimal fSubsidioCausadoTotal = 0.00m;

            if (oNominaXML12B.OtrosPagos != null)
            {


                foreach (var otroPago in oNominaXML12B.OtrosPagos)
                {
                    fImporteTotal += otroPago.Importe;
                    fSubsidioCausadoTotal += otroPago.SubsidioAlEmpleo != null ? otroPago.SubsidioAlEmpleo.SubsidioCausado : 0.00m;
                    var NOtroPago = new OtroPagoMBDB
                    {
                        sTipo = otroPago.TipoOtroPago.ToString(),
                        sClave = otroPago.Clave.ToString(),
                        sConcepto = otroPago.Concepto.ToString(),
                        fImporte00 = otroPago.Importe

                    };

                    string propImporte = $"fImporte{sMes:D2}";
                    var type = typeof(OtroPagoMBDB);

                    var propertyImporte = type.GetProperty(propImporte);

                    if (propertyImporte != null)
                        propertyImporte.SetValue(NOtroPago, otroPago.Importe);
                    lsOtroPago.Add(NOtroPago);



                    if (otroPago.CompensacionSaldosAFavor != null)
                    {
                        oCompensacionSaldoAFavor = new CompensacionSaldoAFavorMBDB
                        {
                            fRemanenteSalFav00 = otroPago.CompensacionSaldosAFavor.RemanenteSalFav,
                            fSaldoAFavor00 = otroPago.CompensacionSaldosAFavor.SaldoAFavor,

                        };

                        string propRemanenteSalFav = $"fRemanenteSalFav{sMes:D2}";
                        string propfSaldoAFavor = $"fSaldoAFavor{sMes:D2}";
                        var typeCSAF = typeof(CompensacionSaldoAFavorMBDB);

                        var propertyRemanente = typeCSAF.GetProperty(propRemanenteSalFav);
                        var propertySaldoAFavor = typeCSAF.GetProperty(propfSaldoAFavor);

                        if (propertyRemanente != null)
                            propertyRemanente.SetValue(oCompensacionSaldoAFavor, otroPago.CompensacionSaldosAFavor.RemanenteSalFav);

                        if (propertySaldoAFavor != null)
                            propertyRemanente.SetValue(oCompensacionSaldoAFavor, otroPago.CompensacionSaldosAFavor.SaldoAFavor);

                    }
                    else
                        oCompensacionSaldoAFavor = null;

                    if (otroPago.SubsidioAlEmpleo != null)
                    {
                        oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB
                        {
                            fSubsidioCausado00 = otroPago.SubsidioAlEmpleo.SubsidioCausado
                        };

                        string propSubsidioCausado = $"fSubsidioCausado{sMes:D2}";



                        var typeSAE = typeof(SubsidioAlEmpleoMBDB);

                        var propertySubsidioCausado = typeSAE.GetProperty(propSubsidioCausado);


                        if (propertySubsidioCausado != null)
                            propertySubsidioCausado.SetValue(oSubsidioAlEmpleo, otroPago.SubsidioAlEmpleo.SubsidioCausado);

                    }


                }

                var oDataOtrosPagos = new OtrosPagosMBDB
                {
                    lsOtroPago = lsOtroPago,
                    fImporteTotal00 = fImporteTotal,
                    fSubsidioCausadoTotal00 = fSubsidioCausadoTotal,
                    oCompensacionSaldoAFavor = oCompensacionSaldoAFavor,
                    oSubsidioAlEmpleo = oSubsidioAlEmpleo
                };



                string propSubsidioCausadoTotal = $"fSubsidioCausadoTotal{sMes:D2}";
                string propImporteTotal = $"fImporteTotal{sMes:D2}";


                var typeOP = typeof(OtrosPagosMBDB);
                var propertySubsidioCausadoTotal = typeOP.GetProperty(propSubsidioCausadoTotal);
                var propertyImporteTotal = typeOP.GetProperty(propImporteTotal);


                if (propertySubsidioCausadoTotal != null)
                    propertySubsidioCausadoTotal.SetValue(oDataOtrosPagos, oNominaXML12B.OtrosPagos != null ? fSubsidioCausadoTotal : 0.00m);

                if (propertyImporteTotal != null)
                    propertyImporteTotal.SetValue(oDataOtrosPagos, oNominaXML12B.OtrosPagos != null ? fImporteTotal : 0.00m);


                oOtrosPagos = oDataOtrosPagos;
                oDataOtrosPagos = null;
            }
            else
            { oOtrosPagos = null; }

            return Task.FromResult(oOtrosPagos);

        }

        public Task<IncapacidadesMBDB> ObtenerDataIncapacidades12B(dll_nom12b.Nomina oNominaXML12B, string sMes)
        {

            IncapacidadesMBDB oIncapacidades = null;
            List<IncapacidadMBDB> lsIncapacidades = new List<IncapacidadMBDB>();
            decimal fImporteTotal = 0.00m;
            int iDiasIncapacidadTotal = 0;



            if (oNominaXML12B.Incapacidades != null)
            {
                foreach (var incapacidad in oNominaXML12B.Incapacidades)
                {
                    var NIncapacidad = new IncapacidadMBDB
                    {
                        sTipoIncapacidad = incapacidad.TipoIncapacidad,
                        iDiasIncapacidad00 = incapacidad.DiasIncapacidad,
                        fImporte00 = incapacidad.ImporteMonetario
                    };
                    fImporteTotal += incapacidad.ImporteMonetario;
                    iDiasIncapacidadTotal += incapacidad.DiasIncapacidad;

                    string propDiasIncapacidad = $"iDiasIncapacidad{sMes:D2}";
                    string propfIImporte = $"fImporte{sMes:D2}";
                    var type = typeof(IncapacidadMBDB);

                    var propertyDias = type.GetProperty(propDiasIncapacidad);
                    var propertyImporte = type.GetProperty(propfIImporte);

                    if (propertyDias != null)
                        propertyDias.SetValue(NIncapacidad, incapacidad.DiasIncapacidad);

                    if (propertyImporte != null)
                        propertyImporte.SetValue(NIncapacidad, incapacidad.ImporteMonetario);

                    lsIncapacidades.Add(NIncapacidad);
                }
                oIncapacidades = new IncapacidadesMBDB
                {
                    lsIncapacidad = lsIncapacidades,
                    fImporteTotal00 = fImporteTotal,
                    iDiasIncapacidadTotal00 = iDiasIncapacidadTotal
                };


                string propDiasIncapacidadTotal = $"iDiasIncapacidad{sMes:D2}";
                string propImporteTotal = $"fImporteTotal{sMes:D2}";


                var typeI = typeof(IncapacidadesMBDB);

                var propertyDiasIncapacidadTotal = typeI.GetProperty(propDiasIncapacidadTotal);
                var propertyImporteTotal = typeI.GetProperty(propImporteTotal);


                if (propertyDiasIncapacidadTotal != null)
                    propertyDiasIncapacidadTotal.SetValue(oIncapacidades, oNominaXML12B.OtrosPagos != null ? iDiasIncapacidadTotal : 0);

                if (propertyImporteTotal != null)
                    propertyImporteTotal.SetValue(oIncapacidades, oNominaXML12B.OtrosPagos != null ? fImporteTotal : 0.00m);



            }
            else
                oIncapacidades = null;


            return Task.FromResult(oIncapacidades);

        }
        #endregion


        #region Metodos dll_nom12a.Nomina 
        public Task<mdl_NominaMBDB> ObtenerData12A(dll_nom12a.Nomina oNominaXML12A, string sMes, decimal fNetoTotal)
        {
            mdl_NominaMBDB oNomina = new mdl_NominaMBDB();
            oNomina.fNetoTotal00 = fNetoTotal;
            oNomina.fTotalPercepciones00 = oNominaXML12A != null ? oNominaXML12A.TotalPercepciones : 0.00m;
            oNomina.fTotalDeducciones00 = oNominaXML12A != null ? oNominaXML12A.TotalDeducciones : 0.00m;
            oNomina.fTotalOtrosPagos00 = oNominaXML12A != null ? oNominaXML12A.TotalOtrosPagos : 0.00m;

            string propTotalPercepciones = $"fTotalPercepciones{sMes:D2}";
            string propTotalDeducciones = $"fTotalDeducciones{sMes:D2}";
            string propTotalOtrosPagos = $"fTotalOtrosPagos{sMes:D2}";
            string propNetoTotal = $"fNetoTotal{sMes:D2}";

            var type = typeof(mdl_NominaMBDB);
            var propertyTotalPercepciones = type.GetProperty(propTotalPercepciones);
            var propertyTotalDeducciones = type.GetProperty(propTotalDeducciones);
            var propertyTotalOtrosPagos = type.GetProperty(propTotalOtrosPagos);
            var propertyNetoTotal = type.GetProperty(propNetoTotal);


            if (propertyTotalPercepciones != null)
                propertyTotalPercepciones.SetValue(oNomina, oNominaXML12A != null ? oNominaXML12A.TotalPercepciones : 0.00m);

            if (propertyTotalDeducciones != null)
                propertyTotalDeducciones.SetValue(oNomina, oNominaXML12A != null ? oNominaXML12A.TotalDeducciones : 0.00m);

            if (propertyTotalOtrosPagos != null)
                propertyTotalOtrosPagos.SetValue(oNomina, oNominaXML12A != null ? oNominaXML12A.TotalOtrosPagos : 0.00m);

            if (propertyNetoTotal != null)
                propertyNetoTotal.SetValue(oNomina, fNetoTotal);



            return Task.FromResult(oNomina);
        }
        public Task<PercepcionesMBDB> ObtenerDataPercepciones12A(dll_nom12a.Nomina oNominaXML12A, string sMes)
        {
            PercepcionesMBDB oPercepcion = null;
            List<PercepcionMBDB> lsPercepcion = new List<PercepcionMBDB>();
            List<HorasExtraMBDB> lsHorasExtra= new List<HorasExtraMBDB>();

            /*Preguntar si esta bien validar varias veces el mismo objeto*/
            if (oNominaXML12A.Percepciones != null)
            {
                PercepcionesMBDB oDataPercepciones = new PercepcionesMBDB
                {
                    fTotalSueldos00 = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSueldos : 0.00m,
                    fTotalGravado00 = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalGravado : 0.00m,
                    fTotalExento00 = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalExento : 0.00m,
                    fTotalJubilacionPensionRetiro00 = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalJubilacionPensionRetiro : 0.00m,
                    fTotalSeparacionIndemnizacion00 = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSeparacionIndemnizacion : 0.00m
                };


                string propTotalSueldos = $"fTotalSueldos{sMes:D2}";
                string propTotalGravado = $"fTotalGravado{sMes:D2}";
                string propTotalExento = $"fTotalExento{sMes:D2}";
                string propJubilacionPensionRetiro = $"fTotalJubilacionPensionRetiro{sMes:D2}";
                string propTotalSeparacionIndemnizacion = $"fTotalSeparacionIndemnizacion{sMes:D2}";

                var typePs = typeof(PercepcionesMBDB);
                var propertyTotalSueldos = typePs.GetProperty(propTotalSueldos);
                var propertyTotalGravado = typePs.GetProperty(propTotalGravado);
                var propertyTotalExento = typePs.GetProperty(propTotalExento);
                var propertyTotalJPR = typePs.GetProperty(propJubilacionPensionRetiro);
                var propertyTotalSI = typePs.GetProperty(propTotalSeparacionIndemnizacion);


                if (propertyTotalSueldos != null)
                    propertyTotalSueldos.SetValue(oDataPercepciones, oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSueldos : 0.00m);

                if (propertyTotalGravado != null)
                    propertyTotalGravado.SetValue(oDataPercepciones, oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalGravado : 0.00m);

                if (propertyTotalExento != null)
                    propertyTotalExento.SetValue(oDataPercepciones, oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalExento : 0.00m);

                if (propertyTotalJPR != null)
                    propertyTotalJPR.SetValue(oDataPercepciones, oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalJubilacionPensionRetiro : 0.00m);


                if (propertyTotalSI != null)
                    propertyTotalSI.SetValue(oDataPercepciones, oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSeparacionIndemnizacion : 0.00m);



                if (oNominaXML12A.Percepciones.Percepcion != null && oNominaXML12A.Percepciones.Percepcion.Length > 0)
                {
                    foreach (var concepto in oNominaXML12A.Percepciones.Percepcion)
                    {
                        var NPercepcion = new PercepcionMBDB
                        {
                            sTipo = concepto.TipoPercepcion,
                            sClave = concepto.Clave,
                            sConcepto = concepto.Concepto,
                            fGravado00 = concepto.ImporteGravado,
                            fExento00 = concepto.ImporteExento
                        };

                        string propGravado = $"fGravado{sMes:D2}";
                        string propExento = $"fExento{sMes:D2}";


                        var typeP = typeof(PercepcionMBDB);

                        var propertyGravado = typeP.GetProperty(propGravado);
                        var propertyExento = typeP.GetProperty(propExento);


                        if (propertyGravado != null)
                        {
                            propertyGravado.SetValue(NPercepcion, concepto.ImporteGravado);
                        }

                        if (propertyExento != null)
                        {
                            propertyExento.SetValue(NPercepcion, concepto.ImporteExento);
                        }


                        if (concepto.AccionesOTitulos != null)
                        {
                            var NAccionesOTitulos = new AccionesOTitulosMBDB
                            {
                                fValorMercado00 = concepto.AccionesOTitulos.ValorMercado,
                                fPrecioAlOtorgarse00 = concepto.AccionesOTitulos.PrecioAlOtorgarse
                            };
                            string propValorMercado = $"fValorMercado{sMes:D2}";
                            string propPrecioAlOtorgarse = $"fPrecioAlOtorgarse{sMes:D2}";


                            var typeAOT = typeof(AccionesOTitulosMBDB);

                            var propertyValorMercado = typeAOT.GetProperty(propGravado);
                            var propertyPrecioAlOtorgarse = typeAOT.GetProperty(propExento);


                            if (propertyValorMercado != null)
                            {
                                propertyGravado.SetValue(NAccionesOTitulos, concepto.AccionesOTitulos.ValorMercado);
                            }

                            if (propertyPrecioAlOtorgarse != null)
                            {
                                propertyPrecioAlOtorgarse.SetValue(NAccionesOTitulos, concepto.AccionesOTitulos.PrecioAlOtorgarse);
                            }
                            oDataPercepciones.oAccionesOTitulos = NAccionesOTitulos;
                        }
                        else
                            oDataPercepciones.oAccionesOTitulos = null;

                        if (concepto.HorasExtra != null && concepto.HorasExtra.Length > 0)
                        {
                            //HorasExtraMBDB NHorasExtra = null;

                 

                            foreach (var horaextra in concepto.HorasExtra)
                            {

                                var NHorasExtra = new HorasExtraMBDB
                                {
                                    sTipoHoraExtra = horaextra.TipoHoras,
                                };

                                string propDias = $"iDias{sMes:D2}";
                                string propHorasExtra = $"iHorasExtra{sMes:D2}";
                                string propImportePagado = $"fImportePagado{sMes:D2}";
                                string propTipoHora = $"sTipoHora{sMes:D2}";



                                var typeHorasExtra = typeof(HoraExtraMBDB);

                                var propertyDias = typeHorasExtra.GetProperty(propDias);
                                var propertyHorasExtra = typeHorasExtra.GetProperty(propHorasExtra);
                                var propertyImportePagado = typeHorasExtra.GetProperty(propImportePagado);
                                var propertyTipoHora = typeHorasExtra.GetProperty(propTipoHora);


                                if (propertyDias != null)
                                {
                                    propertyDias.SetValue(NHorasExtra.oHoraExtra, horaextra.Dias);
                                }

                                if (propertyHorasExtra != null)
                                {
                                    propertyHorasExtra.SetValue(NHorasExtra.oHoraExtra, horaextra.HorasExtra);
                                }

                                if (propertyImportePagado != null)
                                {
                                    propertyImportePagado.SetValue(NHorasExtra.oHoraExtra, horaextra.ImportePagado);
                                }

                                if (propertyTipoHora != null)
                                {
                                    propertyTipoHora.SetValue(NHorasExtra.oHoraExtra, horaextra.TipoHoras);
                                }


                                lsHorasExtra.Add(NHorasExtra);
                             
                            }

                        }
                        else
                            oDataPercepciones.lsHorasExtra = lsHorasExtra;

                        lsPercepcion.Add(NPercepcion);
                    }

                    oDataPercepciones.lsPercepcion = lsPercepcion;

                    if (oNominaXML12A.Percepciones.JubilacionPensionRetiro != null)
                    {
                        var NJubilacionPensionRetiro = new JubilacionPensionRetiroMBDB
                        {
                            fTotalUnaExhibicion00 = oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion,
                            fTotalParcialidad00 = oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalParcialidad,
                            fMontoDiario00 = oNominaXML12A.Percepciones.JubilacionPensionRetiro.MontoDiario,
                            fIngresoAcumulable00 = oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoAcumulable,
                            fIngresoNoAcumulable00 = oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable
                        };

                        string propTotalUnaExhibicion = $"fTotalUnaExhibicion{sMes:D2}";
                        string propTotalParcialidad = $"fTotalParcialidad{sMes:D2}";
                        string propDiario = $"fMontoDiario{sMes:D2}";
                        string propIngresoAcumulable = $"fIngresoAcumulable{sMes:D2}";
                        string propIngresoNoAcumulable = $"fIngresoNoAcumulable{sMes:D2}";


                        var typeJPR = typeof(JubilacionPensionRetiroMBDB);

                        var propertyUnaExhibicion = typeJPR.GetProperty(propTotalUnaExhibicion);
                        var propertyTotalParcialidad = typeJPR.GetProperty(propTotalParcialidad);
                        var propertyDiario = typeJPR.GetProperty(propDiario);
                        var propertyIngresoAcumulable = typeJPR.GetProperty(propIngresoAcumulable);
                        var propertyIngresoNoAcumulable = typeJPR.GetProperty(propIngresoNoAcumulable);


                        if (propertyUnaExhibicion != null)
                        {
                            propertyUnaExhibicion.SetValue(NJubilacionPensionRetiro, oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion);
                        }

                        if (propertyTotalParcialidad != null)
                        {
                            propertyTotalParcialidad.SetValue(NJubilacionPensionRetiro, oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalParcialidad);
                        }

                        if (propertyDiario != null)
                        {
                            propertyDiario.SetValue(NJubilacionPensionRetiro, oNominaXML12A.Percepciones.JubilacionPensionRetiro.MontoDiario);
                        }

                        if (propertyIngresoAcumulable != null)
                        {
                            propertyIngresoAcumulable.SetValue(NJubilacionPensionRetiro, oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoAcumulable);
                        }
                        if (propertyIngresoNoAcumulable != null)
                        {
                            propertyIngresoNoAcumulable.SetValue(NJubilacionPensionRetiro, oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable);
                        }

                        oDataPercepciones.oJubilacionPensionRetiro = NJubilacionPensionRetiro;
                    }
                    else
                        oDataPercepciones.oJubilacionPensionRetiro = null;

                    if (oNominaXML12A.Percepciones.SeparacionIndemnizacion != null)
                    {
                        var NSeparacionIndemnizacion = new SeparacionIndemnizacionMBDB
                        {
                            iNumAniosServicio00 = oNominaXML12A.Percepciones.SeparacionIndemnizacion.NumAñosServicio,
                            fTotalPagado00 = oNominaXML12A.Percepciones.SeparacionIndemnizacion.TotalPagado,
                            fIngresoAcumulable00 = oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoAcumulable,
                            fIngresoNoAcumulable00 = oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable,
                            fUltimoSueldoMensOrd00 = oNominaXML12A.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd,
                        };

                        string propNumServicio = $"iNumAniosServicio{sMes:D2}";
                        string propTotalPagado = $"fTotalPagado{sMes:D2}";
                        string propIngresoAcumulable = $"fIngresoAcumulable{sMes:D2}";
                        string propIngresoNoAcumulable = $"fIngresoNoAcumulable{sMes:D2}";
                        string propUltimoSueldoMesOrd = $"fUltimoSueldoMensOrd{sMes:D2}";


                        var typeJPR = typeof(SeparacionIndemnizacionMBDB);

                        var propertyNumServicio = typeJPR.GetProperty(propNumServicio);
                        var propertyTotalPagado = typeJPR.GetProperty(propTotalPagado);
                        var propertyUltimoSueldoMesOrd = typeJPR.GetProperty(propUltimoSueldoMesOrd);
                        var propertyIngresoAcumulable = typeJPR.GetProperty(propIngresoAcumulable);
                        var propertyIngresoNoAcumulable = typeJPR.GetProperty(propIngresoNoAcumulable);


                        if (propertyNumServicio != null)
                        {
                            propertyNumServicio.SetValue(NSeparacionIndemnizacion, oNominaXML12A.Percepciones.SeparacionIndemnizacion.NumAñosServicio);
                        }

                        if (propertyTotalPagado != null)
                        {
                            propertyTotalPagado.SetValue(NSeparacionIndemnizacion, oNominaXML12A.Percepciones.SeparacionIndemnizacion.TotalPagado);
                        }

                        if (propertyUltimoSueldoMesOrd != null)
                        {
                            propertyUltimoSueldoMesOrd.SetValue(NSeparacionIndemnizacion, oNominaXML12A.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd);
                        }

                        if (propertyIngresoAcumulable != null)
                        {
                            propertyIngresoAcumulable.SetValue(NSeparacionIndemnizacion, oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoAcumulable);
                        }
                        if (propertyIngresoNoAcumulable != null)
                        {
                            propertyIngresoNoAcumulable.SetValue(NSeparacionIndemnizacion, oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable);
                        }
                        oDataPercepciones.oSeparacionIndemnizacion = NSeparacionIndemnizacion;
                    }
                    else
                        oDataPercepciones.oSeparacionIndemnizacion = null;

                    oPercepcion = oDataPercepciones;
                    oDataPercepciones = null;
                }
            }


            return Task.FromResult(oPercepcion);
        }

        public Task<DeduccionesMBDB> ObtenerDataDeducciones12A(dll_nom12a.Nomina oNominaXML12A, string sMes)
        {
            DeduccionesMBDB oDeducciones = null;
            List<DeduccionMBDB> lsDeducciones = new List<DeduccionMBDB>();
            if (oNominaXML12A.Deducciones != null)
            {

                DeduccionesMBDB oDeduccion = new DeduccionesMBDB
                {

                    TotalOtrasDeducciones00 = oNominaXML12A.Deducciones.TotalOtrasDeducciones,
                    TotalImpuestosRetenidos00 = oNominaXML12A.Deducciones.TotalImpuestosRetenidos
                };

                string propOtrasDeducciones = $"TotalOtrasDeducciones{sMes:D2}";
                string propImpuestosRetenidos = $"TotalImpuestosRetenidos{sMes:D2}";


                var typeDs = typeof(DeduccionesMBDB);
                var propertyOtrasDeducciones = typeDs.GetProperty(propOtrasDeducciones);
                var propertyImpuestosRetenidos = typeDs.GetProperty(propImpuestosRetenidos);


                if (propertyOtrasDeducciones != null)
                    propertyOtrasDeducciones.SetValue(oDeduccion, oNominaXML12A != null ? oNominaXML12A.Deducciones.TotalOtrasDeducciones : 0.00m);

                if (propertyImpuestosRetenidos != null)
                    propertyImpuestosRetenidos.SetValue(oDeduccion, oNominaXML12A != null ? oNominaXML12A.Deducciones.TotalImpuestosRetenidos : 0.00m);



                if (oNominaXML12A.Deducciones.Deduccion != null && oNominaXML12A.Deducciones.Deduccion.Length > 0)
                {
                    foreach (var deduccion in oNominaXML12A.Deducciones.Deduccion)
                    {
                        var NDeduccion = new DeduccionMBDB
                        {
                            sTipo = deduccion.TipoDeduccion,
                            sClave = deduccion.Clave,
                            sConcepto = deduccion.Concepto,
                            fImporte00 = deduccion.Importe
                        };

                        string propImporte = $"fImporte{sMes:D2}";


                        var type = typeof(DeduccionMBDB);

                        var propertyImporte = type.GetProperty(propImporte);


                        if (propertyImporte != null)
                        {
                            propertyImporte.SetValue(NDeduccion, deduccion.Importe);
                        }
                        lsDeducciones.Add(NDeduccion);
                    }
                    oDeduccion.lsDeduccion = lsDeducciones;
                }
                else
                    oDeduccion.lsDeduccion = null;



                oDeducciones = oDeduccion;
                oDeduccion = null;

            }

            return Task.FromResult(oDeducciones);
        }

        public Task<OtrosPagosMBDB> ObtenerDataOtrosPagos12A(dll_nom12a.Nomina oNominaXML12A, string sMes)
        {
            var oOtrosPagos = new OtrosPagosMBDB();
            SubsidioAlEmpleoMBDB oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB();
            CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor = null;
            List<OtroPagoMBDB> lsOtroPago = new List<OtroPagoMBDB>();
            decimal fImporteTotal = 0.00m;
            decimal fSubsidioCausadoTotal = 0.00m;

            if (oNominaXML12A.OtrosPagos != null)
            {


                foreach (var otroPago in oNominaXML12A.OtrosPagos)
                {
                    fImporteTotal += otroPago.Importe;
                    fSubsidioCausadoTotal += otroPago.SubsidioAlEmpleo != null ? otroPago.SubsidioAlEmpleo.SubsidioCausado : 0.00m;
                    var NOtroPago = new OtroPagoMBDB
                    {
                        sTipo = otroPago.TipoOtroPago.ToString(),
                        sClave = otroPago.Clave.ToString(),
                        sConcepto = otroPago.Concepto.ToString(),
                        fImporte00 = otroPago.Importe

                    };

                    string propImporte = $"fImporte{sMes:D2}";
                    var type = typeof(OtroPagoMBDB);

                    var propertyImporte = type.GetProperty(propImporte);

                    if (propertyImporte != null)
                        propertyImporte.SetValue(NOtroPago, otroPago.Importe);
                    lsOtroPago.Add(NOtroPago);



                    if (otroPago.CompensacionSaldosAFavor != null)
                    {
                        oCompensacionSaldoAFavor = new CompensacionSaldoAFavorMBDB
                        {
                            fRemanenteSalFav00 = otroPago.CompensacionSaldosAFavor.RemanenteSalFav,
                            fSaldoAFavor00 = otroPago.CompensacionSaldosAFavor.SaldoAFavor,

                        };

                        string propRemanenteSalFav = $"fRemanenteSalFav{sMes:D2}";
                        string propfSaldoAFavor = $"fSaldoAFavor{sMes:D2}";
                        var typeCSAF = typeof(CompensacionSaldoAFavorMBDB);

                        var propertyRemanente = typeCSAF.GetProperty(propRemanenteSalFav);
                        var propertySaldoAFavor = typeCSAF.GetProperty(propfSaldoAFavor);

                        if (propertyRemanente != null)
                            propertyRemanente.SetValue(oCompensacionSaldoAFavor, otroPago.CompensacionSaldosAFavor.RemanenteSalFav);

                        if (propertySaldoAFavor != null)
                            propertyRemanente.SetValue(oCompensacionSaldoAFavor, otroPago.CompensacionSaldosAFavor.SaldoAFavor);

                    }
                    else
                        oCompensacionSaldoAFavor = null;

                    if (otroPago.SubsidioAlEmpleo != null)
                    {
                        oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB
                        {
                            fSubsidioCausado00 = otroPago.SubsidioAlEmpleo.SubsidioCausado
                        };

                        string propSubsidioCausado = $"fSubsidioCausado{sMes:D2}";



                        var typeSAE = typeof(SubsidioAlEmpleoMBDB);

                        var propertySubsidioCausado = typeSAE.GetProperty(propSubsidioCausado);


                        if (propertySubsidioCausado != null)
                            propertySubsidioCausado.SetValue(oSubsidioAlEmpleo, otroPago.SubsidioAlEmpleo.SubsidioCausado);

                    }


                }

                var oDataOtrosPagos = new OtrosPagosMBDB
                {
                    lsOtroPago = lsOtroPago,
                    fImporteTotal00 = fImporteTotal,
                    fSubsidioCausadoTotal00 = fSubsidioCausadoTotal,
                    oCompensacionSaldoAFavor = oCompensacionSaldoAFavor,
                    oSubsidioAlEmpleo = oSubsidioAlEmpleo
                };



                string propSubsidioCausadoTotal = $"fSubsidioCausadoTotal{sMes:D2}";
                string propImporteTotal = $"fImporteTotal{sMes:D2}";


                var typeOP = typeof(OtrosPagosMBDB);
                var propertySubsidioCausadoTotal = typeOP.GetProperty(propSubsidioCausadoTotal);
                var propertyImporteTotal = typeOP.GetProperty(propImporteTotal);


                if (propertySubsidioCausadoTotal != null)
                    propertySubsidioCausadoTotal.SetValue(oDataOtrosPagos, oNominaXML12A.OtrosPagos != null ? fSubsidioCausadoTotal : 0.00m);

                if (propertyImporteTotal != null)
                    propertyImporteTotal.SetValue(oDataOtrosPagos, oNominaXML12A.OtrosPagos != null ? fImporteTotal : 0.00m);


                oOtrosPagos = oDataOtrosPagos;
                oDataOtrosPagos = null;
            }
            else
            { oOtrosPagos = null; }

            return Task.FromResult(oOtrosPagos);

        }

        public Task<IncapacidadesMBDB> ObtenerDataIncapacidades12A(dll_nom12a.Nomina oNominaXML12A, string sMes)
        {

            IncapacidadesMBDB oIncapacidades = null;
            List<IncapacidadMBDB> lsIncapacidades = new List<IncapacidadMBDB>();
            decimal fImporteTotal = 0.00m;
            int iDiasIncapacidadTotal = 0;



            if (oNominaXML12A.Incapacidades != null)
            {
                foreach (var incapacidad in oNominaXML12A.Incapacidades)
                {
                    var NIncapacidad = new IncapacidadMBDB
                    {
                        sTipoIncapacidad = incapacidad.TipoIncapacidad,
                        iDiasIncapacidad00 = incapacidad.DiasIncapacidad,
                        fImporte00 = incapacidad.ImporteMonetario
                    };
                    fImporteTotal += incapacidad.ImporteMonetario;
                    iDiasIncapacidadTotal += incapacidad.DiasIncapacidad;

                    string propDiasIncapacidad = $"iDiasIncapacidad{sMes:D2}";
                    string propfIImporte = $"fImporte{sMes:D2}";
                    var type = typeof(IncapacidadMBDB);

                    var propertyDias = type.GetProperty(propDiasIncapacidad);
                    var propertyImporte = type.GetProperty(propfIImporte);

                    if (propertyDias != null)
                        propertyDias.SetValue(NIncapacidad, incapacidad.DiasIncapacidad);

                    if (propertyImporte != null)
                        propertyImporte.SetValue(NIncapacidad, incapacidad.ImporteMonetario);

                    lsIncapacidades.Add(NIncapacidad);
                }
                oIncapacidades = new IncapacidadesMBDB
                {
                    lsIncapacidad = lsIncapacidades,
                    fImporteTotal00 = fImporteTotal,
                    iDiasIncapacidadTotal00 = iDiasIncapacidadTotal
                };


                string propDiasIncapacidadTotal = $"iDiasIncapacidad{sMes:D2}";
                string propImporteTotal = $"fImporteTotal{sMes:D2}";


                var typeI = typeof(IncapacidadesMBDB);

                var propertyDiasIncapacidadTotal = typeI.GetProperty(propDiasIncapacidadTotal);
                var propertyImporteTotal = typeI.GetProperty(propImporteTotal);


                if (propertyDiasIncapacidadTotal != null)
                    propertyDiasIncapacidadTotal.SetValue(oIncapacidades, oNominaXML12A.OtrosPagos != null ? iDiasIncapacidadTotal : 0);

                if (propertyImporteTotal != null)
                    propertyImporteTotal.SetValue(oIncapacidades, oNominaXML12A.OtrosPagos != null ? fImporteTotal : 0.00m);



            }
            else
                oIncapacidades = null;


            return Task.FromResult(oIncapacidades);

        }

        #endregion
    }
}
