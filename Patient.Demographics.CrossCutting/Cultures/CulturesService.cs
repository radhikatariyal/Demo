using System.Globalization;

namespace Patient.Demographics.CrossCutting.Cultures
{
    public static class CulturesService
    {
        public static string ConvertCultureCodeToFullName(string culture)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);

            return cultureInfo.EnglishName;
        }
    }
}
