using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AWSS2.Helpers
{
    public class AWSCommon
    {

        public AWSCommon()
        {

        }
        
        public Dictionary<string, string> AddAuthHeader(string method, string path, Dictionary<string, string> httpHeaders, string awsAccessKeyId, string awsSecretAccessKey)
        {
            string canonicalString = MakeCanonicalString(
                                                        method,
                                                        httpHeaders.ContainsKey("Content-Md5") ? httpHeaders["Content-Md5"] : null,
                                                        httpHeaders["Content-Type"],
                                                        httpHeaders["Date"],
                                                        path);
            string signature = EncodeCanonicalString(awsSecretAccessKey, canonicalString, false);
            httpHeaders.Add("Authorization", "AWS " + awsAccessKeyId + ":" + signature);

            return httpHeaders;
        }

        private static string EncodeCanonicalString(string awsSecretAccessKey, string canonicalString, bool urlEncode)
        {
            Encoding encoder = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(encoder.GetBytes(awsSecretAccessKey));
            string b64 = Convert.ToBase64String(signature.ComputeHash(encoder.GetBytes(canonicalString.ToCharArray())));

            if (urlEncode)
            {
                return HttpUtility.UrlEncode(b64);
            }
            else
            {
                return b64;
            }
        }
        private static string CreateRequestBody(string method, string path, string query, Dictionary<string, string> headers, string body)
        {
            StringBuilder requestBody = new StringBuilder();

            requestBody.Append(method);
            requestBody.Append(" ");
            requestBody.Append(path);
            if (query != null)
            {
                requestBody.Append("?");
                requestBody.Append(query);
            }
            requestBody.Append(" HTTP/1.1\r\n");
            foreach (KeyValuePair<String, String> header in headers)
            {
                requestBody.Append(header.Key);
                requestBody.Append(": ");
                requestBody.Append(header.Value);
                requestBody.Append("\r\n");
            }
            requestBody.Append("\r\n");
            requestBody.Append(body);
            requestBody.Append("\r\n");

            return requestBody.ToString();
        }


        public void AddHeaders(Dictionary<string, string> httpHeaders, string host)
        {
            if (httpHeaders != null)
            {
                if (!httpHeaders.ContainsKey("Content-Type"))
                {
                    httpHeaders.Add("Content-Type", "text/plain");
                }
            }
            else
            {
                //Add a default date and content type
                httpHeaders.Add("Content-Type", "text/plain");
            }
            httpHeaders.Add("Date", GetDateString());
            httpHeaders.Add("Host", host);
            httpHeaders.Add("AWS-Version", "2006-04-01");
        }

        private static string GetDateString()
        {
            return DateTime.UtcNow.ToString("R");
        }
        private static string MakeCanonicalString(string verb, string contentMD5, string contentType, string date, string path)
        {
            //<HTTP-METHOD>\n<content-MD5>\n<ContentType>\n<date>\n<path>

            StringBuilder sb = new StringBuilder();
            sb.Append(verb);
            sb.Append("\n");
            sb.Append(contentMD5);
            sb.Append("\n");
            sb.Append(contentType);
            sb.Append("\n");
            sb.Append(date);
            sb.Append("\n");
            sb.Append(path);

            return sb.ToString();
        }


    }
}
