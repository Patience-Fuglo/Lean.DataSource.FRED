using System.Collections.Generic;

namespace QuantConnect.DataSource
{
    public static class FredSeries
    {
        public static readonly Dictionary<string, string> Map = new()
        {
            ["CPI"] = "CPIAUCSL",
            ["UNRATE"] = "UNRATE",
            ["GDP"] = "GDP",
        };

        public static string Resolve(string keyOrId)
        {
            return Map.TryGetValue(keyOrId.ToUpperInvariant(), out var id) ? id : keyOrId;
        }
    }
}
