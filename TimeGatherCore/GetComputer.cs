

using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Windows;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeGatherCore
{
    class GetComputer
    {
        public static List<ServerModel> GetComputers()
        {

            var domain = Domain.GetCurrentDomain();
            string ldapconnection = $"LDAP://{domain.Name}";
            List<ServerModel> computerNames = new List<ServerModel>();

            using (DirectoryEntry entry = new DirectoryEntry(ldapconnection))
            {
                using (DirectorySearcher mySearcher = new DirectorySearcher(entry))
                {
                    mySearcher.Filter = ("(objectClass=computer)");

                    // No size limit, reads all objects
                    mySearcher.SizeLimit = 0;

                    // Read data in pages of 250 objects. Make sure this value is below the limit configured in your AD domain (if there is a limit)
                    mySearcher.PageSize = 250;

                    // Let searcher know which properties are going to be used, and only load those
                    mySearcher.PropertiesToLoad.Add("name");
                    mySearcher.PropertiesToLoad.Add("operatingSystem");


                    //save out results for analysis
                    var results = mySearcher.FindAll();
                    WriteResultsToDocs(results);

                    foreach (SearchResult resEnt in results)
                    {
                        var output = new Dictionary<string, List<string>>();
                        try
                        {
                            //convert to something more generic
                            foreach (DictionaryEntry prop in resEnt.Properties)
                            {
                                ResultPropertyValueCollection rpvc = (ResultPropertyValueCollection)prop.Value;
                                var values = new List<string>();
                                foreach (string h in rpvc)
                                {
                                    values.Add(h);
                                }
                                if (values.Count >= 0)
                                {
                                    output.Add(prop.Key.ToString(), values);
                                }
                            }
                            //no array out of bounds hopefully
                            //var name = (output.FirstOrDefault(w => w.Key == "name").Value.FirstOrDefault()) ?? "unknown";
                            string name;
                            if (output.Count == 0)
                            {
                                MessageBox.Show("AD Issue", $"0 records returned from {resEnt.Properties} query.", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                            }


                            var de = output.FirstOrDefault(w => w.Key == "name");
                            if (de.Value.Count == 0)
                            {
                                // don't have a name, so give up on a record.
                                continue;
                            }
                            else
                            {
                                name = de.Value.FirstOrDefault() ?? "unknown";
                            }
                            var operatingSystem = (output.FirstOrDefault(w => w.Key == "operatingsystem").Value.FirstOrDefault()) ?? "unknown";

                            computerNames.Add(new ServerModel() { Name = name, OS = operatingSystem });
                        }
                        catch (Exception ex)
                        {
                            WriteOutExceptionAndCurrentOutputToLog(ex, output);
                            continue;
                        }





                        // Note: Properties can contain multiple values.
                        //if (resEnt.Properties["name"].Count > 0)
                        //{
                        //    var os = (string)resEnt.Properties["operatingSystem"][0];
                        //    if (os.Contains("Server"))
                        //    {
                        //        computerNames.Add(new ServerModel()
                        //        {
                        //            Name = (string)resEnt.Properties["name"][0],
                        //            OS = (string)resEnt.Properties["operatingSystem"][0]
                        //        });
                        //    }


                        //}
                    }
                }
            }

            return computerNames;
        }

        private static void WriteOutExceptionAndCurrentOutputToLog(Exception ex, Dictionary<string, List<string>> output)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string path = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.MyDoc‌​uments), "TimeGather2");
            path = Path.Combine(path, "MyComputerExceptionLog.txt");
            var serializedresults = JsonSerializer.Serialize(output, options);
            File.WriteAllText(path, ex.Message);
            File.WriteAllText(path, serializedresults);
        }

        private static void WriteResultsToDocs(SearchResultCollection results)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string path = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.MyDoc‌​uments), "TimeGather2");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, "output.txt");
            var serializedresults = JsonSerializer.Serialize<SearchResultCollection>(results, options);
            File.WriteAllText(path, serializedresults);
        }
    }
}
