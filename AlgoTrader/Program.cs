using algo_trader.Models;
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

        List<HistoricalHigh> historicalHighs = new List<HistoricalHigh>();
        List<HistoricalHighCatalogued> HistoricalHighsCatalogued = new List<HistoricalHighCatalogued>();
        List<SecondaryHigh> secondaryHighs = new List<SecondaryHigh>();
        List<LongTrade> longTrades = new List<LongTrade>();

        List<HistoricalLow> historicalLows = new List<HistoricalLow>();
        List<HistoricalLowCatalogued> historicalLowsCatalogued = new List<HistoricalLowCatalogued>();
        List<SecondaryLow> secondaryLows = new List<SecondaryLow>();
        List<ShortTrade> shortTrades = new List<ShortTrade>();

        int numberOfSymbols => FxSymbols.Count; //this checks how many items there are and adds it to the dictionary

        int momentCounter = 1;


        public override void Initialize()
        {
            SetStartDate(1970, 1, 1);  //Set Start Date
            SetEndDate(DateTime.Now);    //Set End Date
            SetCash(startingAccountSize);             //Set Strategy Cash
            SetBrokerageModel(BrokerageName.OandaBrokerage, AccountType.Margin);


            var stockPlot = new Chart("Trade Plot");
            var priceHigh = new Series("Historical Highs", SeriesType.Flag, "$", Color.Green);
            var priceLow = new Series("Historical Low", SeriesType.Flag, "$", Color.Red);

            stockPlot.AddSeries(priceHigh);
            stockPlot.AddSeries(priceLow);


            foreach (var symbol in FxSymbols) //loops through the symbols in the fxsymbols list and passing them into the add forex function and then adding them to the dictionary
            {
                var Forex = AddForex(symbol, Resolution.Daily);
            }
        }

        public override void OnData(Slice data)
        {
            Plot("Trade Plot", "Price Low", data);
            Plot("Trade Plot", "Price High", historicalHighs);

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


        public void CheckTrend()
        {
            for (int i = 0; i < barCount; i++)
            {
                if (trackPrice > historicalHighs.Last().ath)
                {
                    Console.WriteLine("We are in an uptrend");
                    List<HistoricalHighCatalogued> historicalHighsCatalogued = new List<HistoricalHighCatalogued>(historicalHighs.Select(x => new HistoricalHighCatalogued { athC = x.ath, aLthC = x.aLth }));
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
                        List<SecondaryLow> secondaryLows = new List<SecondaryLow>();
                        List<LongTrade> longTrades = new List<LongTrade>();

                        LongTrade longTrade1 = new LongTrade()
                        {
                            sl = trackPrice - 0.050M,
                            tp = historicalHighs.First().aLth
                            //something for market order here
                        };

                        SecondaryLow secondaryLow1 = new SecondaryLow();
                        secondaryLow1.secL = trackPrice;
                        secondaryLow1.secHtl = trackPrice - .050M;



                        longTrades.Add(longTrade1);
                        secondaryLows.Add(secondaryLow1);

                        if (trackPrice > secondaryLows.First().secHtl)
                        {
                            ShortTrade shortTrade2 = new ShortTrade()
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
                        List<HistoricalLowCatalogued> historicalLowsCatalogued = new List<HistoricalLowCatalogued>(historicalLows.Select(x => new HistoricalLowCatalogued { atlC = x.atl, aHtlC = x.aHtl }));
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
                            List<SecondaryHigh> secondaryHighs = new List<SecondaryHigh>();
                            List<ShortTrade> shortTrades = new List<ShortTrade>(); 

                            ShortTrade shortTrade1 = new ShortTrade()
                            {
                                sl = trackPrice + 0.050M,
                                tp = historicalLows.First().aHtl
                                //something for market order here
                            };

                            SecondaryHigh secondaryHigh1 = new SecondaryHigh()
                            {
                                secH = trackPrice,
                                secLth = trackPrice - .050M
                            };


                            shortTrades.Add(shortTrade1);
                            secondaryHighs.Add(secondaryHigh1);

                            if (trackPrice > secondaryHighs.First().secLth)
                            {
                                ShortTrade shortTrade2 = new ShortTrade()
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




        public Maximum SetAllTimeHigh(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorHigh = null)
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







            //WHAT IS ALL OF THIS SUPPOSED TO DO? IT SHOWS UP IN LIKE 6 PLACES
            //UNCOMMENT FOR BUILD ERROR


            //RegisterIndicator(symbol, historicalHigh, ResolveConsolidator(symbol, res), selectorHigh);

            //if (EnableAutomaticIndicatorWarmUp)
            //{
            //    WarmUpIndicator(symbol, historicalHigh, res);
            //}

            return historicalHigh;
        }

        public Maximum SetAllTimeHighLow(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorLowOfHigh = null)
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




            //WHAT IS ALL OF THIS SUPPOSED TO DO? IT SHOWS UP IN LIKE 6 PLACES
            //UNCOMMENT FOR BUILD ERROR





            //RegisterIndicator(symbol, historicalLTH, ResolveConsolidator(symbol, res), selectorLowOfHigh);

            //if (EnableAutomaticIndicatorWarmUp)
            //{
            //    WarmUpIndicator(symbol, historicalLTH, res);
            //}

            return historicalLTH;
        }


        public void RecordAllTimeHighEvent(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorHigh = null, Func<IBaseData, decimal> selectorLowOfHigh = null)
        {
            Maximum allTimeHigh = SetAllTimeHigh(symbol, period, res, selectorHigh);
            Maximum allTimeHighLow = SetAllTimeHighLow(symbol, period, res, selectorLowOfHigh);
            HistoricalHigh newHistoricalHigh = new HistoricalHigh();

            newHistoricalHigh.ath = allTimeHigh;
            newHistoricalHigh.aLth = allTimeHighLow;
            newHistoricalHigh.count = historicalHighs.Count;


            historicalHighs.Add(newHistoricalHigh);
        }


        public Minimum SetAllTimeLow(Symbol symbol, int period, Resolution? res, Func<IBaseData, decimal> selectorLow = null)
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




            //WHAT IS ALL OF THIS SUPPOSED TO DO? IT SHOWS UP IN LIKE 6 PLACES
            //UNCOMMENT FOR BUILD ERROR






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
    }
}
