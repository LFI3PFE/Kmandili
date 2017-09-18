using System;
using System.Collections.Generic;
using System.Text;
using Kmandili.Models.LocalModels;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Kmandili.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string Email
        {
            get
            {
                return AppSettings.GetValueOrDefault("Email", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Email", value);
            }
        }

        public static string Password
        {
            get
            {
                return AppSettings.GetValueOrDefault("Password", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Password", value);
            }
        }

        public static int Id
        {
            get
            {
                return AppSettings.GetValueOrDefault("Id", -1);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Id", value);
            }
        }

        public static string Token
        {
            get
            {
                return AppSettings.GetValueOrDefault("Token", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Token", value);
            }
        }

        public static string Type
        {
            get
            {
                return AppSettings.GetValueOrDefault("Type", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Type", value);
            }
        }

        public static string ExpireDate
        {
            get
            {
                return AppSettings.GetValueOrDefault("ExpireDate", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("ExpireDate", value);
            }
        }

        public static void ClearSettings()
        {
            Email = "";
            Password = "";
            Id = -1;
            Token = "";
            Type = "";
            ExpireDate = "";
        }

        public static void SetSettings(string email, string password, int id, string token, string type, string expireDate)
        {
            Email = email;
            Password = password;
            Id = id;
            Token = token;
            Type = type;
            ExpireDate = expireDate;
        }

    }
}
