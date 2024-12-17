//
using InternalWebsite.API.Extensions;
using InternalWebsite.Application.Utils;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InternalWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiktokController : ClCongController
    {
        private readonly ITikTokService _tikTokService;
        public TiktokController(ILogger<CampaignsController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, ITikTokService tikTokService) : base(logger, config, userManager)
        {
            _tikTokService = tikTokService;
        }

        // GET: api/TikTok/auth-url
        [Route("auth-url")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTiktokAuthUrl()
        {
            try
            {
                return Ok(await _tikTokService.GetTiktokAuthUrl());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        // GET: api/TikTok/callback
        [Route("callback")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> HandleTikTokCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Ok(new ResponseHelper(Config).ErrorMessage("Authorization code is missing"));
            }
            try
            {
                return Ok(await _tikTokService.GenerateTikTokToken(code));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        // POST: api/TikTok/refresh-token
        [Route("refresh-token")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return Ok(new ResponseHelper(Config).ErrorMessage("Refresh token is missing"));
            }

            try
            {
                return Ok(await _tikTokService.RefreshToken(request.RefreshToken));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        // POST: api/TikTok/check-token-expiry
        [Route("check-token-expiry")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckTokenExpiry(CheckTokenExpiryRequestDto request)
        {
            if (request.TokenExpiry == 0 || request.RefreshExpiry == 0 || string.IsNullOrEmpty(request.RefreshToken))
            {
                return Ok(new ResponseHelper(Config).ErrorMessage("Missing required parameters"));
            }

            try
            {
                return Ok(await _tikTokService.CheckIfTikTokTokensAreExpired(request.TokenExpiry, request.RefreshExpiry, request.RefreshToken));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex.Message));
            }
        }
    }
}
