using System;
using System.Configuration;

namespace ScreenTime.DataAccess.Registry
{
    public class ApplicationSettings
    {
        public static string LogFileLocation => string.Format(GetSetting<string>("LogFileLocation"), Environment.UserName);
        public static string LogFileName => GetSetting<string>("LogFileName");

        private static T GetSetting<T>(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];

            return (T)Convert.ChangeType(setting, typeof(T));
        }
    }
}
