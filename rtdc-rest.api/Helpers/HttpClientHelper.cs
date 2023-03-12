﻿using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using rtdc_rest.api.config;

namespace rtdc_rest.api.Helpers
{
    public class HttpClientHelper
    {
        public HttpClientHelper()
        {

        }
        public string SendPOSTRequest(string userName, string password, string endPoint, string postData)
         {
            string apiUrl = Configuration.getApiUrl();
            string urlPathForRequest = apiUrl.ToString();

            #region commentOUT
            //string urlPathForRequest = "https://rtdc-apitest.engingrup.com/api/AYK";
            #endregion

            urlPathForRequest = urlPathForRequest + endPoint;

            var authenticationString = $"{userName}:{password}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            var content = new StringContent(postData, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, urlPathForRequest);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
            requestMessage.Content = content;

            var httpClient = new HttpClient();
            
            var response = httpClient.Send(requestMessage);

            return response.Content.ReadAsStringAsync().Result;

            #region--commentOUT
            //var jsonModel = JsonSerializer.Serialize(postData);
            //var postDataByte = Encoding.ASCII.GetBytes(jsonModel);

            //WebRequest webrequest = WebRequest.Create(urlPathForRequest);
            //webrequest.Method = "POST";
            //webrequest.ContentType = "application/json";
            //webrequest.Headers.Add("accept", "application/json");
            //webrequest.Headers.Add("auth-token", getEncodedAuthToken(token)); // getEncodedAuthToken(token));

            //string responseString = "";
            //try
            //{
            //    using (var stream = webrequest.GetRequestStream())
            //    {
            //        stream.Write(postDataByte, 0, postDataByte.Length);
            //    }

            //    var response = (HttpWebResponse)webrequest.GetResponse();
            //    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //}
            //catch (WebException webEx)
            //{
            //    var response = ((HttpWebResponse)webEx.Response);
            //    StreamReader content = new StreamReader(response.GetResponseStream());
            //    responseString = content.ReadToEnd();

            //    return responseString;
            //}

            //return responseString;
            #endregion
        }

    }
}
