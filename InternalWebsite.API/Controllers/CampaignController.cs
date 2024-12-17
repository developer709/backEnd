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
    public class CampaignsController : ClCongController
    {
        private readonly ICampaignService _campaign;
        public CampaignsController(ILogger<CampaignsController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, ICampaignService campaign) : base(logger, config, userManager)
        {
            _campaign = campaign;
        }

       
        [Route("GetCampaign")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCampaigns()
        {
            try
            {
                return Ok(await _campaign.GetCampaign());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetCampaignByUser")]
        public async Task<IActionResult> GetCampaignByUser()
        {
            try
            {
                return Ok(await _campaign.GetCampaignByUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetCampaignById")]
        public async Task<IActionResult> GetCampaignById(string id)
        {
            try
            {
                return Ok(await _campaign.GetCampaignById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        
        [HttpGet]
        [Authorize]
        [Route("GetCampaignDetails")]
        public async Task<IActionResult> GetCampaignDetails(string id)
        {
            try
            {
                return Ok(await _campaign.GetCampaignDetails(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }


        // POST: api/campaign
        [HttpPost]
        [Authorize]
        [Route("UpdateCampaign")]
        public async Task<IActionResult> UpdateCampaign(CampaignDto campaign)
        {
            try
            {
                return Ok(await _campaign.UpdateCampaign(campaign));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("CreateCampaign")]
        public async Task<IActionResult> CreateCampaign(CampaignDto campaign)
        {
            try
            {
                if (!string.IsNullOrEmpty(campaign.Id))
                    return Ok(await _campaign.UpdateCampaign(campaign));
                return Ok(await _campaign.CreateCampaign(campaign));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        // DELETE: api/campaign/{id}
        //[HttpDelete("{id}")]
        [HttpGet ]
        [Authorize]
        [Route("DeleteCampaign")]
        public async Task<IActionResult> DeleteCampaign(string id)
        {
            try
            {
                return Ok(await _campaign.DeleteCampaign(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }

        [Route("GetCampaignBudgetById")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCampaignBudgetById(string id)
        {
            try
            {
                return Ok(await _campaign.GetCampaignBudgetById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        
        [Route("GetCampaignAudienceById")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCampaignAudienceById(string id)
        {
            try
            {
                return Ok(await _campaign.GetCampaignAudienceById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [Route("CampaignStart")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CampaignStart(string id)
        {
            try
            {
                return Ok(await _campaign.CampaignStart(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [Route("GetCampaignContentById")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCampaignContentById(string id)
        {
            try
            {
                return Ok(await _campaign.GetCampaignContentById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        
        [Route("CreateUpdateBudget")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateBudget(CampaignBudgetDto data)
        {
            try
            {
                return Ok(await _campaign.CreateUpdateBudget(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        
        [Route("CreateUpdateAudience")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateAudience(CampaignAudienceDto data)
        {
            try
            {
                return Ok(await _campaign.CreateUpdateAudience(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        
        [Route("CreateUpdateContent")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUpdateContent(CampaignContentDto data)
        {
            try
            {
                return Ok(await _campaign.CreateUpdateContent(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [Route("CampaignStatus")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CampaignStatus(CampaignStatusDto data)
        {
            try
            {
                return Ok(await _campaign.CampaignStatus(data));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
    }
}
