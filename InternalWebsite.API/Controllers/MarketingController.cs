using DevExpress.Data.Linq.Helpers;
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
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InternalWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingsController : ClCongController
    {
        private readonly IMarketingService _marketing;
        public MarketingsController(ILogger<MarketingsController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, IMarketingService marketing) : base(logger, config, userManager)
        {
            _marketing = marketing;
        }

       
        [Route("GetMarketing")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMarketings()
        {
            try
            {
                return Ok(await _marketing.GetMarketing());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetMarketingByUser")]
        public async Task<IActionResult> GetMarketingByUser()
        {
            try
            {
                return Ok(await _marketing.GetMarketingByUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetMarketingById")]
        public async Task<IActionResult> GetMarketingById(string id)
        {
            try
            {
                return Ok(await _marketing.GetMarketingById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }


        // POST: api/marketing
        [HttpPost]
        [Authorize]
        [Route("UpdateMarketing")]
        public async Task<IActionResult> UpdateMarketing(MarketingDto marketing)
        {
            try
            {
                return Ok(await _marketing.UpdateMarketing(marketing));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("CreateMarketing")]
        public async Task<IActionResult> CreateMarketing(MarketingDto marketing)
        {
            try
            {
                if (!string.IsNullOrEmpty(marketing.Id))
                    return Ok(await _marketing.UpdateMarketing(marketing));
                return Ok(await _marketing.CreateMarketing(marketing));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        // DELETE: api/marketing/{id}
        //[HttpDelete("{id}")]
        [HttpGet ]
        [Authorize]
        [Route("DeleteMarketing")]
        public async Task<IActionResult> DeleteMarketing(string id)
        {
            try
            {
                return Ok(await _marketing.DeleteMarketing(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
    }
}
