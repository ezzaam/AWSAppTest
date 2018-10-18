using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using AWSS2.Helpers;
using System.Net.Http;

namespace AWSS2.Controllers
{
    [Produces("application/json")]
    [Route("api/confidentials")]
    public class confidentialsController : Controller
    {
        private HttpContext currentContext;

        private readonly IHostingEnvironment _hostingEnvironment;

        public confidentialsController(IHttpContextAccessor httpContextAccessor, IHostingEnvironment appEnvironment)
        {
            currentContext = httpContextAccessor.HttpContext;
            _hostingEnvironment = appEnvironment;
        }

        [HttpGet("")]
        public bool index(string username)
        {

            bool confid = false;

            var path = _hostingEnvironment.ContentRootPath;

            //make request based on 

            //var MyIni = new IniFile();

            // Or specify a specific name in a specific dir
            var MyIni = new IniFile(path + "\\INI.ini");

            //You can write some values like so:

            var KeyAccess = MyIni.Read("KeyAccess");
            var KeySecret = MyIni.Read("KeySecret");


            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.
            AWSCommon comm = new AWSCommon();
            Dictionary<string, string> httpHeaders = new Dictionary<string, string>();
            comm.AddHeaders(httpHeaders, GetBaseUrl());
            comm.AddAuthHeader("GET", GetBaseUrl(), httpHeaders, KeyAccess, KeySecret);

            string header = string.Empty;

            foreach (var item in httpHeaders)
            {
                header += ";";
                header += item.Key + "." + item.Value;
            }
           
            //client.BaseAddress = new Uri(GetBaseUrl());
            client.DefaultRequestHeaders.Add("Header",header);
           HttpResponseMessage response = client.GetAsync(GetBaseUrl() + "/api/Provider/validate").Result;

            //HttpResponseMessage response = client.GetAsync("api/customer/GetAll").Result;  // Blocking call!  
            if (response.IsSuccessStatusCode)
            {

                
                confid = true;
            }



            return confid;
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