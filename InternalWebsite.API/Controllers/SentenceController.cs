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
    public class SentenceController : ClCongController
    {
        private readonly ISentenceService _icon;
        public SentenceController(ILogger<AccountController> logger,
           UserManager<tblUser> userManager,
           IConfiguration config, ISentenceService icon) : base(logger, config, userManager)
        {
            _icon = icon;
        }

        [HttpGet]
        //[Authorize]
        [Route("GetSentence")]
        public async Task<IActionResult> GetSentence()
        {

            try
            {
                return Ok(await _icon.GetSentence());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetSentenceById")]
        public async Task<IActionResult> GetSentenceById(string id)
        {
            try
            {
                return Ok(await _icon.GetSentenceById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        //[Authorize]
        [Route("GetSentenceByType")]
        public async Task<IActionResult> GetSentenceByType(string type)
        {
            try
            {
                return Ok(await _icon.GetSentenceByType(type));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        //Create
        [HttpPost]
        [Authorize]
        [Route("CreateAndUpdateSentence")]
        public async Task<IActionResult> CreateAndUpdateSentence(SentenceDto data)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                if (data.Id != "0")
                    return Ok(await _icon.UpdateSentence(data));
                return Ok(await _icon.CreateSentence(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [HttpGet]
        [Authorize]
        [Route("DeleteSentence")]
        public async Task<IActionResult> DeleteSentence(string id)
        {
            try
            {
                return Ok(await _icon.DeleteSentence(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
    }
}
