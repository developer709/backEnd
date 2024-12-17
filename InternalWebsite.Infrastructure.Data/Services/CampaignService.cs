
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InternalWebsite.ViewModel.Models.FacebookAdDto;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class CampaignService : GenericRepository<Core.Entities.IdentityRole, CampaignDto>, ICampaignService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly FacebookAdFlowService _facebookAdFlowService;
        private readonly ITapPaymentServices _tapPaymentServices;
        private readonly RoleManager<Core.Entities.IdentityRole> _campaignManager;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ILogger<CampaignService> logger,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> campaignManager, FacebookAdFlowService facebookAdFlowService,
            ITapPaymentServices tapPaymentServices,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _campaignManager = campaignManager;
            _facebookAdFlowService = facebookAdFlowService;
            _tapPaymentServices = tapPaymentServices;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<AppResponse> GetCampaign()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                var campaigns = _context.Campaigns.Where(a => a.UserId == userId && a.Title != "" && a.Title != null).OrderByDescending(a => a.CreatedOn).ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: campaigns, message: "Data retrived successfully.");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetCampaignByUser()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    var campaigns = _context.Campaigns.Where(a => a.UserId == userId).ToList();
                    return new ResponseHelper(_configuration).SuccessMessage(data: campaigns, message: "Data retrived successfully.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetCampaignById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentCampaign = _context.Campaigns.Where(campaign => campaign.Id == guidValue && campaign.UserId == userId).FirstOrDefault();
                        if (currentCampaign != null)
                            return new ResponseHelper(_configuration).SuccessMessage(data: currentCampaign, message: "Data retrived successfully.");
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
        public async Task<AppResponse> GetCampaignDetails(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentCampaign = (from campaign in _context.Campaigns
                                               join facebookCampaigns in _context.FacebookCampaigns
                                               on campaign.Id equals facebookCampaigns.CampaignId
                                               where campaign.Id == guidValue && campaign.UserId == userId
                                               select new
                                               {
                                                   Campaign = campaign,
                                                   FacebookCampaigns = facebookCampaigns
                                               }).FirstOrDefault();

                        if (currentCampaign != null)
                            return new ResponseHelper(_configuration).SuccessMessage(data: currentCampaign, message: "Data retrived successfully.");
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
        public async Task<AppResponse> CreateCampaign(CampaignDto campaignModel)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                else
                {
                    var currentIntegrations = _context.Integrations.Where(inte => inte.UserId == userId && inte.Provider == "Facebook" && inte.Info == "Pending").FirstOrDefault();
                    if (currentIntegrations != null)
                    {
                        var checkPendingPages = await _facebookAdFlowService.PendingPages();
                        if (checkPendingPages != null)
                        {
                            var result = checkPendingPages.Data;

                            var ids = currentIntegrations.PageId.Split(',');
                            var tasks = new List<bool>();

                            // Loop through each ID and add ClaimPageAsync tasks to the list
                            bool allPagesNotFound = true; // Track if no IDs match

                            foreach (var id in ids)
                            {
                                var response = result.Where(a => a.Id == id).FirstOrDefault();
                                if (response != null)
                                {
                                    tasks.Add(true);
                                    allPagesNotFound = false; // If any match is found, set this to false
                                }
                            }
                            _logger.LogInformation("---------------------------------- Create Campaign Page Check ----------------------------------");

                            string responseString = JsonConvert.SerializeObject(result, Formatting.Indented);
                            _logger.LogInformation(responseString);
                            // If no match was found, set Info = "Approve"
                            if (allPagesNotFound)
                            {
                                currentIntegrations.Info = "Approve";
                                _context.SaveChanges();
                            }
                            else if (tasks.All(response => response))
                            {
                                return new ResponseHelper(_configuration).ErrorMessage("Page request successfully sent, Your page has been successfully claimed under Balke Tech. Please check your Facebook access to approve the request.");
                            }
                        }
                    }

                    //else
                    //    return new ResponseHelper(_configuration).ErrorMessage("Page request successfully send ', Your page has been successfully claimed under Balke Tech. Please check your Facebook access to approve the request.");

                    var campaign = new Campaign();
                    campaign.Type = campaignModel.Type;
                    campaign.AdType = campaignModel.AdType;
                    campaign.Title = campaignModel.Title;
                    campaign.Objective = campaignModel.Objective;
                    campaign.Status = "Draft";
                    campaign.UserId = userId;
                    campaign.IsActive = true;
                    campaign.CreatedOn = DateTime.Now;
                    campaign.CreatedBy = userId;
                    _context.Add(campaign);
                    _context.SaveChanges();
                    return _responseHelper.SuccessMessage(message: "Saved Successfully", data: campaign.Id);
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> UpdateCampaign(CampaignDto data)
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
                            var currentCampaign = _context.Campaigns.Where(campaign => campaign.Id == guidValue && campaign.UserId == userId).FirstOrDefault();
                            if (currentCampaign == null)
                            {
                                return new ResponseHelper(_configuration).ErrorMessage(message: "Campaign not found against this ID.");
                            }
                            currentCampaign.AdType = data.AdType;
                            currentCampaign.Title = data.Title;
                            currentCampaign.Objective = data.Objective;
                            currentCampaign.Type = data.Type;
                            currentCampaign.EditBy = userId;
                            currentCampaign.EditOn = DateTime.Now;
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
        public async Task<AppResponse> DeleteCampaign(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    //var campaign = await _campaignManager.FindByIdAsync(id);

                    //if (campaign == null)
                    //{
                    //    return new ResponseHelper(_configuration).ErrorMessage(message: "Role not found.");
                    //}
                    //IdentityResult campaignResult = await _campaignManager.DeleteAsync(campaign);
                    //if (campaignResult.Succeeded)
                    //{
                    //    return _responseHelper.SuccessMessage(message: "Role deleted successfully.");
                    //}
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> GetCampaignBudgetById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentCampaign = _context.CampaignBudgets.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                        if (currentCampaign != null)
                            return new ResponseHelper(_configuration).SuccessMessage(data: currentCampaign, message: "Data retrived successfully.");
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
        public async Task<AppResponse> GetCampaignAudienceById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentCampaign = _context.CampaignAudiences.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                        if (currentCampaign != null)
                            return new ResponseHelper(_configuration).SuccessMessage(data: currentCampaign, message: "Data retrived successfully.");
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
        public async Task<AppResponse> GetCampaignContentById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentCampaign = _context.CampaignContents.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                        if (currentCampaign != null)
                        {
                            var getContentCollection = _context.CampaignContentCollections.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).ToList();
                            currentCampaign.CampaignContentCollections = getContentCollection;

                            var tiktokIdentityContentCollection = _context.TiktokIdentityContentCollections.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).ToList();
                            currentCampaign.TiktokIdentityContentCollections = tiktokIdentityContentCollection;
                            return new ResponseHelper(_configuration).SuccessMessage(data: currentCampaign, message: "Data retrived successfully.");
                        }
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
        public async Task<AppResponse> CreateUpdateBudget(CampaignBudgetDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.CampaignId != "")
                    {
                        if (Guid.TryParse(data.CampaignId, out Guid campaignId))
                        {
                            var currentCampaign = _context.CampaignBudgets.Where(campaign => campaign.CampaignId == campaignId && campaign.CreatedBy == userId).FirstOrDefault();
                            if (currentCampaign == null)
                            {
                                var campaignBudget = new CampaignBudget();
                                campaignBudget.ScheduleType = data.ScheduleType;
                                campaignBudget.NumberOfDay = data.NumberOfDay;
                                campaignBudget.Budget = data.Budget;
                                campaignBudget.StartDate = data.StartDate;
                                campaignBudget.StartTime = data.StartTime;
                                campaignBudget.EndDate = data.EndDate;
                                campaignBudget.EndTime = data.EndTime;
                                campaignBudget.HasEndDate = data.HasEndDate;
                                campaignBudget.CampaignId = campaignId;
                                campaignBudget.IsActive = true;
                                campaignBudget.CreatedOn = DateTime.Now;
                                campaignBudget.CreatedBy = userId;
                                _context.Add(campaignBudget);
                            }
                            else
                            {
                                currentCampaign.ScheduleType = data.ScheduleType;
                                currentCampaign.NumberOfDay = data.NumberOfDay;
                                currentCampaign.Budget = data.Budget;
                                currentCampaign.StartDate = data.StartDate;
                                currentCampaign.StartTime = data.StartTime;
                                currentCampaign.EndDate = data.EndDate;
                                currentCampaign.EndTime = data.EndTime;
                                currentCampaign.HasEndDate = data.HasEndDate;
                                currentCampaign.EditBy = userId;
                                currentCampaign.EditOn = DateTime.Now;
                            }

                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(message: currentCampaign == null ? "Created successfully." : "Updated successfully.");
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
        public async Task<AppResponse> UpdateCampaignContents(List<CampaignContentCollectionDto> contents, Guid campaignId)
        {
            var userId = GetUserDetailId();
            // Get the existing records from the database for this campaign
            var existingRecords = _context.CampaignContentCollections
                .Where(c => c.CampaignId == campaignId)
                .ToList();

            // Step 1: Find and delete records that are not in the frontend list (removed from the frontend)
            var idsFromFrontend = contents
                                .Select(c =>
                                {
                                    Guid parsedId;
                                    return Guid.TryParse(c.Id, out parsedId) ? parsedId : (Guid?)null;
                                })
                                .Where(guid => guid.HasValue) // Only take valid GUIDs
                                .Select(guid => guid.Value)
                                .ToList();
            var recordsToDelete = existingRecords
                .Where(c => !idsFromFrontend.Contains(c.Id))
                .ToList();

            if (recordsToDelete.Any())
            {
                _context.CampaignContentCollections.RemoveRange(recordsToDelete);
            }

            // Step 2: Iterate through the list from the frontend
            foreach (var contentDto in contents)
            {
                if (Guid.TryParse(contentDto.Id, out Guid contentId) && contentId != Guid.Empty)
                {
                    // Step 3: Update existing records
                    var existingRecord = existingRecords.FirstOrDefault(c => c.Id == contentId);
                    if (existingRecord != null)
                    {
                        existingRecord.Image = contentDto.Image;
                        existingRecord.ImageName = contentDto.ImageName;
                        existingRecord.Headline = contentDto.Headline;
                        existingRecord.Caption = contentDto.Caption;
                        existingRecord.Name = contentDto.Name;
                        existingRecord.Size = contentDto.Size;
                        existingRecord.Heading = contentDto.Heading;
                        existingRecord.Description = contentDto.Description;
                        existingRecord.WebUrl = contentDto.WebUrl;
                        existingRecord.ImageHash = contentDto.ImageHash;
                        existingRecord.EditBy = userId;
                        existingRecord.EditOn = DateTime.Now;

                        _context.CampaignContentCollections.Update(existingRecord);
                    }
                }
                else
                {
                    // Step 4: Create new records
                    var newRecord = new CampaignContentCollection
                    {
                        Id = Guid.NewGuid(),  // Generate new Guid for new records
                        CampaignId = campaignId,
                        Image = contentDto.Image,
                        Heading = contentDto.Heading,
                        Caption = contentDto.Caption,
                        Headline = contentDto.Headline,
                        ImageName = contentDto.ImageName,
                        Name = contentDto.Name,
                        Size = contentDto.Size,
                        Description = contentDto.Description,
                        WebUrl = contentDto.WebUrl,
                        ImageHash = contentDto.ImageHash,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userId
                    };

                    _context.CampaignContentCollections.Add(newRecord);
                }
            }

            // Step 5: Save changes to the database
            _context.SaveChanges();
            return _responseHelper.SuccessMessage(message: "Campaign contents updated successfully");
        }
        public async Task<AppResponse> UpdateCampaignTiktokContents(List<TiktokIdentityContentCollectionDto> contents, Guid campaignId)
        {
            var userId = GetUserDetailId();
            // Get the existing records from the database for this campaign
            var existingRecords = _context.TiktokIdentityContentCollections
                .Where(c => c.CampaignId == campaignId)
                .ToList();

            // Step 1: Find and delete records that are not in the frontend list (removed from the frontend)
            var idsFromFrontend = contents
                                .Select(c =>
                                {
                                    Guid parsedId;
                                    return Guid.TryParse(c.Id, out parsedId) ? parsedId : (Guid?)null;
                                })
                                .Where(guid => guid.HasValue) // Only take valid GUIDs
                                .Select(guid => guid.Value)
                                .ToList();
            var recordsToDelete = existingRecords
                .Where(c => !idsFromFrontend.Contains(c.Id))
                .ToList();

            if (recordsToDelete.Any())
            {
                _context.TiktokIdentityContentCollections.RemoveRange(recordsToDelete);
            }

            // Step 2: Iterate through the list from the frontend
            foreach (var contentDto in contents)
            {
                if (Guid.TryParse(contentDto.Id, out Guid contentId) && contentId != Guid.Empty)
                {
                    // Step 3: Update existing records
                    var existingRecord = existingRecords.FirstOrDefault(c => c.Id == contentId);
                    if (existingRecord != null)
                    {
                        existingRecord.Image = contentDto.Image;
                        existingRecord.DisplayName = contentDto.DisplayName;
                        existingRecord.CreatedOn = contentDto.CreatedOn;
                        existingRecord.EditBy = userId;
                        existingRecord.EditOn = DateTime.Now;

                        _context.TiktokIdentityContentCollections.Update(existingRecord);
                    }
                }
                else
                {
                    // Step 4: Create new records
                    var newRecord = new TiktokIdentityContentCollection
                    {
                        Id = Guid.NewGuid(),  // Generate new Guid for new records
                        CampaignId = campaignId,
                        Image = contentDto.Image,
                        DisplayName = contentDto.DisplayName,
                        IsActive = true,
                        CreatedOn = contentDto.CreatedOn ?? DateTime.Now,
                        CreatedBy = userId
                    };

                    _context.TiktokIdentityContentCollections.Add(newRecord);
                }
            }

            // Step 5: Save changes to the database
            _context.SaveChanges();
            return _responseHelper.SuccessMessage(message: "Campaign contents updated successfully");
        }

        public async Task<AppResponse> CreateUpdateAudience(CampaignAudienceDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.CampaignId != "")
                    {
                        if (Guid.TryParse(data.CampaignId, out Guid campaignId))
                        {
                            var currentCampaign = _context.CampaignAudiences.Where(campaign => campaign.CampaignId == campaignId && campaign.CreatedBy == userId).FirstOrDefault();
                            if (currentCampaign == null)
                            {
                                var campaignAudience = new CampaignAudience();
                                campaignAudience.Type = data.Type;
                                campaignAudience.Location = data.Location;
                                campaignAudience.City = data.City;
                                campaignAudience.CityDetails = data.CityDetails;
                                campaignAudience.Gender = data.Gender;
                                campaignAudience.StartAge = data.StartAge;
                                campaignAudience.EndAge = data.EndAge;
                                campaignAudience.Age = data.Age;
                                campaignAudience.CampaignId = campaignId;
                                campaignAudience.IsActive = true;
                                campaignAudience.CreatedOn = DateTime.Now;
                                campaignAudience.CreatedBy = userId;
                                _context.Add(campaignAudience);
                            }
                            else
                            {
                                currentCampaign.Type = data.Type;
                                currentCampaign.Location = data.Location;
                                currentCampaign.City = data.City;
                                currentCampaign.CityDetails = data.CityDetails;
                                currentCampaign.Gender = data.Gender;
                                currentCampaign.StartAge = data.StartAge;
                                currentCampaign.EndAge = data.EndAge;
                                currentCampaign.Age = data.Age;
                                currentCampaign.EditBy = userId;
                                currentCampaign.EditOn = DateTime.Now;
                            }

                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(message: currentCampaign == null ? "Created successfully." : "Updated successfully.");
                        }
                    }
                    return _responseHelper.ErrorMessage(message: $"No data found against this ID: {data.Id}.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateUpdateContent(CampaignContentDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.CampaignId != "")
                    {
                        if (Guid.TryParse(data.CampaignId, out Guid campaignId))
                        {
                            var currentCampaign = _context.CampaignContents.Where(campaign => campaign.CampaignId == campaignId && campaign.CreatedBy == userId).FirstOrDefault();
                            if (currentCampaign == null)
                            {
                                var campaignContent = new CampaignContent();
                                campaignContent.BrandName = data.BrandName;
                                campaignContent.Headline = data.Headline;
                                campaignContent.BrandLogo = data.BrandLogo;
                                campaignContent.Media = data.Media;
                                campaignContent.MediaType = data.MediaType;
                                campaignContent.MediaFormat = data.MediaFormat;
                                campaignContent.Attachment = data.Attachment;
                                campaignContent.CTA = data.CTA;
                                campaignContent.MoreURL = data.MoreURL;
                                campaignContent.DisplayURL = data.DisplayURL;
                                campaignContent.Description = data.Description;
                                campaignContent.SearchFeed = data.SearchFeed;
                                campaignContent.AdFavouriting = data.AdFavouriting;
                                campaignContent.NumberOfVideo = data.NumberOfVideo;
                                campaignContent.AdDisclaimer = data.AdDisclaimer;
                                campaignContent.Disclaimer = data.Disclaimer;
                                campaignContent.BtnColor = data.BtnColor;
                                campaignContent.ThumbnailCollection = data.ThumbnailCollection;
                                campaignContent.PageName = data.PageName;
                                campaignContent.AdFormat = data.AdFormat;
                                campaignContent.Caption = data.Caption;
                                campaignContent.Destination = data.Destination;
                                campaignContent.WebUrl = data.WebUrl;
                                campaignContent.CampaignId = campaignId;
                                campaignContent.IsActive = true;
                                campaignContent.CreatedOn = DateTime.Now;
                                campaignContent.CreatedBy = userId;
                                _context.Add(campaignContent);
                            }
                            else
                            {
                                currentCampaign.BrandName = data.BrandName;
                                currentCampaign.Headline = data.Headline;
                                currentCampaign.BrandLogo = data.BrandLogo;
                                currentCampaign.Media = data.Media;
                                currentCampaign.MediaType = data.MediaType;
                                currentCampaign.MediaFormat = data.MediaFormat;
                                currentCampaign.Attachment = data.Attachment;
                                currentCampaign.CTA = data.CTA;
                                currentCampaign.AdFavouriting = data.AdFavouriting;
                                currentCampaign.NumberOfVideo = data.NumberOfVideo;
                                currentCampaign.SearchFeed = data.SearchFeed;
                                currentCampaign.AdDisclaimer = data.AdDisclaimer;
                                currentCampaign.Disclaimer = data.Disclaimer;
                                currentCampaign.BtnColor = data.BtnColor;
                                currentCampaign.ThumbnailCollection = data.ThumbnailCollection;
                                currentCampaign.MoreURL = data.MoreURL;
                                currentCampaign.DisplayURL = data.DisplayURL;
                                currentCampaign.Description = data.Description;
                                currentCampaign.PageName = data.PageName;
                                currentCampaign.AdFormat = data.AdFormat;
                                currentCampaign.Caption = data.Caption;
                                currentCampaign.Destination = data.Destination;
                                currentCampaign.WebUrl = data.WebUrl;
                                currentCampaign.EditBy = userId;
                                currentCampaign.EditOn = DateTime.Now;
                            }

                            _context.SaveChanges();
                            if(data.CampaignContentCollections?.Any() == true)
                            await UpdateCampaignContents(data.CampaignContentCollections, campaignId);
                            if(data.TiktokIdentityContentCollections?.Any() == true)
                            await UpdateCampaignTiktokContents(data.TiktokIdentityContentCollections, campaignId);
                            return _responseHelper.SuccessMessage(message: currentCampaign == null ? "Created successfully." : "Updated successfully.");
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

        public async Task<AppResponse> CampaignStart(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid guidValue))
                    {
                        var currentCampaign = _context.Campaigns.Where(campaign => campaign.Id == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                        if(currentCampaign.Type == "Snapchat" || currentCampaign.Type ==  "TikTok")
                        {
                            return _responseHelper.SuccessMessage(message: $"Campaign in draft.");
                        }
                        var facebookCampaignObj = _context.FacebookCampaigns.Where(a => a.CampaignId == guidValue && a.CreatedBy == userId).FirstOrDefault();
                        if (currentCampaign != null && currentCampaign.Type == "Facebook")
                        {
                            var currentCampaignBudget = _context.CampaignBudgets.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                            var getWalletAmount = await _tapPaymentServices.GetWallet();
                            Wallet wallet = getWalletAmount.Data as Wallet;
                            var totalBudget = Convert.ToDecimal(currentCampaignBudget.Budget * currentCampaignBudget.NumberOfDay);
                            if (currentCampaignBudget != null && wallet.Balance < Convert.ToDecimal(currentCampaignBudget.Budget))
                            {
                                return _responseHelper.ErrorMessage(message: $"Please recharger your wallet. you did not have enough balance");
                            }
                            //var campaignId = facebookCampaignObj.FbCampaignId;
                            var campaignId = await _facebookAdFlowService.CreateCampaign(currentCampaign, facebookCampaignObj);
                            if (!string.IsNullOrEmpty(campaignId))
                            {
                                var campaignAudiences = _context.CampaignAudiences.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                                var currentCampaignContents = _context.CampaignContents.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).FirstOrDefault();
                                var adsetid = await _facebookAdFlowService.CreateAdSet(campaignId, currentCampaignBudget, currentCampaign, facebookCampaignObj, campaignAudiences, currentCampaignContents);
                                if (!string.IsNullOrEmpty(adsetid))
                                {
                                    var campaignContentCollections = _context.CampaignContentCollections.Where(campaign => campaign.CampaignId == guidValue && campaign.CreatedBy == userId).ToList();
                                    var imagehashList = await _facebookAdFlowService.AddImage(currentCampaignContents, facebookCampaignObj, campaignContentCollections);
                                    if (imagehashList != null && imagehashList.Count == campaignContentCollections.Count)
                                    {
                                        //Iterate through both imagehashList and campaignContentCollections simultaneously
                                        for (int i = 0; i < campaignContentCollections.Count; i++)
                                        {
                                            //Assign the image hash to the corresponding CampaignContentCollection object
                                            campaignContentCollections[i].ImageHash = imagehashList[i];
                                            if (currentCampaignContents.MediaType == "Video")
                                            {
                                                campaignContentCollections[i].Caption = imagehashList[i + 1];
                                                i++;
                                            }
                                        }

                                        //Save the changes to the database
                                        _context.CampaignContentCollections.UpdateRange(campaignContentCollections);
                                        await _context.SaveChangesAsync();
                                    }
                                    else if (imagehashList != null && imagehashList.Count > 0 && currentCampaignContents.MediaType == "Video")
                                    {
                                        int imageHashIndex = 0;
                                        for (int i = 0; i < campaignContentCollections.Count; i++)
                                        {
                                            if (imageHashIndex >= imagehashList.Count)
                                                break; // Exit if imagehashList is exhausted

                                            campaignContentCollections[i].ImageHash = imagehashList[imageHashIndex];
                                            imageHashIndex++;

                                            if (currentCampaignContents.MediaType == "Video")
                                            {
                                                if (imageHashIndex < imagehashList.Count)
                                                {
                                                    // Assign the next value as Heading for video type
                                                    campaignContentCollections[i].Heading = imagehashList[imageHashIndex];
                                                    imageHashIndex++;
                                                }
                                            }
                                        }

                                        //Save the changes to the database
                                        _context.CampaignContentCollections.UpdateRange(campaignContentCollections);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        //Handle case when the number of image hashes doesn't match the number of campaign content collections
                                        Console.WriteLine("Error: Mismatch between image hash count and campaign content collection count.");
                                    }
                                    var adcreativeid = await _facebookAdFlowService.CreateAdCreative(currentCampaign, facebookCampaignObj, campaignContentCollections, currentCampaignContents);
                                    var adId = await _facebookAdFlowService.CreateAd(adsetid, adcreativeid, currentCampaign, facebookCampaignObj);

                                    currentCampaign.Status = "PAUSED";

                                    if (facebookCampaignObj == null)
                                    {
                                        FacebookCampaign facebookCampaign = new FacebookCampaign();

                                        facebookCampaign.CampaignId = guidValue;
                                        facebookCampaign.FbCampaignId = campaignId;
                                        facebookCampaign.AdSetId = adsetid;
                                        facebookCampaign.AdCreativeId = adcreativeid;
                                        facebookCampaign.AdId = adId;
                                        facebookCampaign.MediaType = currentCampaignContents.MediaType;
                                        facebookCampaign.UserId = userId;
                                        facebookCampaign.Status = "PAUSED";
                                        facebookCampaign.IsActive = true;
                                        facebookCampaign.CreatedOn = DateTime.Now;
                                        facebookCampaign.CreatedBy = userId;

                                        _context.FacebookCampaigns.Add(facebookCampaign);
                                        _context.SaveChanges();
                                    }

                                    return _responseHelper.SuccessMessage(message: $"Ad created successfully.");
                                }
                                return _responseHelper.ErrorMessage(message: $"Contact with support.");
                            }
                            return _responseHelper.ErrorMessage(message: $"Contact with support.");
                        }
                        return _responseHelper.ErrorMessage(message: $"No data found against this ID: {id}.");
                    }

                    return _responseHelper.ErrorMessage(message: $"No data found against this ID: {id}.");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(message: "Contact with support.");
            }
        }

        public async Task<AppResponse> CampaignStatus(CampaignStatusDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (data.CampaignId != "")
                    {
                        if (Guid.TryParse(data.CampaignId, out Guid campaignId))
                        {
                            var updateCampaignContents = await _facebookAdFlowService.FacbookCampaignStatus(data);
                            return _responseHelper.SuccessMessage(message: updateCampaignContents == null ? "Created successfully." : "Updated successfully.");
                        }
                    }
                    return _responseHelper.SuccessMessage(message: $"No data found against this ID: {data.CampaignId}.");
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
