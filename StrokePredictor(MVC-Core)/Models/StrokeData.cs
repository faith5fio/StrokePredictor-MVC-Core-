using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StrokePredictor_MVC_Core_.Models
{
    public class StrokeData
    {
        [Key]
        [LoadColumn(0)]
        public string PredictionID { get; set; }

        [Key]
        [LoadColumn(15)]
        public string UserID { get; set; }

        [LoadColumn(1)]
        [Display(Name ="Gender"), ColumnName(@"gender")]
        public string Gender { get; set; }


        [Display(Name ="Age"), Required, LoadColumn(2), ColumnName(@"age")]
        public string Age { get; set; }

        [LoadColumn(12)]
        [Display(Name ="Your Weight (in KG)"), Required]
        public double Weight { get; set; }

        [LoadColumn(13)]
        [Display(Name ="Your Height (in CM)"), Required]
        public double Height { get; set; }

        [LoadColumn(14)]
        [Display(Name ="Have you had any previous strokes?")]
        public bool PrevStroke { get; set; }

        [Display(Name ="Do you currently have heart disease?"), Required, LoadColumn(4), ColumnName(@"heart_disease")]
        public string HeartDisease { get; set; }

        [Display(Name ="Do you currently have high blood pressure?"), Required, LoadColumn(3), ColumnName(@"hypertension")]
        public string High_BP_Hypertension { get; set; }

        [Display(Name ="Choose the option that best desctibes you in relation to smoking: "), Required, LoadColumn(10), ColumnName(@"smoking_status")]
        public string Smoking { get; set; }

        //[Display(Name ="Choose the option that best option that best descibes your activity level: "), Required]
        //public int ActivityLevel { get; set; }

        [Display(Name ="Would you describe your living environment as rural or urban"), Required, LoadColumn(7), ColumnName(@"Residence_type")]
        public string ResidenceType { get; set; }

        [Display(Name ="Have you ever been married?"), Required, LoadColumn(5), ColumnName(@"ever_married")]
        public string Ever_Married { get; set; }

        [Display(Name ="Choose the option that best describes your working type: "), Required, LoadColumn(6), ColumnName(@"work_type")]
        public string Work_Type { get; set; }

        [Display(Name = "What is your average glucose level: (in mg/dL)"), LoadColumn(8), ColumnName(@"avg_glucose_level")]
        public string Glucose_Level { get; set; }

        [LoadColumn(9), ColumnName(@"bmi")]
        public string BMI { get; set; }

        [ColumnName(@"stroke"), LoadColumn(11)]
        public float Stroke { get; set; }

    }
}