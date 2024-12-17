using DevExpress.Data.Linq.Helpers;
using InternalWebsite.API.Extensions;
using InternalWebsite.Application.Utils;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.DTOs;
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
    public class AdminsController : ClCongController
    {
        private readonly IAdminService _admin;
        public AdminsController(ILogger<AdminsController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, IAdminService admin) : base(logger, config, userManager)
        {
            _admin = admin;
        }

       
        [Route("GetAdminUser")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAdminUser()
        {
            try
            {
                return Ok(await _admin.GetAdminUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetUserList")]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                return Ok(await _admin.GetUserList());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetAdminById")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            try
            {
                return Ok(await _admin.GetAdminById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }


        // POST: api/admin
        [HttpPost]
        [Authorize]
        [Route("UpdateAdmin")]
        public async Task<IActionResult> UpdateAdmin(AdminUser admin)
        {
            try
            {
                return Ok(await _admin.UpdateAdmin(admin));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin(AdminUser admin)
        {
            try
            {
                if (!string.IsNullOrEmpty(admin.Id))
                    return Ok(await _admin.UpdateAdmin(admin));
                return Ok(await _admin.CreateAdmin(admin));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        // DELETE: api/admin/{id}
        //[HttpDelete("{id}")]
        [HttpGet ]
        [Authorize]
        [Route("DeleteAdmin")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            try
            {
                return Ok(await _admin.DeleteAdmin(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
    }
}
