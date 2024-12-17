using InternalWebsite.Core.Interfaces;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Repositories.Interfaces;
using InternalWebsite.Infrastructure.Data.Services;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace InternalWebsite.Infrastructure.Data.Ioc
{
    public static class NativeInjectorBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //Repository

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ClCongPrincipal, ClCongPrincipal>();

            // added new services
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IExceptionLogService, ExceptionLogService>();
            //services.AddScoped<IMainHeadingService, MainHeadingService>();
            //services.AddScoped<ITopicService, TopicService>();
            //services.AddScoped<IArticleService, ArticleService>();
            //services.AddScoped<IArticleExternalWebService, ArticleExternalWebService>();
            //services.AddScoped<ISubtopicService, SubtopicService>();
            //services.AddScoped<IRoleService, RoleService>();
            //services.AddScoped<IIconsService, IconsService>();
            //services.AddScoped<INotificationService, NotificationService>();

            //services.AddScoped<IIVideoHighlightService, VideoHighlightService>();
            //services.AddScoped<IVideoPodcastService, VideoPodcastService>();
            //services.AddScoped<IExternalLinkService, ExternalLinkService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<ISentenceService, SentenceService>();
            services.AddScoped<ITapPaymentServices, TapPaymentServices>();
            services.AddScoped<IRefralService, RefralService>();
            services.AddScoped<IIntegrationService, IntegrationService>();
            services.AddScoped<IMarketingService, MarketingService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<SmsInfo, SmsInfo>();
            services.AddScoped<FacebookAdFlowService, FacebookAdFlowService>();
            services.AddScoped<ITikTokService, TikTokService>();
            services.AddScoped<ISnapchatService, SnapchatService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAdminService, AdminService>();

        }
    }
}
