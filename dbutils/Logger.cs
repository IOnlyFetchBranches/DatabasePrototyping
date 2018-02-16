using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbutils
{
    public static class Logger
    {
        public static void LogG(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            Debug.WriteLine("["+time+"]" + ": " +message);
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
            Debug.WriteLine("[" + time + "]" + " " + from+": " + message);

        }
        public static void LogErr(string from, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            Debug.WriteLine("[" + time + "]" + " <EXCEPTION> " + from + ": " + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        

    }
}
