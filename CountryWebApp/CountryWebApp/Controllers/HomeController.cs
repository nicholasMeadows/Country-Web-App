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
using Microsoft.AspNetCore.Session;

namespace CountryWebApp.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            //return Ok("Testing Index");
            
            string access_tokenString = HttpContext.Session.GetString("access_token");
            
           
            if (access_tokenString == null)
            {
                return Redirect("https://localhost:44311/authorize?client_id=6bf53df0b48d4937a0f94626c7c31fc6&response_type=code&redirect_uri=http://localhost:8000/Home/Return");
            }
            else
            {
                
                //make get request to CountryApi    
                string result = MakeApiCall(access_tokenString).Result;
                if (result.Contains("Expired acces token"))
                {
                    AccessTokenModel access_token = JsonConvert.DeserializeObject<AccessTokenModel>(HttpContext.Session.GetString("access_token"));
                    AccessTokenModel newAccessToken = JsonConvert.DeserializeObject<AccessTokenModel>( UseRefreshToken(access_token.refresh_token).Result);
                    newAccessToken.refresh_token = access_token.refresh_token;

                    string newAccessTokenString = JsonConvert.SerializeObject(newAccessToken);
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetString("access_token", newAccessTokenString);

                    result = MakeApiCall(newAccessTokenString).Result;
                    //return Ok(newAccessToken);
                }
                return Ok(result);
            }
        }

        
        public IActionResult Return([FromQuery] string code) {
           
            string access_tokenString = GetAccessToken(code).Result;
            HttpContext.Session.SetString("access_token",access_tokenString);
            return RedirectToAction("Index");
            //return Ok(access_tokenString);
        }

        static async Task<string> MakeApiCall(string access_tokenString)
        {
            AccessTokenModel access_token = JsonConvert.DeserializeObject<AccessTokenModel>(access_tokenString);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token.access_token);

            var response = await httpClient.GetAsync("http://localhost:53676/api/values");
            string contents = await response.Content.ReadAsStringAsync();


            return contents;
        }

        static async Task<string> GetAccessToken(string code)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic NmJmNTNkZjBiNDhkNDkzN2EwZjk0NjI2YzdjMzFmYzY6NDdkOWYzNGI4MjA0NGU3ZDliYWM1MGIwZjI3OTAzMDE=");

                var parameters = new Dictionary<string, string>();

                parameters.Add("grant_type", "authorization_code");
                parameters.Add("code", code);
                parameters.Add("redirect_uri", "http://localhost:8000/Home/Return");

                var response = await httpClient.PostAsync("https://localhost:44319/api/token", new FormUrlEncodedContent(parameters));
                string contents = await response.Content.ReadAsStringAsync();


                return contents;
            }
            catch (Exception e) {
                return e.ToString();
            }
        }

        static async Task<string> UseRefreshToken(string refresh_token)
        {
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic NmJmNTNkZjBiNDhkNDkzN2EwZjk0NjI2YzdjMzFmYzY6NDdkOWYzNGI4MjA0NGU3ZDliYWM1MGIwZjI3OTAzMDE=");

            var parameters = new Dictionary<string, string>();
            //parameters["text"] = text;
            parameters.Add("grant_type", "refresh_token");
            parameters.Add("refresh_token", refresh_token);
            

            var response = await httpClient.PostAsync("https://localhost:44311/api/token", new FormUrlEncodedContent(parameters));
            string contents = await response.Content.ReadAsStringAsync();


            return contents;
        }

    }
    
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }
}
  
