using System;
using System.Collections.Generic;
using System.Text;
using Kmandili.Models.LocalModels;
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
                return AppSettings.GetValueOrDefault<string>("Email", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>("Email", value);
            }
        }

        public static string Password
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>("Password", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>("Password", value);
            }
        }

        public static int Id
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>("Id", -1);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>("Id", value);
            }
        }

        public static string Token
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>("Token", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>("Token", value);
            }
        }

        public static void ClearSettings()
        {
            Email = "";
            Password = "";
            Id = -1;
            Token = "";
        }

    }
}
