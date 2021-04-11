using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace SAM.API.Resources
{
    public class ResourcesUI
    {
        public static string CurrentLanguage
        {
            get
            {
                switch (currentLanguage)
                {
                    case Lang.RU:
                        return "russian";
                    case Lang.UA:
                        return "ukrainian";
                    default:
                        return "english";
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

        static ResourcesUI()
        {
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
                    currentLanguage = Lang.RU;
                    break;
                case "uk":
                case "uk-UA":
                    currentLanguage = Lang.UA;
                    break;
                default:
                    currentLanguage = Lang.EN;
                    break;
            }

            InitializeResourcesManagers();
        }

        private static void InitializeResourcesManagers()
        {
            resourcesMgrs = new Dictionary<Lang, ResourceManager>
            {
                { Lang.EN, new ResourceManager("SAM.API.Resources.ResourcesEn", typeof(ResourcesUI).Assembly) },
                { Lang.RU, new ResourceManager("SAM.API.Resources.ResourcesRu", typeof(ResourcesUI).Assembly) },
                { Lang.UA, new ResourceManager("SAM.API.Resources.ResourcesUa", typeof(ResourcesUI).Assembly) }
            };
        }

        private static string GetString(string name)
        {
            try
            {
                return (resourcesMgrs[currentLanguage]).GetString(name);
            }
            catch
            {
                try
                {
                    return (resourcesMgrs[Lang.EN]).GetString(name);
                }
                catch
                {
                    switch (currentLanguage)
                    {
                        case Lang.RU:
                            return "Ошибка";
                        case Lang.UA:
                            return "Помилка";
                        default:
                            return "Error";
                    }
                }
            }
        }

        private static Lang currentLanguage;
        private static Dictionary<Lang, ResourceManager> resourcesMgrs;

        private enum Lang
        {
            EN,
            RU,
            UA
        }
    }
}
