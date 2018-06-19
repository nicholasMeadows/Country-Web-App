using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CountryWebApp.Models;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace CountryWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromQuery] string code)
        {/*
            DataModel data = new DataModel();
            if (code == null)
            {
                return View(data);
            }

            string access_tokenString = GetAccessToken(code).Result;

            AccessTokenModel accessToken = JsonConvert.DeserializeObject<AccessTokenModel>(access_tokenString);

            TempData["accessToken"] = accessToken.access_token;

            TempData.Keep();
            return View();*/

            //TempData["test"] = "asdf";
            //TempData.Keep();
            HttpContext.Session.SetString("","");
            return View();
        }

        public IActionResult sendGet()
        {
            string data = (string)TempData.Peek("test");
            

            return Redirect("http://www.google.com/" + (string)TempData.Peek("test") );
            /*
            if (data == null)
            {
                return Redirect("https://localhost:44311/authorize?client_id=6bf53df0b48d4937a0f94626c7c31fc6&response_type=code&redirect_uri=http://localhost:54551");
            }
            return Ok("asdf");
            /*
            if (data.access_token == null)
            {
                return Redirect("https://localhost:44311/authorize?client_id=6bf53df0b48d4937a0f94626c7c31fc6&response_type=code&redirect_uri=http://localhost:54551");
            }
            else
            {/*
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Authorization", "Bearer " + data.access_token);
                    string response = wc.DownloadString("http://localhost:53676/api/values");

                    //errorModel err = JsonConvert.DeserializeObject<errorModel>(response);

                    return Ok(response);    
                }

                return Ok("Ok test");
                */
        }
        /*
       // AccessTokenModel accessToken = (AccessTokenModel)TempData["access"];

       // return Redirect("/sdf=" + TempData["access"]);
        if (data.accessToken == null)
        {
            /*
            using (WebClient wc = new WebClient())
            {
                // wc.Headers.Add("Authorization", "Bearer "+accessToken.access_token);
                string response = wc.DownloadString("http://localhost:53676/api/values");

                errorModel err = JsonConvert.DeserializeObject<errorModel>(response);

                if (err.error.message.Equals("Missing access_token"))
                    {*/

        //This redirect goes to the login page for the OAuth 2 server and returns the the index with the request token
        //      return Redirect("https://localhost:44311/authorize?client_id=6bf53df0b48d4937a0f94626c7c31fc6&response_type=code&redirect_uri=http://localhost:54551");
        /*   }
       else {
           return Redirect("asdf");
       }
   }*/

        /*
        }
        else {
            return Ok();
        }           */


        static async Task<string> GetAccessToken(string code)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic NmJmNTNkZjBiNDhkNDkzN2EwZjk0NjI2YzdjMzFmYzY6NDdkOWYzNGI4MjA0NGU3ZDliYWM1MGIwZjI3OTAzMDE=");

            var parameters = new Dictionary<string, string>();
            //parameters["text"] = text;
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("code", code);
            parameters.Add("redirect_uri", "http://localhost:54551");

            var response = await httpClient.PostAsync("https://localhost:44311/api/token", new FormUrlEncodedContent(parameters));
            string contents = await response.Content.ReadAsStringAsync();


            return contents;
        }


    }
}


