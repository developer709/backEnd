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
    public class IntegrationsController : ClCongController
    {
        private readonly IIntegrationService _integration;
        public IntegrationsController(ILogger<IntegrationsController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, IIntegrationService integration) : base(logger, config, userManager)
        {
            _integration = integration;
        }

       
        [Route("GetIntegration")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetIntegrations()
        {
            try
            {
                return Ok(await _integration.GetIntegration());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetIntegrationByUser")]
        public async Task<IActionResult> GetIntegrationByUser()
        {
            try
            {
                return Ok(await _integration.GetIntegrationByUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetIntegrationById")]
        public async Task<IActionResult> GetIntegrationById(string id)
        {
            try
            {
                return Ok(await _integration.GetIntegrationById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }


        // POST: api/integration
        [HttpPost]
        [Authorize]
        [Route("UpdateIntegration")]
        public async Task<IActionResult> UpdateIntegration(IntegrationDto integration)
        {
            try
            {
                return Ok(await _integration.UpdateIntegration(integration));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("CreateIntegration")]
        public async Task<IActionResult> CreateIntegration(IntegrationDto integration)
        {
            try
            {
                if (!string.IsNullOrEmpty(integration.Id))
                    return Ok(await _integration.UpdateIntegration(integration));
                return Ok(await _integration.CreateIntegration(integration));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        // DELETE: api/integration/{id}
        //[HttpDelete("{id}")]
        [HttpGet ]
        [Authorize]
        [Route("DeleteIntegration")]
        public async Task<IActionResult> DeleteIntegration(string id)
        {
            try
            {
                return Ok(await _integration.DeleteIntegration(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
    }
}
