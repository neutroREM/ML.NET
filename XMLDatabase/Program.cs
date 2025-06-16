using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLDatabase
{
    internal class Program
    {
        static Stopwatch timer = new Stopwatch();
        static void Main(string[] args)
        {
            var sampleData = new ML1.ModelInput()
            {
                Col0 = "This restaurant was wonderful",
            };

            var result = ML1.Predict(sampleData);

            var sentiment = result.PredictedLabel == 1 ? "Positive" : "Negative";
            Console.WriteLine($"Text: {sampleData.Col0} \nSentiment: {sentiment}");
            Console.ReadLine();



            #region Meses XMLs 
            mdl_XMLData oData01 = new mdl_XMLData();
            mdl_XMLData oData02 = new mdl_XMLData();
            mdl_XMLData oData03 = new mdl_XMLData();
            mdl_XMLData oData04 = new mdl_XMLData();
            mdl_XMLData oData05 = new mdl_XMLData();
            mdl_XMLData oData06 = new mdl_XMLData();
            mdl_XMLData oData07 = new mdl_XMLData();
            mdl_XMLData oData08 = new mdl_XMLData();
            mdl_XMLData oData09 = new mdl_XMLData();
            mdl_XMLData oData10 = new mdl_XMLData();
            mdl_XMLData oData11 = new mdl_XMLData();
            mdl_XMLData oData12 = new mdl_XMLData();
            cls_XMLProcessor oLector = new cls_XMLProcessor();

            #endregion

            #region Obtención y asignacion de los XMLs
            //string XML33 = ConfigurationManager.AppSettings["RutaXML33"];
            //string XML33fc = ConfigurationManager.AppSettings["RutaXML33fc"];
            string? XML33 = ConfigurationManager.AppSettings["RutaXML33"] ?? string.Empty;
            string? XML33fc = ConfigurationManager.AppSettings["RutaXML33fc"] ?? string.Empty;
            string? XML40 = ConfigurationManager.AppSettings["RutaXML40"] ?? string.Empty;
            string? XML40fad = ConfigurationManager.AppSettings["RutaXML40fad"] ?? string.Empty;
            string? XML40he = ConfigurationManager.AppSettings["RutaXML40he"] ?? string.Empty;
            string? XML40i = ConfigurationManager.AppSettings["RutaXML40i"] ?? string.Empty;
            string? XML401 = ConfigurationManager.AppSettings["RutaXML401"] ?? string.Empty;
            string? XML402 = ConfigurationManager.AppSettings["RutaXML402"] ?? string.Empty;
            string? XML403 = ConfigurationManager.AppSettings["RutaXML403"] ?? string.Empty;
            string? XML404 = ConfigurationManager.AppSettings["RutaXML404"] ?? string.Empty;
            string? XML405 = ConfigurationManager.AppSettings["RutaXML405"] ?? string.Empty;
            string? XML406 = ConfigurationManager.AppSettings["RutaXML406"] ?? string.Empty;
            string? XML407 = ConfigurationManager.AppSettings["RutaXML407"] ?? string.Empty;

            //byte[] ayXML33 = File.ReadAllBytes(XML33);
            //byte[] ayXML33fc = File.ReadAllBytes(XML33fc);
            byte[] ayXML33 = File.ReadAllBytes(XML33);
            byte[] ayXML33fc = File.ReadAllBytes(XML33fc);
            byte[] ayXML40 = File.ReadAllBytes(XML40);
            byte[] ayXML40fad = File.ReadAllBytes(XML40fad);
            byte[] ayXML40he = File.ReadAllBytes(XML40he);
            byte[] ayXML40i = File.ReadAllBytes(XML40i);
            byte[] ayXML401 = File.ReadAllBytes(XML401);
            byte[] ayXML402 = File.ReadAllBytes(XML402);
            byte[] ayXML403 = File.ReadAllBytes(XML403);
            byte[] ayXML404 = File.ReadAllBytes(XML404);
            byte[] ayXML405 = File.ReadAllBytes(XML405);
            byte[] ayXML406 = File.ReadAllBytes(XML406);
            byte[] ayXML407 = File.ReadAllBytes(XML407);
            #endregion

            #region Proceso de lectura de XMLs por mes
            oData01.sProceso = "Fin";
            oData01.iMes = 1;
            oData01.sMesXML = "01";
            oData01.lsyXMLs = new List<byte[]>();
            //oData01.lsyXMLs.Add(ayXML402);
            oData01.lsyXMLs.Add(ayXML33);


            oData02.sProceso = "Continua";
            oData02.iMes = 2;
            oData02.lsyXMLs = new List<byte[]>();
            oData02.lsyXMLs.Add(ayXML40);
            oData02.lsyXMLs.Add(ayXML40i);
            oData02.sMesXML = "02";
            oData02.lsyXMLs.Add(ayXML40he);

            oData03.sProceso = "Continua";
            oData03.iMes = 3;
            oData03.lsyXMLs = new List<byte[]>();
            oData03.lsyXMLs.Add(ayXML40);
            oData03.lsyXMLs.Add(ayXML402);

            oData03.lsyXMLs.Add(ayXML403);
            oData03.lsyXMLs.Add(ayXML404);
            oData03.lsyXMLs.Add(ayXML405);

            oData03.lsyXMLs.Add(ayXML40he);
            oData03.sMesXML = "03";
            oData04.sProceso = "Continua";
            oData04.iMes = 4;
            oData04.lsyXMLs = new List<byte[]>();

            oData04.lsyXMLs.Add(ayXML405);
            oData04.lsyXMLs.Add(ayXML406);
            oData04.sMesXML = "04";
            oData05.sProceso = "Continua";
            oData05.iMes = 5;
            oData05.lsyXMLs = new List<byte[]>();
            oData05.lsyXMLs.Add(ayXML40);
            oData05.lsyXMLs.Add(ayXML40fad);
            oData05.lsyXMLs.Add(ayXML40i);

            oData05.lsyXMLs.Add(ayXML401);
            oData05.lsyXMLs.Add(ayXML403);
            oData05.lsyXMLs.Add(ayXML404);
            oData05.lsyXMLs.Add(ayXML405);

            oData05.sMesXML = "05";
            oData06.sProceso = "Continua";
            oData06.iMes = 6;
            oData06.lsyXMLs = new List<byte[]>();
            oData06.lsyXMLs.Add(ayXML40);
            oData06.lsyXMLs.Add(ayXML40fad);
            oData06.lsyXMLs.Add(ayXML40i);
            oData06.sMesXML = "06";
            oData07.sProceso = "Continua";
            oData07.iMes = 7;
            oData07.lsyXMLs = new List<byte[]>();
            oData07.lsyXMLs.Add(ayXML40);
            oData07.lsyXMLs.Add(ayXML40fad);
            oData07.lsyXMLs.Add(ayXML40he);
            oData07.sMesXML = "07";
            oData08.sProceso = "Continua";
            oData08.iMes = 8;
            oData08.lsyXMLs = new List<byte[]>();
            oData08.lsyXMLs.Add(ayXML40);
            oData08.lsyXMLs.Add(ayXML40fad);
            oData08.lsyXMLs.Add(ayXML40he);
            oData08.lsyXMLs.Add(ayXML33fc);
            oData08.lsyXMLs.Add(ayXML401);
            oData08.sMesXML = "08";
            oData09.sProceso = "Continua";
            oData09.iMes = 9;
            oData09.lsyXMLs = new List<byte[]>();
            oData09.lsyXMLs.Add(ayXML40);
            oData09.lsyXMLs.Add(ayXML40fad);
            oData09.lsyXMLs.Add(ayXML40he);
            oData09.lsyXMLs.Add(ayXML40i);
            oData09.sMesXML = "09";
            oData10.sProceso = "Continua";
            oData10.iMes = 10;
            oData10.lsyXMLs = new List<byte[]>();
            oData10.lsyXMLs.Add(ayXML40);
            oData10.lsyXMLs.Add(ayXML402);
            oData10.lsyXMLs.Add(ayXML40he);
            oData10.lsyXMLs.Add(ayXML40i);
            oData10.sMesXML = "10";

            oData11.sProceso = "Continua";
            oData11.iMes = 11;
            oData11.lsyXMLs = new List<byte[]>();
            oData11.lsyXMLs.Add(ayXML40);

            oData11.lsyXMLs.Add(ayXML40he);
            oData11.lsyXMLs.Add(ayXML40i);
            oData11.sMesXML = "11";

            oData12.sProceso = "Fin";
            oData12.iMes = 12;
            oData12.lsyXMLs = new List<byte[]>();
            oData12.lsyXMLs.Add(ayXML40);
            oData12.sMesXML = "12";
            oData12.lsyXMLs.Add(ayXML40he);
            oData12.lsyXMLs.Add(ayXML40i);
            oData12.lsyXMLs.Add(ayXML406);
            oData12.lsyXMLs.Add(ayXML407);

            #endregion

            timer.Start();
            oLector.XMLCatcher(oData01);
            oLector.XMLCatcher(oData02);
            oLector.XMLCatcher(oData03);
            oLector.XMLCatcher(oData04);
            oLector.XMLCatcher(oData05);
            oLector.XMLCatcher(oData06);
            oLector.XMLCatcher(oData07);
            oLector.XMLCatcher(oData08);
            oLector.XMLCatcher(oData09);
            oLector.XMLCatcher(oData10);
            oLector.XMLCatcher(oData11);
            oLector.XMLCatcher(oData12);
        }
    }
}
