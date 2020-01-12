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
    	private List<TradeBar> _spyBarHistoryHighs = new List<TradeBar>();

        public override void Initialize()
        {
            SetStartDate(2015, 1, 1);  //Set Start Date
            SetCash(2000000);
            SetBrokerageModel(BrokerageName.OandaBrokerage, AccountType.Margin);

            var spy = AddEquity("SPY", Resolution.Daily);


            var stockPlot = new Chart("Trade Plot");
            var assetPriceClose = new Series("ClosePrice", SeriesType.Line);
            var allTimeHighs = new Series("ATH", SeriesType.Scatter, "$", Color.Green, ScatterMarkerSymbol.Triangle);


            // stockPlot.AddSeries(dayHigh);
            stockPlot.AddSeries(assetPriceClose);
            stockPlot.AddSeries(allTimeHighs);
            AddChart(stockPlot);
        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        List<TradeBar> tempAth = new List<TradeBar>();
        public override void OnData(Slice data)
        {
        	TradeBar currentBar = data["SPY"];
        	TradeBar previousBar = null;
        	_spyBarHistory.Add(currentBar);

        	Plot("Trade Plot", "ClosePrice", currentBar.Close);

        	if(_spyBarHistory.IndexOf(currentBar) == 0) {
        		tempAth.Add(currentBar);
        	}

        	if(_spyBarHistory.Count > 1) {
        		previousBar = _spyBarHistory[_spyBarHistory.IndexOf(currentBar) - 1];
        	}

        	if(previousBar != null) {
	        	if(currentBar.High > previousBar.High) {
	        		tempAth.Add(currentBar);

	        		if(tempAth[tempAth.Count - 2].High > currentBar.High) {
	        			tempAth.RemoveAt(tempAth.Count - 1);
	        			Debug($"remove previous bar");
	        		} else {
	        			Plot("Trade Plot", "ATH", currentBar.High);
	        		}
	        	}
        	}

        	Debug($"Temp Ath: {tempAth.Count}");
        }
    }
}