using SaveFolders.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SaveFolders.Services
{
    public class RobocopyService
    {
        public event EventHandler<bool>? FinishRequested;
        public void RunCopy(SaveJob job)
        {
            Task.Run(() =>
            {
                var destination = job.ResolveDestinationPath();
                if (destination == null)
                {
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
                    },
                    EnableRaisingEvents = true
                };

                // Abonnement à la sortie standard
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        // Affiche dans la console / debug
                        System.Diagnostics.Debug.WriteLine(e.Data);
                        // Ou Console.WriteLine(e.Data); si console dispo
                    }
                };

                process.Start();
                process.BeginOutputReadLine();  // Commence la lecture asynchrone

                process.WaitForExit();

                bool success = process.ExitCode <= 3; // Considère 0,1,2,3 comme succès acceptable
                FinishRequested?.Invoke(this, success);
            });


        }
    }
}
