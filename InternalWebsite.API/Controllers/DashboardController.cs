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
    public class DashboardsController : ClCongController
    {
        private readonly IDashboardService _dashboard;
        public DashboardsController(ILogger<DashboardsController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, IDashboardService Dashboard) : base(logger, config, userManager)
        {
            _dashboard = Dashboard;
        }


        [Route("GetUserCount")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserCount(string id)
        {
            try
            {
                return Ok(await _dashboard.GetUserCount(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetCamapignTotal")]
        public async Task<IActionResult> GetCamapignTotal(string id)
        {
            try
            {
                return Ok(await _dashboard.GetCamapignTotal(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("TotalBudget")]
        public async Task<IActionResult> TotalBudget(string id)
        {
            try
            {
                return Ok(await _dashboard.TotalBudget(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

    }
}
