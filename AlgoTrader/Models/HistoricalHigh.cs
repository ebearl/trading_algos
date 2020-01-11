using System;
namespace algo_trader.Models
{
    public class HistoricalHigh
    {
        public decimal ath { get; set; }
        public decimal aLth { get; set; }
        public DateTime closeDate { get; set; }
        public int count { get; set; }
    }
}
