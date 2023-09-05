﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParserDatabase.Model
{
    [Table("Monitorings")]
    public class Monitoring
    {
        [Key] public Guid Id { get; set; }

        /// <summary>
        /// Время парсинга
        /// </summary>
        public DateTime Date { get; set; }

        public double BlockReward { get; set; }
        public double LastBlock { get; set; }

        /// <summary>
        /// Сложность сети
        /// </summary>
        public double Difficulty { get; set; }

        public double Nethash { get; set; }

        /// <summary>
        /// Текущий курс монеты
        /// </summary>
        public double ExRate { get; set; }

        public double ExchangeRateVol { get; set; }
        /// <summary>
        /// Рыночная капитализация
        /// </summary>
        public double MarketCap { get; set; }

        public double PoolFee { get; set; }
        public double EstimatedRewards { get; set; }
        public double BtcRevenue { get; set; }

        public double Revenue { get; set; }
        public double Cost { get; set; }
        public double Profit { get; set; }

        /// <summary>
        /// Всего в обращении монет
        /// </summary>
        [NotMapped] public double TotalCoinsCount { get; set; }

        /// <summary>
        /// Объем торгов за 24 часа
        /// </summary>
        public double DailyEmission { get; set; }

        /// <summary>
        /// Связанная монета
        /// </summary>
        public Coin Coin { get; set; } = null!;
    }
}
