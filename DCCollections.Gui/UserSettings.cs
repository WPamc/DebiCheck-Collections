using System.Text.Json;

namespace DCCollections.Gui
{
    public class UserSettings
    {
        public string? OperationFolderPath { get; set; }
        public string? ParseFolderPath { get; set; }
        public string? LiveOutputFolderPath { get; set; }
        public string? TestOutputFolderPath { get; set; }
        public string? ImportFolderPath { get; set; }
        public int ImportSortColumn { get; set; }
        public bool ImportSortDescending { get; set; }

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
                        return settings;
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
