using Test.API.Authentication.Enums;

namespace Test.API.Authentication.Models
{
    public class LocalizationInfo
    {
        public LanguageCodeIdentifierType Language { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public string Currency { get; set; }
    }
}
