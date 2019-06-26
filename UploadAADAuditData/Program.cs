using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace UploadAADAuditData
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.AppSettings["EventHubConnectionString"];
        private static string _eventHubName = ConfigurationManager.AppSettings["EventHubName"];
        private static string _fileName = ConfigurationManager.AppSettings["FileName"];

        static void Main(string[] args)
        {
            EventHubClient client = null;

            try
            {
                if (File.Exists(_fileName))
                {
                    string auditData = File.ReadAllText(_fileName);
                    JArray auditDataJson = JArray.Parse(auditData);
                    client = EventHubClient.CreateFromConnectionString(_connectionString, _eventHubName);

                    foreach (JObject auditEvent in auditDataJson)
                    {
                        client.Send(new EventData(Encoding.UTF8.GetBytes(auditEvent.ToString())));
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Input file {0} not found.", _fileName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Upload of Azure AD Audit Data to Event Hub failed with the following error: {0}", ex.Message));
            }
            finally
            {
                if (client != null) client.Close();
            }
        }
    }
}
