using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface ISentenceService
    {
        Task<AppResponse> GetSentence(int page = 1, int per_page = 10, string sort_by = "Id", bool sort_desc = false, string filter = null);
        Task<AppResponse> GetSentenceById(string id);
        Task<AppResponse> GetSentenceByType(string type);
        Task<AppResponse> CreateSentence(SentenceDto data);
        Task<AppResponse> UpdateSentence(SentenceDto data);
        Task<AppResponse> DeleteSentence(string id);
    }
}
