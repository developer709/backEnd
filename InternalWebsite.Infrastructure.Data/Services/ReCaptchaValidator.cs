using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class ReCaptchaValidator
    {
        private readonly string _secretKey = "6Le_dSoqAAAAANeamiOr23J1PLFf8sixrimvwGw6";

        public ReCaptchaValidator(string secretKey)
        {
            _secretKey = secretKey;
        }

        public async Task<bool> IsValid(string token)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", token)
                });

                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                var json = await response.Content.ReadAsStringAsync();

                var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(json);

                return reCaptchaResponse.Success;
            }
        }
    }

    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
