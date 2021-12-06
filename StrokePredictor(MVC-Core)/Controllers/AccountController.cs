//using Firebase.Auth;
//using FireSharp.Interfaces;
//using FireSharp.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrokePredictor_MVC_Core_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Auth;
//using FirebaseAuthentication.net;

namespace StrokePredictor_MVC_Core_ .Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private static string ApiKey = "AIzaSyC4-5bgeIQYaqob-3hkJ2LNwMNDmPMETy0 "; //Web Api Key
        //private static string Bucket = "testmvc-c33d0.appspot.com";

        public static string userName;
        public static string userID;
        //IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        //{
        //    BasePath = "https://strokepredictor-default-rtdb.firebaseio.com/" //Database url
        //};
        //IFirebaseClient client;

        public static FirebaseClient firebase = new FirebaseClient("https://strokepredictor-default-rtdb.firebaseio.com/");
        

        /////////////////////////  Methods   /////////////////////////

        public ActionResult CreateNewAccount()
        {
            //Purpose: this method loads the empty create account page (sign up)
            //Return type: returns a view (All input fields are empty)
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateNewAccount(SignUpModel signUpModel)
        {
            //Purpose: this method creates a users account in the auth table in firebase
            //Return type: returns a view (All input fields are empty)
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                var ab = await auth.CreateUserWithEmailAndPasswordAsync(signUpModel.Email, signUpModel.Password, signUpModel.Name + " "
                    +signUpModel.LastName,true);
                userID = ab.User.LocalId;
                await firebase.Child("AdditionalUserInfo/").Child(userID).PutAsync(signUpModel);

                userID = ab.User.LocalId;
                userName = ab.User.DisplayName;


                return RedirectToRoute(new
                {
                    controller = "Prediction",
                    action = "HomePage"
                });
                //UserInfoAdd(signUpModel);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UserInfoAdd(SignUpModel model)
        {
            //Purpose: this method adds the additional info not able to be added in the auth table
            //Return type: returns a view with a success message  or error message
            try
            {
                //AddUserExtraToDB(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        private void AddUserExtraToDB(SignUpModel signUpModel)
        {
            //Purpose: this method adds the data to the correct node
            //Return type: returns a view (All input fields are empty)
            string nodeName = "AdditionalUserInfo/";
            //client = new FireSharp.FirebaseClient(config);

            var data = signUpModel;
            //PushResponse response = client.Push(nodeName, data);
            var AddInfo = firebase
                .Child(nodeName)
                .PostAsync(signUpModel);
            data.Id = userID;

            // AddInfo.Result;
            //SetResponse setResponse = client.Set(nodeName + data.Id, data);
        }
        

        public ActionResult Login()
        {
            //Purpose: this method loads the empty login view
            //Return type: returns an empty view (All input fields are empty)
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginModel model)
        {
            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

                    //currentUser = await firebase.Child("AdditionalUserInfo/").Child(userID).OnceSingleAsync<ProfileModel>();
                    //ModelState.AddModelError(string.Empty, "Continue to home page " + ab.User.LocalId + ab.User.DisplayName);
                    userID = ab.User.LocalId;
                    userName = ab.User.DisplayName;
                    
                    
                    return RedirectToRoute(new
                    {
                        controller = "Prediction",
                        action = "HomePage"
                    });
                    
                    
                    //string token = ab.FirebaseToken;
                    //var userIn = await auth.GetUserAsync(token);
                    //var user = ab.User;
                    //if (token != "")
                    //{
                    //    this.SignInUser(user.Email, token, false);
                    //    return this.RedirectToLocal(returnUrl);
                    //}
                }
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                //}
            }
            catch (Exception ex)
            {
                
                if(ex.Message.Contains("UnknownEmailAddress"))
                    ModelState.AddModelError(string.Empty, "A user with this email does not exist");
                if(ex.Message.Contains("WrongPassword"))
                    ModelState.AddModelError(string.Empty, "Incorrect Password");
            }

            // If we got this far, something failed, redisplay form wit possible errors
            return View(model);
        }

        public ActionResult ForgotPassword()
        {
            //Purpose: this method loads the empty forgot password view
            //Return type: returns an empty view (All input fields are empty)
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                await auth.SendPasswordResetEmailAsync(email);

                ModelState.AddModelError(string.Empty, "Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            ////Method logs the user out
            //var ctx = Request.GetOwinContext();
            
            //var authenticationManager = ctx.Authentication;
            //authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
    }
}