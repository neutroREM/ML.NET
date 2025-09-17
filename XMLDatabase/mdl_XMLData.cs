namespace XMLDatabase
{
    internal class mdl_XMLData
    {
        public int iMes { get; set; }
        public string sRFC { get; set; } = string.Empty;
        public string sMesXML { get; set; } = string.Empty;
        public string sProceso { get; set; } = string.Empty;
        public string NombreEmisor { get; set; } = string.Empty;
        public List<byte[]> lsyXMLs { get; set; } = new List<byte[]>();
        public string sPeriodo { get; set; } = string.Empty;
    }
}
