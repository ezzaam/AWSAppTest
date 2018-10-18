using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using AWSS2.Helpers;
using AWSS2.Models;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Security.Cryptography;

namespace AWSS2.Controllers
{
    [Produces("application/json")]
    [Route("api/Provider/{mail}/{password}")]
    public class ProviderController : Controller
    {
        private HttpContext currentContext;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ProviderController(IHttpContextAccessor httpContextAccessor, IHostingEnvironment appEnvironment)
        {
            currentContext = httpContextAccessor.HttpContext;
            _hostingEnvironment = appEnvironment;
        }

        [HttpGet("")]
        public bool index(string username)
        {
            return false;
        }

        
        [Route("~/api/Provider/login/{mail}/{password}")]
        public Access login(string email, string password)
        {
            string KeyAccess = string.Empty;
            Access access = new Access();

            var path = _hostingEnvironment.ContentRootPath;

            //make request based on 

            //var MyIni = new IniFile();

            // Or specify a specific name in a specific dir
            var MyIni = new IniFile(path + "\\auth.ini");

            var mail = MyIni.Read("email");
            var pwd = MyIni.Read("password");

            if (mail == email & password == pwd)
            {
                Guid g = Guid.NewGuid();
               
                access.KeyAccess = Convert.ToBase64String(g.ToByteArray());
                access.KeySecret = Convert.ToBase64String(g.ToByteArray());


                MyIni.Write("KeyAccess", access.KeyAccess);
                MyIni.Write("KeySecret",access.KeySecret);

            }

            return access;
        }

        [Route("~/api/Provider/validateRequest/{mail}")]
        public bool validateRequest(string email)
        {
            bool auth = false;

            string KeyAccess = string.Empty;
            Access access = new Access();

            var path = _hostingEnvironment.ContentRootPath;

            // Or specify a specific name in a specific dir
            var MyIni = new IniFile(path + "\\auth.ini");

            var mail = MyIni.Read("KeyAccess");
            var pwd = MyIni.Read("KeySecret");
            string authorization = string.Empty;

            //IEnumerable<string> headerValues = null;

            //HttpContext.Request.Headers.TryGetValue("Authorization", out headerValues);
            StringValues authValues = new StringValues();
            HttpContext.Request.Headers.TryGetValue("Authorization", out authValues);

            var id = authValues.FirstOrDefault();
            string SecretKey = authValues.ElementAt(authValues.Count() - 2);
            string signature = authValues.ElementAt(authValues.Count() - 1);

                //var credential = filterContext.HttpContext.Request.Headers["Authorization"].Substring(4).Split(':');
                //var apiKey = Session.Query<ApiKey>().Where(k => k.AccessKey == credential[0]).FirstOrDefault();
                if (SecretKey != null)
                {
                // check the date header is present & within 15 mins
                Encoding encoder = new UTF8Encoding();
                HMACSHA1 signature2 = new HMACSHA1(encoder.GetBytes(SecretKey));
                //string b64 = Convert.ToBase64String(signature2.ComputeHash(encoder.GetBytes(canonicalString.ToCharArray())));
                if(signature2 == signature2)
                {
                    auth = true;
                }

               
            }

            return auth;
        }

    }
}