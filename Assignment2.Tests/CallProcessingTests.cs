using Xunit;

public class CallProcessingTests
{
    [Theory]
    [InlineData("1", "KZ", 4.0, false, 60.00)]
    [InlineData("2", "KZ", 0.5, true, 50.00)]
    [InlineData("3", "US", 10.0, true, 1200.00)]
    [InlineData("4", "DE", 3.0, false, 135.00)]
    [InlineData("5", "XX", 2.0, false, 90.00)]
    [InlineData("6", "KZ", 1.0, true, 45.00)]
    public void CalculateCost_ReturnsExpectedTariff(
        string recordId,
        string country,
        double duration,
        bool isRoaming,
        double expected)
    {
        var record = new CallRecord(recordId, country, duration, isRoaming);

        decimal actual = CallPricing.CalculateCost(in record);

        Assert.Equal((decimal)expected, actual);
    }

    [Fact]
    public void CallRecord_RejectsBlankRecordId()
    {
        Assert.Throws<ArgumentException>(() =>
            new CallRecord("   ", "KZ", 1.0, false));
    }

    [Fact]
    public void CallRecord_RejectsBlankCountry()
    {
        Assert.Throws<ArgumentException>(() =>
            new CallRecord("1", "   ", 1.0, false));
    }

    [Fact]
    public void CallRecord_RejectsNegativeDuration()
    {
        Assert.Throws<ArgumentException>(() =>
            new CallRecord("1", "KZ", -1.0, false));
    }

    [Fact]
    public void CallRecord_RejectsNaNDuration()
    {
        Assert.Throws<ArgumentException>(() =>
            new CallRecord("1", "KZ", double.NaN, false));
    }

    [Fact]
    public void CallRecord_RejectsInfiniteDuration()
    {
        Assert.Throws<ArgumentException>(() =>
            new CallRecord("1", "KZ", double.PositiveInfinity, false));
    }

    [Fact]
    public void CallRecord_RejectsDurationAboveMaximum()
    {
        Assert.Throws<ArgumentException>(() =>
            new CallRecord("1", "KZ", 10_000.1, false));
    }

    [Fact]
    public void CalculateCost_RejectsDefaultCallRecord()
    {
        CallRecord record = default;

        Assert.Throws<ArgumentException>(() =>
            CallPricing.CalculateCost(in record));
    }

    [Theory]
    [InlineData("1", "KZ", 0.0, true, 50.00)]
    [InlineData("2", "KZ", 0.0, false, 0.00)]
    [InlineData("3", "KZ", 0.999999, true, 50.00)]
    [InlineData("4", "KZ", 1.0, true, 45.00)]
    [InlineData("5", "US", 9.999999, true, 450.00)]
    [InlineData("6", "US", 10.0, true, 1200.00)]
    public void CalculateCost_HandlesBoundaries(
        string recordId,
        string country,
        double duration,
        bool isRoaming,
        double expected)
    {
        var record = new CallRecord(recordId, country, duration, isRoaming);

        decimal actual = CallPricing.CalculateCost(in record);

        Assert.Equal((decimal)expected, actual);
    }

    [Fact]
    public void ProcessCallsParallel_MatchesSequentialAndDoesNotModifyInput()
    {
        var records = new CallRecord[1000];

        for (int i = 0; i < records.Length; i++)
        {
            records[i] = (i % 4) switch
            {
                0 => new CallRecord($"KZ-NR-{i}", "KZ", 4.0, false),
                1 => new CallRecord($"KZ-R-{i}", "KZ", 0.5, true),
                2 => new CallRecord($"US-R-{i}", "US", 10.0, true),
                _ => new CallRecord($"DE-NR-{i}", "DE", 3.0, false)
            };
        }

        CallRecord[] original = (CallRecord[])records.Clone();

        decimal sequentialTotal = CallProcessor.ProcessCallsSequential(records);

        for (int i = 0; i < 100; i++)
        {
            decimal parallelTotal = CallProcessor.ProcessCallsParallel(records);

            Assert.Equal(sequentialTotal, parallelTotal);
        }

        Assert.Equal(original, records);
    }

    [Fact]
    public void ProcessCallsParallel_RejectsOddLengthArray()
    {
        var records = new[]
        {
            new CallRecord("1", "KZ", 1.0, false),
            new CallRecord("2", "US", 2.0, false),
            new CallRecord("3", "DE", 3.0, false)
        };

        Assert.Throws<ArgumentException>(() =>
            CallProcessor.ProcessCallsParallel(records));
    }

    [Fact]
    public void ProcessCallsParallel_ReturnsZeroForEmptyArray()
    {
        var records = Array.Empty<CallRecord>();

        decimal result = CallProcessor.ProcessCallsParallel(records);

        Assert.Equal(0m, result);
    }
}