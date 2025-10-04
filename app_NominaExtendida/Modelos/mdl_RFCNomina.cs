using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_NominaExtendida.Modelos
{
    public class mdl_RFCNomina
    {
        public string sRFC { get; set; }

        public int iMes { get; set; }

        public string sPeriodo { get; set; }

        public List<byte[]> lsyXML { get; set; }

        public mdl_RFCNomina()
        {
            sRFC = string.Empty;
            sPeriodo = string.Empty;
            lsyXML = new List<byte[]>();
        }
    }
}
