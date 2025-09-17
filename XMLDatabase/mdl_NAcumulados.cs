using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace XMLDatabase
{
    internal class mdl_NAcumulados
    {
        [BsonElement("_id", Order = 1)]
        public ObjectId _id { get; set; }

        [BsonElement("em_rfc", Order = 2)]
        public string sRFC { get; set; } = string.Empty;

        [BsonElement("em_nombre", Order = 3)]
        public string sNombre { get; set; } = string.Empty;

        [BsonElement("em_numero", Order = 4)]
        public string sNumero { get; set; } = string.Empty;

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

        public mdl_NAcumulados()
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

        [BsonElement("TotalDeducciones00", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones00 { get; set; }

        [BsonElement("TotalPercepciones00", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones00 { get; set; }

        [BsonElement("TotalOtrosPagos00", Order = 3)]
        [BsonRepresentation(BsonType.Decimal128)]

        public decimal fTotalOtrosPagos00 { get; set; }
        [BsonElement("NetoTotal00", Order = 4)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal00 { get; set; }

        [BsonElement("Percepciones", Order = 5)]
        public PercepcionesMBDB oPercepciones { get; set; }

        [BsonElement("Deducciones", Order = 6)]
        public DeduccionesMBDB oDeducciones { get; set; }

        [BsonElement("OtrosPagos", Order = 7)]
        public OtrosPagosMBDB oOtrosPagos { get; set; }

        [BsonElement("Incapacidades", Order = 8)]
        public IncapacidadesMBDB oIncapacidades { get; set; }

        #region Totales Mensuales
        [BsonElement("TotalDeducciones01")]
        [BsonRepresentation(BsonType.Decimal128)]


        public decimal fTotalDeducciones01 { get; set; }

        [BsonElement("TotalPercepciones01")]
        [BsonRepresentation(BsonType.Decimal128)]

        public decimal fTotalPercepciones01 { get; set; }

        [BsonElement("TotalOtrosPagos01")]
        [BsonRepresentation(BsonType.Decimal128)]

        public decimal fTotalOtrosPagos01 { get; set; }

        [BsonElement("NetoTotal01")]
        [BsonRepresentation(BsonType.Decimal128)]


        public decimal fNetoTotal01 { get; set; }

        [BsonElement("TotalDeducciones02")]
        [BsonRepresentation(BsonType.Decimal128)]


        public decimal fTotalDeducciones02 { get; set; }

        [BsonElement("TotalPercepciones02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones02 { get; set; }

        [BsonElement("TotalOtrosPagos02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos02 { get; set; }

        [BsonElement("NetoTotal02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal02 { get; set; }

        [BsonElement("TotalDeducciones03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones03 { get; set; }

        [BsonElement("TotalPercepciones03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones03 { get; set; }

        [BsonElement("TotalOtrosPagos03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos03 { get; set; }

        [BsonElement("NetoTotal03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal03 { get; set; }

        [BsonElement("TotalDeducciones04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones04 { get; set; }

        [BsonElement("TotalPercepciones04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones04 { get; set; }

        [BsonElement("TotalOtrosPagos04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos04 { get; set; }

        [BsonElement("NetoTotal04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal04 { get; set; }

        [BsonElement("TotalDeducciones05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones05 { get; set; }

        [BsonElement("TotalPercepciones05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones05 { get; set; }

        [BsonElement("TotalOtrosPagos05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos05 { get; set; }

        [BsonElement("NetoTotal05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal05 { get; set; }

        [BsonElement("TotalDeducciones06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones06 { get; set; }

        [BsonElement("TotalPercepciones06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones06 { get; set; }

        [BsonElement("TotalOtrosPagos06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos06 { get; set; }

        [BsonElement("NetoTotal06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal06 { get; set; }

        [BsonElement("TotalDeducciones07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones07 { get; set; }

        [BsonElement("TotalPercepciones07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones07 { get; set; }

        [BsonElement("TotalOtrosPagos07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos07 { get; set; }

        [BsonElement("NetoTotal07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal07 { get; set; }

        [BsonElement("TotalDeducciones08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones08 { get; set; }

        [BsonElement("TotalPercepciones08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones08 { get; set; }

        [BsonElement("TotalOtrosPagos08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos08 { get; set; }

        [BsonElement("NetoTotal08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal08 { get; set; }

        [BsonElement("TotalDeducciones09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones09 { get; set; }

        [BsonElement("TotalPercepciones09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones09 { get; set; }

        [BsonElement("TotalOtrosPagos09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos09 { get; set; }

        [BsonElement("NetoTotal09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal09 { get; set; }

        [BsonElement("TotalDeducciones10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones10 { get; set; }

        [BsonElement("TotalPercepciones10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones10 { get; set; }

        [BsonElement("TotalOtrosPagos10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos10 { get; set; }

        [BsonElement("NetoTotal10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal10 { get; set; }

        [BsonElement("TotalDeducciones11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones11 { get; set; }

        [BsonElement("TotalPercepciones11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones11 { get; set; }

        [BsonElement("TotalOtrosPagos11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos11 { get; set; }

        [BsonElement("NetoTotal11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal11 { get; set; }

        [BsonElement("TotalDeducciones12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalDeducciones12 { get; set; }

        [BsonElement("TotalPercepciones12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPercepciones12 { get; set; }

        [BsonElement("TotalOtrosPagos12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalOtrosPagos12 { get; set; }

        [BsonElement("NetoTotal12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fNetoTotal12 { get; set; }
        #endregion

        public mdl_NominaMBDB()
        {
            oPercepciones = new PercepcionesMBDB();
            oDeducciones = new DeduccionesMBDB();
            oOtrosPagos = new OtrosPagosMBDB();
            oIncapacidades = new IncapacidadesMBDB();
        }
    }

    #region Percepciones
    public class PercepcionesMBDB
    {

        [BsonElement("TotalSueldos00", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos00 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion00", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion00 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro00", Order = 3)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro00 { get; set; }

        [BsonElement("TotalGravado00", Order = 4)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado00 { get; set; }

        [BsonElement("TotalExento00", Order = 5)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento00 { get; set; }

        [BsonElement("Percepcion", Order = 6)]
        public List<PercepcionMBDB> lsPercepcion { get; set; }

        [BsonElement("AccionesOTitulos", Order = 7)]
        public AccionesOTitulosMBDB oAccionesOTitulos { get; set; }

        [BsonElement("HorasExtra", Order = 8)]
        public List<HorasExtraMBDB> lsHorasExtra { get; set; }

        [BsonElement("JubilacionPensionRetiro", Order = 9)]
        public JubilacionPensionRetiroMBDB oJubilacionPensionRetiro { get; set; }

        [BsonElement("SeparacionIndemnizacion", Order = 10)]
        public SeparacionIndemnizacionMBDB oSeparacionIndemnizacion { get; set; }

        #region totales mensuales
        [BsonElement("TotalSueldos01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos01 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion01 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro01 { get; set; }

        [BsonElement("TotalGravado01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado01 { get; set; }

        [BsonElement("TotalExento01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento01 { get; set; }

        [BsonElement("TotalSueldos02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos02 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion02 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro02 { get; set; }

        [BsonElement("TotalGravado02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado02 { get; set; }

        [BsonElement("TotalExento02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento02 { get; set; }

        [BsonElement("TotalSueldos03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos03 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion03 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro03 { get; set; }

        [BsonElement("TotalGravado03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado03 { get; set; }

        [BsonElement("TotalExento03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento03 { get; set; }

        [BsonElement("TotalSueldos04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos04 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion04 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro04 { get; set; }

        [BsonElement("TotalGravado04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado04 { get; set; }

        [BsonElement("TotalExento04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento04 { get; set; }

        [BsonElement("TotalSueldos05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos05 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion05 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro05 { get; set; }

        [BsonElement("TotalGravado05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado05 { get; set; }

        [BsonElement("TotalExento05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento05 { get; set; }

        [BsonElement("TotalSueldos06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos06 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion06 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro06 { get; set; }

        [BsonElement("TotalGravado06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado06 { get; set; }

        [BsonElement("TotalExento06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento06 { get; set; }

        [BsonElement("TotalSueldos07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos07 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion07 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro07 { get; set; }

        [BsonElement("TotalGravado07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado07 { get; set; }

        [BsonElement("TotalExento07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento07 { get; set; }

        [BsonElement("TotalSueldos08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos08 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion08 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro08 { get; set; }

        [BsonElement("TotalGravado08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado08 { get; set; }

        [BsonElement("TotalExento08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento08 { get; set; }

        [BsonElement("TotalSueldos09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos09 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion09 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro09 { get; set; }

        [BsonElement("TotalGravado09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado09 { get; set; }

        [BsonElement("TotalExento09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento09 { get; set; }

        [BsonElement("TotalSueldos10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos10 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion10 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro10 { get; set; }

        [BsonElement("TotalGravado10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado10 { get; set; }

        [BsonElement("TotalExento10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento10 { get; set; }

        [BsonElement("TotalSueldos11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos11 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion11 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro11 { get; set; }

        [BsonElement("TotalGravado11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado11 { get; set; }

        [BsonElement("TotalExento11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento11 { get; set; }

        [BsonElement("TotalSueldos12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSueldos12 { get; set; }

        [BsonElement("TotalSeparacionIndemnizacion12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalSeparacionIndemnizacion12 { get; set; }

        [BsonElement("TotalJubilacionPensionRetiro12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalJubilacionPensionRetiro12 { get; set; }

        [BsonElement("TotalGravado12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalGravado12 { get; set; }

        [BsonElement("TotalExento12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalExento12 { get; set; }
        #endregion

        public PercepcionesMBDB()
        {
            lsPercepcion = new List<PercepcionMBDB>();
            oAccionesOTitulos = new AccionesOTitulosMBDB();
            lsHorasExtra = new List<HorasExtraMBDB>();
            oJubilacionPensionRetiro = new JubilacionPensionRetiroMBDB();
            oSeparacionIndemnizacion = new SeparacionIndemnizacionMBDB();
            //}
        }
    }

    public class PercepcionMBDB
    {
        [BsonElement("Tipo", Order = 1)]
        public string sTipo { get; set; } = string.Empty;

        [BsonElement("Clave", Order = 2)]
        public string sClave { get; set; } = string.Empty;

        [BsonElement("Concepto", Order = 3)]
        public string sConcepto { get; set; } = string.Empty;

        [BsonElement("Gravado00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado00 { get; set; }

        [BsonElement("Exento00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento00 { get; set; }

        [BsonElement("Gravado01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado01 { get; set; }

        [BsonElement("Gravado02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado02 { get; set; }

        [BsonElement("Gravado03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado03 { get; set; }

        [BsonElement("Gravado04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado04 { get; set; }

        [BsonElement("Gravado05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado05 { get; set; }

        [BsonElement("Gravado06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado06 { get; set; }

        [BsonElement("Gravado07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado07 { get; set; }

        [BsonElement("Gravado08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado08 { get; set; }

        [BsonElement("Gravado09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado09 { get; set; }

        [BsonElement("Gravado10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado10 { get; set; }


        [BsonElement("Gravado11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado11 { get; set; }

        [BsonElement("Gravado12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fGravado12 { get; set; }

        [BsonElement("Exento01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento01 { get; set; }

        [BsonElement("Exento02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento02 { get; set; }

        [BsonElement("Exento03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento03 { get; set; }

        [BsonElement("Exento04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento04 { get; set; }

        [BsonElement("Exento05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento05 { get; set; }

        [BsonElement("Exento06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento06 { get; set; }

        [BsonElement("Exento07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento07 { get; set; }

        [BsonElement("Exento08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento08 { get; set; }

        [BsonElement("Exento09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento09 { get; set; }

        [BsonElement("Exento10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento10 { get; set; }

        [BsonElement("Exento11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento11 { get; set; }

        [BsonElement("Exento12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fExento12 { get; set; }
    }

    //Percepcion-045 
    public class AccionesOTitulosMBDB
    {

        [BsonElement("ValorMercado00", Order = 1)]
        public decimal fValorMercado00 { get; set; }

        [BsonElement("PrecioAlOtorgarse00", Order = 2)]
        public decimal fPrecioAlOtorgarse00 { get; set; }


        [BsonElement("ValorMercado01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado01 { get; set; }

        [BsonElement("PrecioAlOtorgarse01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse01 { get; set; }



        [BsonElement("ValorMercado02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado02 { get; set; }

        [BsonElement("PrecioAlOtorgarse02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse02 { get; set; }



        [BsonElement("ValorMercado03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado03 { get; set; }

        [BsonElement("PrecioAlOtorgarse03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse03 { get; set; }



        [BsonElement("ValorMercado04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado04 { get; set; }

        [BsonElement("PrecioAlOtorgarse04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse04 { get; set; }



        [BsonElement("ValorMercado05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado05 { get; set; }

        [BsonElement("PrecioAlOtorgarse05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse05 { get; set; }



        [BsonElement("ValorMercado06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado06 { get; set; }

        [BsonElement("PrecioAlOtorgarse06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse06 { get; set; }



        [BsonElement("ValorMercado07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado07 { get; set; }

        [BsonElement("PrecioAlOtorgarse07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse07 { get; set; }



        [BsonElement("ValorMercado08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado08 { get; set; }

        [BsonElement("PrecioAlOtorgarse08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse08 { get; set; }



        [BsonElement("ValorMercado09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado09 { get; set; }

        [BsonElement("PrecioAlOtorgarse09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse09 { get; set; }



        [BsonElement("ValorMercado10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado10 { get; set; }

        [BsonElement("PrecioAlOtorgarse10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse10 { get; set; }



        [BsonElement("ValorMercado11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado11 { get; set; }

        [BsonElement("PrecioAlOtorgarse11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse11 { get; set; }



        [BsonElement("ValorMercado12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fValorMercado12 { get; set; }

        [BsonElement("PrecioAlOtorgarse12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fPrecioAlOtorgarse12 { get; set; }



    }
    ////Percepcion-019
    public class HorasExtraMBDB
    {


        [BsonElement("Tipo", Order = 1)]
        public string sTipoHoraExtra { get; set; } 

        [BsonElement("HoraExtra", Order = 2)]
        public HoraExtraMBDB oHoraExtra { get; set; }


        //[BsonElement("TipoHora01")]
        //public string sTipoHora01 { get; set; } = string.Empty;

        //[BsonElement("TipoHora02")]
        //public string sTipoHora02 { get; set; } = string.Empty;

        //[BsonElement("TipoHora03")]
        //public string sTipoHora03 { get; set; } = string.Empty;

        //[BsonElement("TipoHora04")]
        //public string sTipoHora04 { get; set; } = string.Empty;

        //[BsonElement("TipoHora05")]
        //public string sTipoHora05 { get; set; } = string.Empty;

        //[BsonElement("TipoHora06")]
        //public string sTipoHora06 { get; set; } = string.Empty;

        //[BsonElement("TipoHora07")]
        //public string sTipoHora07 { get; set; } = string.Empty;

        //[BsonElement("TipoHora08")]
        //public string sTipoHora08 { get; set; } = string.Empty;
        //[BsonElement("TipoHora09")]
        //public string sTipoHora09 { get; set; } = string.Empty;
        //[BsonElement("TipoHora10")]
        //public string sTipoHora10 { get; set; } = string.Empty;

        //[BsonElement("TipoHora11")]
        //public string sTipoHora11 { get; set; } = string.Empty;
        //[BsonElement("TipoHora12")]
        //public string sTipoHora12 { get; set; } = string.Empty;
        public HorasExtraMBDB()
        {
            sTipoHoraExtra = string.Empty;
            oHoraExtra = new HoraExtraMBDB();
        }
    }

    public class HoraExtraMBDB
    {
        /*Horas Extra*/
        [BsonElement("Dias00")]
        public int iDias00 { get; set; }
        [BsonElement("Dias01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iDias01 { get; set; }
        [BsonElement("Dias02")]
        public int iDias02 { get; set; }

        [BsonElement("Dias03")]
        public int iDias03 { get; set; }
        [BsonElement("Dias04")]
        public int iDias04 { get; set; }
        [BsonElement("Dias05")]
        public int iDias05 { get; set; }
        [BsonElement("Dias06")]
        public int iDias06 { get; set; }
        [BsonElement("Dias07")]
        public int iDias07 { get; set; }
        [BsonElement("Dias08")]
        public int iDias08 { get; set; }
        [BsonElement("Dias09")]
        public int iDias09 { get; set; }
        [BsonElement("Dias10")]
        public int iDias10 { get; set; }
        [BsonElement("Dias11")]
        public int iDias11 { get; set; }
        [BsonElement("Dias12")]
        public int iDias12 { get; set; }

        [BsonElement("HorasExtra00")]
        public int iHorasExtra00 { get; set; }
        [BsonElement("HorasExtra01")]
        public int iHorasExtra01 { get; set; }

        [BsonElement("HorasExtra02")]
        public int iHorasExtra02 { get; set; }

        [BsonElement("HorasExtra03")]
        public int iHorasExtra03 { get; set; }

        [BsonElement("HorasExtra04")]
        public int iHorasExtra04 { get; set; }

        [BsonElement("HorasExtra05")]
        public int iHorasExtra05 { get; set; }

        [BsonElement("HorasExtra06")]
        public int iHorasExtra06 { get; set; }
        [BsonElement("HorasExtra07")]
        public int iHorasExtra07 { get; set; }

        [BsonElement("HorasExtra08")]
        public int iHorasExtra08 { get; set; }
        [BsonElement("HorasExtra09")]
        public int iHorasExtra09 { get; set; }
        [BsonElement("HorasExtra10")]
        public int iHorasExtra10 { get; set; }

        [BsonElement("HorasExtra11")]
        public int iHorasExtra11 { get; set; }

        [BsonElement("HorasExtra12")]
        public int iHorasExtra12 { get; set; }

        [BsonElement("ImportePagado00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado00 { get; set; }

        [BsonElement("ImportePagado01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado01 { get; set; }
        [BsonElement("ImportePagado02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado02 { get; set; }

        [BsonElement("ImportePagado03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado03 { get; set; }

        [BsonElement("ImportePagado04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado04 { get; set; }

        [BsonElement("ImportePagado05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado05 { get; set; }

        [BsonElement("ImportePagado06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado06 { get; set; }

        [BsonElement("ImportePagado07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado07 { get; set; }
        [BsonElement("ImportePagado08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado08 { get; set; }

        [BsonElement("ImportePagado09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado09 { get; set; }

        [BsonElement("ImportePagado10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado10 { get; set; }
        [BsonElement("ImportePagado11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado11 { get; set; }
        [BsonElement("ImportePagado12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImportePagado12 { get; set; }
    }

    //Percepcion- 039, 044,
    public class JubilacionPensionRetiroMBDB
    {

        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion00 { get; set; }



        [BsonElement("TotalParcialidad00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad00 { get; set; }



        [BsonElement("MontoDiario00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario00 { get; set; }


        [BsonElement("IngresoAcumulable00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable00 { get; set; }




        [BsonElement("IngresoNoAcumulable00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable00 { get; set; }


        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion01 { get; set; }



        [BsonElement("TotalParcialidad01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad01 { get; set; }



        [BsonElement("MontoDiario01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario01 { get; set; }


        [BsonElement("IngresoAcumulable01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable01 { get; set; }




        [BsonElement("IngresoNoAcumulable01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable01 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion02 { get; set; }



        [BsonElement("TotalParcialidad02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad02 { get; set; }



        [BsonElement("MontoDiario02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario02 { get; set; }


        [BsonElement("IngresoAcumulable02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable02 { get; set; }




        [BsonElement("IngresoNoAcumulable02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable02 { get; set; }




        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion03 { get; set; }



        [BsonElement("TotalParcialidad03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad03 { get; set; }



        [BsonElement("MontoDiario03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario03 { get; set; }


        [BsonElement("IngresoAcumulable03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable03 { get; set; }




        [BsonElement("IngresoNoAcumulable03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable03 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion04 { get; set; }



        [BsonElement("TotalParcialidad04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad04 { get; set; }



        [BsonElement("MontoDiario04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario04 { get; set; }


        [BsonElement("IngresoAcumulable04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable04 { get; set; }




        [BsonElement("IngresoNoAcumulable04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable04 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion05 { get; set; }



        [BsonElement("TotalParcialidad05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad05 { get; set; }



        [BsonElement("MontoDiario05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario05 { get; set; }


        [BsonElement("IngresoAcumulable05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable05 { get; set; }




        [BsonElement("IngresoNoAcumulable05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable05 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion06 { get; set; }



        [BsonElement("TotalParcialidad06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad06 { get; set; }



        [BsonElement("MontoDiario06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario06 { get; set; }


        [BsonElement("IngresoAcumulable06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable06 { get; set; }




        [BsonElement("IngresoNoAcumulable06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable06 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion07 { get; set; }



        [BsonElement("TotalParcialidad07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad07 { get; set; }



        [BsonElement("MontoDiario07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario07 { get; set; }


        [BsonElement("IngresoAcumulable07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable07 { get; set; }




        [BsonElement("IngresoNoAcumulable07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable07 { get; set; }


        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion08 { get; set; }



        [BsonElement("TotalParcialidad08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad08 { get; set; }



        [BsonElement("MontoDiario08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario08 { get; set; }


        [BsonElement("IngresoAcumulable08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable08 { get; set; }




        [BsonElement("IngresoNoAcumulable08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable08 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion09 { get; set; }



        [BsonElement("TotalParcialidad09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad09 { get; set; }



        [BsonElement("MontoDiario09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario09 { get; set; }


        [BsonElement("IngresoAcumulable09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable09 { get; set; }




        [BsonElement("IngresoNoAcumulable09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable09 { get; set; }


        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion10 { get; set; }



        [BsonElement("TotalParcialidad10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad10 { get; set; }



        [BsonElement("MontoDiario10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario10 { get; set; }


        [BsonElement("IngresoAcumulable10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable10 { get; set; }




        [BsonElement("IngresoNoAcumulable10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable10 { get; set; }


        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion11 { get; set; }



        [BsonElement("TotalParcialidad11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad11 { get; set; }



        [BsonElement("MontoDiario11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario11 { get; set; }


        [BsonElement("IngresoAcumulable11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable11 { get; set; }




        [BsonElement("IngresoNoAcumulable11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable11 { get; set; }



        /*JubilacionPensionRetiro*/
        [BsonElement("TotalUnaExhibicion12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalUnaExhibicion12 { get; set; }



        [BsonElement("TotalParcialidad12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalParcialidad12 { get; set; }



        [BsonElement("MontoDiario12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fMontoDiario12 { get; set; }


        [BsonElement("IngresoAcumulable12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable12 { get; set; }




        [BsonElement("IngresoNoAcumulable12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable12 { get; set; }

    }

    //Percepcion- 022, 023, 025 
    public class SeparacionIndemnizacionMBDB
    {


        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado00 { get; set; }

        [BsonElement("NumAniosServicio00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio00 { get; set; }

        [BsonElement("UltimoSueldoMensOrd00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd00 { get; set; }


        [BsonElement("IngresoAcumulable00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable00 { get; set; }

        [BsonElement("IngresoNoAcumulable00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable00 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado01 { get; set; }

        [BsonElement("NumAniosServicio01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio01 { get; set; }

        [BsonElement("UltimoSueldoMensOrd01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd01 { get; set; }


        [BsonElement("IngresoAcumulable01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable01 { get; set; }

        [BsonElement("IngresoNoAcumulable01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable01 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado02 { get; set; }

        [BsonElement("NumAniosServicio02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio02 { get; set; }

        [BsonElement("UltimoSueldoMensOrd02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd02 { get; set; }


        [BsonElement("IngresoAcumulable02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable02 { get; set; }

        [BsonElement("IngresoNoAcumulable02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable02 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado03 { get; set; }

        [BsonElement("NumAniosServicio03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio03 { get; set; }

        [BsonElement("UltimoSueldoMensOrd03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd03 { get; set; }


        [BsonElement("IngresoAcumulable03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable03 { get; set; }

        [BsonElement("IngresoNoAcumulable03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable03 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado04 { get; set; }

        [BsonElement("NumAniosServicio04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio04 { get; set; }

        [BsonElement("UltimoSueldoMensOrd04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd04 { get; set; }


        [BsonElement("IngresoAcumulable04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable04 { get; set; }

        [BsonElement("IngresoNoAcumulable04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable04 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado05 { get; set; }

        [BsonElement("NumAniosServicio05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio05 { get; set; }

        [BsonElement("UltimoSueldoMensOrd05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd05 { get; set; }


        [BsonElement("IngresoAcumulable05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable05 { get; set; }

        [BsonElement("IngresoNoAcumulable05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable05 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado06 { get; set; }

        [BsonElement("NumAniosServicio06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio06 { get; set; }

        [BsonElement("UltimoSueldoMensOrd06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd06 { get; set; }


        [BsonElement("IngresoAcumulable06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable06 { get; set; }

        [BsonElement("IngresoNoAcumulable06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable06 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado07 { get; set; }

        [BsonElement("NumAniosServicio07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio07 { get; set; }

        [BsonElement("UltimoSueldoMensOrd07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd07 { get; set; }


        [BsonElement("IngresoAcumulable07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable07 { get; set; }

        [BsonElement("IngresoNoAcumulable07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable07 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado08 { get; set; }

        [BsonElement("NumAniosServicio08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio08 { get; set; }

        [BsonElement("UltimoSueldoMensOrd08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd08 { get; set; }


        [BsonElement("IngresoAcumulable08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable08 { get; set; }

        [BsonElement("IngresoNoAcumulable08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable08 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado09 { get; set; }

        [BsonElement("NumAniosServicio09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio09 { get; set; }

        [BsonElement("UltimoSueldoMensOrd09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd09 { get; set; }


        [BsonElement("IngresoAcumulable09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable09 { get; set; }

        [BsonElement("IngresoNoAcumulable09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable09 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado10 { get; set; }

        [BsonElement("NumAniosServicio10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio10 { get; set; }

        [BsonElement("UltimoSueldoMensOrd10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd10 { get; set; }


        [BsonElement("IngresoAcumulable10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable10 { get; set; }

        [BsonElement("IngresoNoAcumulable10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable10 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado11 { get; set; }

        [BsonElement("NumAniosServicio11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio11 { get; set; }

        [BsonElement("UltimoSueldoMensOrd11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd11 { get; set; }


        [BsonElement("IngresoAcumulable11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable11 { get; set; }

        [BsonElement("IngresoNoAcumulable11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable11 { get; set; }

        /*SeparacionIndemnizacion*/

        [BsonElement("TotalPagado12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fTotalPagado12 { get; set; }

        [BsonElement("NumAniosServicio12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iNumAniosServicio12 { get; set; }

        [BsonElement("UltimoSueldoMensOrd12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fUltimoSueldoMensOrd12 { get; set; }


        [BsonElement("IngresoAcumulable12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoAcumulable12 { get; set; }

        [BsonElement("IngresoNoAcumulable12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fIngresoNoAcumulable12 { get; set; }


    }
    #endregion

    #region Deducciones
    public class DeduccionesMBDB
    {
        [BsonElement("TotalOtrasDeducciones00", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones00 { get; set; }

        [BsonElement("TotalImpuestosRetenidos00", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos00 { get; set; }

        [BsonElement("Deduccion", Order = 3)]
        public List<DeduccionMBDB> lsDeduccion { get; set; }

        [BsonElement("TotalOtrasDeducciones01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones01 { get; set; }

        [BsonElement("TotalImpuestosRetenidos01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos01 { get; set; }

        [BsonElement("TotalOtrasDeducciones02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones02 { get; set; }

        [BsonElement("TotalImpuestosRetenidos02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos02 { get; set; }

        [BsonElement("TotalOtrasDeducciones03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones03 { get; set; }

        [BsonElement("TotalImpuestosRetenidos03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos03 { get; set; }

        [BsonElement("TotalOtrasDeducciones04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones04 { get; set; }

        [BsonElement("TotalImpuestosRetenidos04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos04 { get; set; }

        [BsonElement("TotalOtrasDeducciones05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones05 { get; set; }

        [BsonElement("TotalImpuestosRetenidos05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos05 { get; set; }

        [BsonElement("TotalOtrasDeducciones06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones06 { get; set; }

        [BsonElement("TotalImpuestosRetenidos06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos06 { get; set; }

        [BsonElement("TotalOtrasDeducciones07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones07 { get; set; }

        [BsonElement("TotalImpuestosRetenidos07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos07 { get; set; }

        [BsonElement("TotalOtrasDeducciones08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones08 { get; set; }

        [BsonElement("TotalImpuestosRetenidos08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos08 { get; set; }

        [BsonElement("TotalOtrasDeducciones09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones09 { get; set; }

        [BsonElement("TotalImpuestosRetenidos09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos09 { get; set; }

        [BsonElement("TotalOtrasDeducciones10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones10 { get; set; }

        [BsonElement("TotalImpuestosRetenidos10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos10 { get; set; }

        [BsonElement("TotalOtrasDeducciones11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones11 { get; set; }

        [BsonElement("TotalImpuestosRetenidos11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos11 { get; set; }

        [BsonElement("TotalOtrasDeducciones12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalOtrasDeducciones12 { get; set; }

        [BsonElement("TotalImpuestosRetenidos12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalImpuestosRetenidos12 { get; set; }

        public DeduccionesMBDB()
        {
            lsDeduccion = new List<DeduccionMBDB>();
        }

    }

    public class DeduccionMBDB
    {
        [BsonElement("Tipo")]
        public string sTipo { get; set; } = string.Empty;

        [BsonElement("Clave")]
        public string sClave { get; set; } = string.Empty;

        [BsonElement("Concepto")]
        public string sConcepto { get; set; } = string.Empty;

        [BsonElement("Importe00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte00 { get; set; }


        [BsonElement("Importe01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte01 { get; set; }

        [BsonElement("Importe02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte02 { get; set; }

        [BsonElement("Importe03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte03 { get; set; }

        [BsonElement("Importe04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte04 { get; set; }

        [BsonElement("Importe05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte05 { get; set; }

        [BsonElement("Importe06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte06 { get; set; }

        [BsonElement("Importe07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte07 { get; set; }

        [BsonElement("Importe08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte08 { get; set; }

        [BsonElement("Importe09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte09 { get; set; }
        [BsonElement("Importe10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte10 { get; set; }
        [BsonElement("Importe11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte11 { get; set; }
        [BsonElement("Importe12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte12 { get; set; }


    }
    #endregion

    #region OtrosPagos
    //Percepcion - 038
    public class OtrosPagosMBDB
    {

        [BsonElement("ImporteTotal00", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal00 { get; set; }

        [BsonElement("SubsidioCausadoTotal00", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal00 { get; set; }

        [BsonElement("OtroPago", Order = 3)]
        public List<OtroPagoMBDB> lsOtroPago { get; set; }

        [BsonElement("SubsidioAlEmpleo", Order = 4)]
        public SubsidioAlEmpleoMBDB oSubsidioAlEmpleo { get; set; }

        [BsonElement("CompensacionSaldoAFavor", Order = 5)]
        public CompensacionSaldoAFavorMBDB oCompensacionSaldoAFavor { get; set; }

        #region Totales Mensuales
        [BsonElement("ImporteTotal01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal01 { get; set; }

        [BsonElement("SubsidioCausadoTotal01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal01 { get; set; }

        [BsonElement("ImporteTotal02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal02 { get; set; }

        [BsonElement("SubsidioCausadoTotal02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal02 { get; set; }

        [BsonElement("ImporteTotal03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal03 { get; set; }

        [BsonElement("SubsidioCausadoTotal03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal03 { get; set; }

        [BsonElement("ImporteTotal04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal04 { get; set; }

        [BsonElement("SubsidioCausadoTotal04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal04 { get; set; }

        [BsonElement("ImporteTotal05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal05 { get; set; }

        [BsonElement("SubsidioCausadoTotal05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal05 { get; set; }

        [BsonElement("ImporteTotal06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal06 { get; set; }

        [BsonElement("SubsidioCausadoTotal06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal06 { get; set; }

        [BsonElement("ImporteTotal07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal07 { get; set; }

        [BsonElement("SubsidioCausadoTotal07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal07 { get; set; }

        [BsonElement("ImporteTotal08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal08 { get; set; }

        [BsonElement("SubsidioCausadoTotal08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal08 { get; set; }

        [BsonElement("ImporteTotal09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal09 { get; set; }

        [BsonElement("SubsidioCausadoTotal09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal09 { get; set; }

        [BsonElement("ImporteTotal10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal10 { get; set; }

        [BsonElement("SubsidioCausadoTotal10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal10 { get; set; }

        [BsonElement("ImporteTotal11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal11 { get; set; }

        [BsonElement("SubsidioCausadoTotal11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal11 { get; set; }

        [BsonElement("ImporteTotal12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal12 { get; set; }

        [BsonElement("SubsidioCausadoTotal12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausadoTotal12 { get; set; }
        #endregion
        public OtrosPagosMBDB()
        {
            lsOtroPago = new List<OtroPagoMBDB>();
            oSubsidioAlEmpleo = new SubsidioAlEmpleoMBDB();
            oCompensacionSaldoAFavor = new CompensacionSaldoAFavorMBDB();

        }

    }

    public class OtroPagoMBDB
    {
        [BsonElement("Tipo")]
        public string sTipo { get; set; } = string.Empty;

        [BsonElement("Clave")]
        public string sClave { get; set; } = string.Empty;

        [BsonElement("Concepto")]
        public string sConcepto { get; set; } = string.Empty;

        [BsonElement("Importe00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte00 { get; set; }


        [BsonElement("Importe01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte01 { get; set; }

        [BsonElement("Importe02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte02 { get; set; }

        [BsonElement("Importe03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte03 { get; set; }

        [BsonElement("Importe04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte04 { get; set; }

        [BsonElement("Importe05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte05 { get; set; }

        [BsonElement("Importe06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte06 { get; set; }

        [BsonElement("Importe07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte07 { get; set; }

        [BsonElement("Importe08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte08 { get; set; }

        [BsonElement("Importe09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte09 { get; set; }
        [BsonElement("Importe10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte10 { get; set; }
        [BsonElement("Importe11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte11 { get; set; }
        [BsonElement("Importe12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte12 { get; set; }

    }

    public class SubsidioAlEmpleoMBDB
    {
        [BsonElement("SubsidioCausado00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado00 { get; set; }
        [BsonElement("SubsidioCausado01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado01 { get; set; }

        [BsonElement("SubsidioCausado02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado02 { get; set; }

        [BsonElement("SubsidioCausado03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado03 { get; set; }

        [BsonElement("SubsidioCausado04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado04 { get; set; }

        [BsonElement("SubsidioCausado05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado05 { get; set; }

        [BsonElement("SubsidioCausado06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado06 { get; set; }

        [BsonElement("SubsidioCausado07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado07 { get; set; }

        [BsonElement("SubsidioCausado08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado08 { get; set; }

        [BsonElement("SubsidioCausado09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado09 { get; set; }
        [BsonElement("SubsidioCausado10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado10 { get; set; }
        [BsonElement("SubsidioCausado11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado11 { get; set; }
        [BsonElement("SubsidioCausado12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSubsidioCausado12 { get; set; }



    }

    public class CompensacionSaldoAFavorMBDB
    {
        [BsonElement("SaldoAFavor00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor00 { get; set; }



        [BsonElement("RemanenteSalFav00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav00 { get; set; }

        /*Horas Extra*/
        [BsonElement("SaldoAFavor01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor01 { get; set; }

        [BsonElement("Anio01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio01 { get; set; }

        [BsonElement("RemanenteSalFav01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav01 { get; set; }


        /*Horas Extra*/
        [BsonElement("SaldoAFavor02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor02 { get; set; }

        [BsonElement("Anio02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio02 { get; set; }

        [BsonElement("RemanenteSalFav02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav02 { get; set; }



        /*Horas Extra*/
        [BsonElement("SaldoAFavor03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor03 { get; set; }

        [BsonElement("Anio03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio03 { get; set; }


        [BsonElement("RemanenteSalFav03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav03 { get; set; }


        /*Horas Extra*/
        [BsonElement("SaldoAFavor04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor04 { get; set; }

        [BsonElement("Anio04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio04 { get; set; }


        [BsonElement("RemanenteSalFav04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav04 { get; set; }



        /*Horas Extra*/
        [BsonElement("SaldoAFavor05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor05 { get; set; }

        [BsonElement("Anio05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio05 { get; set; }


        [BsonElement("RemanenteSalFav05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav05 { get; set; }


        /*Horas Extra*/
        [BsonElement("SaldoAFavor06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor06 { get; set; }

        [BsonElement("Anio06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio06 { get; set; }


        [BsonElement("RemanenteSalFav06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav06 { get; set; }



        /*Horas Extra*/
        [BsonElement("SaldoAFavor07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor07 { get; set; }

        [BsonElement("Anio07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio07 { get; set; }


        [BsonElement("RemanenteSalFav07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav07 { get; set; }



        /*Horas Extra*/
        [BsonElement("SaldoAFavor08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor08 { get; set; }

        [BsonElement("Anio08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio08 { get; set; }


        [BsonElement("RemanenteSalFav08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav08 { get; set; }



        /*Horas Extra*/
        [BsonElement("SaldoAFavor09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor09 { get; set; }

        [BsonElement("Anio09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio09 { get; set; }


        [BsonElement("RemanenteSalFav09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav09 { get; set; }


        /*Horas Extra*/
        [BsonElement("SaldoAFavor10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor10 { get; set; }

        [BsonElement("Anio10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio10 { get; set; }


        [BsonElement("RemanenteSalFav10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav10 { get; set; }

        /*Horas Extra*/
        [BsonElement("SaldoAFavor11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor11 { get; set; }

        [BsonElement("Anio11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio11 { get; set; }


        [BsonElement("RemanenteSalFav11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav11 { get; set; }

        /*Horas Extra*/
        [BsonElement("SaldoAFavor12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fSaldoAFavor12 { get; set; }

        [BsonElement("Anio12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iAnio12 { get; set; }


        [BsonElement("RemanenteSalFav12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fRemanenteSalFav12 { get; set; }



    }
    #endregion

    #region Incapacidades
    //Percepcion-014
    public class IncapacidadesMBDB
    {


        [BsonElement("ImporteTotal00", Order = 1)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal00 { get; set; }

        [BsonElement("DiasIncapacidadTotal00", Order = 2)]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iDiasIncapacidadTotal00 { get; set; }

        [BsonElement("Incapacidad", Order = 3)]
        public List<IncapacidadMBDB> lsIncapacidad { get; set; }

        #region Totales Mensuales
        [BsonElement("ImporteTotal01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal01 { get; set; }

        [BsonElement("DiasIncapacidadTotal01")]
        public int iDiasIncapacidadTotal01 { get; set; }

        [BsonElement("ImporteTotal02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal02 { get; set; }

        [BsonElement("DiasIncapacidadTotal02")]
        public int iDiasIncapacidadTotal02 { get; set; }

        [BsonElement("ImporteTotal03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal03 { get; set; }

        [BsonElement("DiasIncapacidadTotal03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public int iDiasIncapacidadTotal03 { get; set; }

        [BsonElement("ImporteTotal04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal04 { get; set; }

        [BsonElement("DiasIncapacidadTotal04")]
        public int iDiasIncapacidadTotal04 { get; set; }

        [BsonElement("ImporteTotal05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal05 { get; set; }

        [BsonElement("DiasIncapacidadTotal05")]
        public int iDiasIncapacidadTotal05 { get; set; }

        [BsonElement("ImporteTotal06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal06 { get; set; }

        [BsonElement("DiasIncapacidadTotal06")]
        public int iDiasIncapacidadTotal06 { get; set; }

        [BsonElement("ImporteTotal07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal07 { get; set; }

        [BsonElement("DiasIncapacidadTotal07")]
        public int iDiasIncapacidadTotal07 { get; set; }

        [BsonElement("ImporteTotal08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal08 { get; set; }

        [BsonElement("DiasIncapacidadTotal08")]
        public int iDiasIncapacidadTotal08 { get; set; }

        [BsonElement("ImporteTotal09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal09 { get; set; }

        [BsonElement("DiasIncapacidadTotal09")]
        public int iDiasIncapacidadTotal09 { get; set; }

        [BsonElement("ImporteTotal10")]
        public decimal fImporteTotal10 { get; set; }

        [BsonElement("DiasIncapacidadTotal10")]
        public int iDiasIncapacidadTotal10 { get; set; }

        [BsonElement("ImporteTotal11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal11 { get; set; }

        [BsonElement("DiasIncapacidadTotal11")]
        public int iDiasIncapacidadTotal11 { get; set; }

        [BsonElement("ImporteTotal12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporteTotal12 { get; set; }

        [BsonElement("DiasIncapacidadTotal12")]
        public int iDiasIncapacidadTotal12 { get; set; }

        #endregion

        public IncapacidadesMBDB()
        {
            lsIncapacidad = new List<IncapacidadMBDB>();
        }

    }

    public class IncapacidadMBDB
    {
        [BsonElement("Tipo")]
        public string sTipoIncapacidad { get; set; } = string.Empty;

        [BsonElement("DiasIncapacidad00")]
        public int iDiasIncapacidad00 { get; set; }
        [BsonElement("Importe00")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte00 { get; set; }

        [BsonElement("DiasIncapacidad01")]
        public int iDiasIncapacidad01 { get; set; }

        [BsonElement("DiasIncapacidad02")]
        public int iDiasIncapacidad02 { get; set; }

        [BsonElement("DiasIncapacidad03")]
        public int iDiasIncapacidad03 { get; set; }

        [BsonElement("DiasIncapacidad04")]
        public int iDiasIncapacidad04 { get; set; }

        [BsonElement("DiasIncapacidad05")]
        public int iDiasIncapacidad05 { get; set; }

        [BsonElement("DiasIncapacidad06")]
        public int iDiasIncapacidad06 { get; set; }

        [BsonElement("DiasIncapacidad07")]
        public int iDiasIncapacidad07 { get; set; }

        [BsonElement("DiasIncapacidad08")]
        public int iDiasIncapacidad08 { get; set; }

        [BsonElement("DiasIncapacidad09")]
        public int iDiasIncapacidad09 { get; set; }

        [BsonElement("DiasIncapacidad10")]
        public int iDiasIncapacidad10 { get; set; }

        [BsonElement("DiasIncapacidad11")]
        public int iDiasIncapacidad11 { get; set; }

        [BsonElement("DiasIncapacidad12")]
        public int iDiasIncapacidad12 { get; set; }



        [BsonElement("Importe01")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte01 { get; set; }


        [BsonElement("Importe02")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte02 { get; set; }

        [BsonElement("Importe03")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte03 { get; set; }

        [BsonElement("Importe04")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte04 { get; set; }

        [BsonElement("Importe05")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte05 { get; set; }

        [BsonElement("Importe06")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte06 { get; set; }

        [BsonElement("Importe07")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte07 { get; set; }

        [BsonElement("Importe08")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte08 { get; set; }

        [BsonElement("Importe09")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte09 { get; set; }
        [BsonElement("Importe10")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte10 { get; set; }
        [BsonElement("Importe11")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte11 { get; set; }
        [BsonElement("Importe12")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal fImporte12 { get; set; }

    }
    #endregion
}
