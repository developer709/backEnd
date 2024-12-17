using DevExpress.Xpo;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class ClCongPrincipal : ClaimsPrincipal
    {
        public ClCongPrincipal(ClaimsPrincipal principal) : base(principal)
        {
        }
        public ClCongPrincipal(IPrincipal principal) : base(principal)
        {
        }
        public static ClCongPrincipal Create(ClaimsPrincipal principal)
        {
            return new ClCongPrincipal(principal);
        }
        public int Id => Convert.ToInt32(FindFirstNullable("Id"));
        public string UserId => FindFirstNullable("UserId");
        public string UserName => FindFirstNullable("UserName");
        public string Email => FindFirstNullable("Email") ?? "Unknown";
        public int[] Seller
        {
            get
            {
                var claim = FindFirst("Seller");
                if (claim != null)
                {
                    return JsonConvert.DeserializeObject<int[]>(claim.Value);
                    //return JsonConvert.DeserializeObject<List<int>>(claim.Value);
                }
                return null;
            }
        }
        public int UserType => Convert.ToInt32(FindFirstNullable("UserType"));
        public int SellerType => Convert.ToInt32(FindFirstNullable("SellerType"));
        public int CityId => Convert.ToInt32(FindFirstNullable("CityId"));
        public int BrickId => Convert.ToInt32(FindFirstNullable("BrickId"));
        public int ChannelId => Convert.ToInt32(FindFirstNullable("ChannelId"));
        private string FindFirstNullable(string type)
        {
            var claim = FindFirst(type);
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }
    }
}