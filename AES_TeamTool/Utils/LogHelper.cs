using System;
using System.IO;

namespace AES_TeamTool.Utils
{
    public class LogHelper
    {
        public static string LogPath { get; set; }
        private static StreamWriter writer;

        static LogHelper()
        {
#if DEBUG
            LogPath = ValueQuery.GetAppSetting("devLogPath");
#endif

#if (!DEBUG)
              LogPath = DynamicValueQuery.GetAppSetting("prodLogPath");
#endif
        }

        public static void WriteText(LogType type, string logContent)
        {
            string timeSpan = DateTime.Now.ToString("yyyy-MM-dd");
            string fileFullName = Path.Combine(LogPath, timeSpan);
            if (File.Exists(fileFullName))
            {
                writer = File.AppendText(fileFullName);
            }
            else
            {
                writer = File.CreateText(fileFullName);
            }
            writer.WriteLine($"[{DateTime.Now.ToString()}]|[{type.ToString() ?? "Default"}]: {logContent}");
            writer.Close();
        }
    }

    public enum LogType
    {
        DEBUG = 1,
        INFO = 2,
        WARN = 4,
        ERROR = 8
    }
}