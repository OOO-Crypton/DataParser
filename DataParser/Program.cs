using DataParser;
using DataParserDatabase.DatabaseSettings;
using DataParserDatabase.Model;

internal class Program
{
    private static void Main(string[] args)
    {
        Start().Wait();
    }

    private static async Task Start()
    {
        Context db = new();
        Connector connector = new(70, 2800);
        await connector.UploadAllCoins();
        List<Coin> coins = db.Coins.ToList();
        List<int> errors = new();
        for (int i = 0; i < coins.Count; i++)
        {
            if(!await connector.MonitoreCoin(coins[i].CoinNumber))
            {
                errors.Add(coins[i].CoinNumber);
            }
            Thread.Sleep(1500);
        }
        if (errors.Count != 0)
        {
            Console.WriteLine("Errors in CoinNumber:\n");
            errors.ForEach(x => Console.WriteLine(x));
        }
    }
}