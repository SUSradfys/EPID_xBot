using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace EPID_xBot
{
    public static class Settings
    {
        private const string settings_file = @"C:\Program Files\EPID_xBot\Settings.xml";

        public static string ARIA_SERVER = getFromXml(settings_file, "ARIA_SERVER");
        public static string ARIA_USERNAME = getFromXml(settings_file, "ARIA_USERNAME");
        public static string ARIA_PASSWORD = getFromXml(settings_file, "ARIA_PASSWORD");
        public static string ARIA_DATABASE = getFromXml(settings_file, "ARIA_DATABASE");

        public static string DoseCrit = getFromXml(settings_file, "DoseCrit");
        public static string DistCrit = getFromXml(settings_file, "DistCrit");
        public static string DoseThreshold = getFromXml(settings_file, "DoseThreshold");
        public static string AcceptCrit = getFromXml(settings_file, "AcceptCrit");
        public static string LocalDose = getFromXml(settings_file, "LocalDose");

        public static string xPorterPath = @"C:\Program Files\EPID_xBot\xPorts.xml";

        

        private static string getFromXml(string file, string varible)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            return doc.SelectSingleNode("Settings/" + varible).InnerText;
        }
    }
}

