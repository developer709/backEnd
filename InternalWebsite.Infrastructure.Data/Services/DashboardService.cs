using Dapper;
using DevExpress.Xpo;
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.DTOs;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class DashboardService : GenericRepository<Core.Entities.IdentityRole, DashboardDto>, IDashboardService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly IEmailSenderService _emailSenderService;

        public DashboardService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper,  IEmailSenderService emailSenderService,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
        }
        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }

        private SqlConnection SqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<AppResponse> GetUserCount(string filter = "all")
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var today = DateTime.Today;
                DateTime startDate;
                DateTime endDate = today; // Default endDate is today

                // Determine the filter range
                switch (filter.ToLower())
                {
                    case "last7days":
                        startDate = today.AddDays(-7);
                        break;
                    case "last30days":
                        startDate = today.AddDays(-30);
                        break;
                    case "last3months":
                        startDate = today.AddMonths(-3);
                        break;
                    case "year2024":
                        startDate = new DateTime(2024, 1, 1);
                        endDate = new DateTime(2024, 12, 31).AddDays(1).AddTicks(-1);
                        break;
                    default: // "all" or invalid filter
                        return new ResponseHelper(_configuration).SuccessMessage(data: _context.ApplicationUsers.Count(), message: "Data retrived successfully.");
                }

                // Query for the filtered count
                var dashboardCount = _context.ApplicationUsers
                    .Count(u => u.CreatedOn >= startDate && u.CreatedOn <= endDate);

                return new ResponseHelper(_configuration).SuccessMessage(data: dashboardCount, message: "Data retrived successfully.");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetCamapignTotal(string filter = "all")
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    var today = DateTime.Today;
                    DateTime startDate;
                    DateTime endDate = today; // Default to today

                    // Determine the filter range
                    switch (filter.ToLower())
                    {
                        case "last7days":
                            startDate = today.AddDays(-7);
                            break;
                        case "last30days":
                            startDate = today.AddDays(-30);
                            break;
                        case "last3months":
                            startDate = today.AddMonths(-3);
                            break;
                        case "year2024":
                            startDate = new DateTime(2024, 1, 1);
                            endDate = new DateTime(2024, 12, 31).AddDays(1).AddTicks(-1);
                            break;
                        default: // "all" or invalid filter
                            startDate = DateTime.MinValue;
                            endDate = DateTime.MaxValue;
                            break;
                    }

                    // Filter campaigns based on the determined date range
                    var dashboards = _context.Campaigns
                        .Where(c => c.CreatedOn >= startDate && c.CreatedOn <= endDate)
                        .ToList();

                    // Group campaigns by type and count
                    var groupedCounts = dashboards
                        .GroupBy(c => c.Type)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Create SocialCampaign DTO with counts
                    var SocialCampaign = new SocialCampaign
                    {
                        Twitter = groupedCounts.GetValueOrDefault("Twitter", 0),
                        Instagram = groupedCounts.GetValueOrDefault("Instagram", 0),
                        Tiktok = groupedCounts.GetValueOrDefault("TikTok", 0),
                        Snapchat = groupedCounts.GetValueOrDefault("Snapchat", 0),
                        Facebook = groupedCounts.GetValueOrDefault("Facebook", 0),
                        Total = dashboards.Count
                    };

                    return new ResponseHelper(_configuration).SuccessMessage(data: SocialCampaign, message: "Data retrieved successfully.");
                }

                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not correct");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> TotalBudget(string filter = "all")
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    // Initialize date range
                    DateTime? startDate = null;
                    DateTime? endDate = null;
                    DateTime today = DateTime.Today;

                    // Determine the filter range
                    switch (filter.ToLower())
                    {
                        case "last7days":
                            startDate = today.AddDays(-7);
                            break;
                        case "last30days":
                            startDate = today.AddDays(-30);
                            break;
                        case "last3months":
                            startDate = today.AddMonths(-3);
                            break;
                        case "year2024":
                            startDate = new DateTime(2024, 1, 1);
                            endDate = new DateTime(2024, 12, 31).AddDays(1).AddTicks(-1); // Full 2024 year
                            break;
                        default: // "all" or invalid filter
                            startDate = null; // No filter applied
                            endDate = null;
                            break;
                    }

                    using (var connection = CreateConnection())
                    {
                        var sqlQuery = "CalculateTotalBudget";

                        // Prepare stored procedure parameters
                        var parameters = new DynamicParameters();
                        parameters.Add("@StartDate", startDate);
                        parameters.Add("@EndDate", endDate);

                        var budget = connection.Query<TotalCampaignBudget>(sqlQuery, parameters,
                            commandType: CommandType.StoredProcedure, buffered: true, commandTimeout: 3600).FirstOrDefault();

                        return _responseHelper.SuccessMessage(data: budget, message: "Data retrieved successfully.");
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
