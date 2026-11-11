using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipkartFeature.Entities
{
    internal class Product
    {
        public class Phone
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Rating { get; set; }
        }

        public class SearchCriteria
        {
            public string Keyword { get; set; }
            public decimal? MaxPrice { get; set; }
        }
    }
}
