using InternalWebsite.Application.ViewModels;
using InternalWebsite.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface ITikTokService
    {
        Task<AppResponse> GetTiktokAuthUrl();
        Task<AppResponse> GenerateTikTokToken(string code);
        Task<AppResponse> RefreshToken(string refreshToken);
        Task<AppResponse> CheckIfTikTokTokensAreExpired(long tokenExpiry, long refreshExpiry, string refreshToken);
    }
}
