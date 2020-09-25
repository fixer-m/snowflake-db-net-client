using Snowflake.Client.Json;

namespace Snowflake.Client
{
    public class ClientAppInfo
    {
        public string DriverName { get; private set; }
        public string DriverVersion { get; private set; }
        public LoginRequestClientEnv Environment { get; private set; }

        public ClientAppInfo()
        {
            Environment = new LoginRequestClientEnv()
            {
                Application = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                OSVersion = System.Environment.OSVersion.VersionString,
                NETRuntime = "NETCore",
                NETVersion = "2.0",
            };

            DriverVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DriverName = ".NET";
        }
    }
}