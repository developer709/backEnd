using OfficeOpenXml.Packaging.Ionic.Zip;
using System;
using System.Collections.Generic;
using Infobip.Api.Client;
using Infobip.Api.Client.Api;
using Infobip.Api.Client.Model;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class SmsInfo
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://jjdm4v.api.infobip.com";
        private readonly string _apiKey = "App 8fa0bc6577fa66c1b3043919c9de621d-4435c73e-104a-479e-b588-10ef92f9d2d4";

        public SmsInfo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public  bool VerifyPinAsync(string to, string pwd)
        {
            try
            {
                string message = "Verication code is : " + pwd;
                var url = $"{_baseUrl}/sms/2/text/advanced";

                var requestContent = new
                {
                    messages = new[]
                    {
                        new
                        {
                            destinations = new[]
                            {
                                new { to = to }
                            },
                            from = "447491163443",
                            text = message
                        }
                    }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Headers.Add("Authorization", _apiKey);
                requestMessage.Headers.Add("Accept", "application/json");
                requestMessage.Content = jsonContent;

                var response = _httpClient.Send(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    //var respon = response.Content.rea();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
