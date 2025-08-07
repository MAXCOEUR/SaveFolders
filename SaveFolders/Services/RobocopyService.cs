using SaveFolders.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveFolders.Services
{
    public class RobocopyService
    {
        public void RunCopy(SaveJob job)
        {
            var destination = job.ResolveDestinationPath();
            if (destination == null)
            {
                // Le disque n'est pas branché
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{job.SourcePath}\" \"{destination}\" /E /Z /R:2 /W:2",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
