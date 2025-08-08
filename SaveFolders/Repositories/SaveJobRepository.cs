using SaveFolders.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SaveFolders.Repositories
{
    public class SaveJobRepository
    {
        private const string FileName = "save_jobs.json";

        public List<SaveJob> Load()
        {
            try
            {
                if (!File.Exists(FileName))
                    return new List<SaveJob>();

                var json = File.ReadAllText(FileName);
                return JsonSerializer.Deserialize<List<SaveJob>>(json) ?? new List<SaveJob>();
            }
            catch (Exception ex)
            {
                // Log l'erreur si besoin
                Debug.WriteLine($"Erreur lors du chargement des jobs : {ex.Message}");
                // Ou afficher un message, ou autre gestion selon contexte
                return new List<SaveJob>();
            }
        }

        public void Save(List<SaveJob> jobs)
        {
            try
            {
                var json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FileName, json);
            }
            catch (Exception ex)
            {
                // Log l'erreur si besoin
                Debug.WriteLine($"Erreur lors de la sauvegarde des jobs : {ex.Message}");
                // Ou gérer l'erreur autrement (ex : throw, message utilisateur...)
            }
        }
    }

}
