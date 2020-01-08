using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantConnect.Algorithm.CSharp
{
    public class PPI : QCAlgorithm
    {
        //Define user variables
        private int startingAccountSize = 2000000;
        private int maxPosition = 100000;
        private int minPosition = 10000;

        int candleSize = 28800;

        //resolution variable - this needs to be changed to the monthly but we need to make a custom quotebar
        Resolution res = Resolution.Daily;

        //Program variables
        public decimal usd;
        public decimal open;
        public decimal high;
        public decimal low;
        public decimal close;
        public decimal sl;
        public decimal tp;
        public int trackPrice;
        public int barCount;        

        //list of symbols
        List<string> FxSymbols = new List<string>()
        {
            "AUDCAD", "AUDCHF", "AUDHKD", "AUDJPY", "AUDNZD", "AUDSGD", "AUDUSD", "CADCHF", "CADHKD", "CADJPY", "CADSGD",
            "CHFHKD", "CHFJPY", "CHFZAR", "EURAUD", "EURCAD", "EURCHF", "EURCZK", "EURDKK", "EURGBP", "EURHKD", "EURJPY",
            "EURNOK", "EURNZD", "EURPLN", "EURSEK", "EURSGD", "EURTRY", "EURUSD", "EURZAR", "GBPAUD", "GBPCAD", "GBPCHF",
            "GBPHKD", "GBPJPY", "GBPNZD", "GBPPLN", "GBPSGD", "GBPUSD", "GBPZAR", "HKDJPY", "NZDCAD", "NZDCHF", "NZDHKD",
            "NZDJPY", "NZDSGD", "NZDUSD", "SGDCHF", "SGDHKD", "SGDJPY", "TRYJPY", "USDCAD", "USDCHF", "USDCNH", "USDCZK",
            "USDDKK", "USDHKD", "USDHUF", "USDJPY", "USDMXN", "USDNOK", "USDPLN", "USDSAR", "USDSEK", "USDSGD", "USDTHB",
            "USDTRY", "USDZAR", "ZARJPY"
        };

        List<historicalHigh> historicalHighs = new List<historicalHigh>();
        List<historicalHighCatalogued> historicalHighsCatalogued = new List<historicalHighCatalogued>();
        List<secondaryHigh> secondaryHighs = new List<secondaryHigh>();
        List<longTrade> longTrades = new List<longTrade>();

        List<historicalLow> historicalLows = new List<historicalLow>();
        List<historicalLowCatalogued> historicalLowsCatalogued = new List<historicalLowCatalogued>();
        List<secondaryLow> secondaryLows = new List<secondaryLow>();
        List<shortTrade> shortTrades = new List<shortTrade>();

        int numberOfSymbols => FxSymbols.Count; //this checks how many items there are and adds it to the dictionary

        int momentCounter = 1;


        //WHAT DOES THIS DO?
        public override void Initialize()
        {
            SetStartDate(1970, 1, 1);  //Set Start Date
            SetEndDate(DateTime.Now);    //Set End Date
            SetCash(startingAccountSize);             //Set Strategy Cash
            SetBrokerageModel(BrokerageName.OandaBrokerage, AccountType.Margin);

            //loop through our list of symbols and add them to the subscription manager
            //foreach (var symbol in FxSymbols) //loops through the symbols in the fxsymbols list and passing them into the add forex function and then adding them to the dictionary
            //{
            //    var Forex = AddForex(symbol, res);
            //    //Data.Add(symbol, new SymbolData(Forex.Symbol, Forex.BaseCurrencySymbol));
                
            //}

            ////loop through the dictionary 
            //foreach (var key in Data) //adds all entries into the dictionary
            //{
            //    //assigning each value associated with the keywords to a new variable name 'symbolData'
            //    var symbolData = key.Value;

            //    //using the helper method to assign a macd indicator to the macd attribute of our instance of SymbolData
            //    //symbolData.Macd = MACD(symbolData.Symbol, fastPeriod, slowPeriod, signalPeriod, MovingAverageType.Exponential, res);
            //}




            // Log values from history request of second-resolution data
            /*foreach (var data in secondHistory)
            {
                foreach (var key in data.Keys)
                {
                    Log(key.Value + ": " + data.Time + " > " + data[key].Value);
                }
            }*/
        }

        public override void OnData(Slice data)
        {

            foreach (var historicalHigh in historicalHighs.ToList())
            {
                if (historicalHighs.Last().ath > historicalHighs[historicalHighs.Count - 2].ath)
                {
                    Console.WriteLine("The current ath {0} is greater than the previous {1}", historicalHighs.Last().ath, historicalHighs[historicalHighs.Count - 2].ath);
                    Console.ReadLine();
                    //create horizontal rays at the ath and aLTH
                    //continue;
                }
                else
                {
                    Console.WriteLine("The current ath {0} is less than the previous {1} THIS IS NO GOOD DELETE IT FROM THE LIST", historicalHighs.Last().ath, historicalHighs[historicalHighs.Count - 2].ath);
                    Console.ReadLine();
                    historicalHighs.Remove(historicalHighs.Last());
                    Console.WriteLine("The current ath and aLTH are now {0} and {1}, respectively", historicalHighs.Last().ath, historicalHighs.Last().aLth);
                    continue;
                }
            }
            foreach (var historicalLow in historicalLows.ToList()) //something on the if statements needs to be fixed so that it doesn't keep looping to 0
            {
                if (historicalLows.Last().atl < historicalLows[historicalLows.Count - 2].atl)
                {
                    Console.WriteLine("The current atl {0} is less than the previous {1}", historicalLows.Last().atl, historicalLows[historicalLows.Count - 2].atl);
                    Console.ReadLine();
                    //create horizontal rays at the atl and aHTL
                    //continue;
                }
                else if (historicalLows.Last().atl > historicalLows[historicalLows.Count - 2].atl)
                {
                    Console.WriteLine("The current atl {0} is greater than the previous {1} THIS IS NO GOOD DELETE IT FROM THE LIST", historicalLows.Last().atl, historicalLows[historicalLows.Count - 2].atl);
                    Console.ReadLine();
                    historicalLows.Remove(historicalLows.Last());
                    Console.WriteLine("The current atl and aLTH are now {0} and {1}, respectively", historicalLows.Last().atl, historicalLows.Last().aHtl);
                    Console.ReadLine();
                    //return;
                }
                //return;
            }
        }
        //check to see if we 
        public void CheckTrend()
        {
            for (int i = 0; i < barCount; i++)
            {
                if (trackPrice > historicalHighs.Last().ath)
                {
                    Console.WriteLine("We are in an uptrend");
                    List<historicalHighCatalogued> historicalHighsCatalogued = new List<historicalHighCatalogued>(historicalHighs.Select(x => new historicalHighCatalogued { athC = x.ath, aLthC = x.aLth }));
                    //historicalHighs.Clear();
                    Console.WriteLine("There are {0} in the all time low catalogue", historicalHighsCatalogued.Count());
                    historicalHighsCatalogued.ForEach(atlC => Console.WriteLine("{0}\t", atlC));
                    Console.WriteLine("There are {0} in the historical lows list", historicalLows.Count());
                    historicalHighs.ForEach(Console.WriteLine);
                    //historicalHighs.OrderBy;

                    Console.ReadLine();

                    if (trackPrice < historicalLows.First().aHtl) //this is where we can be discriminant where we want to place our trades
                    {
                        //enter market order
                        List<secondaryLow> secondaryLows = new List<secondaryLow>();
                        List<longTrade> longTrades = new List<longTrade>();

                        longTrade longTrade1 = new longTrade()
                        {
                            sl = trackPrice - 0.050M,
                            tp = historicalHighs.First().aLth
                            //something for market order here
                        };

                        secondaryLow secondaryLow1 = new secondaryLow();
                        secondaryLow1.secL = trackPrice;
                        secondaryLow1.secHtl = trackPrice - .050M;



                        longTrades.Add(longTrade1);
                        secondaryLows.Add(secondaryLow1);

                        if (trackPrice > secondaryLows.First().secHtl)
                        {
                            shortTrade shortTrade2 = new shortTrade()
                            {
                                sl = trackPrice + 0.050M,
                                tp = historicalLows.First().aHtl
                                //something for market order here
                            };
                        }
                    }

                    if (trackPrice < historicalLows.Last().atl)
                    {
                        Console.WriteLine("We are in a downtrend");
                        List<historicalLowCatalogued> historicalLowsCatalogued = new List<historicalLowCatalogued>(historicalLows.Select(x => new historicalLowCatalogued { atlC = x.atl, aHtlC = x.aHtl }));
                        //historicalLows.Clear();
                        Console.WriteLine("There are {0} in the all time low catalogue", historicalLowsCatalogued.Count());
                        historicalLowsCatalogued.ForEach(athC => Console.WriteLine("{0}\t", athC.ToString()));
                        Console.WriteLine("There are {0} in the historical lows list", historicalLows.Count());
                        historicalLows.ForEach(Console.WriteLine);
                        historicalLows.OrderByDescending(x => x.atl); //this might be superflulous


                        Console.ReadLine();

                        if (trackPrice > historicalHighs.First().aLth) //this is where we can be discriminant where we want to place our trades
                        {
                            //enter market order
                            List<secondaryHigh> secondaryHighs = new List<secondaryHigh>();
                            List<shortTrade> shortTrades = new List<shortTrade>();

                            shortTrade shortTrade1 = new shortTrade()
                            {
                                sl = trackPrice + 0.050M,
                                tp = historicalLows.First().aHtl
                                //something for market order here
                            };

                            secondaryHigh secondaryHigh1 = new secondaryHigh()
                            {
                                secH = trackPrice,
                                secLth = trackPrice - .050M
                            };


                            shortTrades.Add(shortTrade1);
                            secondaryHighs.Add(secondaryHigh1);

                            if (trackPrice > secondaryHighs.First().secLth)
                            {
                                shortTrade shortTrade2 = new shortTrade()
                                {
                                    sl = trackPrice + 0.050M,
                                    tp = historicalLows.First().aHtl
                                    //something for market order here
                                };
                            }
                        }
                    }
                    barCount++;
                    return;
                }
            }
        }

        //this is our custom class
        public class SymbolData
        {
            public Symbol Symbol;
            public string BaseSymbol;
            public MovingAverageConvergenceDivergence Macd;

            public SymbolData(Symbol symbol, string baseSymbol)
            {
                Symbol = symbol;

                BaseSymbol = baseSymbol;
            }

        }



        public Maximum setAllTimeHigh(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorHigh = null)
        {
            var name = CreateIndicatorName(symbol, $"Historical High({period})", res);
            var historicalHigh = new Maximum(name, period);

            // assign a default value for the selector function
            if (selectorHigh == null)
            {
                var subscription = GetSubscription(symbol);
                if (typeof(QuoteBar).IsAssignableFrom(subscription.Type))
                {
                    // if we have trade bar data we'll use the High property, if not x => x.Value will be set in RegisterIndicator
                    selectorHigh = x => ((QuoteBar)x).High;
                }
            }

            //RegisterIndicator(symbol, historicalHigh, ResolveConsolidator(symbol, res), selectorHigh);

            //if (EnableAutomaticIndicatorWarmUp)
            //{
            //    WarmUpIndicator(symbol, historicalHigh, res);
            //}

            return historicalHigh;
        }

        public Maximum setAllTimeHighLow(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorLowOfHigh = null)
        {
            var name = CreateIndicatorName(symbol, $"Historical Low To High({period})", res);
            var historicalLTH = new Maximum(name, period);

            // assign a default value for the selector function
            if (selectorLowOfHigh == null)
            {
                var subscription = GetSubscription(symbol);
                if (typeof(QuoteBar).IsAssignableFrom(subscription.Type))
                {
                    // if we have trade bar data we'll use the High property, if not x => x.Value will be set in RegisterIndicator
                    selectorLowOfHigh = x => ((QuoteBar)x).Low;
                }
            }

            //RegisterIndicator(symbol, historicalLTH, ResolveConsolidator(symbol, res), selectorLowOfHigh);

            //if (EnableAutomaticIndicatorWarmUp)
            //{
            //    WarmUpIndicator(symbol, historicalLTH, res);
            //}

            return historicalLTH;
        }

        public class historicalHigh
        {            
            public decimal ath { get; set; }
            public decimal aLth { get; set; }
            public DateTime closeDate { get; set; }
            public int count { get; set; }            
        }

        public void RecordAllTimeHighEvent(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorHigh = null, Func<IBaseData, decimal> selectorLowOfHigh = null)
        {
            Maximum allTimeHigh = setAllTimeHigh(symbol, period, res, selectorHigh);
            Maximum allTimeHighLow = setAllTimeHighLow(symbol, period, res, selectorLowOfHigh);            
            historicalHigh newHistoricalHigh = new historicalHigh();

            newHistoricalHigh.ath = allTimeHigh;
            newHistoricalHigh.aLth = allTimeHighLow;
            var lastHigh = historicalHighs[historicalHighs.Count];
            newHistoricalHigh.count = 0;

            
            historicalHighs.Add(newHistoricalHigh);
        }



        public class historicalHighCatalogued : historicalHigh
        {
            public decimal athC { get; set; }
            public decimal aLthC { get; set; }
            public DateTime closeDateC { get; set; }
        }

        public class secondaryHigh
        {
            public decimal secH { get; set; }
            public decimal secLth { get; set; }
            public DateTime secCloseDate { get; set; }
        }

        public class longTrade
        {
            public decimal sl { get; set; }
            public decimal tp { get; set; }
        }

        public Minimum atl(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorLow = null)
        {
            var name = CreateIndicatorName(symbol, $"Historical Low({period})", res);
            var historicalLow = new Minimum(name, period);

            // assign a default value for the selector function
            if (selectorLow == null)
            {
                var subscription = GetSubscription(symbol);
                if (typeof(QuoteBar).IsAssignableFrom(subscription.Type))
                {
                    // if we have trade bar data we'll use the Low property, if not x => x.Value will be set in RegisterIndicator
                    selectorLow = x => ((QuoteBar)x).Low;
                }
            }

            //RegisterIndicator(symbol, historicalLow, ResolveConsolidator(symbol, res), selectorLow);

            //if (EnableAutomaticIndicatorWarmUp)
            //{
            //    WarmUpIndicator(symbol, historicalLow, res);
            //}

            return historicalLow;
        }

        public Minimum aHtl(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorHighOfLow = null)
        {
            var name = CreateIndicatorName(symbol, $"Historical High To Low({period})", res);
            var historicalHTL = new Minimum(name, period);

            // assign a default value for the selector function
            if (selectorHighOfLow == null)
            {
                var subscription = GetSubscription(symbol);
                if (typeof(QuoteBar).IsAssignableFrom(subscription.Type))
                {
                    // if we have trade bar data we'll use the Low property, if not x => x.Value will be set in RegisterIndicator
                    selectorHighOfLow = x => ((QuoteBar)x).High;
                }
            }

            //RegisterIndicator(symbol, historicalHTL, ResolveConsolidator(symbol, res), selectorHighOfLow);

            //if (EnableAutomaticIndicatorWarmUp)
            //{
            //    WarmUpIndicator(symbol, historicalHTL, res);
            //}

            return historicalHTL;
        }

        public class historicalLow
        {
            public decimal atl { get; set; }
            public decimal aHtl { get; set; }
            public DateTime closeDate { get; set; }
        }

        public class historicalLowCatalogued : historicalLow
        {
            public decimal atlC { get; set; }
            public decimal aHtlC { get; set; }
            public DateTime closeDateC { get; set; }
        }

        public class secondaryLow
        {
            public decimal secL { get; set; }
            public decimal secHtl { get; set; }
            public DateTime secCloseDate { get; set; }
        }

        public class shortTrade
        {
            public decimal sl { get; set; }
            public decimal tp { get; set; }
        }
    }
}
