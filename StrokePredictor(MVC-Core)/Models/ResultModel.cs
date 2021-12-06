using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StrokePredictor_MVC_Core_.Models
{
    public class ResultModel
    {
        [Key]
        public string ID { get; set; }

        [Key]
        public string UserID { get; set; }

        [Key]
        public string PredictionID { get; set; }

        [Display(Name = "Body Mass Index (BMI)")]
        public double BMI { get; set; }

        [Display(Name ="Risk of stroke prediction ")]
        public float StrokeResult { get; set; }

        [Display(Name = "Date "), DataType(DataType.Date)]
        public string Date { get; set; }
    }
}
