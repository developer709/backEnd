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
using System.Threading.Tasks;
using System;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Drawing;

namespace InternalWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefralController : ClCongController
    {
        private readonly IRefralService _refral;
        public RefralController(ILogger<AccountController> logger,
           UserManager<tblUser> userManager,
           IConfiguration config, IRefralService refral) : base(logger, config, userManager)
        {
            _refral = refral;
        }
        [HttpGet]
        //[Authorize]
        [Route("GetRefrals")]
        public async Task<IActionResult> GetRefrals()
        {

            try
            {
                return Ok(await _refral.GetRefrals());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetRefralById")]
        public async Task<IActionResult> GetRefralById(string id)
        {
            try
            {
                return Ok(await _refral.GetRefralById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }


        [HttpGet]
        [Authorize]
        [Route("GetRefralSByRefranceId")]
        public async Task<IActionResult> GetRefralSByRefranceId(string id)
        {
            try
            {
                return Ok(await _refral.GetRefralSByRefranceId(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        //Create
        [HttpPost]
        [Authorize]
        [Route("CreateAndUpdateRefral")]
        public async Task<IActionResult> CreateAndUpdateRefral(RefralDto data)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                if (data.Id != "")
                    return Ok(await _refral.UpdateRefral(data));
                return Ok(await _refral.CreateRefral(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        //Create
        [HttpGet]
        [Authorize]
        [Route("UpdateRefralByUser")]
        public async Task<IActionResult> UpdateRefralByUser()
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                return Ok(await _refral.UpdateRefralByUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [HttpGet]
        [Authorize]
        [Route("DeleteRefral")]
        public async Task<IActionResult> DeleteRefral(string id)
        {
            try
            {
                return Ok(await _refral.DeleteRefral(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
    }
}
