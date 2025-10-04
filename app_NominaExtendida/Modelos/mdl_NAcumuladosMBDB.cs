using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app_NominaExtendida.Modelos
{
    public class mdl_NAcumuladosMBDB
    {
        [BsonElement("_id", Order = 1)]
        public ObjectId _id { get; set; }

        [BsonElement("em_rfcolaborador", Order = 2)]
        public string sRFColaborador { get; set; } = string.Empty;

        [BsonElement("em_nombre", Order = 3)]
        public string sNombreColaborador { get; set; } = string.Empty;

        [BsonElement("em_numero", Order = 4)]
        public string sNumeroColaborador { get; set; } = string.Empty;

        [BsonElement("em_imss", Order = 5)]
        public string sIMSS { get; set; } = string.Empty;

        [BsonElement("em_regpat", Order = 6)]
        public string sRegPat { get; set; } = string.Empty;

        [BsonElement("UUIDs", Order = 8)]
        public List<string> lsUUID { get; set; }

        [BsonElement("Nomina", Order = 7)]
        public mdl_NominaMBDB Nomina { get; set; }

        [BsonElement("date_insr")]
        public string sFechaRegistro { get; set; } = string.Empty;

        [BsonElement("sr_usuario")]
        public string sr_usuario { get; set; } = string.Empty;

        [BsonElement("sr_recno")]
        public int sr_recno { get; set; }

        [BsonElement("sr_deleted")]
        public string sr_deleted { get; set; }

        [BsonElement("sr_fecha")]
        public DateTime sr_fecha { get; set; }

        public mdl_NAcumuladosMBDB()
        {
            Nomina = new mdl_NominaMBDB();
            lsUUID = new List<string>();
            sFechaRegistro = DateTime.UtcNow.ToString("dd/MM/yyyy:HH:mm:ss");
            sr_usuario = string.Empty;
            sr_recno = 0;
            sr_deleted = string.Empty;
            sr_fecha = DateTime.MinValue;
        }
    }

    public class mdl_NominaMBDB
    {

        [BsonElement("TotalDeducciones", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalDeducciones { get; set; }

        [BsonElement("TotalPercepciones", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalPercepciones { get; set; }

        [BsonElement("TotalOtrosPagos", Order = 3)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalOtrosPagos { get; set; }

        [BsonElement("NetoTotal", Order = 4)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afNetoTotal { get; set; }

        [BsonElement("Percepciones", Order = 5)]
        public mdl_PercepcionesMBDB oPercepciones { get; set; }

        [BsonElement("Deducciones", Order = 6)]
        public mdl_DeduccionesMBDB oDeducciones { get; set; }

        [BsonElement("OtrosPagos", Order = 7)]
        public mdl_OtrosPagosMBDB oOtrosPagos { get; set; }

        [BsonElement("Incapacidades", Order = 8)]
        public mdl_IncapacidadesMBDB oIncapacidades { get; set; }

      

        public mdl_NominaMBDB()
        {
            oPercepciones = new mdl_PercepcionesMBDB();
            oDeducciones = new mdl_DeduccionesMBDB();
            oOtrosPagos = new mdl_OtrosPagosMBDB();
            oIncapacidades = new mdl_IncapacidadesMBDB();

            afNetoTotal = new decimal[13];
            afTotalDeducciones = new decimal[13];
            afTotalPercepciones = new decimal[13];
            afTotalOtrosPagos = new decimal[13];
        }
    }

    #region Percepciones
    public class mdl_PercepcionesMBDB
    {

        [BsonElement("TotalSueldos", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalSueldos { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalSeparacionIndemnizacion { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro", Order = 3)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalJubilacionPensionRetiro { get; set; }

        [BsonElement("TotalGravado", Order = 4)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalGravado { get; set; }

        [BsonElement("TotalExento", Order = 5)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalExento { get; set; }

        [BsonElement("Percepcion", Order = 6)]
        public List<mdl_PercepcionMBDB> lsPercepcion { get; set; }

        [BsonElement("AccionesOTitulos", Order = 7)]
        public mdl_AccionesOTitulosMBDB oAccionesOTitulos { get; set; }

        [BsonElement("HorasExtra", Order = 8)]
        public List<mdl_HorasExtraMBDB> lsHorasExtra { get; set; }

        [BsonElement("JubilacionPensionRetiro", Order = 9)]
        public mdl_JubilacionPensionRetiroMBDB oJubilacionPensionRetiro { get; set; }

        [BsonElement("SeparacionIndemnizacion", Order = 10)]
        public mdl_SeparacionIndemnizacionMBDB oSeparacionIndemnizacion { get; set; }


        public mdl_PercepcionesMBDB()
        {
            lsPercepcion = new List<mdl_PercepcionMBDB>();
            oAccionesOTitulos = new mdl_AccionesOTitulosMBDB();
            lsHorasExtra = new List<mdl_HorasExtraMBDB>();
            oJubilacionPensionRetiro = new mdl_JubilacionPensionRetiroMBDB();
            oSeparacionIndemnizacion = new mdl_SeparacionIndemnizacionMBDB();
            //}

            afTotalExento = new decimal[13];
            afTotalGravado = new decimal[13];
            afTotalJubilacionPensionRetiro = new decimal[13];
            afTotalSeparacionIndemnizacion = new decimal[13];
            afTotalSueldos = new decimal[13];

        }
    }

    public class mdl_PercepcionMBDB
    {
        [BsonElement("Tipo", Order = 1)]
        public string sTipo { get; set; } = string.Empty;

        [BsonElement("Clave", Order = 2)]
        public string sClave { get; set; } = string.Empty;

        [BsonElement("Concepto", Order = 3)]
        public string sConcepto { get; set; } = string.Empty;

        [BsonElement("Gravado")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afGravado { get; set; }

        [BsonElement("Exento")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afExento { get; set; }

   

        public mdl_PercepcionMBDB()
        {
            afExento = new decimal[13];
            afGravado = new decimal[13];
        }
    }

    //Percepcion-045 
    public class mdl_AccionesOTitulosMBDB
    {

        [BsonElement("ValorMercado", Order = 1)]
        public decimal[] afValorMercado { get; set; }

        [BsonElement("PrecioAlOtorgarse", Order = 2)]
        public decimal[] afPrecioAlOtorgarse { get; set; }

        public mdl_AccionesOTitulosMBDB()
        {
            afPrecioAlOtorgarse = new decimal[13];
            afValorMercado = new decimal[13];
        }

    }
    ////Percepcion-019
    public class mdl_HorasExtraMBDB
    {


        [BsonElement("Tipo", Order = 1)]
        public string sTipoHoraExtra { get; set; }

        [BsonElement("HoraExtra", Order = 2)]
        public mdl_HoraExtraMBDB oHoraExtra { get; set; }
        
        public mdl_HorasExtraMBDB()
        {
            sTipoHoraExtra = string.Empty;
            oHoraExtra = new mdl_HoraExtraMBDB();
        }
    }

    public class mdl_HoraExtraMBDB
    {
        /*Horas Extra*/
        [BsonElement("Dias")]
        public int[] aiDias { get; set; }


        [BsonElement("HorasExtra")]
        public int[] aiHorasExtra { get; set; }


        [BsonElement("ImportePagado")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afImportePagado { get; set; }

        public mdl_HoraExtraMBDB()
        {
            afImportePagado = new decimal[13];
            aiDias = new int[13];
            aiHorasExtra = new int[13];
        }
    }

    //Percepcion- 039, 044,
    public class mdl_JubilacionPensionRetiroMBDB
    {

        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalUnaExhibicion { get; set; }

        [BsonElement("TotalParcialidad")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalParcialidad { get; set; }

        [BsonElement("MontoDiario")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afMontoDiario { get; set; }

        [BsonElement("IngresoAcumulable")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afIngresoAcumulable { get; set; }

        [BsonElement("IngresoNoAcumulable")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afIngresoNoAcumulable { get; set; }

        public mdl_JubilacionPensionRetiroMBDB()
        {
            afIngresoAcumulable = new decimal[13];
            afIngresoNoAcumulable = new decimal[13];
            afMontoDiario = new decimal[13];
            afTotalParcialidad = new decimal[13];
            afTotalUnaExhibicion = new decimal[13];
        }

    }

    //Percepcion- 022, 023, 025 
    public class mdl_SeparacionIndemnizacionMBDB
    {
        [BsonElement("TotalPagado")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalPagado { get; set; }

        [BsonElement("NumAniosServicio")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int[] aiNumAniosServicio { get; set; }

        [BsonElement("UltimoSueldoMensOrd")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afUltimoSueldoMensOrd { get; set; }


        [BsonElement("IngresoAcumulable")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afIngresoAcumulable { get; set; }

        [BsonElement("IngresoNoAcumulable")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afIngresoNoAcumulable { get; set; }
        public mdl_SeparacionIndemnizacionMBDB()
        {

            afIngresoNoAcumulable = new decimal[13];
            afIngresoAcumulable = new decimal[13];
            afTotalPagado = new decimal[13];
            afUltimoSueldoMensOrd = new decimal[13];
            aiNumAniosServicio = new int[13];
        }
    }
    #endregion

    #region Deducciones
    public class mdl_DeduccionesMBDB
    {
        [BsonElement("TotalOtrasDeducciones", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalOtrasDeducciones { get; set; }

        [BsonElement("TotalImpuestosRetenidos", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afTotalImpuestosRetenidos { get; set; }

        [BsonElement("Deduccion", Order = 3)]
        public List<mdl_DeduccionMBDB> lsDeduccion { get; set; }

        public mdl_DeduccionesMBDB()
        {
            lsDeduccion = new List<mdl_DeduccionMBDB>();
            afTotalImpuestosRetenidos = new decimal[13];
            afTotalOtrasDeducciones = new decimal[13];
        }

    }

    public class mdl_DeduccionMBDB
    {
        [BsonElement("Tipo")]
        public string sTipo { get; set; } = string.Empty;

        [BsonElement("Clave")]
        public string sClave { get; set; } = string.Empty;

        [BsonElement("Concepto")]
        public string sConcepto { get; set; } = string.Empty;

        [BsonElement("Importe")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afImporte { get; set; }

        public mdl_DeduccionMBDB()
        {
            afImporte = new decimal[13];
        }

    }
    #endregion

    #region OtrosPagos
    //Percepcion - 038
    public class mdl_OtrosPagosMBDB
    {

        [BsonElement("ImporteTotal", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afImporteTotal { get; set; }

        [BsonElement("SubsidioCausadoTotal", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afSubsidioCausadoTotal { get; set; }

        [BsonElement("OtroPago", Order = 3)]
        public List<mdl_OtroPagoMBDB> lsOtroPago { get; set; }

        [BsonElement("SubsidioAlEmpleo", Order = 4)]
        public mdl_SubsidioAlEmpleoMBDB oSubsidioAlEmpleo { get; set; }

        [BsonElement("CompensacionSaldoAFavor", Order = 5)]
        public mdl_CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor { get; set; }
        
        public mdl_OtrosPagosMBDB()
        {

            afImporteTotal = new decimal[13];
            afSubsidioCausadoTotal = new decimal[13];
            lsOtroPago = new List<mdl_OtroPagoMBDB>();
            oSubsidioAlEmpleo = new mdl_SubsidioAlEmpleoMBDB();
            oCompensacionSaldoAFavor = new mdl_CompensacionSaldoAFavorMBDB();

        }

    }

    public class mdl_OtroPagoMBDB
    {
        [BsonElement("Tipo")]
        public string sTipo { get; set; } = string.Empty;

        [BsonElement("Clave")]
        public string sClave { get; set; } = string.Empty;

        [BsonElement("Concepto")]
        public string sConcepto { get; set; } = string.Empty;

        [BsonElement("Importe")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afImporte { get; set; }

        public mdl_OtroPagoMBDB()
        {

            afImporte = new decimal[13];
        }

    }

    public class mdl_SubsidioAlEmpleoMBDB
    {
        [BsonElement("SubsidioCausado")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afSubsidioCausado { get; set; }
        public mdl_SubsidioAlEmpleoMBDB()
        {

            afSubsidioCausado = new decimal[13];
        }
    }

    public class mdl_CompensacionSaldoAFavorMBDB
    {
        [BsonElement("SaldoAFavor")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afSaldoAFavor { get; set; }



        [BsonElement("RemanenteSalFav")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afRemanenteSalFav { get; set; }


        [BsonElement("Anio")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int[] aiAnio { get; set; }

        public mdl_CompensacionSaldoAFavorMBDB()
        {
            afRemanenteSalFav = new decimal[13];
            afSaldoAFavor = new decimal[13];
            aiAnio = new int[13];
        }
    }
    #endregion

    #region Incapacidades
    //Percepcion-014
    public class mdl_IncapacidadesMBDB
    {


        [BsonElement("ImporteTotal", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afImporteTotal { get; set; }

        [BsonElement("DiasIncapacidadTotal", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public int[] aiDiasIncapacidadTotal { get; set; }

        [BsonElement("Incapacidad", Order = 3)]
        public List<mdl_IncapacidadMBDB> lsIncapacidad { get; set; }

  

        public mdl_IncapacidadesMBDB()
        {
            afImporteTotal = new decimal[13];
            aiDiasIncapacidadTotal = new int[13];   

            lsIncapacidad = new List<mdl_IncapacidadMBDB>();
        }

    }

    public class mdl_IncapacidadMBDB
    {
        [BsonElement("Tipo")]
        public string sTipoIncapacidad { get; set; } = string.Empty;

        [BsonElement("DiasIncapacidad")]
        public int[] aiDiasIncapacidad { get; set; }
        [BsonElement("Importe")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal[] afImporte { get; set; }
        public mdl_IncapacidadMBDB()
        {
            afImporte = new decimal[13];
            aiDiasIncapacidad = new int[13];
        }

    }
    #endregion
}
