
using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Lookup = InternalWebsite.Core.Entities.Lookup;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class LookupService : GenericRepository<Lookup, LookupDto>, ILookupService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;

        public LookupService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetLookup()
        {
            try
            {

                var data = _context.Lookups.ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: data, message: "Data retrived successfully.");


            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetLookupById(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid LookupId))
                {
                    var currentLookup = _context.Lookups.Where(a => a.Id == LookupId).FirstOrDefault();
                    if (currentLookup != null)
                        return new ResponseHelper(_configuration).SuccessMessage(data: currentLookup, message: "Data retrived successfully.");
                }


                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> GetLookupByType(string type)
        {
            try
            {
                var currentLookup = _context.Lookups.Where(a => a.Type.ToLower() == type.ToLower()).ToList();
                if (currentLookup != null)
                    return new ResponseHelper(_configuration).SuccessMessage(data: currentLookup, message: "Data retrived successfully.");


                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {type}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateLookup(LookupDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    Lookup LookupObj = new Lookup();
                    LookupObj.Name = data.Name;
                    LookupObj.Type = data.Type;
                    LookupObj.Value = data.Value;
                    LookupObj.Role = data.Role;
                    LookupObj.Source = data.Source;
                    LookupObj.Description = data.Description;
                    LookupObj.IsActive = true;
                    LookupObj.CreatedOn = DateTime.Now;
                    LookupObj.CreatedBy = userId;

                    _context.Lookups.Add(LookupObj);
                    _context.SaveChanges();

                    return _responseHelper.SuccessMessage(data: LookupObj, message: "Lookup Saved Successfully");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> UpdateLookup(LookupDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(data.Id, out Guid LookupId))
                    {
                        var currentLookup = _context.Lookups.Where(a => a.Id == LookupId && a.CreatedBy == userId).FirstOrDefault();
                        if (currentLookup != null)
                        {
                            currentLookup.Name = data.Name;
                            currentLookup.Type = data.Type;
                            currentLookup.Value = data.Value;
                            currentLookup.Role = data.Role;
                            currentLookup.Source = data.Source;
                            currentLookup.Description = data.Description;
                            currentLookup.EditOn = DateTime.Now;
                            currentLookup.EditBy = userId;
                            currentLookup.IsActive = data.IsActive;
                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(data: currentLookup, message: "Lookup Updated Successfully");
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
        public async Task<AppResponse> DeleteLookup(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid LookupId))
                    {
                        var currentLookup = _context.Lookups.Where(a => a.Id == LookupId && a.CreatedBy == userId).FirstOrDefault();
                        if (currentLookup != null)
                        {
                            _context.Remove(currentLookup);
                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(message: "Lookup Deleted Successfully");
                        }
                    }
                    return _responseHelper.SuccessMessage(message: "No data found against this ID");
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
