
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
    public class RolesController : ClCongController
    {
        private readonly IRoleService _role;
        public RolesController(ILogger<AccountController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, IRoleService role) : base(logger, config, userManager)
        {
            _role = role;
        }

       
        [Route("GetRole")]
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                return Ok(await _role.GetRole());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
        [HttpGet]
        [Authorize]
        [Route("GetRoleByUser")]
        public async Task<IActionResult> GetRoleByUser()
        {
            try
            {
                return Ok(await _role.GetRoleByUser());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetRoleById")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            try
            {
                return Ok(await _role.GetRoleById(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }

        }
       

        // POST: api/roles
        [HttpPost]
        [Authorize]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole(RoleDto role)
        {
            try
            {
                return Ok(await _role.UpdateRole(role));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        [HttpPost]
        [Authorize]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleDto role)
        {
            try
            {
                if (role.Id != "")
                    return Ok(await _role.UpdateRole(role));
                return Ok(await _role.CreateRole(role));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
        // DELETE: api/roles/{id}
        //[HttpDelete("{id}")]
        [HttpGet ]
        [Authorize]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                return Ok(await _role.DeleteRole(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage());
            }
        }
    }
}
