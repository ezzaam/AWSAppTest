using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using AWSS2.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using AWSS2.Helpers;

namespace AWSS2.Controllers
{
    [Produces("application/json")]
    [Route("api/authenticate")]
    
    public class authenticateController : Controller
    {
        private HttpContext currentContext;

        private readonly IHostingEnvironment _hostingEnvironment;

        public authenticateController(IHttpContextAccessor httpContextAccessor, IHostingEnvironment appEnvironment)
        {
            currentContext = httpContextAccessor.HttpContext;
            _hostingEnvironment = appEnvironment;
        }

        [Route("api/authenticate")]
        public bool index()
        {
            return false;
        }
               
        [HttpGet]
        [Route("~/api/authenticate/GetUserByCredentials/{mail}/{password}")]
        public bool GetUserByCredentials(string login, string password)
        {
            return true;
            //some code
        }

        [Route("~/api/authenticate/login/{mail}/{password}")]
        public bool login(string mail, string password)
        {
            bool authenticated = false;


            var path = _hostingEnvironment.ContentRootPath;

            //make request based on 

            //var MyIni = new IniFile();

            // Or specify a specific name in a specific dir
            var MyIni = new IniFile(path + "\\INI.ini");

            //You can write some values like so:

            HttpClient client = new HttpClient();
            
            //client.BaseAddress = new Uri(GetBaseUrl());
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(GetBaseUrl() + "/api/Provider/login/" + mail + "/" + password).Result;

            //HttpResponseMessage response = client.GetAsync("api/customer/GetAll").Result;  // Blocking call!  
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");
                // Get the response
               var customerJsonString = response.Content.ReadAsStringAsync();
                //Console.WriteLine("Your response data is: " + customerJsonString);
                string obj = Convert.ToString(customerJsonString);
                // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
               var deserialized = JsonConvert.DeserializeObject<IEnumerable<Access>>(obj);
                // Do something with it

                authenticated = true;
            }

           
            return authenticated;
        }

        public string GetBaseUrl()
        {
            var request = currentContext.Request;

            var host = request.Host.ToUriComponent();

            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }
    }
}