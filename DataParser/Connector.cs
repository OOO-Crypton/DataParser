using DataParserDatabase.DatabaseSettings;
using DataParserDatabase.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataParser
{
    public class Connector
    {
        private readonly string PathToSite = "https://whattomine.com/";
        private readonly HttpClient httpClient = new();
        private readonly Context db = new();

        public double HashRate = 0;
        public double ElectricityCost = 0;
        public double Power = 0;
        public Connector(double hashRate, double power, double cost = 0.1)
        {
            HashRate = hashRate;
            Power = power;
            ElectricityCost = cost;
        }

        public async Task UploadAllCoins()
        {
            List<Coin> coins = new();
            HttpResponseMessage responce = await httpClient.GetAsync(PathToSite + "calculators.json");
            responce.EnsureSuccessStatusCode();
            string body = await responce.Content.ReadAsStringAsync();
            body = body.Remove(0, 10);
            body = body.Remove(body.Length - 3, 2);
            Regex regex = new("\"(?<Name>\\w+(.\\w+)?)\":{\"id\":(?<CoinNumber>\\d+),\"tag\":\"(?<Tag>\\w+)\",\"algorithm\":\"(?<Algorithm>\\w+)\"");
            MatchCollection matches = regex.Matches(body);
            foreach (Match item in regex.Matches(body).Cast<Match>())
            {
                Algorithm? algo = await db.Algorithms.SingleOrDefaultAsync(x => x.Name == item.Groups["Algorithm"].Value);
                if (algo == null) 
                { 
                    Console.WriteLine($"Error: unknown Algorithm [${item.Groups["Algorithm"].Value}]");
                    algo = new()
                    {
                        Name = item.Groups["Algorithm"].Value,
                    };
                    db.Algorithms.Add(algo);
                    await db.SaveChangesAsync();
                }
                Coin? coin = await db.Coins.SingleOrDefaultAsync(x => x.CoinNumber == int.Parse(item.Groups["CoinNumber"].Value));
                if (coin == null)
                {
                    coin = new()
                    {
                        Name = item.Groups["Name"].Value,
                        CoinNumber = int.Parse(item.Groups["CoinNumber"].Value),
                        Tag = item.Groups["Tag"].Value,
                        Algorithm = algo,
                    };
                    db.Coins.Add(coin);
                    await db.SaveChangesAsync();
                    Console.WriteLine($"Added new Coin: [${item.Groups["Name"].Value}]");
                }
            }
        }

        /// <summary>
        /// true если все хорошо
        /// </summary>
        /// <param name="coinNumber"></param>
        /// <returns></returns>
        public async Task<bool> MonitoreCoin(int coinNumber)
        {
            string str = PathToSite + @"coins/" + coinNumber.ToString() + ".json?cost=" + ElectricityCost.ToString() + "&cost_currency=USD&hr=" + HashRate.ToString() + "&p=" + Power.ToString();
            HttpResponseMessage responce = await httpClient.GetAsync(str);
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"\n\n******************************************\n\nError at URL [${str}]");
                Console.WriteLine(await responce.Content.ReadAsStringAsync() + "\n\n");
                return false;
            }
            string body = await responce.Content.ReadAsStringAsync();

            string id = new Regex("\"id\":(?<CoinNumber>\\d+),").Match(body).Groups["CoinNumber"].Value.MakeString();
            string block_reward = new Regex("\"block_reward\":(?<BlockReward>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?),").Match(body).Groups["BlockReward"].Value.MakeString();
            string last_block = new Regex("\"last_block\":(?<LastBlock>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?),").Match(body).Groups["LastBlock"].Value.MakeString();
            string difficulty = new Regex("\"difficulty\":(?<Difficulty>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?),").Match(body).Groups["Difficulty"].Value.MakeString();
            string nethash = new Regex("\"nethash\":(?<Nethash>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?),").Match(body).Groups["Nethash"].Value.MakeString();
            string exchange_rate = new Regex("\"exchange_rate\":(?<ExRate>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?),").Match(body).Groups["ExRate"].Value.MakeString();
            string exchange_rate_vol = new Regex("\"exchange_rate_vol\":(?<ExchangeRateVol>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?),").Match(body).Groups["ExchangeRateVol"].Value.MakeString();
            string market_cap = new Regex("\"market_cap\":\"\\$(?<MarketCap>\\d+(,?\\d+)*)\",").Match(body).Groups["MarketCap"].Value.MakeString();
            string pool_fee = new Regex("\"pool_fee\":\"(?<PoolFee>\\d+(,?\\d+)*(\\.\\d+(,?\\d+)*(e(\\+|\\-)\\d+)?)?)\",").Match(body).Groups["PoolFee"].Value.MakeString();
            string estimated_rewards = new Regex("\"estimated_rewards\":\"(?<EstimatedRewards>(\\d+(,\\d)*\\.?\\d+)*)\",").Match(body).Groups["EstimatedRewards"].Value.MakeString();
            string btc_revenue = new Regex("\"btc_revenue\":\"(?<BtcRevenue>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?)\",").Match(body).Groups["BtcRevenue"].Value.MakeString();
            string revenue = new Regex("\"revenue\":\"\\$(?<Revenue>(\\d+\\.?\\d+)*)\",").Match(body).Groups["Revenue"].Value.MakeString();
            string cost = new Regex("\"cost\":\"\\$(?<Cost>\\d+(\\.\\d+(e(\\+|\\-)\\d+)?)?)\",").Match(body).Groups["Cost"].Value.MakeString();
            string profit = new Regex("\"profit\":\"(?<Profit>(-?\\$\\d+\\.?\\d+)*)\"").Match(body).Groups["Profit"].Value.MakeString();

            Coin? coin = await db.Coins.SingleOrDefaultAsync(x => x.CoinNumber == int.Parse(id));
            if (coin == null)
            {
                Console.WriteLine($"Error: unknown Coin [${id}]");
                return false;
            }
            else
            {
                Monitoring monitoring = new()
                {
                    Date = DateTime.Now,
                    BlockReward = double.Parse(block_reward),
                    LastBlock = double.Parse(last_block),
                    Difficulty = double.Parse(difficulty),
                    Nethash = double.Parse(nethash),
                    ExRate = double.Parse(exchange_rate),
                    ExchangeRateVol = double.Parse(exchange_rate_vol),
                    MarketCap = double.Parse(market_cap),
                    PoolFee = double.Parse(pool_fee),
                    EstimatedRewards = double.Parse(estimated_rewards),
                    BtcRevenue = double.Parse(btc_revenue),
                    Revenue = double.Parse(revenue),
                    Cost = double.Parse(cost),
                    Profit = double.Parse(profit),
                    //DailyEmission = double.Parse(match.Groups["DailyEmission"].Value),
                    Coin = coin
                };
                db.Attach(coin);
                db.Monitorings.Add(monitoring);
                await db.SaveChangesAsync();
                Console.WriteLine($"Success update for Coin [${coin.Name}]");
                return true;
            }
        }

    }
}
