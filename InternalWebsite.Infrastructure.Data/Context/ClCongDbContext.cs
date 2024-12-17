using InternalWebsite.Core.Entities;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace InternalWebsite.Infrastructure.Data.Context
{
    public partial class ClCongDbContext : IdentityDbContext<tblUser, Core.Entities.IdentityRole, Guid>
    {
        public ClCongDbContext()
        {
        }

        public ClCongDbContext(DbContextOptions<ClCongDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<tblUser> ApplicationUsers { get; set; }
        public virtual DbSet<tblUserRole> UserRole { get; set; }
        public virtual DbSet<AppUserVerification> AppUserVerifications { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<TemplateField> TemplateFields { get; set; }
        public virtual DbSet<Sentence> Sentences { get; set; }
        //public virtual DbSet<ApiLinks> ApiLinkss { get; set; }
        //public virtual DbSet<ArticleExternalWeb> ArticleExternalWebs { get; set; }
        //public virtual DbSet<UserLocation> UserLocations { get; set; }
        //public virtual DbSet<Article> Articles { get; set; }
        //public virtual DbSet<VideoHighlight> VideoHighlights { get; set; }
        //public virtual DbSet<VideoPodcast> VideoPodcasts { get; set; }
        //public virtual DbSet<MainHeading> MainHeadings { get; set; }
        //public virtual DbSet<Payload> Payloads { get; set; }
        //public virtual DbSet<Region> Regions { get; set; }
        //public virtual DbSet<Icons> IconsList{ get; set; }
        //public virtual DbSet<Subtopic> Subtopics { get; set; }
        //public virtual DbSet<SubtopicHighlight> SubtopicHighlights { get; set; }
        //public virtual DbSet<Topic> Topics { get; set; }
        //public virtual DbSet<WriterAuth> WriterAuths { get; set; }
        //public virtual DbSet<Notification> Notifications { get; set; }
        //public virtual DbSet<ExternalLink> ExternalLinks { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<CompanyInfo> CompanyInfoes { get; set; }
        public virtual DbSet<Integration> Integrations { get; set; }
        public virtual DbSet<Marketing> Marketings { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<FacebookCampaign> FacebookCampaigns { get; set; }
        public virtual DbSet<CampaignBudget> CampaignBudgets { get; set; }
        public virtual DbSet<CampaignAudience> CampaignAudiences { get; set; }
        public virtual DbSet<CampaignContent> CampaignContents { get; set; }
        public virtual DbSet<CampaignContentCollection> CampaignContentCollections { get; set; }
        public virtual DbSet<TiktokIdentityContentCollection> TiktokIdentityContentCollections { get; set; }
        public virtual DbSet<Refral> Refrals { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}