using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RedditGetter;
using RedditGetter.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RedditAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private String[] tickers = ShittyThings.tickers;
        private static readonly String[] badwords = {"SCOFF","SCON", "", "true","false","null","url","icon","null","iconurlwidth","height","amp","resizedicons","enddate", "count", "width", "height", "link"};
        private static readonly String[] urls =
        {
            "https://www.reddit.com/r/investing/.json","https://www.reddit.com/r/wallstreetbets/search.json?q=-flair%3AMeme%20-flair%3ASatire%20-flair%3AShitpost&restrict_sr=1&t=day&sort=hot","https://www.reddit.com/r/stocks/.json","https://www.reddit.com/r/SecurityAnalysis/.json?f=flair_name%3A%22Discussion%22"
        };
        private readonly ILogger<StockController> _logger;

        //public StockController(ILogger <StockController>logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet]
        public IEnumerable<Stock> Get()
        {
            Console.Write("im in the right method");
            String[] t = getStocks();
            List<Stock> stocks = new List<Stock>();
            Console.Write("at the end of the line the t string array is ");
            foreach (String s in t) {
                bool makeNew = true;
                foreach(Stock f in stocks)
                {
                    if(f.name.ToUpper().Equals(s.ToUpper()))
                    {
                        f.mentions++;
                        makeNew = false;
                        break;
                    }
                }
                if (makeNew)
                {
                    stocks.Add(new Stock
                    {
                        name = s.ToUpper(),
                        mentions = 1,
                        ticker = "nil",
                        collectedTime = DateTime.Now
                    }) ;
                    Console.WriteLine("i just putted stock " + s);
                }

                    
            }
            Console.WriteLine("done putting shits so stocks len is "+stocks.Count());
            stocks.Sort(
                delegate (Stock p1, Stock p2)
                {
                    return -(p1.mentions.CompareTo(p2.mentions));
                });
            return stocks.ToArray();
        }
        [NonAction]
        public bool isStock(String word)
        {
            //return (tickers.Contains(word));
            using (var wb = new WebClient())
            {
                Console.Write(word);
                var response = wb.DownloadString("https://www.marketwatch.com/tools/quotes/lookup.asp?siteId=mktw&Lookup=" + word + "&Country=us&type=All");
                if (response.ToString().Contains("no matches found"))
                {
                    return false;
                }
                else return true;
            }

        }
        public String []getdataforstock(String ticker)
        {
            //after we know the stock exists, we are gonna make a web call based on this ...ticker? and return the real ticker and real name in a format like this
            //retvar[0]="AAPL", retvar[1]="Apple Inc."
            String[] retvar = { };
            String url = "https://www.marketwatch.com/tools/quotes/lookup.asp?siteId=mktw&Lookup=" + ticker + "&Country=us&type=All";
            using (var wb = new WebClient())
            {
                var response = wb.DownloadString(url.ToString()).ToString();
            }
            return retvar;
        }
        [NonAction]
        public String getRidOfShit(String t)
        {
            String retvar = t;
            char[] shitWeDontWant = "!.$,\"\\{}[];:-=+_!@'#%^&*().,<>/?1234567890`~".ToCharArray();
            foreach (var shit in shitWeDontWant)
            {
                retvar = retvar.Replace(shit.ToString(), string.Empty);
            }
            return retvar;
        }
        [NonAction]
        public String[] getStocks()
        {
            using (var wb=new WebClient()) {
                List<String> bad = new List<string>();
                List<String> retVar = new List<String>();
                foreach (String url in urls)
                {
                    Console.WriteLine(url);
                    var response = wb.DownloadString(url);
                    Console.WriteLine(response.ToString().Substring(0,254));
                
                    String responseString = response.ToString();
                    String[] words = responseString.Split(" ");
                    foreach (String word in words)
                    {
                        Console.WriteLine("working with word " + word);
                        String poss = getRidOfShit(word);
                        bool iss;
                        if (bad.Contains(poss) || (badwords.Contains(poss)) || (poss.Length > 15) || (poss.Length < 2))
                        {
                            iss = false;
                            Console.WriteLine(poss+" didnt meet the criteria");
                        }
                        else
                        {
                            if (retVar.Contains(poss))
                            {
                                Console.WriteLine("i saved time cos it already existed. no need to check again");
                                iss = true;
                            }
                            else { iss = isStock(poss); }
                        }
                        if (iss)
                        {
                            Console.WriteLine(poss + " is a stock");
                            retVar.Add(poss);
                        }
                        else if (!iss)
                        {
                            bad.Add(poss);
                            Console.WriteLine(poss + " is not a stock");
                        }
                    }
                }
                return retVar.ToArray();
            }
        }
    }
}
