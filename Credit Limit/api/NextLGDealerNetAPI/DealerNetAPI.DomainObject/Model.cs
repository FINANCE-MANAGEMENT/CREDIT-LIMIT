using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Model:Base
    {
        public string Product { get; set; }
        public string SubProduct { get; set; }
        public string ProductLevel3 { get; set; }
        public string ModelCategory { get; set; }
        public string ModelSeries { get; set; }
        public string ModelSubCategory { get; set; }
        public string StarRating { get; set; }
        public string ModelYear { get; set; }
        public string ModelPrefix { get; set; }
        public string ModelNo { get; set; }
        public string ModelDesc { get; set; }
        public string PayoutValue { get; set; }
        public string SchemeType { get; set; }
    }
}
