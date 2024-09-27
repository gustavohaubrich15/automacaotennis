namespace AutomationTennis.Utils
{
    public static class CountryNameAdjuster
    {
        private static readonly Dictionary<string, string> _countryNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ukr", "ua" },
            { "den", "dk" },
            { "ger", "de" },
            { "slo", "si" },
            { "cro", "hr" },
            { "sui", "ch" },
            { "tpe", "tw" },
            { "ned", "nl" },
            { "chn", "cn" },
            { "kaz", "kz" },
            { "kor", "kr" },
            { "pol", "pl" },
            { "bul", "bg" }
        };

        public static string AdjustCountryName(string countryName)
        {
            if (string.IsNullOrEmpty(countryName))
                return countryName;

            countryName = countryName.ToLower();
            if (_countryNameMap.TryGetValue(countryName, out var adjustedName))
            {
                return adjustedName;
            }

            return countryName;
        }
    }
}
