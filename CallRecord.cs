public readonly record struct CallRecord
{
    public string RecordId { get; }
    public string DestinationCountry { get; }
    public double DurationMinutes { get; }
    public bool IsRoaming { get; }

    public CallRecord(
        string recordId,
        string destinationCountry,
        double durationMinutes,
        bool isRoaming)
    {
        if (string.IsNullOrWhiteSpace(recordId))
            throw new ArgumentException("Record ID cannot be null, empty, or whitespace.");

        if (string.IsNullOrWhiteSpace(destinationCountry))
            throw new ArgumentException("Destination country cannot be null, empty, or whitespace.");

        if (double.IsNaN(durationMinutes) ||
            double.IsInfinity(durationMinutes) ||
            durationMinutes < 0 ||
            durationMinutes > 10_000)
        {
            throw new ArgumentException("Duration must be finite and between 0 and 10,000 minutes.");
        }

        RecordId = recordId;
        DestinationCountry = destinationCountry;
        DurationMinutes = durationMinutes;
        IsRoaming = isRoaming;
    }
}