using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface ISnapchatService
    {
        Task<AppResponse> GetSnapchat();
        Task<AppResponse> GetSnapchatByUser();
        Task<AppResponse> GetSnapchatById(string id);
        Task<AppResponse> UpdateSnapchat(SnapchatDto data);
        Task<AppResponse> CreateSnapchat(SnapchatDto data);
        Task<AppResponse> DeleteSnapchat(string id);
    }
}
