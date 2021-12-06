/*
 * Purpose: class is responsible for all user actions 
 * related to creating a predition
 */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrokePredictor_MVC_Core_.Models;
using StrokePredictor_MVC_Core_.Controllers;
using System.Web;
using Firebase.Database.Query;

namespace StrokePredictor_MVC_Core_.Controllers
{
    public class PredictionController : Controller
    {
        public ProfileModel user = new ProfileModel();
        public string userID=AccountController.userID;
        private string userAge, userGender;

        // GET: Prediction
        public async Task<ActionResult> HomePage()
        {
            //Purpose: this method loads the users home page
            //Return type: returns a view

            ViewData["Name"] = AccountController.userName;
            CurrentUser(AccountController.userID);
            List<ResultModel> results = new List<ResultModel>();
            var databaseObjects = await AccountController.firebase.Child("Results").Child(userID).OrderByKey().OnceAsync<ResultModel>();

            foreach (var result in databaseObjects)
            {
                results.Add(result.Object);
            }
            return View(results);
        }

        public async void CurrentUser(string userID)
        {
            ProfileModel profile = new ProfileModel();
            user
             = await AccountController.firebase.Child("AdditionalUserInfo/").Child(userID).OnceSingleAsync<ProfileModel>();
            userAge = user.Age.ToString();
            userGender = user.Gender;
            //return user;
        }

        [HttpGet]
        public ActionResult CreateNewPrediction()
        {
            //Purpose: this method loads the empty create prediction view
            //Return type: returns an empty view (All input fields are empty except age an gender)
            
            ViewData["Age"] = userAge + userGender;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewPrediction(StrokeData strokeData)
        {
            //Purpose: this method receives the users data to make a new prediction
            //Return type: returns an error if there is one or redirects the user to the result

            string nodeName = "PredictionInfo/";
            double strokeHeight_in_meters = strokeData.Height / 100;
            strokeData.UserID = userID;
            Random random = new Random();
            strokeData.PredictionID = DateTime.UtcNow.Year.ToString() + DateTime.UtcNow.Month + DateTime.UtcNow.Day + random.Next();
            ResultModel resultModel = new ResultModel();
            
            //Calculate the BMI
            strokeData.BMI = (strokeData.Weight / (strokeHeight_in_meters * strokeHeight_in_meters)).ToString();
            resultModel.BMI = Math.Round(Convert.ToDouble(strokeData.BMI), 2);
            resultModel.PredictionID = strokeData.PredictionID;
            resultModel.UserID = strokeData.UserID;


            if(strokeData.BMI != null || strokeData.BMI != "")
            {
                var predictionResult = Prediction.Predict(strokeData);
                ModelState.AddModelError(string.Empty, predictionResult.Prediction.ToString());
                
                var save = AccountController.firebase.Child(nodeName).Child(userID).Child(strokeData.PredictionID).PutAsync(strokeData);
                ModelState.AddModelError(string.Empty, "Go to results" + strokeData.UserID + " " + save.Status.ToString());
                resultModel.StrokeResult = predictionResult.Prediction;
                return RedirectToAction("Results", resultModel);
                //redirect to action
            }
  
            return View();
        }

        public async Task<ActionResult> ViewHistory()
        {
            //Purpose: this method loads the users history list 
            //Return type: list of all previous predictions if there any
            List<ResultModel> results = new List<ResultModel>();
            var databaseObjects = await AccountController.firebase.Child("Results").Child(userID).OrderByKey().OnceAsync<ResultModel>();

            foreach(var result in databaseObjects)
            {
                results.Add(result.Object);
            }
            
            return View(results);
        }

        public async Task<ActionResult> UserProfile()
        {
            //Purpose: this method loads the users profile page
            //Return type: view with the users information

            ProfileModel profile = new ProfileModel();
            CurrentUser(AccountController.userID);
            profile
             = await AccountController.firebase.Child("AdditionalUserInfo/").Child(userID).OnceSingleAsync<ProfileModel>();

            return View(profile);
        }

        public async Task<ActionResult> Results(ResultModel resultModel)
        {
            StrokeData strokeData = new StrokeData();
            strokeData = await AccountController.firebase.Child("PredictionInfo/").Child(userID).Child(resultModel.PredictionID).OnceSingleAsync<StrokeData>();
            resultModel.Date = DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString();
            
            await AccountController.firebase.Child("Results").Child(resultModel.UserID).Child(resultModel.PredictionID).PutAsync(resultModel);
            
            //var predictionInfo = 
            
            return View( new FullResultModel {
                StrokeInfo = strokeData,
                ResultInfo = resultModel
            });
        }

        public async Task<ActionResult> DeletePrediction(string predId)
        {
            await AccountController.firebase.Child("Results/").Child(userID).Child(predId).DeleteAsync();
            return RedirectToAction("ViewHistory");
        }

        public async Task<ActionResult> OpenPrediction( string predId)
        {
            ResultModel result = new ResultModel();
            result = await AccountController.firebase.Child("Results/").Child(userID).Child(predId).OnceSingleAsync<ResultModel>();
            return RedirectToAction("Results", result);
        }


    }
}