using Microsoft.Office.Interop.Excel;
using Proxy.Models;
using Routing;
using Routing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;

namespace HeavyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Environment.GetEnvironmentVariable("host") ?? "localhost:8080";
            string address = string.Format("http://{0}/", host);

            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IRouting> factory = new ChannelFactory<IRouting>(binding, address);
            IRouting client = factory.CreateChannel();

            string input;

            do
            {
                Console.Clear();
                Console.WriteLine("Bienvenue sur le client .Net de Let's Go Biking !");
                Console.WriteLine("\nChoisissez ce que vous voulez faire :");
                Console.WriteLine("\t- path : Établir un itinéraire entre 2 adresses avec des vélos si nécessaire");
                Console.WriteLine("\t- stats : Voir les utilisations des différentes stations de JCDecaux depuis notre serveur");
                Console.WriteLine("\t- export : Exporter les utilisations des différentes stations de JCDecaux depuis notre serveur vers un fichier Excel (stats.xls)");
                Console.WriteLine("\t- quit : Quitter le client .Net");
                Console.Write("\nChoix : ");
                input = Console.ReadLine().ToLower().Trim();

                if (input.Equals("path"))
                {
                    SearchRoute(client);
                }
                else if (input.Equals("stats"))
                {
                    WriteLogs(client);
                    Console.WriteLine("\nAppuyez sur une touche pour revenir au menu principal ...");
                    Console.ReadLine();
                }
                else if (input.Equals("export"))
                {
                    Console.WriteLine("\nNouvelle feuille de travail en cours de création ...");
                    WriteLogsInExcel(client);
                    Console.WriteLine("\nAppuyez sur une touche pour revenir au menu principal ...");
                    Console.ReadLine();
                }
                else if (input.Equals("quit"))
                {
                    Console.WriteLine("Fermeture ...");
                }
                else
                {
                    Console.WriteLine("Mauvaise entrée, veuillez réessayer ...");
                    Console.WriteLine("\nAppuyez sur une touche pour revenir au menu principal ...");
                    Console.ReadLine();
                }
            } while (!input.Equals("quit"));
        }

        private static void SearchRoute(IRouting client)
        {
            do
            {
                Console.Write("Adresse de départ : ");
                string startAddress = Console.ReadLine().Trim().Replace(" ", "+"); // 11 Rue Saint Jacques, Lyon

                Console.Write("Adresse d'arrivée : ");
                string endAddress = Console.ReadLine().Trim().Replace(" ", "+"); // 19 Place Louis Pradel, Lyon OR 127 Avenue du Prado, Marseille

                if (startAddress.Equals("null") || endAddress.Equals("null") || startAddress.Equals("") || endAddress.Equals(""))
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("\nAu moins une adresse est incorrecte ...");
                }
                else
                {
                    Position startAddressPosition = client.GetPosition(startAddress);
                    Position endAddressPosition = client.GetPosition(endAddress);
                    Station nearestStationFromStartPosition = client.FindNearestStationFromStart(startAddressPosition.latitude, startAddressPosition.longitude);
                    Station nearestStationFromEndPosition = client.FindNearestStationFromEnd(endAddressPosition.latitude, endAddressPosition.longitude);

                    Position[] positions = new Position[] { startAddressPosition, nearestStationFromStartPosition.position, nearestStationFromEndPosition.position, endAddressPosition };

                    GeoJson completePath = client.GetPath(positions);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("\nParcours complet à suivre :\n");
                    WriteStepsInstruction(completePath.features);

                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine("\nAppuyez sur 'Entrée' pour revenir au menu principal ou sur tout autre touche pour relancer une requête...");
                Console.ResetColor();
            } while (Console.ReadKey().Key != ConsoleKey.Enter);
        }

        private static void WriteStepsInstruction(List<Feature> features)
        {
            Console.ResetColor();
            foreach (Feature feature in features)
            {
                foreach (Segment segment in feature.properties.segments)
                {
                    foreach (Step step in segment.steps)
                    {
                        Console.WriteLine(step.instruction);
                    }
                }
            }
        }

        private static void WriteLogs(IRouting client)
        {
            foreach (KeyValuePair<string, string> item in client.GetLogs())
            {
                string[] separator = new string[] { "@&#&#&@" };

                string[] value = item.Value.Split(separator, StringSplitOptions.None);

                Console.WriteLine("[{0}] {1} - {2}", item.Key.Split(separator, StringSplitOptions.None)[0], value[0], value[1]);
            }
        }

        private static void WriteLogsInExcel(IRouting client)
        {
            Application excel = new Application
            {
                DisplayAlerts = false,
                Visible = false
            };

            if (excel != null)
            {
                string outputDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                string filePath = new Uri(Path.Combine(outputDirectory, "stats.xls")).LocalPath;

                FileInfo file = new FileInfo(filePath);

                Workbooks workBooks = excel.Workbooks;
                Workbook workBook;
                Sheets sheets;
                Worksheet workSheet;

                if (!file.Exists)
                {
                    workBook = workBooks.Add();
                    sheets = workBook.Worksheets;
                    workSheet = (Worksheet) sheets.Item[1];
                }
                else
                {
                    workBook = workBooks.Open(filePath, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, true);
                    sheets = workBook.Worksheets;
                    workSheet = (Worksheet) sheets.Add();
                }

                workSheet.Name = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                workSheet.Cells[1, 1] = "Date et heure de la requête";
                workSheet.Cells[1, 2] = "Ville concernée";
                workSheet.Cells[1, 3] = "Numéro de station";

                int index = 2;
                foreach (KeyValuePair<string, string> item in client.GetLogs())
                {
                    string[] separator = new string[] { "@&#&#&@" };

                    string[] value = item.Value.Split(separator, StringSplitOptions.None);

                    workSheet.Cells[index, 1] = item.Key.Split(separator, StringSplitOptions.None)[0];
                    workSheet.Cells[index, 2] = value[0];
                    workSheet.Cells[index++, 3] = value[1];
                }

                workSheet.Columns.AutoFit();

                Console.WriteLine("\nFeuille de travail créée et disponible dans le fichier Excel {0}, sous le nom {1}.", filePath, workSheet.Name);

                workBook.SaveAs(filePath, XlFileFormat.xlExcel7, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, true);

                Marshal.FinalReleaseComObject(workSheet);
                Marshal.FinalReleaseComObject(sheets);
                workBook.Close(true, Type.Missing, false);
                Marshal.FinalReleaseComObject(workBook);
                Marshal.FinalReleaseComObject(workBooks);

                excel.Quit();
                Marshal.FinalReleaseComObject(excel);
                //KillExcel();
            }
            else
            {
                Console.WriteLine("Microsoft Excel n'est pas installée ...");
            }
        }

        private static void KillExcel()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");

            foreach (Process ExcelProcess in AllProcesses)
            {
                ExcelProcess.Kill();
            }

            AllProcesses = null;
        }
    }
}
