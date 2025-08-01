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
            ApplicationConfiguration.Initialize();
            PAMC.LoginGui.LoginForm f = new PAMC.LoginGui.LoginForm();
            f.ShowDialog();
            if (f.Continue)
            {
                var dcDb = new DatabaseService();
                dcDb.EnsureDailyCounterForToday();
                var eftDb = new EFT_Collections.DatabaseService();
                eftDb.EnsureDailyCounterForToday();

                Application.Run(new MainForm());




            }

        }
    }
}