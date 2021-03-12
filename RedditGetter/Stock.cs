using System;
namespace RedditGetter
{
    public class Stock
    {
        public String name { get; set; }
        public String ticker { get; set; }
        public int mentions { get; set; }
        public DateTime collectedTime { get; set; }
        public Stock()
        { }
            
        
    }
}
