using System.Diagnostics;

namespace Team2QA.Automation.Tests.Utils;

public static class ApiWaitUtils
{
    public static async Task<decimal> WaitForBalanceUpdateAsync(
        Func<Task<decimal>> getBalanceFunc,
        decimal expectedBalance,
        int timeoutInSeconds = 10,
        int pollingIntervalMs = 500)
    {
        var stopwatch = Stopwatch.StartNew();
        
        while (stopwatch.Elapsed.TotalSeconds < timeoutInSeconds)
        {
            var currentBalance = await getBalanceFunc();
            
            if (currentBalance == expectedBalance)
            {
                return currentBalance; 
            }
            
            await Task.Delay(pollingIntervalMs);
        }

        throw new TimeoutException($"Balance did not reach {expectedBalance} within {timeoutInSeconds} seconds.");
    }
}