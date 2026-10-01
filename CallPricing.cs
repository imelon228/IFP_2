public static class CallPricing
{
    public static decimal CalculateCost(in CallRecord record)
    {
        return (record.RecordId, record.DestinationCountry, record.DurationMinutes, record.IsRoaming) switch
        {
            (null, _, _, _) => throw new ArgumentException("Invalid record."),
            (_, null, _, _) => throw new ArgumentException("Invalid record."),
            (_, _, double.NaN, _) => throw new ArgumentException("Invalid duration."),
            (_, _, var duration, _) when double.IsInfinity(duration)
                || duration < 0
                || duration > 10_000
                => throw new ArgumentException("Invalid duration."),
            ("", _, _, _) => throw new ArgumentException("Invalid record ID."),
            (_, "", _, _) => throw new ArgumentException("Invalid country code."),
            (_, _, _, _) when string.IsNullOrWhiteSpace(record.RecordId)
                => throw new ArgumentException("Invalid record ID."),
            (_, _, _, _) when string.IsNullOrWhiteSpace(record.DestinationCountry)
                => throw new ArgumentException("Invalid country code."),
            (_, "KZ", var duration, true) when duration < 1.0
                => 50.00m,
            (_, "KZ", var duration, false)
                => decimal.Round((decimal)duration * 15.00m, 2, MidpointRounding.AwayFromZero),
            (_, _, var duration, true) when duration >= 10.0
                => decimal.Round((decimal)duration * 120.00m, 2, MidpointRounding.AwayFromZero),
            (_, _, var duration, _)
                => decimal.Round((decimal)duration * 45.00m, 2, MidpointRounding.AwayFromZero)
        };
    }
}