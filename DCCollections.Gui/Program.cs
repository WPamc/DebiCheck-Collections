namespace DCCollections.Gui
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var dcDb = new DatabaseService();
            dcDb.EnsureDailyCounterForToday();
            var eftDb = new EFT_Collections.DatabaseService();
            eftDb.EnsureDailyCounterForToday();
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}