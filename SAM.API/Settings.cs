using SAM.API.Types;

namespace SAM.API
{
    public class Settings
    {
        public LangType AppLanguage { get; set; }

        public LangType GameLanguage { get; set; }

        public Settings()
        {
            AppLanguage = LangType.EN;
            GameLanguage = LangType.EN;
        }
    }
}
