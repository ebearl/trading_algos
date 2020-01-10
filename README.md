# trading_algos

## Requirements 0:
  * Create a custom bar that will display the monthly data.
  * Create a method that will iterate through every quote bar that is possible through a fx broker. (this will be subject to change based on the user).
  * Create high, low, close, open.
  * Create a variable for trackPrice so that the chart understands the current price for quotebar.
  * Create a list of symbols for the fx broker. (this will be subject to change based on the user and broker).
  * Add a way to get multiple timeframes other than the monthly custom bar data that we want.
    *We will use similar logic for the historical values to trade on smaller time frames.
  * Create a chart method to plot the indicator on the chart by using the built in add chart and add indicator method.


## Requirement 1:
  * Create class for symbol data - holds symbol, baseSymbol, MacD, symbol data object //I'm not sure this is needed
  * Create class for historical highs - holds ath (decimal), aLTH (decimal), occurrence(datetime), Id(int)
  * Create class for historical lows - holds atl (decimal), aHTL (decimal), occurrence (datetime), Id(int)
  * Create class for historical highs catalogued - holds ath (decimal), aLTH (decimal), occurrence(datetime), Id(int)
  * Create class for historical lows catalogued - holds atl (decimal), aHTL (decimal), occurrence (datetime), Id(int)
  * Create class for secondary highs - holds ath (decimal), aLTH (decimal), occurrence(datetime), Id(int)
  * Create class for secondary lows - holds atl (decimal), aHTL (decimal), occurrence (datetime), Id(int)
  * Create class for short trades - holds sl (decimal), tp (decimal), occurrence (datetime), market order (built in action) //we may not need this class.
  * Create class for long trades - holds sl (decimal), tp (decimal), occurrence (datetime), market order (built in action) //we may not need this class.
  * Create class for downtrend  - holds dt (string "new downtrend from {0} to {1}", historicalHighs.last(), historicalLows.first()), occurrence (datetime), Id (int).
  * Create class for uptrend  - holds ut (string "new uptrend from {0} to {1}", historicalLows.last(), historicalHighs.first()), occurrence (datetime), Id (int).


## Requirement 2:
  ##### Create a method to log the historical all time highs, all time low to highs. Will do the following:
    * creates a new historical high class.
    * sets the attributes in the new historical high class (ath, aLth, occurrence, Id).
    * sets the Id of the new historical high class to 0.
    * retrieves the previous all time high.
    * adds the new historical high class to the historical highs list.
    * increments the historical high list by 1.
    * sorts the list historical high list from lowest value recorded ath to highest value recorded, so historicalHigh.last() is the greatest value.
    * add horizontal rays to the ath and aLth values.

  ##### Create a method to log the historical all time lows, all time high to lows. Will do the following:
    * creates a new historical low class.
    * sets the attributes in the new historical low class (atl, aHtl, occurrence, Id).
    * sets the Id of the new historical low class to 0.
    * retrieves the previous all time low.
    * adds the new historical high class to the historical lows list.
    * increments the historical low list by 1.
    * sorts the list historical low list from highest value recorded atl to lowest value recorded, so historicallow.last() is the least value.
    * add horizontal rays to the atl and aHtl values.


## Requirement 3:
  ##### Create a method for the algo to recognize a trend.
    *if trackPrice < historicalLows.last().atl && downtrend.last().occurrence > uptrend.last().occurrence.
        *creates a new downtrend class.
        *sets the attributes of the new downtrend class (dt, occurrence, Id).
        *retrieves the previous downtrend class.
        *adds the new downtrend class to the downtrend list and the trend list.
        *increments both lists by 1.
        *adds a line plot from historicalHighs.last().ath to historicalLows.first().atl.
    *if trackprice > historicalHighs.last().ath && uptrend.last().occurrence > downtrend.last().occurrence.
        *creates a new uptrend class.
        *sets the attributes of the new uptrend class (ut, occurrence, Id).
        *retrieves the previous uptrend class
        *adds the new uptrend class to the uptrend list and the trend list.
        *incremends both lists by 1.
        *adds a line plot from historicalLows.last().atl to historicalHighs.first().ath.


  ##### Create a method to log the secondary highs, secondary low to highs. Will do the following:
    * triggers when trackPrice > list historicalHighs.first().aLth and price is in a downtrend.
    * creates a new secondary high class.
    * sets the attributes in the new secondary high class (secHigh, secLth, occurrence, Id).
      * secHigh = high of quotebar of trackPrice.
      * secLth = low of quotebar of trackPrice.
        *occurrence = the datetime when trackPrice > historicalHighs.first().aLth.*--Id = 0.
    * retreives the previous secondary high.
    * adds the new secondary high class to the secondary high list.
    * increments the secondary high list by 1.
    * removes the historicalHighs.first() values from the historicalHighs list.
    * adds the removed historicalHighs.first() values to the historicalHighsCatalogued list.
    * remove the horizontal lines for the current historical high.
    * add the horizontal lines for secHigh and secLth.


  ##### Create a method to log the secondary lows, secondary high to lows. Will do the following:
    * triggers when trackPrice < list historicalLows.first().aHtl and price is in an uptrend.
    * creates a new secondary low class
    * sets the attributes in the new secondary low class (secLow, secHtl, occurrence, Id).
      * secLow = low of the quotebar of trackPrice.
      * secHtl = high of the quotebar of trackPrice.
      * occurrence = the datetime when trackprice < historicalLows.first().aHtl.
      * Id = 0.
    * retrieves the previous secondary low.
    * adds the new secondary low class to the secondary low list.
    * increments the secondary low list by 1.
    * removes the historicalLows.first() values from the historicalLows list.
    * adds the removed historicalLows.first() values to the historicalLowsCatalogued list.
    * create a log statement to log the values when trackPrice < list historicalLows.first().aHtl. //in the future this will be an alert sent to the user's email
    * remove the horizontal lines for the current historical low.
    * add the horizontal lines for secLow and secHth.


  ##### Create a method to enter a short trade //this may be subject to change and may not need a method for it.
  ###### instead of entering all of this information we may just want an alert and then trade on smaller timeframes after price is greater than historicalHighs.first().aLth.
      * triggers when trackPrice > list historicalHighs.first().aLth || list historicalHighs.last().aLth and price is in a downtrend.
  ###### side note: there will be a point when historicalHighs.first().aLth = historicalHighs.last().aLth and will be the secondary high.
      * creates a new shortTrade class
      * sets the attributes in the new shortTrade class (sl, tp, occurrence, market order)
        * sl = list historicalHigh.First().ath + (some decimal value idk) || list historicalHigh.Last().ath + (some decimal value idk)
        * tp = list historicalLow.First().aHtl if not taken out by tp yet else set it to the next value in the list.
        * occurrence = the datetime when trackprice > historicalHighs.first().aLth.
        * market order short //uhh triggered this is some built in value.
      * adds the new shortTrade to the shortTrade list.
      * increments the shortTrade list by 1.
      * create a log statement to log the values when trackPrice < list historicalHighs.first().aLth. //in the future this will be an alert sent to the user's email


##### Create a method to enter a long trade //this may be subject to change and may not need a method for it.
  ###### instead of entering all of this information we may just want an alert and then trade on smaller timeframes after price is less than historicalLows.first().aHth.
      * triggers when trackPrice < list historicalLows.first().aHth || list historicalLows.last().aHth and price is in an uptrend.
  ###### side note: there will be a point when historicalHighs.first().aLth = historicalHighs.last().aLth and will be the secondary high.
      * creates a new shortTrade class
      * sets the attributes in the new shortTrade class (sl, tp, occurrence, market order)
          * sl = list historicalLows.First().atl + (some decimal value idk) || list historicalLows.Last().atl - (some decimal value idk)
          * tp = list historicalHighs.First().aLth if not taken out by tp yet else set it to the next value in the list.
          * occurrence = the datetime when trackprice < historicalLows.first().aHtl.
          * market order long //uhh triggered this is some built in value.
      * a1dds the new shortTrade to the shortTrade list.
      * increments the longTrade list by 1.
      * create a log statement to log the values when trackPrice < list historicalHighs.first().aLth. //in the future this will be an alert sent to the user's email`


## unknowns:
  * How to get the algo to recognize the actual low value that lead to a historical high and vice versa. Right now we are compensating by getting every respective LTH or HTL and pegging that as

  the actual low that lead to the high and vice versa.

  * Some of the secondary high/low trades are good but not completely optimized.
  * Need to learn how tf to get multi-tf and make lower tf trades.

helpful information:
  *(https://www.quantconnect.com/lean/documentation/topic15.html) - creating a custom indicator
  *the first recorded values for the historicalHighs and Lows lists will probably be 0 or whatever the first bar highs/lows are.
