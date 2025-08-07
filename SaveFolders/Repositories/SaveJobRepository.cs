using SaveFolders.Models;
using System;
using System.Collections.Generic;
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
            if (!File.Exists(FileName)) return new List<SaveJob>();

            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<SaveJob>>(json) ?? new List<SaveJob>();
        }

        public void Save(List<SaveJob> jobs)
        {
            var json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}
