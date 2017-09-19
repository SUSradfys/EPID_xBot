using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPID_xBot
{
    class SFTP
    {
        const string host = "10.0.129.22";
        const string username = "epidQA";
        const string workingdirectory = "/home/epidQA/incoming/";
        const int port = 22;
        //const string pass = "EAAAAD7WBwKSpdA6ZvRq0pjWo6gc3hwHZ0H8Ivkp59kUsd/I";
        const string pass ="EAAAAKIse1a1dXELxya3lnpbD5kuXU/ZFHMpEYqcT4UKgZ8A";
        const string key = "adgadqqg";

        public static void Send(string fileName)
        {
            Console.WriteLine("Creating client and connecting");
            using (var client = new SftpClient(host, port, username, Crypto.DecryptStringAES(pass, key)))
            {
                try
                { 
                    client.Connect();
                }
                catch (Exception)
                {
                    throw;
                }
                Console.WriteLine("Connected to {0}", host);

                client.ChangeDirectory(workingdirectory);
                Console.WriteLine("Changed directory to {0}", workingdirectory);

                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    Console.WriteLine("Uploading {0} ({1:N0} bytes)", fileName, fileStream.Length);
                    client.BufferSize = 4 * 1024; // bypass Payload error large files
                    client.UploadFile(fileStream, Path.GetFileName(fileName));
                }
                client.Disconnect();
            }
        }

        public static bool Verify(string filename)
        {
            bool fileExist;
            using (var client = new SftpClient(host, port, username, Crypto.DecryptStringAES(pass, key)))
            {
                client.Connect();
                client.ChangeDirectory(workingdirectory);
                fileExist = client.Exists(filename);
            }
            return fileExist;
        }
    }
}