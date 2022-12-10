using Snowflake.Client.Json;

namespace Snowflake.Client.Model
{
    public class ClientAppInfo
    {
        public string DriverName { get; }
        public string DriverVersion { get; }
        public LoginRequestClientEnv Environment { get; }

        public ClientAppInfo()
        {
            Environment = new LoginRequestClientEnv()
            {
                Application = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                OSVersion = $"({System.Environment.OSVersion.VersionString})",
#if NETFRAMEWORK
                NETRuntime = "NETFramework",
                NETVersion = "4.6",
#else
                NETRuntime = "NETCore",
                NETVersion = "2.0",
#endif
            };

            DriverVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
            DriverName = ".NET_Snowflake.Client";
        }
    }
}