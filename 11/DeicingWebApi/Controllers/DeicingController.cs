using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using DeicingApp;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace DeicingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeicingController : ControllerBase
    {
        // GET: Deicing/status
        [HttpGet("status")]
        public string Get()
        {
            return JsonConvert.SerializeObject(Car.Instance.GetCarStatus());
        }

        [HttpGet("GetCity/{name}")]
        public void GetName(string name)
        {
            City c = City.GetMyCity();
            City.name = name;
        }

        public int GetTemp(string cityName)
        {
            string str;
            using (StreamReader strr = new StreamReader(HttpWebRequest.Create($"http://api.apixu.com/v1/current.json?key=f23f1e67a5b94765988182759191604&q={cityName}").GetResponse().GetResponseStream()))
                str = strr.ReadToEnd();
            var temp = JObject.Parse(str)["current"]["temp_c"].ToString();
            return Convert.ToInt32(temp);
        }

        [HttpGet("invoke")]
        public int Invoke()
        {
            try
            {
                FileManager.Instance.Set("3", "../controllerStatus.txt", true);
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        // POST: deicing
        [HttpPost]
        public int Post([FromBody] int planeId)
        {
            try
            {
                if (GetTemp(City.name) <= 10)
                {
                    if (Car.Instance.GetCarStatus().Equals("0"))
                    {
                        string toStatus = "2" + " " + planeId.ToString();
                        FileManager.Instance.Set(toStatus, "../controllerStatus.txt", true);
                        return 0; //запрос принят
                    }
                    else
                    {
                        return 1;
                    }
                }
                else return 1;
            }
            catch
            {
                return 1;
            }
        }

    }
}