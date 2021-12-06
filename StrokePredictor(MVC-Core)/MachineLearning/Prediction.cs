using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using StrokePredictor_MVC_Core_.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static System.Environment;

namespace StrokePredictor_MVC_Core_
{
    public class Prediction
    {
        private readonly static string dataPath = 
            Path.Combine(
                @"C:\Users\Faith\source\repos\StrokePredictor(MVC-Core)\StrokePredictor(MVC-Core)\MachineLearning\healthcare-dataset-stroke-data.csv"
                );
        private readonly static string modelPath = Path.Combine(@"C:\Users\Faith\source\repos\StrokePredictor(MVC-Core)\StrokePredictor(MVC-Core)\MachineLearning\", "StrokePredictionModel.zip");
        public static readonly Lazy<PredictionEngine<StrokeData, PredictionOutput>> PredictEngine = new Lazy<PredictionEngine<StrokeData, PredictionOutput>>(() => CreatePredictionEngine(), true);
        private static MLContext mLContext = new MLContext(seed:0);
        private static PredictionEngine<StrokeData, PredictionOutput> predictor;
        static ITransformer model;
        static IDataView dataView; //to laod the data

        private static IDataView LoadDataset(string dataPath)
        {

            dataView = mLContext.Data.LoadFromTextFile<StrokeData>(dataPath, hasHeader: true, separatorChar: ',');
            //string featurescolumn = "Features";
            //var pipeLine = mLContext.Transforms.Concatenate(featurescolumn,
            //    "Gender", "Age", "HeartDisease",
            //    "High_BP_Hypertension", "Smoking", "Residence type",
            //    "Ever-married", "WorkType", "Glucose_Level", "BMi")
            //    .Append(mLContext.BinaryClassification.Trainers.AveragedPerceptron(featurescolumn));

            ////Try with current ml context and a new ml context to see difference in there is any
            ////Delete Later!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            ////Train the model
            //var model = pipeLine.Fit(dataView);

            ////Save it
            //using (var fileStream = new
            //    FileStream(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            //{
            //    mLContext.Model.Save(model, dataView.Schema, fileStream);
            //}

            //predictor = mLContext.Model.CreatePredictionEngine<StrokeData, PredictionOutput>(model);



            return dataView;
        }//Load DataSet

        private static void SaveModel()
        {
            //var featurescolumn = "Features";
            //var pipeLine = mLContext.Transforms.Concatenate(@"Features",
            //    @"gender", @"ever_married", @"work_type", @"Residence_type", @"smoking_status",
            //                              @"age", @"hypertension", @"heart_disease", @"avg_glucose_level", @"bmi")
            //    .Append(mLContext.BinaryClassification.Trainers.FastTree(labelColumnName:@"stroke", featureColumnName: @"Features"));

            var pipeLine = mLContext.Transforms
                .Categorical.OneHotEncoding(new[] { new InputOutputColumnPair(@"gender", @"gender"),
                    new InputOutputColumnPair(@"ever_married", @"ever_married"),
                    new InputOutputColumnPair(@"work_type", @"work_type"),
                    new InputOutputColumnPair(@"Residence_type", @"Residence_type"),
                    new InputOutputColumnPair(@"age", @"age"),
                                        new InputOutputColumnPair(@"hypertension", @"hypertension"),
                                        new InputOutputColumnPair(@"heart_disease", @"heart_disease"),
                                        new InputOutputColumnPair(@"avg_glucose_level", @"avg_glucose_level"),
                                        new InputOutputColumnPair(@"bmi", @"bmi"),
                    new InputOutputColumnPair(@"smoking_status", @"smoking_status") })
                                    .Append(mLContext.Transforms.Concatenate(
                                        @"Features", new[] { @"gender", @"ever_married", @"work_type", @"Residence_type", @"smoking_status",
                                            @"age", @"hypertension", @"heart_disease", @"avg_glucose_level", @"bmi" }))
                                    .Append(mLContext.Transforms.Conversion.ConvertType(@"stroke", @"stroke", Microsoft.ML.Data.DataKind.Boolean))
                                    .Append(mLContext.BinaryClassification.Trainers.FastTree(labelColumnName: @"stroke", featureColumnName: @"Features"))
                                    .Append(mLContext.Transforms.Conversion.ConvertType(@"PredictedLabel", @"PredictedLabel"))
            ;

            //var pipeLine = mLContext.Transforms.Categorical.OneHotEncoding(new[] { new InputOutputColumnPair(@"gender", @"gender"), new InputOutputColumnPair(@"ever_married", @"ever_married"), new InputOutputColumnPair(@"work_type", @"work_type"), new InputOutputColumnPair(@"Residence_type", @"Residence_type"), new InputOutputColumnPair(@"smoking_status", @"smoking_status") })
            //                        .Append(mLContext.Transforms.ReplaceMissingValues(new[] { new InputOutputColumnPair(@"age", @"age"), new InputOutputColumnPair(@"hypertension", @"hypertension"), new InputOutputColumnPair(@"heart_disease", @"heart_disease"), new InputOutputColumnPair(@"avg_glucose_level", @"avg_glucose_level"), new InputOutputColumnPair(@"bmi", @"bmi") }))
            //                        .Append(mLContext.Transforms.Concatenate(@"Features", new[] { @"gender", @"ever_married", @"work_type", @"Residence_type", @"smoking_status", @"age", @"hypertension", @"heart_disease", @"avg_glucose_level", @"bmi" }))
            //                        .Append(mLContext.Transforms.Conversion.MapValueToKey(@"stroke", @"stroke"))
            //                        .Append(mLContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mLContext.BinaryClassification.Trainers.FastTree(new FastTreeBinaryTrainer.Options() { NumberOfLeaves = 119, MinimumExampleCountPerLeaf = 4, NumberOfTrees = 158, MaximumBinCountPerFeature = 160, LearningRate = 1F, FeatureFraction = 0.980011330006043F, LabelColumnName = @"stroke", FeatureColumnName = @"Features" }), labelColumnName: @"stroke"))
            //                        .Append(mLContext.Transforms.Conversion.MapKeyToValue(@"PredictedLabel", @"PredictedLabel"));

            //Train the model
            model = pipeLine.Fit(dataView);

            //Save it
            using (var fileStream = new
                FileStream(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                mLContext.Model.Save(model, dataView.Schema, fileStream);
            }
        }

        private static PredictionEngine<StrokeData, PredictionOutput> CreatePredictionEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<StrokeData, PredictionOutput>(mlModel);
            //predictor = mLContext.Model.CreatePredictionEngine<StrokeData, PredictionOutput>(model);
            //return predictor;
        }

        public static PredictionOutput Predict(StrokeData data)
        {
            LoadDataset(dataPath);
            SaveModel();
            

            var predEngine = PredictEngine.Value;
            return predEngine.Predict(data);
        }

    }
}