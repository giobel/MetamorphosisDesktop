using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetamorphosisDeskApp.Model
{
    public class RevitSummary : RevitBase
    {
        public string DBFileName { get; set; }
        public int CategoryCount { get; set; }
        public string ColorSet { get; set; }
        
    }
}
