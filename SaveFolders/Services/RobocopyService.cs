using SaveFolders.Models;
using System.Diagnostics;
using System.IO;

namespace SaveFolders.Services
{
    public class RobocopyService
    {
        public event EventHandler<bool>? FinishRequested;
        public event Action<int>? ProgressChanged;

        public async Task RunCopy(SaveJob job)
        {
            try
            {
                var destination = job.ResolveDestinationPath();
                if (destination == null)
                {
                    FinishRequested?.Invoke(this, false);
                    return;
                }

                int totalFiles = 0;
                try
                {
                    totalFiles = Directory.GetFiles(job.SourcePath, "*", SearchOption.AllDirectories).Length;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors du calcul des fichiers source : {ex.Message}");
                    FinishRequested?.Invoke(this, false);
                    return;
                }

                int processedFiles = 0;

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{job.SourcePath}\" \"{destination}\" /E /Z /TEE /V /R:2 /W:2 /LOG:\"log.txt\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        var line = e.Data;
                        Debug.WriteLine(line);

                        if (line.Contains("identique", StringComparison.OrdinalIgnoreCase)
                            || line.Contains("Plus ancien", StringComparison.OrdinalIgnoreCase)
                            || line.Contains("modifié", StringComparison.OrdinalIgnoreCase)
                            || line.Contains("Nouveau fichier", StringComparison.OrdinalIgnoreCase)
                            || line.Contains("Plus récent", StringComparison.OrdinalIgnoreCase))
                        {
                            processedFiles++;

                            if (totalFiles > 0)
                            {
                                int percent = (int)((double)processedFiles / totalFiles * 100);
                                ProgressChanged?.Invoke(percent);
                            }
                        }
                    }
                };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    await process.WaitForExitAsync(); // <-- attend vraiment la fin
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de l’exécution de robocopy : {ex.Message}");
                    FinishRequested?.Invoke(this, false);
                    return;
                }

                bool success = process.ExitCode <= 3;
                FinishRequested?.Invoke(this, success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur inattendue dans RunCopy : {ex.Message}");
                FinishRequested?.Invoke(this, false);
            }
        }

    }
}
