using System;
using System.Collections.Generic;

namespace fodt2ANSI
{
    class MainClass
    {
        public static FodtParser parser = null;

        public static void Main(string[] args)
        {
            Fodt.BashColour.serialize();

            ///home/pixelzerg/git/clannad/Raw Art/s1/f1.fodt

            //System.Xml.Linq.XDocument doc = new System.Xml.Linq.XDocument();
            string path = "";
            #if DEBUG
            //path = "/home/pixelzerg/git/clannad/Raw Art/s1/f1.fodt";
            path = @"C:\Users\PixelZerg\Documents\GitHub\clannad\Raw Art\s1\f1.fodt";
#else
            if (args.Length > 0)
            {
                if (args[0] != "--help")
                {
                    path = args[0];
                }
                else
                {
                    Console.WriteLine("Pass the full the path of the .fodt file as an argument.");
                    Console.WriteLine("You will be prompted to provide a path if the program is started with no arguments");
                    Environment.Exit(Environment.ExitCode);
                }
            }
            else
            {
                Console.Write("Enter the full path of the .fodt file: ");
                path = Console.ReadLine();
            }
#endif

            System.IO.FileInfo f = new System.IO.FileInfo(path);
            if (!f.Exists)
            {
                Console.WriteLine("Could not locate the file!");
                Environment.Exit(Environment.ExitCode);
            }

            //doc.Load(f.FullName);
            parser = new FodtParser(System.Xml.Linq.XDocument.Load(f.FullName));
            DoStuff();
        }

        public static void DoStuff()
        {
            parser.ParseStyles();
            List<string> ANSI = parser.BuildANSI();
            string ret = "";
            foreach (string s in ANSI)
            {
                ret += "printf \"";
                ret += s;
                ret += "\"";
                //ret += "\\n";
                ret += Environment.NewLine;
            }
            new OutputBox(ret).ShowDialog();
        }
    }
}
