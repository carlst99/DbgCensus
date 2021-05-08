namespace DbgCensus.Rest
{
    /// <summary>
    /// Contains constant fields for the various Census language codes.
    /// </summary>
    public struct CensusLanguage
    {
        public static readonly CensusLanguage ENGLISH = new("en");
        public static readonly CensusLanguage FRENCH = new("fr");
        public static readonly CensusLanguage GERMAN = new("de");
        public static readonly CensusLanguage ITALIAN = new("it");
        public static readonly CensusLanguage SPANISH = new("es");
        public static readonly CensusLanguage TURKISH = new("tr");

        public string LanguageCode { get; }

        public CensusLanguage(string languageCode)
        {
            LanguageCode = languageCode;
        }

        public static implicit operator string(CensusLanguage l) => l.ToString();
        public static implicit operator CensusLanguage(string s) => new(s);

        public override string ToString() => LanguageCode;
    }
}
