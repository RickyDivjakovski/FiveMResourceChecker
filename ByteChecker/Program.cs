using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool error = false;
            if (args.Length != 1)
            {
                Console.WriteLine("Args length is less than or more than 1.");
                error = true;
            }
            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Directory " + args[0] + " does not exist, please input a directory.");
                error = true;
            }
            if (error) System.Environment.Exit(1);

            string output = "";
            foreach (string file in Directory.EnumerateFiles(args[0], "*.lua", SearchOption.AllDirectories))
            {
                bool fileerror = false;
                string filecontents = "";

                StreamReader reader = new StreamReader(file);
                filecontents = reader.ReadToEnd().Replace("\r\n", "\n");
                reader.Close();
                reader.Dispose();

                int counter = 0;

                foreach (string line in filecontents.Split('\n'))
                {
                    counter++;
                    if (line.Contains("\\x"))
                    {
                        if (fileerror == false)
                        {
                            output = output + "\n" + "File: " + file.Replace(args[0] + "\\", "").Replace("\\", "/") + ": \n";
                            fileerror = true;
                        }
                        string bytes = "";

                        foreach (string bytecode in line.Split('\"'))
                        {
                            if (bytecode.Contains("\\x"))
                            {
                                foreach (string s in bytecode.Split('\\'))
                                {
                                    string hexbyte = s.Replace("x", "");
                                    if (!string.IsNullOrWhiteSpace(s)) bytes = bytes + System.Text.Encoding.ASCII.GetString(new[] { Byte.Parse(hexbyte, NumberStyles.HexNumber)});
                                }
                                bytes = bytes + " ";
                            }
                        }

                        output = output + "    Line " + counter + ": " + bytes + "\n";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(output))
            {
                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bytecode found!!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("No bytecode found");
            }

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.ReadKey();
        }
    }
}
