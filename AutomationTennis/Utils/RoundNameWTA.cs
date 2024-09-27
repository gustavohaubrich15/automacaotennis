namespace AutomationTennis.Utils
{
    public static class RoundNameWTA
    {
        private static readonly Dictionary<string, string> _roundNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "f", "Final" },
            { "s", "Semifinal" },
            { "q", "Quartas de final" },
            { "3", "Terceira rodada" },
            { "2", "Segunda rodada" },
            { "1", "Primeira rodada" }
        };

        public static string GetRoundName(string drawLevelType, string roundID)
        {
            if (drawLevelType.Equals("Q", StringComparison.OrdinalIgnoreCase))
            {
                return "Qualifying";
            }

            if (_roundNameMap.TryGetValue(roundID, out var roundName))
            {
                return roundName;
            }

            return "";
        }
    }
}
