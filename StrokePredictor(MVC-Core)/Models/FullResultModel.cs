using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrokePredictor_MVC_Core_.Models
{
    public class FullResultModel
    {
        public StrokeData StrokeInfo { get; set; }

        public ResultModel ResultInfo { get; set; }
    }
}
