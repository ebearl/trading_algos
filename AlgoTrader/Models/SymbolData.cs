using System;
using QuantConnect;
using QuantConnect.Indicators;

namespace algo_trader.Models
{
    public class SymbolData
    {
        Symbol Symbol;
        string BaseSymbol;
        MovingAverageConvergenceDivergence Macd;

        public SymbolData(Symbol symbol, string baseSymbol)
        {
            Symbol = symbol;

            BaseSymbol = baseSymbol;
        }

    }
}
