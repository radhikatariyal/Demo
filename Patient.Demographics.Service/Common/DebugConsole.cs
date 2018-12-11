using System;
using System.Diagnostics;

namespace Patient.Demographics.Service.Common
{
    public class DebugConsole
    {
        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("T")} > {message}");
        }
    }
}
