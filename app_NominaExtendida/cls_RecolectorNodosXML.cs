using app_NominaExtendida.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_NominaExtendida
{
    internal class cls_RecolectorNodosXML
    {
        public static int iMes = 0;

        public cls_RecolectorNodosXML(int mes)
        {
            iMes =mes;
        }

        #region Metodos
        public Task<mdl_NominaMBDB> ObtenerDataNomina12C(Nom12C.Nomina oNominaXML12C, string sMes, decimal fNetoTotal)
        {
            mdl_NominaMBDB oNomina = new mdl_NominaMBDB();
            oNomina.afNetoTotal[0] = fNetoTotal;
            oNomina.afTotalPercepciones[0] = oNominaXML12C != null ? oNominaXML12C.TotalPercepciones : 0.00m;
            oNomina.afTotalDeducciones[0] = oNominaXML12C != null ? oNominaXML12C.TotalDeducciones : 0.00m;
            oNomina.afTotalOtrosPagos[0] = oNominaXML12C != null ? oNominaXML12C.TotalOtrosPagos : 0.00m;
            oNomina.afNetoTotal[iMes] = fNetoTotal;
            oNomina.afTotalPercepciones[iMes] = oNominaXML12C != null ? oNominaXML12C.TotalPercepciones : 0.00m;
            oNomina.afTotalDeducciones[iMes] = oNominaXML12C != null ? oNominaXML12C.TotalDeducciones : 0.00m;
            oNomina.afTotalOtrosPagos[iMes] = oNominaXML12C != null ? oNominaXML12C.TotalOtrosPagos : 0.00m;

         

            return Task.FromResult(oNomina);
        }

        public Task<mdl_PercepcionesMBDB> ObtenerDataPercepciones(Nom12C.Nomina oNominaXML12C, string sMes)
        {
            mdl_PercepcionesMBDB oPercepcion = null;
            List<mdl_PercepcionMBDB> lsPercepcion = new List<mdl_PercepcionMBDB>();
            List<mdl_HorasExtraMBDB> lsHorasExtra = new List<mdl_HorasExtraMBDB>();
            /*Preguntar si esta bien validar varias veces el mismo objeto*/
            if (oNominaXML12C.Percepciones != null)
            {
                mdl_PercepcionesMBDB oDataPercepciones = new mdl_PercepcionesMBDB();

                oDataPercepciones.afTotalSueldos[0] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSueldos : 0.00m;
                oDataPercepciones.afTotalSueldos[0] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalGravado : 0.00m;
                oDataPercepciones.afTotalExento[0] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalExento : 0.00m;
                oDataPercepciones.afTotalJubilacionPensionRetiro[0] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalJubilacionPensionRetiro : 0.00m;
                oDataPercepciones.afTotalSeparacionIndemnizacion[0] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSeparacionIndemnizacion : 0.00m;

                oDataPercepciones.afTotalSueldos[iMes] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSueldos : 0.00m;
                oDataPercepciones.afTotalSueldos[iMes] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalGravado : 0.00m;
                oDataPercepciones.afTotalExento[iMes] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalExento : 0.00m;
                oDataPercepciones.afTotalJubilacionPensionRetiro[iMes] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalJubilacionPensionRetiro : 0.00m;
                oDataPercepciones.afTotalSeparacionIndemnizacion[iMes] = oNominaXML12C.Percepciones != null ? oNominaXML12C.Percepciones.TotalSeparacionIndemnizacion : 0.00m;






                if (oNominaXML12C.Percepciones.Percepcion != null && oNominaXML12C.Percepciones.Percepcion.Length > 0)
                {
                    foreach (var concepto in oNominaXML12C.Percepciones.Percepcion)
                    {
                        var NPercepcion = new mdl_PercepcionMBDB
                        {
                            sTipo = concepto.TipoPercepcion,
                            sClave = concepto.Clave,
                            sConcepto = concepto.Concepto,

                        };

                        NPercepcion.afGravado[0] = concepto.ImporteGravado;
                        NPercepcion.afExento[0] = concepto.ImporteExento;


                        NPercepcion.afGravado[iMes] = concepto.ImporteGravado;
                        NPercepcion.afExento[iMes] = concepto.ImporteExento;



                        if (concepto.AccionesOTitulos != null)
                        {
                            var NAccionesOTitulos = new mdl_AccionesOTitulosMBDB();

                            NAccionesOTitulos.afValorMercado[0] = concepto.AccionesOTitulos.ValorMercado;
                            NAccionesOTitulos.afPrecioAlOtorgarse[0] = concepto.AccionesOTitulos.PrecioAlOtorgarse;

                            NAccionesOTitulos.afValorMercado[iMes] = concepto.AccionesOTitulos.ValorMercado;
                            NAccionesOTitulos.afPrecioAlOtorgarse[iMes] = concepto.AccionesOTitulos.PrecioAlOtorgarse;


                            oDataPercepciones.oAccionesOTitulos = NAccionesOTitulos;
                        }
                        else
                            oDataPercepciones.oAccionesOTitulos = null;

                        if (concepto.HorasExtra != null && concepto.HorasExtra.Length > 0)
                        {
                            //mdl_HorasExtraMBDB NHorasExtra = null;



                            foreach (var horaextra in concepto.HorasExtra)
                            {

                                var NHorasExtra = new mdl_HorasExtraMBDB
                                {
                                    sTipoHoraExtra = horaextra.TipoHoras,
                                };

                             
                                NHorasExtra.oHoraExtra.aiHorasExtra[0] = horaextra.HorasExtra;
                                NHorasExtra.oHoraExtra.aiDias[0] = horaextra.Dias;
                                NHorasExtra.oHoraExtra.afImportePagado[0] = horaextra.ImportePagado;

                                 NHorasExtra.oHoraExtra.aiHorasExtra[iMes] = horaextra.HorasExtra;
                                NHorasExtra.oHoraExtra.aiDias[iMes] = horaextra.Dias;
                                NHorasExtra.oHoraExtra.afImportePagado[iMes] = horaextra.ImportePagado;

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
                        var NJubilacionPensionRetiro = new mdl_JubilacionPensionRetiroMBDB();

                        NJubilacionPensionRetiro.afTotalUnaExhibicion[0] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion;
                        NJubilacionPensionRetiro.afTotalParcialidad[0] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalParcialidad;
                        NJubilacionPensionRetiro.afMontoDiario[0] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.MontoDiario;
                        NJubilacionPensionRetiro.afIngresoAcumulable[0] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoAcumulable;
                        NJubilacionPensionRetiro.afIngresoNoAcumulable[0] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable;

                        NJubilacionPensionRetiro.afTotalUnaExhibicion[iMes] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion;
                        NJubilacionPensionRetiro.afTotalParcialidad[iMes] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.TotalParcialidad;
                        NJubilacionPensionRetiro.afMontoDiario[iMes] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.MontoDiario;
                        NJubilacionPensionRetiro.afIngresoAcumulable[iMes] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoAcumulable;
                        NJubilacionPensionRetiro.afIngresoNoAcumulable[iMes] = oNominaXML12C.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable;



                        oDataPercepciones.oJubilacionPensionRetiro = NJubilacionPensionRetiro;
                    }
                    else
                        oDataPercepciones.oJubilacionPensionRetiro = null;

                    if (oNominaXML12C.Percepciones.SeparacionIndemnizacion != null)
                    {
                        var NSeparacionIndemnizacion = new mdl_SeparacionIndemnizacionMBDB();

                        NSeparacionIndemnizacion.aiNumAniosServicio[0] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.NumAñosServicio;
                        NSeparacionIndemnizacion.afTotalPagado[0] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.TotalPagado;
                        NSeparacionIndemnizacion.afIngresoAcumulable[0] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoAcumulable;
                        NSeparacionIndemnizacion.afIngresoNoAcumulable[0] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable;
                        NSeparacionIndemnizacion.afUltimoSueldoMensOrd[0] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd;

                        NSeparacionIndemnizacion.aiNumAniosServicio[iMes] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.NumAñosServicio;
                        NSeparacionIndemnizacion.afTotalPagado[iMes] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.TotalPagado;
                        NSeparacionIndemnizacion.afIngresoAcumulable[iMes] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoAcumulable;
                        NSeparacionIndemnizacion.afIngresoNoAcumulable[iMes] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable;
                        NSeparacionIndemnizacion.afUltimoSueldoMensOrd[iMes] = oNominaXML12C.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd;


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

        public Task<mdl_DeduccionesMBDB> ObtenerDataDeducciones(Nom12C.Nomina oNominaXML12C, string sMes)
        {
            mdl_DeduccionesMBDB oDeducciones = null;
            List<mdl_DeduccionMBDB> lsDeducciones = new List<mdl_DeduccionMBDB>();
            if (oNominaXML12C.Deducciones != null)
            {

                mdl_DeduccionesMBDB oDeduccion = new mdl_DeduccionesMBDB();


                oDeduccion.afTotalOtrasDeducciones[0] = oNominaXML12C.Deducciones.TotalOtrasDeducciones;
                oDeduccion.afTotalImpuestosRetenidos[0] = oNominaXML12C.Deducciones.TotalImpuestosRetenidos;


                oDeduccion.afTotalOtrasDeducciones[iMes] = oNominaXML12C.Deducciones.TotalOtrasDeducciones;
                oDeduccion.afTotalImpuestosRetenidos[iMes] = oNominaXML12C.Deducciones.TotalImpuestosRetenidos;


                if (oNominaXML12C.Deducciones.Deduccion != null && oNominaXML12C.Deducciones.Deduccion.Length > 0)
                {
                    foreach (var deduccion in oNominaXML12C.Deducciones.Deduccion)
                    {
                        var NDeduccion = new mdl_DeduccionMBDB
                        {
                            sTipo = deduccion.TipoDeduccion,
                            sClave = deduccion.Clave,
                            sConcepto = deduccion.Concepto,
                            
                        };
                        NDeduccion.afImporte[0] = deduccion.Importe;
                        NDeduccion.afImporte[iMes] = deduccion.Importe;
                        
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

        public Task<mdl_OtrosPagosMBDB> ObtenerDataOtrosPagos(Nom12C.Nomina oNominaXML12C, string sMes)
        {
            var oOtrosPagos = new mdl_OtrosPagosMBDB();
            mdl_SubsidioAlEmpleoMBDB oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();
            mdl_CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor = null;
            List<mdl_OtroPagoMBDB> lsOtroPago = new List<mdl_OtroPagoMBDB>();
            decimal fImporteTotal = 0.00m;
            decimal fSubsidioCausadoTotal = 0.00m;

            if (oNominaXML12C.OtrosPagos != null)
            {


                foreach (var otroPago in oNominaXML12C.OtrosPagos)
                {
                    fImporteTotal += otroPago.Importe;
                    fSubsidioCausadoTotal += otroPago.SubsidioAlEmpleo != null ? otroPago.SubsidioAlEmpleo.SubsidioCausado : 0.00m;
                    var NOtroPago = new mdl_OtroPagoMBDB
                    {
                        sTipo = otroPago.TipoOtroPago.ToString(),
                        sClave = otroPago.Clave.ToString(),
                        sConcepto = otroPago.Concepto.ToString(),
                       

                    };
                    NOtroPago.afImporte[0] = otroPago.Importe;
                    NOtroPago.afImporte[iMes] = otroPago.Importe;
                    
                    lsOtroPago.Add(NOtroPago);




                    if (otroPago.CompensacionSaldosAFavor != null)
                    {
                        oCompensacionSaldoAFavor = new mdl_CompensacionSaldoAFavorMBDB();

                        oCompensacionSaldoAFavor.afRemanenteSalFav[0] = otroPago.CompensacionSaldosAFavor.RemanenteSalFav;
                        oCompensacionSaldoAFavor.afSaldoAFavor[0] = otroPago.CompensacionSaldosAFavor.SaldoAFavor;
                        oCompensacionSaldoAFavor.afRemanenteSalFav[iMes] = otroPago.CompensacionSaldosAFavor.RemanenteSalFav;
                        oCompensacionSaldoAFavor.afSaldoAFavor[iMes] = otroPago.CompensacionSaldosAFavor.SaldoAFavor;




                    }
                    else
                        oCompensacionSaldoAFavor = null;

                    if (otroPago.SubsidioAlEmpleo != null)
                    {
                        oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();

                        oSubsidioAlEmpleo.afSubsidioCausado[0] = otroPago.SubsidioAlEmpleo.SubsidioCausado;
                        oSubsidioAlEmpleo.afSubsidioCausado[iMes] = otroPago.SubsidioAlEmpleo.SubsidioCausado;


                    }


                }


                var oDataOtrosPagos = new mdl_OtrosPagosMBDB
                {
                    lsOtroPago = lsOtroPago,

                    oCompensacionSaldoAFavor = oCompensacionSaldoAFavor,
                    oSubsidioAlEmpleo = oSubsidioAlEmpleo
                };

                oDataOtrosPagos.afImporteTotal[0] = fImporteTotal;
                oDataOtrosPagos.afSubsidioCausadoTotal[0] = fSubsidioCausadoTotal;

                oDataOtrosPagos.afImporteTotal[iMes] = fImporteTotal;
                oDataOtrosPagos.afSubsidioCausadoTotal[iMes] = fSubsidioCausadoTotal;


                oOtrosPagos = oDataOtrosPagos;
                oDataOtrosPagos = null;
            }
            else
            { oOtrosPagos = null; }

            return Task.FromResult(oOtrosPagos);

        }

        public Task<mdl_IncapacidadesMBDB> ObtenerDataIncapacidades(Nom12C.Nomina oNominaXML12C, string sMes)
        {

            mdl_IncapacidadesMBDB oIncapacidades = null;
            List<mdl_IncapacidadMBDB> lsIncapacidades = new List<mdl_IncapacidadMBDB>();
            decimal fImporteTotal = 0.00m;
            int iDiasIncapacidadTotal = 0;



            if (oNominaXML12C.Incapacidades != null)
            {
                foreach (var incapacidad in oNominaXML12C.Incapacidades)
                {
                    var NIncapacidad = new mdl_IncapacidadMBDB
                    {
                        sTipoIncapacidad = incapacidad.TipoIncapacidad,

                    };

                    NIncapacidad.aiDiasIncapacidad[0] = incapacidad.DiasIncapacidad;
                    NIncapacidad.afImporte[0] = incapacidad.ImporteMonetario;
                    NIncapacidad.aiDiasIncapacidad[iMes] = incapacidad.DiasIncapacidad;
                    NIncapacidad.afImporte[iMes] = incapacidad.ImporteMonetario;

                    fImporteTotal += incapacidad.ImporteMonetario;
                    iDiasIncapacidadTotal += incapacidad.DiasIncapacidad;

                    lsIncapacidades.Add(NIncapacidad);
                }
                oIncapacidades = new mdl_IncapacidadesMBDB
                {
                    lsIncapacidad = lsIncapacidades,

                };
                oIncapacidades.afImporteTotal[0] = fImporteTotal;
                oIncapacidades.aiDiasIncapacidadTotal[0] = iDiasIncapacidadTotal;
                oIncapacidades.afImporteTotal[iMes] = fImporteTotal;
                oIncapacidades.aiDiasIncapacidadTotal[iMes] = iDiasIncapacidadTotal;

                


            }
            else
                oIncapacidades = null;


            return Task.FromResult(oIncapacidades);

        }
        #endregion

        #region Metodos Nom12B.Nomina 
        public Task<mdl_NominaMBDB> ObtenerDataNomina12B(Nom12B.Nomina oNominaXML12B, string sMes, decimal fNetoTotal)
        {
            mdl_NominaMBDB oNomina = new mdl_NominaMBDB();
            oNomina.afNetoTotal[0] = fNetoTotal;
            oNomina.afTotalPercepciones[0] = oNominaXML12B != null ? oNominaXML12B.TotalPercepciones : 0.00m;
            oNomina.afTotalDeducciones[0] = oNominaXML12B != null ? oNominaXML12B.TotalDeducciones : 0.00m;
            oNomina.afTotalOtrosPagos[0] = oNominaXML12B != null ? oNominaXML12B.TotalOtrosPagos : 0.00m;

        
            oNomina.afNetoTotal[iMes] = fNetoTotal;
            oNomina.afTotalPercepciones[iMes] = oNominaXML12B != null ? oNominaXML12B.TotalPercepciones : 0.00m;
            oNomina.afTotalDeducciones[iMes] = oNominaXML12B != null ? oNominaXML12B.TotalDeducciones : 0.00m;
            oNomina.afTotalOtrosPagos[iMes] = oNominaXML12B != null ? oNominaXML12B.TotalOtrosPagos : 0.00m;

        

            return Task.FromResult(oNomina);
        }

        public Task<mdl_PercepcionesMBDB> ObtenerDataPercepciones12B(Nom12B.Nomina oNominaXML12B, string sMes)
        {
            mdl_PercepcionesMBDB oPercepcion = null;
            List<mdl_PercepcionMBDB> lsPercepcion = new List<mdl_PercepcionMBDB>();
            List<mdl_HorasExtraMBDB> lsHorasExtra = new List<mdl_HorasExtraMBDB>();
            /*Preguntar si esta bien validar varias veces el mismo objeto*/
            if (oNominaXML12B.Percepciones != null)
            {
                mdl_PercepcionesMBDB oDataPercepciones = new mdl_PercepcionesMBDB();

                oDataPercepciones.afTotalSueldos[0] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSueldos : 0.00m;
                oDataPercepciones.afTotalSueldos[0] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalGravado : 0.00m;
                oDataPercepciones.afTotalExento[0] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalExento : 0.00m;
                oDataPercepciones.afTotalJubilacionPensionRetiro[0] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalJubilacionPensionRetiro : 0.00m;
                oDataPercepciones.afTotalSeparacionIndemnizacion[0] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSeparacionIndemnizacion : 0.00m;


               

                oDataPercepciones.afTotalSueldos[iMes] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSueldos : 0.00m;
                oDataPercepciones.afTotalSueldos[iMes] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalGravado : 0.00m;
                oDataPercepciones.afTotalExento[iMes] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalExento : 0.00m;
                oDataPercepciones.afTotalJubilacionPensionRetiro[iMes] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalJubilacionPensionRetiro : 0.00m;
                oDataPercepciones.afTotalSeparacionIndemnizacion[iMes] = oNominaXML12B.Percepciones != null ? oNominaXML12B.Percepciones.TotalSeparacionIndemnizacion : 0.00m;


               


                if (oNominaXML12B.Percepciones.Percepcion != null && oNominaXML12B.Percepciones.Percepcion.Length > 0)
                {
                    foreach (var concepto in oNominaXML12B.Percepciones.Percepcion)
                    {
                        var NPercepcion = new mdl_PercepcionMBDB
                        {
                            sTipo = concepto.TipoPercepcion,
                            sClave = concepto.Clave,
                            sConcepto = concepto.Concepto,

                        };
                        NPercepcion.afGravado[0] = concepto.ImporteGravado;
                        NPercepcion.afExento[0] = concepto.ImporteExento;

                        NPercepcion.afGravado[iMes] = concepto.ImporteGravado;
                        NPercepcion.afExento[iMes] = concepto.ImporteExento;


                        if (concepto.AccionesOTitulos != null)
                        {
                            var NAccionesOTitulos = new mdl_AccionesOTitulosMBDB();

                            NAccionesOTitulos.afValorMercado[0] = concepto.AccionesOTitulos.ValorMercado;
                            NAccionesOTitulos.afPrecioAlOtorgarse[0] = concepto.AccionesOTitulos.PrecioAlOtorgarse;


                            NAccionesOTitulos.afValorMercado[iMes] = concepto.AccionesOTitulos.ValorMercado;
                            NAccionesOTitulos.afPrecioAlOtorgarse[iMes] = concepto.AccionesOTitulos.PrecioAlOtorgarse;


                            oDataPercepciones.oAccionesOTitulos = NAccionesOTitulos;
                        }
                        else
                            oDataPercepciones.oAccionesOTitulos = null;

                        if (concepto.HorasExtra != null && concepto.HorasExtra.Length > 0)
                        {
                            //mdl_HorasExtraMBDB NHorasExtra = null;



                            foreach (var horaextra in concepto.HorasExtra)
                            {

                                var NHorasExtra = new mdl_HorasExtraMBDB
                                {
                                    sTipoHoraExtra = horaextra.TipoHoras,
                                };



                                NHorasExtra.oHoraExtra.aiHorasExtra[0] = horaextra.HorasExtra;
                                NHorasExtra.oHoraExtra.aiDias[0] = horaextra.Dias;
                                NHorasExtra.oHoraExtra.afImportePagado[0] = horaextra.ImportePagado;


                                NHorasExtra.oHoraExtra.aiHorasExtra[iMes] = horaextra.HorasExtra;
                                NHorasExtra.oHoraExtra.aiDias[iMes] = horaextra.Dias;
                                NHorasExtra.oHoraExtra.afImportePagado[iMes] = horaextra.ImportePagado;

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
                        var NJubilacionPensionRetiro = new mdl_JubilacionPensionRetiroMBDB();

                        NJubilacionPensionRetiro.afTotalUnaExhibicion[0] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion;
                        NJubilacionPensionRetiro.afTotalParcialidad[0] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalParcialidad;
                        NJubilacionPensionRetiro.afMontoDiario[0] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.MontoDiario;
                        NJubilacionPensionRetiro.afIngresoAcumulable[0] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoAcumulable;
                        NJubilacionPensionRetiro.afIngresoNoAcumulable[0] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable;


                       
                        NJubilacionPensionRetiro.afTotalUnaExhibicion[iMes] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion;
                        NJubilacionPensionRetiro.afTotalParcialidad[iMes] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.TotalParcialidad;
                        NJubilacionPensionRetiro.afMontoDiario[iMes] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.MontoDiario;
                        NJubilacionPensionRetiro.afIngresoAcumulable[iMes] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoAcumulable;
                        NJubilacionPensionRetiro.afIngresoNoAcumulable[iMes] = oNominaXML12B.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable;


                       

                        oDataPercepciones.oJubilacionPensionRetiro = NJubilacionPensionRetiro;
                    }
                    else
                        oDataPercepciones.oJubilacionPensionRetiro = null;

                    if (oNominaXML12B.Percepciones.SeparacionIndemnizacion != null)
                    {
                        var NSeparacionIndemnizacion = new mdl_SeparacionIndemnizacionMBDB();

                        NSeparacionIndemnizacion.aiNumAniosServicio[0] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.NumAñosServicio;
                        NSeparacionIndemnizacion.afTotalPagado[0] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.TotalPagado;
                        NSeparacionIndemnizacion.afIngresoAcumulable[0] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoAcumulable;
                        NSeparacionIndemnizacion.afIngresoNoAcumulable[0] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable;
                        NSeparacionIndemnizacion.afUltimoSueldoMensOrd[0] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd;

                        NSeparacionIndemnizacion.aiNumAniosServicio[iMes] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.NumAñosServicio;
                        NSeparacionIndemnizacion.afTotalPagado[iMes] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.TotalPagado;
                        NSeparacionIndemnizacion.afIngresoAcumulable[iMes] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoAcumulable;
                        NSeparacionIndemnizacion.afIngresoNoAcumulable[iMes] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable;
                        NSeparacionIndemnizacion.afUltimoSueldoMensOrd[iMes] = oNominaXML12B.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd;


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

        public Task<mdl_DeduccionesMBDB> ObtenerDataDeducciones12B(Nom12B.Nomina oNominaXML12B, string sMes)
        {
            mdl_DeduccionesMBDB oDeducciones = null;
            List<mdl_DeduccionMBDB> lsDeducciones = new List<mdl_DeduccionMBDB>();
            if (oNominaXML12B.Deducciones != null)
            {

                mdl_DeduccionesMBDB oDeduccion = new mdl_DeduccionesMBDB();

                oDeduccion.afTotalOtrasDeducciones[0] = oNominaXML12B.Deducciones.TotalOtrasDeducciones;
                oDeduccion.afTotalImpuestosRetenidos[0] = oNominaXML12B.Deducciones.TotalImpuestosRetenidos;

                oDeduccion.afTotalOtrasDeducciones[iMes] = oNominaXML12B.Deducciones.TotalOtrasDeducciones;
                oDeduccion.afTotalImpuestosRetenidos[iMes] = oNominaXML12B.Deducciones.TotalImpuestosRetenidos;



                if (oNominaXML12B.Deducciones.Deduccion != null && oNominaXML12B.Deducciones.Deduccion.Length > 0)
                {
                    foreach (var deduccion in oNominaXML12B.Deducciones.Deduccion)
                    {
                        var NDeduccion = new mdl_DeduccionMBDB
                        {
                            sTipo = deduccion.TipoDeduccion,
                            sClave = deduccion.Clave,
                            sConcepto = deduccion.Concepto,

                        };
                        NDeduccion.afImporte[0] = deduccion.Importe;
                        NDeduccion.afImporte[iMes] = deduccion.Importe;

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

        public Task<mdl_OtrosPagosMBDB> ObtenerDataOtrosPagos12B(Nom12B.Nomina oNominaXML12B, string sMes)
        {
            var oOtrosPagos = new mdl_OtrosPagosMBDB();
            mdl_SubsidioAlEmpleoMBDB oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();
            mdl_CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor = null;
            List<mdl_OtroPagoMBDB> lsOtroPago = new List<mdl_OtroPagoMBDB>();
            decimal fImporteTotal = 0.00m;
            decimal fSubsidioCausadoTotal = 0.00m;

            if (oNominaXML12B.OtrosPagos != null)
            {


                foreach (var otroPago in oNominaXML12B.OtrosPagos)
                {
                    fImporteTotal += otroPago.Importe;
                    fSubsidioCausadoTotal += otroPago.SubsidioAlEmpleo != null ? otroPago.SubsidioAlEmpleo.SubsidioCausado : 0.00m;
                    var NOtroPago = new mdl_OtroPagoMBDB
                    {
                        sTipo = otroPago.TipoOtroPago.ToString(),
                        sClave = otroPago.Clave.ToString(),
                        sConcepto = otroPago.Concepto.ToString(),


                    };
                    NOtroPago.afImporte[0] = otroPago.Importe;
                    NOtroPago.afImporte[iMes] = otroPago.Importe;

                    lsOtroPago.Add(NOtroPago);



                    if (otroPago.CompensacionSaldosAFavor != null)
                    {

                        oCompensacionSaldoAFavor = new mdl_CompensacionSaldoAFavorMBDB();

                        oCompensacionSaldoAFavor.afRemanenteSalFav[0] = otroPago.CompensacionSaldosAFavor.RemanenteSalFav;
                        oCompensacionSaldoAFavor.afSaldoAFavor[0] = otroPago.CompensacionSaldosAFavor.SaldoAFavor;
                        oCompensacionSaldoAFavor.afRemanenteSalFav[iMes] = otroPago.CompensacionSaldosAFavor.RemanenteSalFav;
                        oCompensacionSaldoAFavor.afSaldoAFavor[iMes] = otroPago.CompensacionSaldosAFavor.SaldoAFavor;


                    }
                    else
                        oCompensacionSaldoAFavor = null;

                    if (otroPago.SubsidioAlEmpleo != null)
                    {
                        oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();

                        oSubsidioAlEmpleo.afSubsidioCausado[0] = otroPago.SubsidioAlEmpleo.SubsidioCausado;
                        oSubsidioAlEmpleo.afSubsidioCausado[iMes] = otroPago.SubsidioAlEmpleo.SubsidioCausado;

                    }


                }

                var oDataOtrosPagos = new mdl_OtrosPagosMBDB
                {
                    lsOtroPago = lsOtroPago,
                   
                    oCompensacionSaldoAFavor = oCompensacionSaldoAFavor,
                    oSubsidioAlEmpleo = oSubsidioAlEmpleo
                };
                oDataOtrosPagos.afImporteTotal[0] = fImporteTotal;
                oDataOtrosPagos.afSubsidioCausadoTotal[0] = fSubsidioCausadoTotal;

                oDataOtrosPagos.afImporteTotal[iMes] = fImporteTotal;
                oDataOtrosPagos.afSubsidioCausadoTotal[iMes] = fSubsidioCausadoTotal;

                oOtrosPagos = oDataOtrosPagos;
                oDataOtrosPagos = null;
            }
            else
            { oOtrosPagos = null; }

            return Task.FromResult(oOtrosPagos);

        }

        public Task<mdl_IncapacidadesMBDB> ObtenerDataIncapacidades12B(Nom12B.Nomina oNominaXML12B, string sMes)
        {

            mdl_IncapacidadesMBDB oIncapacidades = null;
            List<mdl_IncapacidadMBDB> lsIncapacidades = new List<mdl_IncapacidadMBDB>();
            decimal fImporteTotal = 0.00m;
            int iDiasIncapacidadTotal = 0;



            if (oNominaXML12B.Incapacidades != null)
            {
                foreach (var incapacidad in oNominaXML12B.Incapacidades)
                {
                    var NIncapacidad = new mdl_IncapacidadMBDB
                    {
                        sTipoIncapacidad = incapacidad.TipoIncapacidad,

                    };

                    NIncapacidad.aiDiasIncapacidad[0] = incapacidad.DiasIncapacidad;
                    NIncapacidad.afImporte[0] = incapacidad.ImporteMonetario;
                    NIncapacidad.aiDiasIncapacidad[iMes] = incapacidad.DiasIncapacidad;
                    NIncapacidad.afImporte[iMes] = incapacidad.ImporteMonetario;

                    fImporteTotal += incapacidad.ImporteMonetario;
                    iDiasIncapacidadTotal += incapacidad.DiasIncapacidad;

                    lsIncapacidades.Add(NIncapacidad);
                }
                oIncapacidades = new mdl_IncapacidadesMBDB
                {
                    lsIncapacidad = lsIncapacidades,

                };
                oIncapacidades.afImporteTotal[0] = fImporteTotal;
                oIncapacidades.aiDiasIncapacidadTotal[0] = iDiasIncapacidadTotal;
                oIncapacidades.afImporteTotal[iMes] = fImporteTotal;
                oIncapacidades.aiDiasIncapacidadTotal[iMes] = iDiasIncapacidadTotal;






            }
            else
                oIncapacidades = null;


            return Task.FromResult(oIncapacidades);

        }
        #endregion


        #region Metodos Nom12A.Nomina 
        public Task<mdl_NominaMBDB> ObtenerDataNomina12A(Nom12A.Nomina oNominaXML12A, string sMes, decimal fNetoTotal)
        {
            mdl_NominaMBDB oNomina = new mdl_NominaMBDB();
            oNomina.afNetoTotal[0] = fNetoTotal;
            oNomina.afTotalPercepciones[0] = oNominaXML12A != null ? oNominaXML12A.TotalPercepciones : 0.00m;
            oNomina.afTotalDeducciones[0] = oNominaXML12A != null ? oNominaXML12A.TotalDeducciones : 0.00m;
            oNomina.afTotalOtrosPagos[0] = oNominaXML12A != null ? oNominaXML12A.TotalOtrosPagos : 0.00m;
            oNomina.afNetoTotal[iMes] = fNetoTotal;
            oNomina.afTotalPercepciones[iMes] = oNominaXML12A != null ? oNominaXML12A.TotalPercepciones : 0.00m;
            oNomina.afTotalDeducciones[iMes] = oNominaXML12A != null ? oNominaXML12A.TotalDeducciones : 0.00m;
            oNomina.afTotalOtrosPagos[iMes] = oNominaXML12A != null ? oNominaXML12A.TotalOtrosPagos : 0.00m;

            return Task.FromResult(oNomina);
        }
        public Task<mdl_PercepcionesMBDB> ObtenerDataPercepciones12A(Nom12A.Nomina oNominaXML12A, string sMes)
        {
            mdl_PercepcionesMBDB oPercepcion = null;
            List<mdl_PercepcionMBDB> lsPercepcion = new List<mdl_PercepcionMBDB>();
            List<mdl_HorasExtraMBDB> lsHorasExtra = new List<mdl_HorasExtraMBDB>();

            /*Preguntar si esta bien validar varias veces el mismo objeto*/
            if (oNominaXML12A.Percepciones != null)
            {
                mdl_PercepcionesMBDB oDataPercepciones = new mdl_PercepcionesMBDB();

                oDataPercepciones.afTotalSueldos[0] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSueldos : 0.00m;
                oDataPercepciones.afTotalSueldos[0] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalGravado : 0.00m;
                oDataPercepciones.afTotalExento[0] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalExento : 0.00m;
                    oDataPercepciones.afTotalJubilacionPensionRetiro[0] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalJubilacionPensionRetiro : 0.00m;
                oDataPercepciones.afTotalSeparacionIndemnizacion[0] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSeparacionIndemnizacion : 0.00m;
                oDataPercepciones.afTotalSueldos[iMes] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSueldos : 0.00m;
                oDataPercepciones.afTotalSueldos[iMes] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalGravado : 0.00m;
                oDataPercepciones.afTotalExento[iMes] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalExento : 0.00m;
                    oDataPercepciones.afTotalJubilacionPensionRetiro[iMes] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalJubilacionPensionRetiro : 0.00m;
                oDataPercepciones.afTotalSeparacionIndemnizacion[iMes] = oNominaXML12A.Percepciones != null ? oNominaXML12A.Percepciones.TotalSeparacionIndemnizacion : 0.00m;




               


                if (oNominaXML12A.Percepciones.Percepcion != null && oNominaXML12A.Percepciones.Percepcion.Length > 0)
                {
                    foreach (var concepto in oNominaXML12A.Percepciones.Percepcion)
                    {
                        var NPercepcion = new mdl_PercepcionMBDB
                        {
                            sTipo = concepto.TipoPercepcion,
                            sClave = concepto.Clave,
                            sConcepto = concepto.Concepto,

                        };

                        NPercepcion.afGravado[0] = concepto.ImporteGravado;
                        NPercepcion.afExento[0] = concepto.ImporteExento;

                        NPercepcion.afGravado[iMes] = concepto.ImporteGravado;
                        NPercepcion.afExento[iMes] = concepto.ImporteExento;





                        if (concepto.AccionesOTitulos != null)
                        {
                            var NAccionesOTitulos = new mdl_AccionesOTitulosMBDB();

                            NAccionesOTitulos.afValorMercado[0] = concepto.AccionesOTitulos.ValorMercado;
                            NAccionesOTitulos.afPrecioAlOtorgarse[0] = concepto.AccionesOTitulos.PrecioAlOtorgarse;


                            NAccionesOTitulos.afValorMercado[iMes] = concepto.AccionesOTitulos.ValorMercado;
                            NAccionesOTitulos.afPrecioAlOtorgarse[iMes] = concepto.AccionesOTitulos.PrecioAlOtorgarse;


                            oDataPercepciones.oAccionesOTitulos = NAccionesOTitulos;
                        }
                        else
                            oDataPercepciones.oAccionesOTitulos = null;

                        if (concepto.HorasExtra != null && concepto.HorasExtra.Length > 0)
                        {
                            //mdl_HorasExtraMBDB NHorasExtra = null;



                            foreach (var horaextra in concepto.HorasExtra)
                            {

                                var NHorasExtra = new mdl_HorasExtraMBDB
                                {
                                    sTipoHoraExtra = horaextra.TipoHoras,
                                };

                                NHorasExtra.oHoraExtra.aiHorasExtra[0] = horaextra.HorasExtra;
                                NHorasExtra.oHoraExtra.aiDias[0] = horaextra.Dias;
                                NHorasExtra.oHoraExtra.afImportePagado[0] = horaextra.ImportePagado;


                                NHorasExtra.oHoraExtra.aiHorasExtra[iMes] = horaextra.HorasExtra;
                                NHorasExtra.oHoraExtra.aiDias[iMes] = horaextra.Dias;
                                NHorasExtra.oHoraExtra.afImportePagado[iMes] = horaextra.ImportePagado;



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
                        var NJubilacionPensionRetiro = new mdl_JubilacionPensionRetiroMBDB();

                        NJubilacionPensionRetiro.afTotalUnaExhibicion[0] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion;
                        NJubilacionPensionRetiro.afTotalParcialidad[0] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalParcialidad;
                        NJubilacionPensionRetiro.afMontoDiario[0] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.MontoDiario;
                        NJubilacionPensionRetiro.afIngresoAcumulable[0] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoAcumulable;
                        NJubilacionPensionRetiro.afIngresoNoAcumulable[0] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable;
                        
                        NJubilacionPensionRetiro.afTotalUnaExhibicion[iMes] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicion;
                        NJubilacionPensionRetiro.afTotalParcialidad[iMes] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.TotalParcialidad;
                        NJubilacionPensionRetiro.afMontoDiario[iMes] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.MontoDiario;
                        NJubilacionPensionRetiro.afIngresoAcumulable[iMes] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoAcumulable;
                        NJubilacionPensionRetiro.afIngresoNoAcumulable[iMes] = oNominaXML12A.Percepciones.JubilacionPensionRetiro.IngresoNoAcumulable;
                        

                        oDataPercepciones.oJubilacionPensionRetiro = NJubilacionPensionRetiro;
                    }
                    else
                        oDataPercepciones.oJubilacionPensionRetiro = null;

                    if (oNominaXML12A.Percepciones.SeparacionIndemnizacion != null)
                    {
                        var NSeparacionIndemnizacion = new mdl_SeparacionIndemnizacionMBDB();

                        NSeparacionIndemnizacion.aiNumAniosServicio[0] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.NumAñosServicio;
                        NSeparacionIndemnizacion.afTotalPagado[0] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.TotalPagado;
                        NSeparacionIndemnizacion.afIngresoAcumulable[0] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoAcumulable;
                        NSeparacionIndemnizacion.afIngresoNoAcumulable[0] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable;
                        NSeparacionIndemnizacion.afUltimoSueldoMensOrd[0] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd;
                      
                        NSeparacionIndemnizacion.aiNumAniosServicio[iMes] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.NumAñosServicio;
                        NSeparacionIndemnizacion.afTotalPagado[iMes] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.TotalPagado;
                        NSeparacionIndemnizacion.afIngresoAcumulable[iMes] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoAcumulable;
                        NSeparacionIndemnizacion.afIngresoNoAcumulable[iMes] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.IngresoNoAcumulable;
                        NSeparacionIndemnizacion.afUltimoSueldoMensOrd[iMes] = oNominaXML12A.Percepciones.SeparacionIndemnizacion.UltimoSueldoMensOrd;
                      

                      
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

        public Task<mdl_DeduccionesMBDB> ObtenerDataDeducciones12A(Nom12A.Nomina oNominaXML12A, string sMes)
        {
            mdl_DeduccionesMBDB oDeducciones = null;
            List<mdl_DeduccionMBDB> lsDeducciones = new List<mdl_DeduccionMBDB>();
            if (oNominaXML12A.Deducciones != null)
            {

                mdl_DeduccionesMBDB oDeduccion = new mdl_DeduccionesMBDB();


                oDeduccion.afTotalOtrasDeducciones[0] = oNominaXML12A.Deducciones.TotalOtrasDeducciones;
                oDeduccion.afTotalImpuestosRetenidos[0] = oNominaXML12A.Deducciones.TotalImpuestosRetenidos;
                
                oDeduccion.afTotalOtrasDeducciones[iMes] = oNominaXML12A.Deducciones.TotalOtrasDeducciones;
                oDeduccion.afTotalImpuestosRetenidos[iMes] = oNominaXML12A.Deducciones.TotalImpuestosRetenidos;
                

            

                if (oNominaXML12A.Deducciones.Deduccion != null && oNominaXML12A.Deducciones.Deduccion.Length > 0)
                {
                    foreach (var deduccion in oNominaXML12A.Deducciones.Deduccion)
                    {
                        var NDeduccion = new mdl_DeduccionMBDB
                        {
                            sTipo = deduccion.TipoDeduccion,
                            sClave = deduccion.Clave,
                            sConcepto = deduccion.Concepto,
                           
                        };

                       NDeduccion.afImporte[0] = deduccion.Importe;
                       NDeduccion.afImporte[iMes] = deduccion.Importe;

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

        public Task<mdl_OtrosPagosMBDB> ObtenerDataOtrosPagos12A(Nom12A.Nomina oNominaXML12A, string sMes)
        {
            var oOtrosPagos = new mdl_OtrosPagosMBDB();
            mdl_SubsidioAlEmpleoMBDB oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();
            mdl_CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor = null;
            List<mdl_OtroPagoMBDB> lsOtroPago = new List<mdl_OtroPagoMBDB>();
            decimal fImporteTotal = 0.00m;
            decimal fSubsidioCausadoTotal = 0.00m;

            if (oNominaXML12A.OtrosPagos != null)
            {


                foreach (var otroPago in oNominaXML12A.OtrosPagos)
                {
                    fImporteTotal += otroPago.Importe;
                    fSubsidioCausadoTotal += otroPago.SubsidioAlEmpleo != null ? otroPago.SubsidioAlEmpleo.SubsidioCausado : 0.00m;
                    var NOtroPago = new mdl_OtroPagoMBDB
                    {
                        sTipo = otroPago.TipoOtroPago.ToString(),
                        sClave = otroPago.Clave.ToString(),
                        sConcepto = otroPago.Concepto.ToString(),
                       

                    };
                   NOtroPago.afImporte[0] = otroPago.Importe;
                   NOtroPago.afImporte[iMes] = otroPago.Importe;

                         lsOtroPago.Add(NOtroPago);



                    if (otroPago.CompensacionSaldosAFavor != null)
                    {
                        oCompensacionSaldoAFavor = new mdl_CompensacionSaldoAFavorMBDB();

                        oCompensacionSaldoAFavor.afRemanenteSalFav[0] = otroPago.CompensacionSaldosAFavor.RemanenteSalFav;
                        oCompensacionSaldoAFavor.afSaldoAFavor[0] = otroPago.CompensacionSaldosAFavor.SaldoAFavor;

                        
                        oCompensacionSaldoAFavor.afRemanenteSalFav[iMes] = otroPago.CompensacionSaldosAFavor.RemanenteSalFav;
                        oCompensacionSaldoAFavor.afSaldoAFavor[iMes] = otroPago.CompensacionSaldosAFavor.SaldoAFavor;

                        

                       
                    }
                    else
                        oCompensacionSaldoAFavor = null;

                    if (otroPago.SubsidioAlEmpleo != null)
                    {
                        oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();

                        oSubsidioAlEmpleo.afSubsidioCausado[0] = otroPago.SubsidioAlEmpleo.SubsidioCausado;
                        oSubsidioAlEmpleo.afSubsidioCausado[iMes] = otroPago.SubsidioAlEmpleo.SubsidioCausado;
                        

                    }


                }

                var oDataOtrosPagos = new mdl_OtrosPagosMBDB
                {
                    lsOtroPago = lsOtroPago,
                   
                    oCompensacionSaldoAFavor = oCompensacionSaldoAFavor,
                    oSubsidioAlEmpleo = oSubsidioAlEmpleo
                };

                oDataOtrosPagos.afImporteTotal[0] = fImporteTotal;
                oDataOtrosPagos.afSubsidioCausadoTotal[0] = fSubsidioCausadoTotal;

                oDataOtrosPagos.afImporteTotal[iMes] = fImporteTotal;
                oDataOtrosPagos.afSubsidioCausadoTotal[iMes] = fSubsidioCausadoTotal;




                oOtrosPagos = oDataOtrosPagos;
                oDataOtrosPagos = null;
            }
            else
            { oOtrosPagos = null; }

            return Task.FromResult(oOtrosPagos);

        }

        public Task<mdl_IncapacidadesMBDB> ObtenerDataIncapacidades12A(Nom12A.Nomina oNominaXML12A, string sMes)
        {

            mdl_IncapacidadesMBDB oIncapacidades = null;
            List<mdl_IncapacidadMBDB> lsIncapacidades = new List<mdl_IncapacidadMBDB>();
            decimal fImporteTotal = 0.00m;
            int iDiasIncapacidadTotal = 0;



            if (oNominaXML12A.Incapacidades != null)
            {
                foreach (var incapacidad in oNominaXML12A.Incapacidades)
                {
                    var NIncapacidad = new mdl_IncapacidadMBDB
                    {
                        sTipoIncapacidad = incapacidad.TipoIncapacidad,
                       
                    };

                    NIncapacidad.aiDiasIncapacidad[0] = incapacidad.DiasIncapacidad;
                    NIncapacidad.afImporte[0] = incapacidad.ImporteMonetario;
                    
                    NIncapacidad.aiDiasIncapacidad[iMes] = incapacidad.DiasIncapacidad;
                    NIncapacidad.afImporte[iMes] = incapacidad.ImporteMonetario;
                    
                    fImporteTotal += incapacidad.ImporteMonetario;
                    iDiasIncapacidadTotal += incapacidad.DiasIncapacidad;

                    lsIncapacidades.Add(NIncapacidad);
                }
                oIncapacidades = new mdl_IncapacidadesMBDB
                {
                    lsIncapacidad = lsIncapacidades,
                   
                };
                oIncapacidades.afImporteTotal[0] = fImporteTotal;
                oIncapacidades.aiDiasIncapacidadTotal[0] = iDiasIncapacidadTotal;
                oIncapacidades.afImporteTotal[iMes] = fImporteTotal;
                oIncapacidades.aiDiasIncapacidadTotal[iMes] = iDiasIncapacidadTotal;


               

            }
            else
                oIncapacidades = null;


            return Task.FromResult(oIncapacidades);

        }

        #endregion
    }
}
