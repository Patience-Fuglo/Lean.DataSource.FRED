using System;
using System.Collections.Generic;
using System.Globalization;
using NodaTime;
using QuantConnect.Data;
using QuantConnect.Logging;

namespace QuantConnect.DataSource
{
    // Single-file reader: data/alternative/fred/<series>.csv (YYYY-MM-DD,value)
    public class FRED : BaseData
    {
        public decimal Observation { get; set; }
        public string SeriesId { get; set; }

        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            var seriesId = FredSeries.Resolve(config.Symbol.Value);
            var path = $"data/alternative/fred/{seriesId}.csv";
            Log.Debug($"[GETSOURCE] {config.Symbol.Value} -> {path}");
            return new SubscriptionDataSource(path, SubscriptionTransportMedium.LocalFile, FileFormat.Csv);
        }

        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            var parts = line.Split(',');
            if (parts.Length < 2) return null;

            var time  = DateTime.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            var value = Convert.ToDecimal(parts[1].Trim(), CultureInfo.InvariantCulture);

            return new FRED
            {
                Symbol      = config.Symbol,
                Time        = time,
                EndTime     = time.AddDays(1),
                SeriesId    = FredSeries.Resolve(config.Symbol.Value),
                Observation = value,
                Value       = value
            };
        }

        public override DateTimeZone DataTimeZone()             => TimeZones.Utc;
        public override List<Resolution> SupportedResolutions() => DailyResolution;
        public override Resolution DefaultResolution()          => Resolution.Daily;
        public override bool IsSparseData()                     => true;
        public override bool RequiresMapping()                  => false;

        public override BaseData Clone() => new FRED
        {
            Symbol = Symbol, Time = Time, EndTime = EndTime,
            SeriesId = SeriesId, Observation = Observation, Value = Value
        };

        public override string ToString() => $"{SeriesId}:{Time:yyyy-MM-dd}={Observation}";
    }
}
