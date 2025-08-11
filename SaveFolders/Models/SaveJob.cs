using SaveFolders.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveFolders.Models
{
    public class SaveJob
    {
        public string Id { get; set; }
        public string SourcePath { get; set; } = string.Empty;
        public string DiskSerial { get; set; } = string.Empty;
        public string DestinationFolderName { get; set; } = string.Empty;

        public SaveJob()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public string? ResolveDestinationPath()
        {
            string? drive = DriveHelper.FindDriveLetterFromSerial(DiskSerial);
            return drive != null ? $@"{drive}:\{DestinationFolderName}" : null;
        }

        public override bool Equals(object? obj)
        {
            return obj is SaveJob other && other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
