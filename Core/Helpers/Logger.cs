using Serilog;

namespace Coffeeroom.Core.Helpers
{
    public class Logger
    {
        public static void LogInfo(string title, string description)
        {
            Log.Information(title + " | " + description);
        }
        public static void LogError(string title, string description)
        {
            Log.Error(title + " | " + description);
        }
    }
}
