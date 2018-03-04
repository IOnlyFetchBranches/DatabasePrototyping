using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbutils
{
    /// <summary>
    /// Handles logging across an application
    /// </summary>
    public abstract  class Logger
    {

        private static FileStream logFile; //Stores data through this;
        private static StreamWriter writer;

        
        //Control, when flipped it will tell the Logger to write to file
        private static bool toFile = false; //off by default;

        public static string NewFile(string logName, bool singleFile)
        {
            string name;
            if (logName == null)
            {
                if (!singleFile)
                    name = DateTime.Now.ToString("yy-MM-dd hh-mm-ss");
                else
                    name = "main";
            }
            else
            {
                name = logName;
            }
            //Make Dir [If it doesnt exist]
            var dir = Directory.CreateDirectory(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\.dbproto\\logs\\");
            //formulate path
            string path = dir.FullName + name +
                          ".log";

            //This <Condition> ? then : else works EVERYWHERE and its nice
            logFile = File.Open(path, singleFile ? FileMode.OpenOrCreate : FileMode.Create);
            //Open writer.
            writer = new StreamWriter(logFile, Encoding.UTF8);

            writer.AutoFlush = true; //Enable auto flush, which writes to file after every write() call

            toFile = true; //Set control

            return path;
        }

        public static void LogG(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            var fMessage = "[" + time + "]" + ": " + message;
            Debug.WriteLine(fMessage);

            if (toFile)
            {
                writer.Write(fMessage);
                writer.Write(Environment.NewLine);
               
            }
        }
        
        public static void WriteL(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Debug.WriteLine(message);
        }
        public static void LogG(string from,string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            var fMessage = "[" + time + "]" + " " + from + ": " + message;
            Debug.WriteLine(fMessage);


            if (toFile)
            {
                writer.Write(fMessage);
                writer.Write(Environment.NewLine);
            }

        }
        public static void LogErr(string from, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            var fMessage = "[" + time + "]" + " <EXCEPTION> " + from + ": " + message;
            Debug.WriteLine(fMessage);

            if (toFile)
            {
                writer.Write(fMessage);
                writer.Write(Environment.NewLine);
            }

            Console.ForegroundColor = ConsoleColor.Green;
        }

        

    }
}
