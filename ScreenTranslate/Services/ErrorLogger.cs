using System;
using System.IO;
namespace ScreenTranslate.Services
{
    public static class ErrorLogger
    {
        public static void Log(string operation, Exception exception)
        {
            try
            {
                var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScreenTranslate");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "errors.log"), DateTimeOffset.Now.ToString("u") + " | " + operation + " | " + exception.GetType().Name + Environment.NewLine);
            }
            catch { }
        }
    }
}
