using System;
using QuantConnect;
using QuantConnect.Indicators;

namespace algo_trader.Models
{
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
}
