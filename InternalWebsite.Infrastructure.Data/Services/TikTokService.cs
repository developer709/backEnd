using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using InternalWebsite.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class TikTokService : GenericRepository<CampaignDto, CampaignDto>, ITikTokService
    {
        private readonly IConfiguration _configuration;
        private static readonly List<string> requiredScopes = new List<string>(); // Add "video.publish", "video.upload" as needed
        private readonly HttpClient _httpClient;
        private readonly ResponseHelper _responseHelper;
        public TikTokService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            HttpClient httpClient, ResponseHelper responseHelper, IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _httpClient = httpClient;
            _responseHelper = responseHelper;
            _configuration = configuration;
        }
        public async Task<AppResponse> RefreshToken(string refreshToken)
        {
            try
            {
                var clientKey = _configuration["TIKTOK:TIKTOK_AD_FLOW_CLIENT_KEY"].ToString();
                var clientSecret = _configuration["TIKTOK:TIKTOK_AD_FLOW_CLIENT_SECRET"].ToString();

                var requestData = new Dictionary<string, string>
                {
                    { "client_key", clientKey },
                    { "client_secret", clientSecret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                };

                var content = new FormUrlEncodedContent(requestData);

                HttpResponseMessage response = await _httpClient.PostAsync("https://open.tiktokapis.com/v2/oauth/token/", content);

                if (!response.IsSuccessStatusCode)
                    return _responseHelper.ErrorMessage(message: "Failed to generate the access tokens");


                var responseData = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseData);

                if (data.error_description != null)
                {
                    return _responseHelper.ErrorMessage(message: (string)data.error_description);
                }

                var expiresIn = (int?)data.expires_in;
                var refreshExpiresIn = (int?)data.refresh_expires_in;
                var scope = (string)data.scope;

                // Ensure all required scopes are granted
                foreach (var requiredScope in requiredScopes)
                {
                    if (!scope.Contains(requiredScope))
                    {
                        return _responseHelper.ErrorMessage(message: "Please grant all the permissions.");
                    }
                }

                // Convert seconds to epoch timestamp
                if (expiresIn != null)
                {
                    var expiresDate = DateTime.UtcNow.AddSeconds(expiresIn.Value);
                    data.expires_in = new DateTimeOffset(expiresDate).ToUnixTimeSeconds();
                }

                if (refreshExpiresIn != null)
                {
                    var refreshExpiresDate = DateTime.UtcNow.AddSeconds(refreshExpiresIn.Value);
                    data.refresh_expires_in = new DateTimeOffset(refreshExpiresDate).ToUnixTimeSeconds();
                }
                return _responseHelper.SuccessMessage(data: data, message: "Data retrieved successfully.");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(message: ex.Message);
            }
        }
        public async Task<AppResponse> CheckIfTikTokTokensAreExpired(long tokenExpiry, long refreshExpiry, string refreshToken)
        {
            try
            {
                // 0: token not expired, 1: token is expired
                int statusCode = 0;
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Get current time in milliseconds

                if (now > tokenExpiry)
                {
                    statusCode = 1;

                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        return _responseHelper.ErrorMessage("Refresh token is missing to renew the expired token.");
                    }

                    if (now > refreshExpiry)
                    {
                        return _responseHelper.ErrorMessage("Login required.");
                    }
                }

                return _responseHelper.SuccessMessage(data: statusCode, message: "Token status checked successfully.");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex.Message);
            }
        }
        public async Task<AppResponse> GetTiktokAuthUrl()
        {
            try
            {
                // Generate random strings for CSRF state and code verifier
                var csrfState = Guid.NewGuid().ToString("N").Substring(0, 8); // Similar to JavaScript random string
                var codeVerifier = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

                // Create the code challenge using SHA256 hash
                using (var sha256 = SHA256.Create())
                {
                    var codeChallengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                    var codeChallenge = Convert.ToBase64String(codeChallengeBytes)
                                            .Replace("+", "-")
                                            .Replace("/", "_")
                                            .Replace("=", ""); // Base64 URL encoding
                    var clientKey = _configuration["TIKTOK:TIKTOK_AD_FLOW_CLIENT_KEY"].ToString();
                    var redirectURL = _configuration["TIKTOK:TIKTOK_AD_FLOW_REDIRECT_URI"].ToString();

                    var url = $"https://www.tiktok.com/v2/auth/authorize/?client_key={clientKey}&scope=user.info.basic,{string.Join(",", requiredScopes)}&response_type=code&redirect_uri={redirectURL}&state={csrfState}&code_challenge={codeChallenge}&code_challenge_method=S256";

                    return _responseHelper.SuccessMessage(data: url, message: "URL generated successfully.");
                }
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex.Message);
            }
        }
        public async Task<AppResponse> GenerateTikTokToken(string code)
        {
            try
            {
                var clientKey = _configuration["TIKTOK:TIKTOK_AD_FLOW_CLIENT_KEY"].ToString();
                var clientSecret = _configuration["TIKTOK:TIKTOK_AD_FLOW_CLIENT_SECRET"].ToString();
                var redirectURL = _configuration["TIKTOK:TIKTOK_AD_FLOW_REDIRECT_URI"].ToString();

                var requestData = new Dictionary<string, string>
                {
                    { "client_key", clientKey },
                    { "client_secret", clientSecret },
                    { "code", Uri.UnescapeDataString(code) }, // Decodes URI component
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", redirectURL }
                };

                var content = new FormUrlEncodedContent(requestData);

                HttpResponseMessage response = await _httpClient.PostAsync("https://open.tiktokapis.com/v2/oauth/token/", content);

                if (!response.IsSuccessStatusCode)
                {
                    return _responseHelper.ErrorMessage("Failed to generate the access tokens.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseData);

                if (data.error_description != null)
                {
                    return _responseHelper.ErrorMessage((string)data.error_description);
                }

                var expiresIn = (int?)data.expires_in;
                var refreshExpiresIn = (int?)data.refresh_expires_in;
                var scope = (string)data.scope;

                // Ensure all required scopes are granted
                foreach (var requiredScope in requiredScopes)
                {
                    if (!scope.Contains(requiredScope))
                    {
                        return _responseHelper.ErrorMessage("Please grant all the permissions.");
                    }
                }

                // Convert seconds to epoch timestamp
                if (expiresIn != null)
                {
                    var expiresDate = DateTime.UtcNow.AddSeconds(expiresIn.Value);
                    data.expires_in = new DateTimeOffset(expiresDate).ToUnixTimeSeconds();
                }

                if (refreshExpiresIn != null)
                {
                    var refreshExpiresDate = DateTime.UtcNow.AddSeconds(refreshExpiresIn.Value);
                    data.refresh_expires_in = new DateTimeOffset(refreshExpiresDate).ToUnixTimeSeconds();
                }

                return _responseHelper.SuccessMessage(data: data, message: "Token generated successfully.");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex.Message);
            }
        }

    }
}
