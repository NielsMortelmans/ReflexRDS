using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ReflexRDS
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.SetWindowSize(250, 750);

            printTitle();

            string defaultMessage = ConfigurationSettings.AppSettings["defaultMessage"];
            string inputFile = ConfigurationSettings.AppSettings["inputFile"];
            string outputFile = ConfigurationSettings.AppSettings["outputFile"];
            int sleepSeconds = Int32.Parse(ConfigurationSettings.AppSettings["sleepSeconds"]);
            bool debug = bool.Parse(ConfigurationSettings.AppSettings["debug"]);
            bool first = true;
            int maxWait = 30;
            int waited = 0;
            string hashStored = "";
            string fileHash = "";

            if (performChecks(inputFile,outputFile))
            {
                while (true)
                {
                    fileHash = GetHashSha256(inputFile);
                    if (first)
                    {
                        hashStored = fileHash;
                        System.IO.File.WriteAllText(outputFile, defaultMessage);
                        first = false;
                        Console.WriteLine("First iteration, setting RDS to default message");
                    }

                    if(hashStored.Equals(fileHash))
                    {
                        if (maxWait.Equals(waited))
                        {
                            output("File has not been altered for the duration of the song. Setting default value and sleeping for " + sleepSeconds + " seconds", debug);
                            System.IO.File.WriteAllText(outputFile, defaultMessage);
                            sleep(sleepSeconds);
                        }
                        else
                        {
                            output("File has not been altered, max duration not reached. Going to sleep for " + sleepSeconds + " seconds",debug);
                            waited += sleepSeconds;
                            sleep(sleepSeconds);
                        }

                    }
                    else
                    {
                        output("File changed, setting new RDS and duration. Going to sleep for " + sleepSeconds + " seconds", debug);
                        string input = System.IO.File.ReadAllText(inputFile);
                        int minutes = Int32.Parse(input.Substring(1, 2));
                        int seconds = Int32.Parse(input.Substring(4, 2));
                        string rds = "";

                        hashStored = fileHash;
                        maxWait = (minutes * 60) + seconds;

                        if(input.Length > 64)
                        {
                            rds = input.Substring(8, 72);
                        }
                        else
                        {
                            rds = input.Substring(8, input.Length - 8);
                        }

                        rds.Replace(",", "-");

                        System.IO.File.WriteAllText(outputFile, rds);

                        Console.WriteLine("Setting RDS to : "+rds);

                        waited = sleepSeconds;
                        sleep(sleepSeconds);
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid configuration: one or more files could not be found!");
            }
        }

        private static bool performChecks(string inputFile, string outputFile)
        {
            if (System.IO.File.Exists(inputFile) && System.IO.File.Exists(outputFile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetHashSha256(string filename)
        {
            SHA256 Sha256 = SHA256.Create();

            using (FileStream stream = File.OpenRead(filename))
            {
                return Encoding.Default.GetString(Sha256.ComputeHash(stream));
            }
        }

        private static void printTitle()
        {
            /*Console.WriteLine("#     #   #     #        ");
            Console.WriteLine("##    # # ##   ##  ####  ");
            Console.WriteLine("# #   # # # # # # #    # ");
            Console.WriteLine("#  #  # # #  #  # #    # ");
            Console.WriteLine("#   # # # #     # #    # ");
            Console.WriteLine("#    ## # #     # #    # ");
            Console.WriteLine("#     # # #     #  ####  ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("###                                                          ");
            Console.WriteLine(" #  #    # #####  #    #  ####  ##### #####  # ######  ####  ");
            Console.WriteLine(" #  ##   # #    # #    # #        #   #    # # #      #      ");
            Console.WriteLine(" #  # #  # #    # #    #  ####    #   #    # # #####   ####  ");
            Console.WriteLine(" #  #  # # #    # #    #      #   #   #####  # #           # ");
            Console.WriteLine(" #  #   ## #    # #    # #    #   #   #   #  # #      #    # ");
            Console.WriteLine("### #    # #####   ####   ####    #   #    # # ######  ####  ");
            Console.WriteLine("");
            Console.WriteLine("");*/
            Console.WriteLine("######                                        ######  ######   #####  ");
            Console.WriteLine("#     # ###### ###### #      ###### #    #    #     # #     # #     # ");
            Console.WriteLine("#     # #      #      #      #       #  #     #     # #     # #       ");
            Console.WriteLine("######  #####  #####  #      #####    ##      ######  #     #  #####  ");
            Console.WriteLine("#   #   #      #      #      #        ##      #   #   #     #       # ");
            Console.WriteLine("#    #  #      #      #      #       #  #     #    #  #     # #     # ");
            Console.WriteLine("#     # ###### #      ###### ###### #    #    #     # ######   #####  ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" #####                                                         ");
            Console.WriteLine("#     # ###### #    # ###### #####    ##   #####  ####  #####  ");
            Console.WriteLine("#       #      ##   # #      #    #  #  #    #   #    # #    # ");
            Console.WriteLine("#  #### #####  # #  # #####  #    # #    #   #   #    # #    # ");
            Console.WriteLine("#     # #      #  # # #      #####  ######   #   #    # #####  ");
            Console.WriteLine("#     # #      #   ## #      #   #  #    #   #   #    # #   #  ");
            Console.WriteLine(" #####  ###### #    # ###### #    # #    #   #    ####  #    # ");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private static void sleep(int duration)
        {
            Thread.Sleep(duration * 1000);
        }

        private static void output(string message, bool debug)
        {
            if (debug)
            {
                Console.WriteLine(message);
            }
        }
    }
}
