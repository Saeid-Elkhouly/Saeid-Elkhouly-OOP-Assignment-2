namespace PatternsLab
{
    // I chose eager initialization because AppConfig is a core configuration object that the application is expected to use, so there is no strong reason to delay its creation.
    public sealed class AppConfig
    {
        private  static readonly AppConfig _instance = new ();
        public static AppConfig Instance => _instance;

        private AppConfig()
        {
             LoadCount++;
             Console.WriteLine($"[AppConfig] Loading settings from disk... (load #{LoadCount})");
             Thread.Sleep(300);
             DbConnection = "Server=localhost;Db=School";

        }
        public static int LoadCount;



        public string DbConnection { get; private set; } = null!;
        public string Theme { get; private set; } = "Light";

        public  void ChangeTheme(string theme)
        {
            this.Theme = theme;
        }

  
    }

    public class DatabaseService
    {
        public AppConfig Config { get; } = AppConfig.Instance;

        public void Connect() => Console.WriteLine($"Connecting to {Config.DbConnection}");
    }

    public class UiService
    {
        public AppConfig Config { get; } = AppConfig.Instance;

        public void Render() => Console.WriteLine($"UI is using theme: {Config.Theme}");
    }
}