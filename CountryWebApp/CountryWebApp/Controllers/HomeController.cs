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
using CountryWebApp.Models;

namespace CountryWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromQuery] string code)
        {
            if(code == null)    
                return View();

            string access_token = GetResponseString(code).Result;

            return Redirect("http://www.google.com/SUCCESS?access=" + access_token);
        }

        public IActionResult sendGet()
        {

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://localhost:53676/api/values");

                if (json.Contains("Missing access_token"))
                {
                    return Redirect("https://localhost:44311/authorize?client_id=6bf53df0b48d4937a0f94626c7c31fc6&response_type=code&redirect_uri=http://localhost:54551");
                }
            }
            return Redirect("http://www.google.com"); 
        }

        static async Task<string> GetResponseString(string code)
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

            AccessTokenModel test = JsonConvert.DeserializeObject<AccessTokenModel>(contents);
            return test.access_token;
        }
    }
}

