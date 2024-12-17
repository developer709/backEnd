using DevExpress.Data.Linq.Helpers;
using DevExpress.XtraEditors.Filtering;
using InternalWebsite.API.Extensions;
using InternalWebsite.Application.Utils;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace InternalWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnapchatsController : ClCongController
    {
        private readonly ISnapchatService _Snapchat;
        private readonly IHttpClientFactory _httpClientFactory;
        public SnapchatsController(ILogger<SnapchatsController> logger,
         UserManager<tblUser> userManager, IHttpClientFactory httpClientFactory,
         IConfiguration config, ISnapchatService Snapchat) : base(logger, config, userManager)
        {
            _Snapchat = Snapchat;
            _httpClientFactory = httpClientFactory;
        }


        [Route("GetSnapchat")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSnapchats()
        {
            try
            {
                return Ok(await _Snapchat.GetSnapchat());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetSnapchatByUser")]
        public async Task<IActionResult> GetSnapchatByUser()
        {
            try
            {
                return Ok(await _Snapchat.GetSnapchatByUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetSnapchatById")]
        public async Task<IActionResult> GetSnapchatById(string id)
        {
            try
            {
                return Ok(await _Snapchat.GetSnapchatById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }


        // POST: api/Snapchat
        [HttpPost]
        [Authorize]
        [Route("UpdateSnapchat")]
        public async Task<IActionResult> UpdateSnapchat(SnapchatDto Snapchat)
        {
            try
            {
                return Ok(await _Snapchat.UpdateSnapchat(Snapchat));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        //[Authorize]
        [Route("CreateSnapchat")]
        public async Task<IActionResult> CreateSnapchat(SnapchatDto Snapchat)
        {
            try
            {
                //if (!string.IsNullOrEmpty(Snapchat.Id))
                //    return Ok(await _Snapchat.UpdateSnapchat(Snapchat));
                return Ok(await _Snapchat.CreateSnapchat(Snapchat));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        // DELETE: api/Snapchat/{id}
        //[HttpDelete("{id}")]
        [HttpGet]
        [Authorize]
        [Route("DeleteSnapchat")]
        public async Task<IActionResult> DeleteSnapchat(string id)
        {
            try
            {
                return Ok(await _Snapchat.DeleteSnapchat(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Set the Bearer token in the authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var userInfoResponse = await client.GetAsync("https://kit.snapchat.com/v1/me"); // Ensure correct API endpoint here

                if (userInfoResponse.IsSuccessStatusCode)
                {
                    var userInfo = await userInfoResponse.Content.ReadAsStringAsync();
                    return Ok(userInfo);  // Return user info
                }
                else
                {
                    // Log the error response
                    var errorContent = await userInfoResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)userInfoResponse.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                // Log exception details for debugging
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

     //       var clientId = "2fb8903c-dc58-4a74-b16e-67c909638785";
     //       var clientSecret = "MgjBTQwjsKKnplMJucOwJAqFC6sgl5ApC-9uH6R8-EU";
     //       var redirectUri = "http://localhost:5000/snapchat/callback";

     //       var client = _httpClientFactory.CreateClient();
     //       var tokenResponse = await client.PostAsync("https://accounts.snapchat.com/login/oauth2/access_token", new FormUrlEncodedContent(new[]
     //       {
     //new KeyValuePair<string, string>("grant_type", "authorization_code"),
     //new KeyValuePair<string, string>("code", code),
     //new KeyValuePair<string, string>("client_id", clientId),
     //new KeyValuePair<string, string>("client_secret", clientSecret),
     //new KeyValuePair<string, string>("redirect_uri", redirectUri)
     //}));

     //       var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
     //       // Deserialize tokenResult, store access token, and use it to fetch user data
     //       return Ok(tokenResult);
        }
    }
}
