using System.Runtime.CompilerServices;

namespace SmartApp.Services
{
    public class MyModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
