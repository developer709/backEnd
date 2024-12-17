
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class RefralService : GenericRepository<Refral, RefralDto>, IRefralService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;

        public RefralService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _configuration = configuration;
        }
        public async Task<AppResponse> GetRefrals()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }
                var data = _context.Refrals.ToList();
                return new ResponseHelper(_configuration).SuccessMessage(data: data, message: "Data retrived successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetRefralById(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                if (Guid.TryParse(id, out Guid RefralId))
                {
                    var currentObject = _context.Refrals.Where(a => a.Id == RefralId).FirstOrDefault();
                    if (currentObject != null)
                        return new ResponseHelper(_configuration).SuccessMessage(data: currentObject, message: "Data retrived successfully.");
                }

                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetRefralSByRefranceId(string id)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                if (Guid.TryParse(id, out Guid RefralId))
                {
                    var currentObject = _context.Refrals.Where(a => a.RefranceId == RefralId).ToList();
                    if (currentObject != null)
                        return new ResponseHelper(_configuration).SuccessMessage(data: currentObject, message: "Data retrived successfully.");
                }

                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateRefral(RefralDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                Refral NewObj = new Refral();
                if (Guid.TryParse(data.RefranceId, out Guid RefranceId))
                {
                    NewObj.RefranceId = RefranceId;
                }
                NewObj.UserId = userId;
                NewObj.IsActive = true;
                NewObj.CreatedOn = DateTime.Now;
                NewObj.CreatedBy = userId;

                await _context.Refrals.AddAsync(NewObj);
                await _context.SaveChangesAsync();

                return _responseHelper.SuccessMessage(data: NewObj, message: "Refral Saved Successfully");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> UpdateRefral(RefralDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                if (Guid.TryParse(data.Id, out Guid RefralId))
                {
                    var currentObject = _context.Refrals.Where(a => a.Id == RefralId && a.CreatedBy == userId).FirstOrDefault();
                    if (currentObject != null)
                    {
                        if (Guid.TryParse(data.RefranceId, out Guid RefranceId))
                        {
                            currentObject.RefranceId = RefranceId;
                        }
                        currentObject.IsActive = data.IsActive;
                        currentObject.EditOn = DateTime.Now;
                        currentObject.EditBy = userId;
                        await _context.SaveChangesAsync();

                        return _responseHelper.SuccessMessage(data: currentObject, message: "Refral Updated Successfully");
                    }
                }
                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {data.Id}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> UpdateRefralByUser()
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var currentObjects = _context.Refrals.Where(a => a.RefranceId == userId && a.IsActive == true).ToList();
                if (currentObjects != null && currentObjects.Any())
                {
                    // Update all matching records
                    foreach (var currentObject in currentObjects)
                    {
                        currentObject.IsActive = false;
                        currentObject.EditOn = DateTime.Now;
                        currentObject.EditBy = userId;
                    }

                    // Save changes
                    await _context.SaveChangesAsync();

                    return _responseHelper.SuccessMessage(data: currentObjects, message: "Refral(s) Updated Successfully");
                }
                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {userId}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> DeleteRefral(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                if (Guid.TryParse(id, out Guid RefralId))
                {
                    var currentObject = _context.Refrals.Where(a => a.Id == RefralId && a.CreatedBy == userId).FirstOrDefault();
                    if (currentObject != null)
                    {
                        _context.Remove(currentObject);
                        _context.SaveChanges();
                        return _responseHelper.SuccessMessage(message: "Refral Deleted Successfully");
                    }
                }
                return _responseHelper.SuccessMessage(message: "No data found against this ID");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
    }
}
