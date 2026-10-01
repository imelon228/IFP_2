using System.Threading;

public static class CallProcessor
{
    public static decimal ProcessCallsParallel(CallRecord[] records)
    {
        if (records is null)
            throw new ArgumentException("Records cannot be null.");

        if (records.Length % 2 != 0)
            throw new ArgumentException("Records array must have an even length.");

        if (records.Length == 0)
            return 0m;

        int mid = records.Length / 2;

        CallRecord[] firstHalf = [.. records[..mid]];
        CallRecord[] secondHalf = [.. records[mid..]];

        decimal[] firstResults = new decimal[firstHalf.Length];
        decimal[] secondResults = new decimal[secondHalf.Length];

        Exception? firstException = null;
        Exception? secondException = null;

        Thread firstThread = new Thread(() =>
        {
            try
            {
                for (int i = 0; i < firstHalf.Length; i++)
                {
                    firstResults[i] = CallPricing.CalculateCost(in firstHalf[i]);
                }
            }
            catch (Exception ex)
            {
                firstException = ex;
            }
        });

        Thread secondThread = new Thread(() =>
        {
            try
            {
                for (int i = 0; i < secondHalf.Length; i++)
                {
                    secondResults[i] = CallPricing.CalculateCost(in secondHalf[i]);
                }
            }
            catch (Exception ex)
            {
                secondException = ex;
            }
        });

        firstThread.Start();
        secondThread.Start();

        firstThread.Join();
        secondThread.Join();

        if (firstException is not null)
            throw firstException;

        if (secondException is not null)
            throw secondException;

        decimal total = 0m;

        foreach (decimal cost in firstResults)
            total += cost;

        foreach (decimal cost in secondResults)
            total += cost;

        return total;
    }

    public static decimal ProcessCallsSequential(CallRecord[] records)
    {
        if (records is null)
            throw new ArgumentException("Records cannot be null.");

        decimal total = 0m;

        foreach (var record in records)
        {
            total += CallPricing.CalculateCost(in record);
        }

        return total;
    }
}