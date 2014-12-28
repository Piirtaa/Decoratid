using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    public static class ProcessUtil
    {
        public static void Do(string cmd)
        {
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(@"cmd.exe")
                {

                    Arguments = "/K " + cmd,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    
                }
            };

            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    var errorMessage = process.StandardError.ReadToEnd();
                    throw new InvalidOperationException(errorMessage);
                }
                Console.WriteLine("The process has exited.");
                process.Dispose();
            };

            process.Start();


        }
    }

}
