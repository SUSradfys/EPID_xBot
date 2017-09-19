using EvilDICOM.Network;
using EvilDICOM.Network.Querying;
using EvilDICOM.Network.DIMSE.IOD;
using System;
using System.Data;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Helpers;
using System.Xml.Serialization;
using System.Text;


namespace EPID_xBot
{
    /// <summary>
    /// xBot sends instructions to a ARIA Database Daemon service.
    /// The information is used to conduct a DICOM CMOVE operation from the ARIA OIS to third party systems.
    /// The EvilDICOM (https://github.com/rexcardan/Evil-DICOM) library is used as a middle hand.
    /// </summary>
    class Program
    {
        //static variables for the DB Daemon
        static string AEtitleDB = "ARIADB";
        static int portDB = 57347;

        // mail part
        static string recipient = "dicomdaemon.onk.sus.@skane.se";
        static string subject = "EPID export";

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Execute();
            }
            catch (Exception exc)
            {
                sendMail.Program.send(recipient, subject, exc.Message);
            }

        }

        static void Execute()
        {
            List<CFindImageIOD> iods = new List<CFindImageIOD>();
            // define the DB Deamon entity
            Entity daemon = Entity.CreateLocal(AEtitleDB, portDB);

            // define the local service class
            var me = Entity.CreateLocal("EvilDICOMC", 50400);
            var scu = new DICOMSCU(me);

            // define the query builder
            var qb = new QueryBuilder(scu, daemon);
            ushort msgId = 1;

            // xml Deserialize
            string xml = File.ReadAllText(Settings.xPorterPath);
            var xport = xml.ParseXML<xports>();

            // Define the reciever
            Entity reciever = new Entity(xport.xporter.AEtitle, xport.xporter.ipstring, xport.xporter.port);
            DICOMSCP scp = new DICOMSCP(reciever);

            // Query plan
            DataTable plans = new DataTable();
            if (!String.IsNullOrEmpty(xport.xporter.SQLstring))
            {
                SqlInterface.Connect();
                plans = xport.Query();
                SqlInterface.Disconnect();
            }

            // loop through plans
            foreach (DataRow row in plans.Rows)
            {
                var patId = ((string)row["PatientId"]);
                // Remove special characters from patId
                byte[] patIdBytes;
                patIdBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(patId);
                patId = Encoding.UTF8.GetString(patIdBytes);
                var ImageUID = (string)row["ImageUID"];
                var RefUID = (string)row["RefUID"];
                double MU = (double)row["MU"];
                
                iods.Add(new CFindImageIOD() { PatientId = patId, SOPInstanceUID = ImageUID });
                iods.Add(new CFindImageIOD() { PatientId = patId, SOPInstanceUID = RefUID });

                if (xport.xporter.active)
                { 
                    // Generate linking text file
                    string[] content = { ImageUID, RefUID, MU.ToString(), Settings.DoseCrit, Settings.DistCrit, Settings.DoseThreshold, Settings.LocalDose, Settings.AcceptCrit };
                    File.WriteAllLines(ImageUID + ".dat", content);

                    // SFTP it
                    SFTP.Send(ImageUID + ".dat");

                    // Delte file
                    File.Delete(ImageUID + ".dat");
                }
            }


            // change lastActivity
            if (plans.Rows.Count > 0)  // CHange 2000 to 0 later on
            {
                DateTime lastPlan = (DateTime)plans.Rows[plans.Rows.Count - 1]["DateTime"];

                // write xml
                using (FileStream fs = new FileStream(Settings.xPorterPath, FileMode.Create))
                {
                    XmlSerializer _xSer = new XmlSerializer(typeof(xports));

                    _xSer.Serialize(fs, xport);
                }
            }

            // if active send
            if (xport.xporter.active)
            {
                Console.WriteLine(iods.Count.ToString());
                // Remove duplicate UIDs
                if (!xport.xporter.allowDoublets)
                    iods = ListHandler.Unique(iods);

                Console.WriteLine(iods.Count.ToString());
                foreach (var iod in iods)
                {
                    // Send it
                    scu.SendCMoveImage(daemon, iod, xport.xporter.AEtitle, ref msgId);
                    // Add logging

                    // verify file recieved
                    var recieved = SFTP.Verify("RI." + iod.SOPInstanceUID + ".dcm");
                    if (!recieved)
                        throw new FileNotFoundException("CMove Operation not verified.");
                }
            }

        }
    }

    
}
