using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Web.Script.Serialization;

using SAM.API.Types;

namespace SAM.API.Resources
{
    public static class ResourcesUI
    {
        public const string SettingsFileName = "settings.json";

        public static LangType AppLanguage
        {
            get => settings.AppLanguage;
            set
            {
                if (settings.AppLanguage != value)
                {
                    settings.AppLanguage = value;
                    SaveSettings();
                }
            }
        }

        public static LangType GameLanguage
        {
            get => settings.GameLanguage;
            set
            {
                if (settings.GameLanguage != value)
                {
                    settings.GameLanguage = value;
                    SaveSettings();                   
                }
            }
        }

        public static string REFRESH_GAMES => GetString("REFRESH_GAMES");
        public static string REFRESH_GAMES_TOOLTIP => GetString("REFRESH_GAMES_TOOLTIP");
        public static string SEARCH_GAME => GetString("SEARCH_GAME");
        public static string SEARCH_GAME_TOOLTIP => GetString("SEARCH_GAME_TOOLTIP");
        public static string GAME_FILTER => GetString("GAME_FILTER");
        public static string SHOW_GAMES => GetString("SHOW_GAMES");
        public static string SHOW_DEMOS => GetString("SHOW_DEMOS");
        public static string SHOW_MODS => GetString("SHOW_MODS");
        public static string SHOW_JUNK => GetString("SHOW_JUNK");
        public static string DISPLAY_MSG => GetString("DISPLAY_MSG");
        public static string REFRESH_ACHIEVS => GetString("REFRESH_ACHIEVS");
        public static string REFRESH_ACHIEVS_TOOLTIP => GetString("REFRESH_ACHIEVS_TOOLTIP");
        public static string RESET => GetString("RESET");
        public static string RESET_TOOLTIP => GetString("RESET_TOOLTIP");
        public static string COMMIT => GetString("COMMIT");
        public static string COMMIT_TOOLTIP => GetString("COMMIT_TOOLTIP");
        public static string DOWNLOAD => GetString("DOWNLOAD");
        public static string ACHIEVS_HEAD => GetString("ACHIEVS_HEAD");
        public static string ACHIEVS_NAME_HEAD => GetString("ACHIEVS_NAME_HEAD");
        public static string ACHIEVS_DESCR_HEAD => GetString("ACHIEVS_DESCR_HEAD");
        public static string LOCK_ACHIEVS => GetString("LOCK_ACHIEVS");
        public static string INVERT_ACHIEVS => GetString("INVERT_ACHIEVS");
        public static string INVERT_ACHIEVS_TOOLTIP => GetString("INVERT_ACHIEVS_TOOLTIP");
        public static string UNLOCK_ACHIEVS => GetString("UNLOCK_ACHIEVS");
        public static string UNLOCK_ACHIEVS_TOOLTIP => GetString("UNLOCK_ACHIEVS_TOOLTIP");
        public static string LOCK_ACHIEVS_TOOLTIP => GetString("LOCK_ACHIEVS_TOOLTIP");
        public static string ACHIEVS_FILTER => GetString("ACHIEVS_FILTER");
        public static string ACHIEVS_FILTER_TOOLTIP => GetString("ACHIEVS_FILTER_TOOLTIP");
        public static string ACHIEVS_FILTER_CLEAR => GetString("ACHIEVS_FILTER_CLEAR");
        public static string ACHIEVS_FILTER_CLEAR_TOOLTIP => GetString("ACHIEVS_FILTER_CLEAR_TOOLTIP");
        public static string GAME_STATUS_RETRIEVE_MSG => GetString("GAME_STATUS_RETRIEVE_MSG");
        public static string GAME_STATUS_RETRIEVE_ERROR => GetString("GAME_STATUS_RETRIEVE_ERROR");
        public static string GAME_STATUS_SCHEMA_ERROR => GetString("GAME_STATUS_SCHEMA_ERROR");
        public static string GAME_STATUS_STATS_ERROR => GetString("GAME_STATUS_STATS_ERROR");
        public static string GAME_STATUS_RETRIEVING_PROCESS => GetString("GAME_STATUS_RETRIEVING_PROCESS");
        public static string STATS_HEAD => GetString("STATS_HEAD");
        public static string NAME_HEAD => GetString("NAME_HEAD");
        public static string VALUE_HEAD => GetString("VALUE_HEAD");
        public static string EXTRA_HEAD => GetString("EXTRA_HEAD");
        public static string UNDERSTAND_MSG => GetString("UNDERSTAND_MSG");
        public static string DOWNLOAD_GAMES_ICONS => GetString("DOWNLOAD_GAMES_ICONS");
        public static string DOWNLOAD_ACHIEVS_ICONS => GetString("DOWNLOAD_ACHIEVS_ICONS");
        public static string DLG_EXCEPT_ERROR => GetString("DLG_EXCEPT_ERROR");
        public static string DLG_STEAM_NOT_RUN_ERROR => GetString("DLG_STEAM_NOT_RUN_ERROR");
        public static string DLG_STEAM_DIR_ERROR => GetString("DLG_STEAM_DIR_ERROR");
        public static string DLG_OWN_GAME_ERROR => GetString("DLG_OWN_GAME_ERROR");
        public static string DLG_INVALID_ID_ERROR => GetString("DLG_INVALID_ID_ERROR");
        public static string DLG_FAILED_START_SAM_GAME_ERROR => GetString("DLG_FAILED_START_SAM_GAME_ERROR");
        public static string DLG_LOAD_STATS_ERROR => GetString("DLG_LOAD_STATS_ERROR");
        public static string DLG_STORE_ACHIEV_STATE_ERROR => GetString("DLG_STORE_ACHIEV_STATE_ERROR");
        public static string DLG_STORE_STAT_STATE_ERROR => GetString("DLG_STORE_STAT_STATE_ERROR");
        public static string DLG_STORE_ERROR => GetString("DLG_STORE_ERROR");
        public static string DLG_FAMILY_SHARE_ERROR => GetString("DLG_FAMILY_SHARE_ERROR");
        public static string DLG_PARSE_ID_ERROR => GetString("DLG_PARSE_ID_ERROR");
        public static string DLG_STORE_MSG => GetString("DLG_STORE_MSG");
        public static string DLG_RESET_STATS_MSG => GetString("DLG_RESET_STATS_MSG");
        public static string DLG_RESET_ACHIEVES_MSG => GetString("DLG_RESET_ACHIEVES_MSG");
        public static string DLG_CONFIRM_RESET_MSG => GetString("DLG_CONFIRM_RESET_MSG");
        public static string DLG_FAILED_TO_RESET_ERROR => GetString("DLG_FAILED_TO_RESET_ERROR");
        public static string DLG_PROTECTED_ACHIEV_ERROR => GetString("DLG_PROTECTED_ACHIEV_ERROR");
        public static string SETTINGS => GetString("SETTINGS");
        public static string APP_LANG => GetString("APP_LANG");
        public static string GAME_LANG => GetString("GAME_LANG");
        public static string ENG_LANG => GetString("ENG_LANG");
        public static string RUS_LANG => GetString("RUS_LANG");
        public static string UKR_LANG => GetString("UKR_LANG");
        public static string SEARCH_LABEL => GetString("SEARCH_LABEL");

        static ResourcesUI()
        {
            if (settings == null)
            {
                settings = new Settings();
            }

            UpdateSettings();
            InitializeResourcesManagers();
        }

        public static void UpdateSettings()
        {
            if (File.Exists(SettingsFileName))
            {
                var json = File.ReadAllText(SettingsFileName);
                if (!string.IsNullOrEmpty(json))
                {
                    if (serializer == null)
                    {
                        serializer = new JavaScriptSerializer();
                    }

                    try
                    {
                        settings = serializer.Deserialize<Settings>(json);
                    }
                    catch
                    {
                        GetDefaultSettings();
                    }
                }
                else
                {
                    GetDefaultSettings();
                }
            }
            else
            {
                GetDefaultSettings();
            }
        }

        public static string GetLangName(this LangType lang)
        {
            switch (lang)
            {
                case LangType.RU:
                    return "russian";
                case LangType.UA:
                    return "ukrainian";
                default:
                    return "english";
            }
        }

        private static void InitializeResourcesManagers()
        {
            resourcesMgrs = new Dictionary<LangType, ResourceManager>
            {
                { LangType.EN, new ResourceManager("SAM.API.Resources.ResourcesEn", typeof(ResourcesUI).Assembly) },
                { LangType.RU, new ResourceManager("SAM.API.Resources.ResourcesRu", typeof(ResourcesUI).Assembly) },
                { LangType.UA, new ResourceManager("SAM.API.Resources.ResourcesUa", typeof(ResourcesUI).Assembly) }
            };
        }

        private static string GetString(string name)
        {
            try
            {
                return (resourcesMgrs[AppLanguage]).GetString(name);
            }
            catch
            {
                try
                {
                    return (resourcesMgrs[LangType.EN]).GetString(name);
                }
                catch
                {
                    switch (AppLanguage)
                    {
                        case LangType.RU:
                            return "Ошибка";
                        case LangType.UA:
                            return "Помилка";
                        default:
                            return "Error";
                    }
                }
            }
        }

        private static void GetDefaultSettings()
        {
            if (settings == null)
            {
                settings = new Settings();
            }

            var systemCulture = CultureInfo.CurrentCulture.ToString();
            switch (systemCulture)
            {
                case "ru":
                case "ru-BY":
                case "ru-KZ":
                case "ru-KG":
                case "ru-MD":
                case "ru-RU":
                case "ru-UA":
                    settings.AppLanguage = LangType.RU;
                    settings.GameLanguage = LangType.RU;
                    break;
                case "uk":
                case "uk-UA":
                    settings.AppLanguage = LangType.UA;
                    settings.GameLanguage = LangType.UA;
                    break;
                default:
                    settings.AppLanguage = LangType.EN;
                    settings.GameLanguage = LangType.EN;
                    break;
            }

            SaveSettings();
        }

        private static void SaveSettings()
        {
            if (serializer == null)
            {
                serializer = new JavaScriptSerializer();
            }

            var json = serializer.Serialize(settings);
            File.WriteAllText(SettingsFileName, json);
        }

        private static Settings settings;
        private static Dictionary<LangType, ResourceManager> resourcesMgrs;
        private static JavaScriptSerializer serializer;
    }
}
