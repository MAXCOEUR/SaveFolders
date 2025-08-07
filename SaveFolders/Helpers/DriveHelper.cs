using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SaveFolders.Helpers
{
    public static class DriveHelper
    {
        public static List<(string DriveLetter, string VolumeSerial)> GetRemovableDrives()
        {
            var result = new List<(string, string)>();
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Removable))
            {
                var serial = GetVolumeSerial(drive.Name.Substring(0, 2));
                if (serial != null)
                    result.Add((drive.Name.Substring(0, 1), serial));
            }
            return result;
        }

        public static string? FindDriveLetterFromSerial(string serial)
        {
            return GetRemovableDrives().FirstOrDefault(x => x.VolumeSerial.Equals(serial, StringComparison.OrdinalIgnoreCase)).DriveLetter;
        }

        public static string? GetVolumeSerial(string driveLetter) // Ex: "E:"
        {
            using var searcher = new ManagementObjectSearcher(
                $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter}'");

            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["VolumeSerialNumber"]?.ToString();
            }

            return null;
        }
    }
}
