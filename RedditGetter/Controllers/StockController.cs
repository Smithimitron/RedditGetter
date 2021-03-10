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
        private static readonly String[] badwords = {"", "true","false","null","url","icon","null","iconurlwidth","height","amp","resizedicons","enddate", "count", "width", "height", "link"};
        private static readonly String[] urls =
        {
            "https://www.reddit.com/r/wallstreetbets/search.rss?q=-flair%3AMeme%20-flair%3ASatire%20-flair%3AShitpost&restrict_sr=1&t=day&sort=hot","https://www.reddit.com/r/stocks/.rss","https://www.reddit.com/r/SecurityAnalysis/.rss?f=flair_name%3A%22Discussion%22"
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
                    if(f.name.Equals(s.ToUpper()))
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
                        collectedTime = DateTime.Now
                    });
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
            return (tickers.Contains(word.ToUpper()));
            //using (var wb = new WebClient())
            //{
            //    Console.Write(word);
            //    var response = wb.DownloadString("https://www.marketwatch.com/tools/quotes/lookup.asp?siteId=mktw&Lookup=" + word + "&Country=us&type=All");
            //    if (response.ToString().Contains("no matches found"))
            //    {
            //        return false;
            //    }
            //    else return true;
            //}

        }
        [NonAction]
        public String getRidOfShit(String t)
        {
            String retvar = t;
            char[] shitWeDontWant = "!.,\"\\{}[];:-=+_!@#$%^&*().,<>/?1234567890`~".ToCharArray();
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
                https://localhost:5001/Stock
                    var response = wb.DownloadString(url);
                    Console.WriteLine(response.ToString().Substring(0,254));
                
                    String responseString = response.ToString();
                    responseString = getRidOfShit(responseString);
                    String[] words = responseString.Split(" ");
                    foreach (String word in words)
                    {
                        Console.WriteLine("working with word " + word);
                        String poss = word.ToUpper();
                        bool iss;
                        if (bad.Contains(poss) || (badwords.Contains(poss.ToLower())) || (poss.Length > 15))
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
                        else continue;
                    }
                }
                return retVar.ToArray();
            }
        }
    }
}
