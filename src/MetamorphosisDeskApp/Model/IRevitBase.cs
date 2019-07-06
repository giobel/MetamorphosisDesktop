using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetamorphosisDeskApp.Model
{
    
    public interface IRevitBase
    {
        string DBFileName { get; set; }

        string CategoryName { get; set; }

        int CategoryCount { get; set; }

        string ColorSet { get; set; }

    }
}
