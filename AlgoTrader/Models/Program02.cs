using System;
using System.Collections.Generic;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Data.Custom;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Securities.Equity;
using QuantConnect.Interfaces;
using System.Drawing;

namespace QuantConnect.Algorithm.CSharp
{
    public class ResistanceDynamicAtmosphericScrubbers : QCAlgorithm
    {

    	private List<TradeBar> _spyBarHistory = new List<TradeBar>();

        public override void Initialize()
        {
            SetStartDate(2015, 1, 1);  //Set Start Date
            SetCash(2000000);
            SetBrokerageModel(BrokerageName.OandaBrokerage, AccountType.Margin);

            var spy = AddEquity("SPY", Resolution.Daily);

            _spyBarHistory = History<TradeBar>("SPY", TimeSpan.FromDays(365)).ToList();


            var stockPlot = new Chart("Trade Plot");
            var assetPriceClose = new Series("ClosePrice", SeriesType.Candle);
            var dayHigh = new Series("Day High", SeriesType.Scatter, "$", Color.Green, ScatterMarkerSymbol.Triangle);
            var dayLow = new Series("Day Low", SeriesType.Scatter, "$", Color.Red, ScatterMarkerSymbol.TriangleDown);


            stockPlot.AddSeries(dayHigh);
            stockPlot.AddSeries(dayLow);
            AddChart(stockPlot);
        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        public override void OnData(Slice data)
        {
            Plot("Trade Plot", "ClosePrice", data["SPY"].Close);
            Plot("Trade Plot", "Day High", data["SPY"].High);
            Plot("Trade Plot", "Day Low", data["SPY"].Low);
        }

    }
}
