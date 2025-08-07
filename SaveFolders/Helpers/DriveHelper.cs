using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SaveFolders.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Management;

    public static class DriveHelper
    {
        private static List<(string DriveLetter, string VolumeSerial)>? _cachedDrives;

        /// <summary>
        /// Recharge manuellement le cache.
        /// </summary>
        public static void RefreshCache()
        {
            _cachedDrives = LoadRemovableDrives();
        }

        /// <summary>
        /// Récupère les lecteurs avec cache (charge une seule fois).
        /// </summary>
        public static List<(string DriveLetter, string VolumeSerial)> GetRemovableDrives()
        {
            if (_cachedDrives == null)
                _cachedDrives = LoadRemovableDrives();

            return _cachedDrives;
        }

        /// <summary>
        /// Recherche une lettre de lecteur à partir d'un numéro de série.
        /// </summary>
        public static string? FindDriveLetterFromSerial(string serial)
        {
            var result =  GetRemovableDrives()
                .FirstOrDefault(x => x.VolumeSerial.Equals(serial, StringComparison.OrdinalIgnoreCase))
                .DriveLetter;
            if (string.IsNullOrEmpty(result))
                return "disque pas brancher";
            return result;
        }

        /// <summary>
        /// Charge la liste des lecteurs amovibles et leur numéro de série.
        /// </summary>
        private static List<(string DriveLetter, string VolumeSerial)> LoadRemovableDrives()
        {
            var result = new List<(string, string)>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;

                var driveLetter = drive.Name.Substring(0, 2); // ex: "E:"
                var serial = GetVolumeSerial(driveLetter);

                if (serial != null)
                    result.Add((driveLetter.TrimEnd(':'), serial)); // stocke juste "E"
            }

            return result;
        }

        /// <summary>
        /// Récupère le numéro de série d'un lecteur.
        /// </summary>
        public static string? GetVolumeSerial(string driveLetter) // Ex: "E:"
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter}'");

                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["VolumeSerialNumber"]?.ToString();
                }
            }
            catch
            {
                // Ignore errors silently or log if needed
            }

            return null;
        }
    }

}
