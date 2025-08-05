using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace DCCollections.Gui
{
    public class UserSettings
    {
        public string? OperationFolderPath { get; set; }
        public string? ParseFolderPath { get; set; }
        public string? LiveOutputFolderPath { get; set; }
        public string? TestOutputFolderPath { get; set; }
        public string? ImportFolderPath { get; set; }

        public class LibraryPathEntry
        {
            public string Path { get; set; } = string.Empty;
            public bool IncludeSubfolders { get; set; } = true;
        }

        [JsonIgnore]
        public List<string>? LegacyLibraryPaths { get; set; }

        public List<LibraryPathEntry> LibraryPaths { get; set; } = new();
        public int ImportSortColumn { get; set; }
        public bool ImportSortDescending { get; set; }

        public int ArchiveOlderThanDays { get; set; } = 30;
        public bool ArchiveForceUnimported { get; set; } = false;
        public string ArchiveLastFileType { get; set; } = "All File Types";


        private static string SettingsFile => Path.Combine(AppContext.BaseDirectory, "usersettings.json");

        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<UserSettings>(json);
                    if (settings != null)
                    {
                        if ((settings.LibraryPaths == null || settings.LibraryPaths.Count == 0) && settings.LegacyLibraryPaths != null && settings.LegacyLibraryPaths.Count > 0)
                        {
                            settings.LibraryPaths = settings.LegacyLibraryPaths.Select(p => new LibraryPathEntry { Path = p, IncludeSubfolders = true }).ToList();
                            settings.LegacyLibraryPaths = null;
                        }
                        if (settings.LibraryPaths == null)
                            settings.LibraryPaths = new();
                        return settings;
                    }
                }
            }
            catch { }
            return new UserSettings();
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }
    }
}
