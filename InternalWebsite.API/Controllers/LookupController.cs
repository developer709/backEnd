using InternalWebsite.Application.Utils;
using InternalWebsite.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using InternalWebsite.API.Extensions;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using InternalWebsite.ViewModel.DTOs;
using Microsoft.AspNetCore.Authorization;
using InternalWebsite.ViewModel.Models;

namespace InternalWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ClCongController
    {
        private readonly ILookupService _icon;
        public LookupController(ILogger<AccountController> logger,
           UserManager<tblUser> userManager,
           IConfiguration config, ILookupService icon) : base(logger, config, userManager)
        {
            _icon = icon;
        }

        [HttpGet]
        //[Authorize]
        [Route("GetLookup")]
        public async Task<IActionResult> GetLookup()
        {

            try
            {
                return Ok(await _icon.GetLookup());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetLookupById")]
        public async Task<IActionResult> GetLookupById(string id)
        {
            try
            {
                return Ok(await _icon.GetLookupById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        //[Authorize]
        [Route("GetLookupByType")]
        public async Task<IActionResult> GetLookupByType(string type)
        {
            try
            {
                return Ok(await _icon.GetLookupByType(type));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        //Create
        [HttpPost]
        [Authorize]
        [Route("CreateAndUpdateLookup")]
        public async Task<IActionResult> CreateAndUpdateLookup(LookupDto data)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                if (data.Id != "0")
                    return Ok(await _icon.UpdateLookup(data));
                return Ok(await _icon.CreateLookup(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [HttpGet]
        [Authorize]
        [Route("DeleteLookup")]
        public async Task<IActionResult> DeleteLookup(string id)
        {
            try
            {
                return Ok(await _icon.DeleteLookup(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
    }
}
