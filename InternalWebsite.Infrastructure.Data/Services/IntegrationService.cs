using DevExpress.Xpo;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class IntegrationService : GenericRepository<Core.Entities.IdentityRole, IntegrationDto>, IIntegrationService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly RoleManager<Core.Entities.IdentityRole> _integrationManager;
        private readonly FacebookAdFlowService _facebookAdFlowService;
        private readonly ILogger<IntegrationService> _logger;

        public IntegrationService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
           ILogger<IntegrationService> logger,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> integrationManager,
            FacebookAdFlowService facebookAdFlowService,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _logger = logger;
            _facebookAdFlowService = facebookAdFlowService;
            _responseHelper = responseHelper;
            _integrationManager = integrationManager;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetIntegration()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                var integrations = _context.Integrations.Where(a => a.UserId == userId).ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: integrations, message: "Data retrived successfully.");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetIntegrationByUser()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    var integrations = _context.Integrations.Where(a => a.UserId == userId).ToList();
                    return new ResponseHelper(_configuration).SuccessMessage(data: integrations, message: "Data retrived successfully.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetIntegrationById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentRole = _context.Integrations.Where(integration => integration.Id == guidValue && integration.CreatedBy == userId).FirstOrDefault();
                        if (currentRole != null)
                            return new ResponseHelper(_configuration).SuccessMessage(data: currentRole, message: "Data retrived successfully.");
                    }

                    return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateIntegration(IntegrationDto integrationModel)
        {
            try
            {
                _logger.LogInformation("---------------------------------- Integration Create Start ----------------------------------");
                string responseString = JsonConvert.SerializeObject(integrationModel, Formatting.Indented);
                _logger.LogInformation(responseString.ToString());
                if (string.IsNullOrEmpty(integrationModel.Name))
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "Name cannot be null or empty.");
                }
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                var currentIntegration = _context.Integrations.Where(integra => integra.Provider == integrationModel.Provider && integra.CreatedBy == userId).FirstOrDefault();
                if (currentIntegration != null)
                {
                    currentIntegration.Name = integrationModel.Name;
                    currentIntegration.Provider = integrationModel.Provider;
                    currentIntegration.Data = integrationModel.Data;
                    currentIntegration.Info = "Pending";
                    currentIntegration.SessionId = integrationModel.SessionId;
                    currentIntegration.PageId = integrationModel.PageId;
                    currentIntegration.EditBy = userId;
                    currentIntegration.EditOn = DateTime.Now;
                }
                else
                {
                    var integration = new Integration();
                    integration.Name = integrationModel.Name;
                    integration.Provider = integrationModel.Provider;
                    integration.Data = integrationModel.Data;
                    integration.Info = "Pending";
                    integration.SessionId = integrationModel.SessionId;
                    integration.PageId = integrationModel.PageId;
                    integration.UserId = userId;
                    integration.IsActive = true;
                    integration.CreatedOn = DateTime.Now;
                    integration.CreatedBy = userId;
                    _context.Add(integration);
                }
                if (string.IsNullOrEmpty(integrationModel.Id))
                {
                    _logger.LogInformation("---------------------------------- Integration Create Page Check ----------------------------------");
                    string responseString1 = JsonConvert.SerializeObject(integrationModel, Formatting.Indented);
                    _logger.LogInformation(responseString1.ToString());

                    var ids = integrationModel.PageId.Split(',');
                    var tasks = new List<bool>();
                    _logger.LogInformation("---------------------------------- Integration  Ids ----------------------------------");
                    string responseString2 = JsonConvert.SerializeObject(ids, Formatting.Indented);
                    _logger.LogInformation(responseString2.ToString());
                    // Loop through each ID and add ClaimPageAsync tasks to the list
                    foreach (var id in ids)
                    {
                        var response = await _facebookAdFlowService.ClaimPageAsync(id.Trim());
                        tasks.Add(response);
                    }

                    // Wait for all tasks to complete
                    if (tasks.All(response => response))
                    {
                        _context.SaveChanges();  // Save only if all requests succeeded
                        return _responseHelper.SuccessMessage(message: "Page request successfully send ', Your page has been successfully claimed under Balke Tech. Please check your Facebook access to approve the request.");

                    }

                }
                return _responseHelper.ErrorMessage();
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> UpdateIntegration(IntegrationDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.Id != "")
                    {
                        if (Guid.TryParse(data.Id, out Guid guidValue))
                        {
                            var currentIntegration = _context.Integrations.Where(integration => integration.Id == guidValue && integration.UserId == userId).FirstOrDefault();
                            if (currentIntegration == null)
                            {
                                return new ResponseHelper(_configuration).ErrorMessage(message: "Integration not found against this ID.");
                            }
                            currentIntegration.Name = data.Name;
                            currentIntegration.Data = data.Data;
                            currentIntegration.Info = data.Info;
                            currentIntegration.SessionId = data.SessionId;
                            currentIntegration.Provider = data.Provider;
                            currentIntegration.SessionId = data.SessionId;
                            currentIntegration.PageId = data.PageId;
                            currentIntegration.EditBy = userId;
                            currentIntegration.EditOn = DateTime.Now;
                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(message: "Updated successfully.");
                        }

                    }
                    return _responseHelper.SuccessMessage(message: $"No data found against this ID: {data.Id}.");

                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");



            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> DeleteIntegration(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {

                        var integration = _context.Integrations.Where(a => a.Id == guidValue && a.CreatedBy == userId).FirstOrDefault();

                        if (integration == null)
                        {
                            return new ResponseHelper(_configuration).ErrorMessage(message: "Record not found.");
                        }
                        _context.Remove(integration);
                        _context.SaveChanges();

                        return _responseHelper.SuccessMessage(message: "Disconnect successfully.");
                    }
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
    }
}
