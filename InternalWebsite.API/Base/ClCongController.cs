using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InternalWebsite.Application.Utils;
using InternalWebsite.API.Controllers;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.ViewModel.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InternalWebsite.API.Extensions
{
    public class ClCongController : ControllerBase
    {
        protected readonly ILogger Logger;
        protected readonly IConfiguration Config;
        protected readonly UserManager<tblUser> UserManager;

        public ClCongController(ILogger logger,
            IConfiguration config, UserManager<tblUser> userManager)
        {
            Logger = logger;
            Config = config;
            UserManager = userManager;
        }
    }

    public class ClCongController<T> : ControllerBase
    {
        protected readonly ILogger Logger;
        protected readonly IConfiguration Config;
        protected readonly UserManager<tblUser> UserManager;
        protected readonly T Repository;

        public ClCongController(ILogger logger,
            IConfiguration config, UserManager<tblUser> userManager, T repository)
        {
            Logger = logger;
            Config = config;
            UserManager = userManager;
            Repository = repository;
        }
    }


    public class ClCongController<T, TT, TTt> : ControllerBase where T : IGenericRepository<TT, TTt> where TT : class
    {
        protected readonly ILogger Logger;
        protected readonly IConfiguration Config;
        protected readonly UserManager<tblUser> UserManager;
        protected readonly T Repository;

        public ClCongController(ILogger logger,
            IConfiguration config, UserManager<tblUser> userManager, T repository)
        {
            Logger = logger;
            Config = config;
            UserManager = userManager;
            Repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TT model)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                var result = await Repository.Add(model);
                return Ok(new ResponseHelper(Config).SuccessMessage(data: result, miscData: null));
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await Repository.Remove(id);
                return Ok(new ResponseHelper(Config).SuccessMessage(data: result, miscData: null));
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateIsPublic(long id, bool state)
        {
            try
            {
                var result = await Repository.ChangePublicState(id, state);
                return Ok(new ResponseHelper(Config).SuccessMessage(data: result, miscData: null));
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TT model)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseHelper(Config).ErrorMessage(message: "Invalid Object"));
            try
            {
                var result = await Repository.Update(model);
                return Ok(new ResponseHelper(Config).SuccessMessage(data: result, miscData: null));
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<TT>>> GetAll([FromQuery] PaginatorDto paginatorDto = null)
        {
            try
            {
                var result = await Repository.GetAll(paginatorDto);
                return Ok(new ResponseHelper(Config).SuccessMessage(data: result.Item1, miscData: result.Item2));
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.ToString());
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }
    }
}