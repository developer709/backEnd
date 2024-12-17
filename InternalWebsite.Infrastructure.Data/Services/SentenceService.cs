using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Sentence = InternalWebsite.Core.Entities.Sentence;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class SentenceService : GenericRepository<Sentence, SentenceDto>, ISentenceService
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;

        public SentenceService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper,
            IConfiguration configuration) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _configuration = configuration;
        }
        //public async Task<AppResponse> GetSentence()
        //{
        //    try
        //    {

        //        var data = _context.Sentences.ToList();
        //        return new ResponseHelper(_configuration).SuccessMessage(data: data, message: "Data retrived successfully.");


        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseHelper(_configuration).ErrorMessage(ex);
        //    }
        //}
        public async Task<AppResponse> GetSentence(int page = 1, int per_page = 10, string sort_by = "Id", bool sort_desc = false, string filter = null)
        {
            try
            {
                // Fetch the data
                var query = _context.Sentences.AsQueryable();

                // Apply filtering if a filter is provided
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(s => s.Text.Contains(filter) || s.Type.Contains(filter) || s.Language.Contains(filter));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sort_by))
                {
                    query = sort_desc
                        ? query.OrderByDescending(e => EF.Property<object>(e, sort_by))
                        : query.OrderBy(e => EF.Property<object>(e, sort_by));
                }

                // Calculate total count for pagination metadata
                int totalRecords =await query.CountAsync();

                // Apply pagination
                var data =await query
                    .Skip((page - 1) * per_page)
                    .Take(per_page)
                    .ToListAsync();

                // Create a pagination metadata object
                var pagination = new
                {
                    current_page = page,
                    per_page = per_page,
                    total_records = totalRecords,
                    total_pages = (int)Math.Ceiling(totalRecords / (double)per_page)
                };

                // Return the paginated data and metadata
                return new ResponseHelper(_configuration).SuccessMessage(
                    data: new { sentences = data, pagination = pagination },
                    message: "Data retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

      
        public async Task<AppResponse> GetSentenceById(string id)
        {
            try
            {
                if (Guid.TryParse(id, out Guid SentenceId))
                {
                    var currentSentence = _context.Sentences.Where(a => a.Id == SentenceId).FirstOrDefault();
                    if (currentSentence != null)
                        return new ResponseHelper(_configuration).SuccessMessage(data: currentSentence, message: "Data retrived successfully.");
                }


                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {id}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> GetSentenceByType(string type)
        {
            try
            {
                var currentSentence = _context.Sentences.Where(a => a.Type.ToLower() == type.ToLower()).ToList();
                if (currentSentence != null)
                    return new ResponseHelper(_configuration).SuccessMessage(data: currentSentence, message: "Data retrived successfully.");


                return _responseHelper.SuccessMessage(message: $"No data found against this ID: {type}.");
            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> CreateSentence(SentenceDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    Sentence SentenceObj = new Sentence();
                    SentenceObj.Type = data.Type;
                    SentenceObj.Text = data.Text;
                    SentenceObj.Size = data.Size;
                    SentenceObj.Language = data.Language;
                    SentenceObj.IsActive = true;
                    SentenceObj.CreatedOn = DateTime.Now;
                    SentenceObj.CreatedBy = userId;

                    _context.Sentences.Add(SentenceObj);
                    _context.SaveChanges();

                    return _responseHelper.SuccessMessage(data: SentenceObj, message: "Sentence Saved Successfully");
                }
                return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");

            }
            catch (Exception ex)
            {
                return new ResponseHelper(_configuration).ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> UpdateSentence(SentenceDto data)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(data.Id, out Guid SentenceId))
                    {
                        var currentSentence = _context.Sentences.Where(a => a.Id == SentenceId && a.CreatedBy == userId).FirstOrDefault();
                        if (currentSentence != null)
                        {
                            currentSentence.Text = data.Text;
                            currentSentence.EditOn = DateTime.Now;
                            currentSentence.EditBy = userId;
                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(data: currentSentence, message: "Sentence Updated Successfully");
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
        public async Task<AppResponse> DeleteSentence(string id)
        {

            try
            {
                var userId = GetUserDetailId();
                if (userId != Guid.Empty)
                {
                    if (Guid.TryParse(id, out Guid SentenceId))
                    {
                        var currentSentence = _context.Sentences.Where(a => a.Id == SentenceId && a.CreatedBy == userId).FirstOrDefault();
                        if (currentSentence != null)
                        {
                            _context.Remove(currentSentence);
                            _context.SaveChanges();
                            return _responseHelper.SuccessMessage(message: "Sentence Deleted Successfully");
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
