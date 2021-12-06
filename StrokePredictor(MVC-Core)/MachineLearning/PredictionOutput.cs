using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StrokePredictor_MVC_Core_
{
    public class PredictionOutput
    {
        [ColumnName("PredictedLabel")]
        public float Prediction { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }
}