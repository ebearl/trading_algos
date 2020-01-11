/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

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
    /// <summary>
    /// This algorithm demonstrates the various ways you can call the History function,
    /// what it returns, and what you can do with the returned values.
    /// </summary>
    public class HistoryAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private int _count;
        private SimpleMovingAverage _spyDailySma;
        private decimal trackPrice;

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2013, 10, 08);  //Set Start Date
            SetEndDate(DateTime.Now);    //Set End Date
            SetCash(100000);             //Set Strategy Cash
            SetBrokerageModel(BrokerageName.InteractiveBrokersBrokerage, AccountType.Cash);

            var stockPlot = new Chart("Trade Plot");
            var highPlots = new Series("High Price", SeriesType.Line, 0);
            stockPlot.AddSeries(highPlots);
            AddChart(stockPlot);

            // Find more symbols here: http://quantconnect.com/data
            var SPY = AddSecurity(SecurityType.Equity, "SPY", Resolution.Daily).Symbol;
            //var DOW = AddSecurity(SecurityType.Equity, "DOW", Resolution.Daily).Symbol;
            // specifying the exchange will allow the history methods that accept a number of bars to return to work properly
            //Securities["CHRIS/CME_SP1"].Exchange = new EquityExchange();

            // we can get history in initialize to set up indicators and such
            _spyDailySma = new SimpleMovingAverage(14);

            // get the last calendar year's worth of SPY data at the configured resolution (daily)
            var tradeBarHistory = History<TradeBar>("SPY", TimeSpan.FromDays(365));
            //var tradeBarHistory1 = History<TradeBar>("DOW", TimeSpan.FromDays(365));
            AssertHistoryCount("History<TradeBar>(\"SPY\", TimeSpan.FromDays(365))", tradeBarHistory, 250, SPY);
            //AssertHistoryCount("History<TradeBar>(\"DOW\", TimeSpan.FromDays(365))", tradeBarHistory1, 250, DOW);

            // we can loop over the return value from these functions and we get TradeBars
            // we can use these TradeBars to initialize indicators or perform other math
            foreach (TradeBar tradeBar in tradeBarHistory)
            {
                _spyDailySma.Update(tradeBar.EndTime, tradeBar.Close);
            }

            // foreach (TradeBar tradeBar in tradeBarHistory1)
            // {
            // 	_spyDailySma.Update(tradeBar.EndTime, tradeBar.Close);
            // }

            // sometimes it's necessary to get the history for many configured symbols

            // request the last year's worth of history for all configured symbols at their configured resolutions
            var allHistory = History(TimeSpan.FromDays(365));
            AssertHistoryCount("History(TimeSpan.FromDays(365))", allHistory, 250, SPY); //DOW);

            // request the last calendar years worth of history for the specified securities
            allHistory = History(Securities.Keys, TimeSpan.FromDays(365));
            AssertHistoryCount("History(Securities.Keys, TimeSpan.FromDays(365))", allHistory, 250, SPY);//, DOW);

            // if we loop over this allHistory, we get Slice objects
            foreach (Slice slice in allHistory)
            {
                // do something with each slice, these will come in time order
                // and will NOT have auxilliary data, just price data and your custom data
                // if those symbols were specified
            }

            // we can access all the closing prices in chronological order using this get function
            var closeHistorySPY = allHistory.Get("SPY", Field.Close);
            //var closeHistoryDOW = allHistory.Get("DOW", Field.Close);
            AssertHistoryCount("allHistory.Get(\"SPY\", Field.Close)", closeHistorySPY, 250);
            //AssertHistoryCount("allHistory.Get(\"DOW\", Field.Close)", closeHistoryDOW, 390);

            foreach (decimal close in closeHistorySPY)
            {
                // do something with each closing value in order
                //horizontal array plots
                var assetPrice = new Series("Price", SeriesType.Line, "$", Color.Green);
            }

            // foreach (decimal close in closeHistoryDOW)
            // {
            // 	// do something with each closing value in order
            // 	//horizontal array plots
            // }

            // we can convert the close history into your normal double array (double[]) using the ToDoubleArray method
            double[] doubleArray = closeHistorySPY.ToDoubleArray();
            //double[] doubleArray2 = closeHistoryDOW.ToDoubleArray();

            // for the purposes of regression testing, we're explicitly requesting history
            // using the universe symbols. Requests for universe symbols are filtered out
            // and never sent to the history provider.
            var universeSecurityHistory = History(UniverseManager.Keys, TimeSpan.FromDays(10)).ToList();
            if (universeSecurityHistory.Count != 0)
            {
                throw new Exception("History request for universe symbols incorrectly returned data. "
                    + "These requests are intended to be filtered out and never sent to the history provider.");
            }






        }

        /// <summary>
        /// OnEndOfDay Event Handler - At the end of each trading day we fire this code.
        /// To avoid flooding, we recommend running your plotting at the end of each day.
        /// </summary>
        public override void OnEndOfDay()
        {
            //Log the end of day prices:
            Plot("Trade Plot", "Price", trackPrice);
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="data">Slice object keyed by symbol containing the stock data</param>
        public override void OnData(Slice data)
        {
            _count++;
            trackPrice = data["SPY"].Close;

            // if (_count > 5)
            // {
            //     throw new Exception("Invalid number of bars arrived. Expected exactly 5");
            // }

            // if (!Portfolio.Invested)
            // {
            //     SetHoldings("SPY", 1);
            //     SetHoldings("DOW", 1);
            //     Debug("Purchased Stock");
            // }
        }

        private void AssertHistoryCount<T>(string methodCall, IEnumerable<T> history, int expected, params Symbol[] expectedSymbols)
        {
            history = history.ToList();
            var count = history.Count();
            if (count != expected)
            {
                throw new Exception(methodCall + " expected " + expected + ", but received " + count);
            }

            IEnumerable<Symbol> unexpectedSymbols = null;
            if (typeof(T) == typeof(Slice))
            {
                var slices = (IEnumerable<Slice>) history;
                unexpectedSymbols = slices.SelectMany(slice => slice.Keys)
                    .Distinct()
                    .Where(sym => !expectedSymbols.Contains(sym))
                    .ToList();
            }
            else if (typeof(IBaseData).IsAssignableFrom(typeof(T)))
            {
                var slices = (IEnumerable<IBaseData>)history;
                unexpectedSymbols = slices.Select(data => data.Symbol)
                    .Distinct()
                    .Where(sym => !expectedSymbols.Contains(sym))
                    .ToList();
            }
            else if (typeof(T) == typeof(decimal))
            {
                // if the enumerable doesn't contain symbols then we can't assert that certain symbols exist
                // this case is used when testing data dictionary extensions that select a property value,
                // such as dataDictionaries.Get("MySymbol", "MyProperty") => IEnumerable<decimal>
                return;
            }

            if (unexpectedSymbols == null)
            {
                throw new Exception("Unhandled case: " + typeof(T).GetBetterTypeName());
            }

            var unexpectedSymbolsString = string.Join(" | ", unexpectedSymbols);
            if (!string.IsNullOrWhiteSpace(unexpectedSymbolsString))
            {
                throw new Exception($"{methodCall} contains unexpected symbols: {unexpectedSymbolsString}");
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp, Language.Python };

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "1"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "359.952%"},
            {"Drawdown", "1.100%"},
            {"Expectancy", "0"},
            {"Net Profit", "1.686%"},
            {"Sharpe Ratio", "4.502"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0.019"},
            {"Beta", "0.989"},
            {"Annual Standard Deviation", "0.191"},
            {"Annual Variance", "0.036"},
            {"Information Ratio", "3.93"},
            {"Tracking Error", "0.002"},
            {"Treynor Ratio", "0.868"},
            {"Total Fees", "$3.26"}
        };
    }
}