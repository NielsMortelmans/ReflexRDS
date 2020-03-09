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
            //Console.SetWindowSize(250, 750);

            printTitle();

            string defaultMessage = ConfigurationManager.AppSettings["defaultMessage"];
            string inputFile = ConfigurationManager.AppSettings["inputFile"];
            string outputFile = ConfigurationManager.AppSettings["outputFile"];
            int sleepSeconds = Int32.Parse(ConfigurationManager.AppSettings["sleepSeconds"]);
            bool debug = bool.Parse(ConfigurationManager.AppSettings["debug"]);
            bool first = true;
            int maxWait = 30;
            int waited = 0;
            string hashStored = "";
            string fileHash = "";

            performChecks(inputFile, outputFile);
            
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
                        string input = loadInput(inputFile);

                    if (!input.Equals(""))
                    {
                        int minutes = Int32.Parse(input.Substring(1, 2));
                        int seconds = Int32.Parse(input.Substring(4, 2));
                        string rds = "";

                        hashStored = fileHash;
                        maxWait = (minutes * 60) + seconds;

                        if (input.Length > 64)
                        {
                            rds = input.Substring(8, 72);
                        }
                        else
                        {
                            rds = input.Substring(8, input.Length - 8);
                        }

                        rds = rds.Replace(',', '-');

                        System.IO.File.WriteAllText(outputFile, rds);

                        Console.WriteLine("Setting RDS to : " + rds);

                        waited = sleepSeconds;
                        sleep(sleepSeconds);
                    }
                    else
                    {
                        Console.WriteLine("Could not read inputfile. Setting default value and sleeping for " + sleepSeconds + " seconds", debug);
                        System.IO.File.WriteAllText(outputFile, defaultMessage);
                        sleep(sleepSeconds);
                    }
                    }
                }
        }

        private static void performChecks(string inputFile, string outputFile)
        {
            if (!System.IO.File.Exists(inputFile))
            {
                System.IO.File.Create(inputFile);
            }
                if(!System.IO.File.Exists(outputFile))
            {
                System.IO.File.Create(outputFile);
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

        private static string loadInput(string inputFile)
        {
            string retValue = "";
                try
                {
                    using (FileStream fileStream = new FileStream(
                        inputFile,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            retValue = streamReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    retValue = "";
                }
            return retValue;
        }
    }
}
