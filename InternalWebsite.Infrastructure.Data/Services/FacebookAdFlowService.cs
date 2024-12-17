using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static InternalWebsite.Infrastructure.Data.Services.FacebookAdFlowService;
using static InternalWebsite.ViewModel.Models.FacebookAdDto;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class FacebookAdFlowService : GenericRepository<Core.Entities.IdentityRole, RoleDto>
    {
        private readonly HttpClient _httpClient;
        private readonly ResponseHelper _responseHelper;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Core.Entities.IdentityRole> _roleManager;
        private readonly ILogger<FacebookAdFlowService> _logger;

        public FacebookAdFlowService(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
           ClCongPrincipal clCongPrincipal,
           ILogger<FacebookAdFlowService> logger,
           ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> roleManager,
           IConfiguration configuration, HttpClient httpClient) : base(context,
         httpContextAccessor, clCongPrincipal

         )
        {
            _logger = logger;
            _roleManager = roleManager;
            _httpClient = httpClient;
            _responseHelper = responseHelper;
            _configuration = configuration;
        }

        //public static async Task Main(string[] args)
        //{
        //    var campaignId = await CreateCampaign();
        //    var adSetId = await CreateAdSet(campaignId);
        //    var imageHash = await AddImage();
        //    var adCreativeId = await CreateAdCreative(imageHash);
        //    await CreateAd(adSetId, adCreativeId);
        //}

        // Step 1: Create Campaign
        public async Task<string> CreateCampaign(Campaign campaign, FacebookCampaign facebookCampaign)
        {
            try
            {
                var accessToken = _configuration["Facebook:AccessToken"].ToString();
                var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
                var accountId = _configuration["Facebook:AccountId"].ToString();

                var campaignUrl = $"{baseUrl}/{accountId}/campaigns";
                if (facebookCampaign != null)
                {
                    campaignUrl = $"{baseUrl}/{accountId}/campaigns/" + facebookCampaign.FbCampaignId;
                }

                using (var client = new HttpClient())
                {
                    var payload = new
                    {
                        name = campaign.Title,
                        objective = "OUTCOME_AWARENESS",
                        //objective = campaign.Objective,
                        status = "PAUSED",
                        special_ad_categories = new JArray(),
                        access_token = accessToken
                    };
                    AppResponse appResponse = await SendHttpRequestAsync(HttpMethod.Post, campaignUrl, payload);

                    if (appResponse.SuccessFlag) // Assuming `AppResponse` has a Success property
                    {
                        string responseBody = appResponse.Data?.ToString();

                        if (!string.IsNullOrEmpty(responseBody))
                        {
                            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                            string campaignId = jsonResponse?.id;  // Assuming 'id' is the field for the campaign ID
                            _logger.LogInformation(responseBody.ToString());
                            return campaignId;
                        }
                    }
                    return "";

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public DateTimeResult CombineDateAndTime(DateTime? startDate, DateTime? startTime, DateTime? endDate, DateTime? endTime, bool isInstant = false, int NumberOfDay = 0)
        {
            var result = new DateTimeResult();
            if (isInstant)
            {
                // Instant case: Use current UTC time in ISO 8601 format
                DateTimeOffset startDateTimeWithUtc = DateTimeOffset.UtcNow;
                result.StartTime = startDateTimeWithUtc.ToString("O"); // "O" provides ISO 8601 format (yyyy-MM-ddTHH:mm:ss.fffffffK)

                DateTimeOffset endDateTimeWithUtc = startDateTimeWithUtc.AddDays(NumberOfDay);  // Adjust days as needed
                result.EndTime = endDateTimeWithUtc.ToString("O");
            }
            else if (startDate.HasValue && startTime.HasValue)
            {
                // Combine Start Date and Start Time
                DateTime startDateTime = startDate.Value.Date + startTime.Value.TimeOfDay;

                // Assuming UTC offset of -07:00
                TimeSpan offset = new TimeSpan(-7, 0, 0);
                DateTimeOffset startDateTimeWithOffset = new DateTimeOffset(startDateTime, offset);

                // Format start time into ISO 8601 string
                result.StartTime = startDateTimeWithOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");

                // Check if End Date and End Time are provided
                if (endDate.HasValue && endTime.HasValue)
                {
                    // Combine End Date and End Time
                    DateTime endDateTime = endDate.Value.Date + endTime.Value.TimeOfDay;
                    DateTimeOffset endDateTimeWithOffset = new DateTimeOffset(endDateTime, offset);

                    // Format end time into ISO 8601 string
                    result.EndTime = endDateTimeWithOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");
                }
                else
                {
                    DateTime startDateTime1 = startDate.Value.Date + startTime.Value.TimeOfDay;

                    // Extend by one day
                    DateTime extendedDateTime = startDateTime1.AddDays(1);

                    // Assuming UTC offset of -07:00
                    TimeSpan offset1 = new TimeSpan(-7, 0, 0);
                    DateTimeOffset extendedDateTimeWithOffset = new DateTimeOffset(extendedDateTime, offset1);

                    // Format the extended time into ISO 8601 string
                    result.ExtendTime = extendedDateTimeWithOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");
                }
            }

            return result;
        }


        // Step 2: Create Ad Set
        public async Task<string> CreateAdSet(string campaignId, CampaignBudget campaignBudget, Campaign campaign, FacebookCampaign facebookCampaign, CampaignAudience campaignAudience, CampaignContent CampaignContents)
        {
            var accessToken = _configuration["Facebook:AccessToken"].ToString();
            var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            var accountId = _configuration["Facebook:AccountId"].ToString();

            // Conversion rate (can be dynamic if needed via an API)

            var adSetUrl = $"{baseUrl}/{accountId}/adsets";
            if (facebookCampaign != null)
            {
                adSetUrl = $"{baseUrl}/{accountId}/adsets/" + facebookCampaign.AdSetId;
            }
            var customLocationsList = JsonConvert.DeserializeObject<List<CustomLocation>>(campaignAudience.City);
            var customLocations = new JArray(
                                            customLocationsList.Select(location => new JObject
                                            {
                                                { "latitude", location.Position.Lat },
                                                { "longitude", location.Position.Lng },
                                                { "radius", 15 },
                                                { "distance_unit", "mile" } // Adjust the unit as required (mile/km/meter)
                                            })
                                        );
            using (var client = new HttpClient())
            {
                // Convert budget from USD to SAR
                decimal usdToSarRate = 3.75m;

                // Calculate daily budget in SAR
                decimal dailyBudgetInUsd = Convert.ToDecimal(campaignBudget.Budget); // Budget per day in USD
                decimal dailyBudgetInSar = dailyBudgetInUsd * usdToSarRate; // Convert to SAR
                int dailyBudgetInHalalas = Convert.ToInt32(dailyBudgetInSar * 100); // Convert to halalas

                // Ensure daily budget meets Facebook's minimum requirement (SAR 3.78)
                int minimumDailyBudgetInHalalas = 378;
                if (dailyBudgetInHalalas < minimumDailyBudgetInHalalas)
                {
                    dailyBudgetInHalalas = minimumDailyBudgetInHalalas;
                }

                // Calculate bid amount per day (total budget / number of days)
                decimal totalBudgetInUsd = Convert.ToDecimal(campaignBudget.Budget) * campaignBudget.NumberOfDay; // Total campaign budget in USD
                decimal totalBudgetInSar = totalBudgetInUsd * usdToSarRate; // Convert to SAR
                int bidAmountInHalalas = Convert.ToInt32(totalBudgetInSar * 100 / campaignBudget.NumberOfDay); // Bid amount per day in halalas

                // Ensure the bid amount doesn't exceed the daily budget
                if (bidAmountInHalalas > dailyBudgetInHalalas)
                {
                    bidAmountInHalalas = dailyBudgetInHalalas; // Cap the bid amount
                }

                var dateTimeResult = new DateTimeResult();
                if (campaignBudget.ScheduleType == "Instant")
                {
                    var currentDate = DateTime.Now;
                    campaignBudget.StartDate = currentDate;
                    campaignBudget.StartTime = currentDate;
                    campaignBudget.EndDate = currentDate.AddMinutes(1).AddDays(campaignBudget.NumberOfDay);
                    campaignBudget.EndTime = currentDate.AddMinutes(1).AddDays(campaignBudget.NumberOfDay);
                    dateTimeResult = CombineDateAndTime(campaignBudget.StartDate, campaignBudget.StartTime, campaignBudget.EndDate, campaignBudget.EndTime, true, campaignBudget.NumberOfDay);
                }
                else
                    dateTimeResult = CombineDateAndTime(campaignBudget.StartDate, campaignBudget.StartTime, campaignBudget.EndDate, campaignBudget.EndTime);
                // Prepare the payload for Facebook API
                //var checkFbPosition = campaign.AdType == "Story" ? "story" : "feed";
                var payload = new
                {
                    name = campaign.Title,
                    campaign_id = campaignId,
                    daily_budget = dailyBudgetInHalalas,  // Daily budget in halalas
                    billing_event = "IMPRESSIONS",        // Billing per impression
                    optimization_goal = "IMPRESSIONS",    // Optimize for impressions
                    bid_amount = bidAmountInHalalas,      // Bid amount per day
                    targeting = new JObject
                    {
                        ["geo_locations"] = new JObject
                        {
                            ["custom_locations"] = customLocations   // Target countries
                        },
                        ["publisher_platforms"] = new JArray(   // Specify platforms
                        "facebook"
                    ),
                        ["facebook_positions"] = CampaignContents.MediaType == "Video" && CampaignContents.AdFormat == "Collection" ?  new JArray("feed") : new JArray("feed", "story"),
                        ["age_min"] = campaignAudience.Age == "Custom" ? campaignAudience.StartAge : 18,
                        ["age_max"] = campaignAudience.Age == "Custom" ? campaignAudience.EndAge : 65,
                        ["genders"] = campaignAudience.Gender == "Male" ? new JArray(1) : campaignAudience.Gender == "Female" ? new JArray(2) : new JArray(0),
                    },
                    start_time = dateTimeResult.StartTime,
                    end_time = dateTimeResult.EndTime ?? dateTimeResult.ExtendTime,
                    status = "PAUSED",
                    access_token = accessToken,
                };

                // Send the request
                AppResponse appResponse = await SendHttpRequestAsync(HttpMethod.Post, adSetUrl, payload);

                _logger.LogInformation(appResponse.ToString());

                if (appResponse.SuccessFlag) // Assuming `AppResponse` has a Success property
                {
                    string responseBody = appResponse.Data?.ToString();
                    _logger.LogInformation(responseBody.ToString());

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                        string adSetId = jsonResponse?.id;  // Assuming 'id' is the field for the campaign ID
                        return adSetId;
                    }
                }

                return "";
            }
        }

        // Step 3: Add Image and get hash
        public async Task<List<string>> AddImage(CampaignContent CampaignContents, FacebookCampaign facebookCampaign, List<CampaignContentCollection> campaignContentCollections)
        {
            var accessToken = _configuration["Facebook:AccessToken"].ToString();
            var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            var accountId = _configuration["Facebook:AccountId"].ToString();

            //if (facebookCampaign != null)
            //{
            //    adImageUrl = $"{baseUrl}/{accountId}/adimages/" + facebookCampaign.adim;
            //}

            using (var client = new HttpClient())
            {
                if (CampaignContents.MediaType == "Video")
                {
                    var videoIds = new List<string>();
                    var adVideoUrl = $"{baseUrl}/{accountId}/advideos";

                    foreach (var mediaPath in campaignContentCollections)
                    {
                        // Remove any leading or trailing whitespace from the video path
                        var trimmedMediaPath = mediaPath.Image; // Assuming the video path is stored in `Video`

                        // Ensure the correct file path is used for each video
                        var videoName = trimmedMediaPath.Replace("Resources/Videos/", "");
                        var videoUrl = Path.Combine(Directory.GetCurrentDirectory(), trimmedMediaPath);
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StringContent(accessToken), "access_token");

                            // Check if the video file exists before uploading
                            if (File.Exists(videoUrl))
                            {
                                content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(videoUrl)), "source", videoName);

                                var response = await client.PostAsync(adVideoUrl, content); // `adVideoUrl` is the video endpoint
                                var resultContent = await response.Content.ReadAsStringAsync();
                                var result = JObject.Parse(resultContent);

                                // Extract the video ID for the uploaded video
                                string videoId = result["id"]?.ToString();

                                if (!string.IsNullOrEmpty(videoId))
                                {
                                    Console.WriteLine($"Video uploaded with ID: {videoId}");
                                    videoIds.Add(videoId); // Add the video ID to the list
                                }
                                else
                                {
                                    Console.WriteLine("Failed to upload video. No video ID returned.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"File not found: {videoUrl}");
                            }
                        }
                        var adImageUrl = $"{baseUrl}/{accountId}/adimages";
                        string imageHash = await GetImageHashAsync(mediaPath.ImageName, accessToken, client, adImageUrl);
                        if (!string.IsNullOrEmpty(imageHash))
                            videoIds.Add(imageHash);
                    }
                    return videoIds;

                }
                else
                {
                    var adImageUrl = $"{baseUrl}/{accountId}/adimages";

                    // Split the media string by comma, handle single or multiple images
                    var mediaPaths = CampaignContents.Media.Split(',');

                    // Initialize a list to store the image hashes
                    var imageHashes = new List<string>();

                    foreach (var mediaPath in campaignContentCollections)
                    {
                        // Remove any leading or trailing whitespace from each media path
                        string imageHash = await GetImageHashAsync(mediaPath.Image, accessToken, client, adImageUrl);
                        if (!string.IsNullOrEmpty(imageHash))
                            imageHashes.Add(imageHash);
                    }

                    // Optionally, return or further process the image hashes
                    return imageHashes;  // Return the hashes as a comma-separated string if needed
                }

            }
        }

        public async Task<string> GetImageHashAsync(string Image, string accessToken, HttpClient client, string adImageUrl)
        {
            var trimmedMediaPath = Image;

            // Ensure the correct file path is used for each image
            var imageName = trimmedMediaPath.Replace("Resources/Images/", "");
            var imageUrl = Path.Combine(Directory.GetCurrentDirectory(), trimmedMediaPath);

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(accessToken), "access_token");

                // Check if file exists before uploading
                if (File.Exists(imageUrl))
                {
                    content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(imageUrl)), "filename", imageName);

                    var response = await client.PostAsync(adImageUrl, content);
                    var resultContent = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(resultContent);

                    // Extract the hash for the uploaded image
                    string imageHash = result["images"][imageName]?["hash"]?.ToString();

                    if (!string.IsNullOrEmpty(imageHash))
                    {
                        return imageHash;
                        // Add the image hash to the list
                    }
                }
                else
                {
                    return "";
                }
            }
            return "";
        }

        // Step 4: Create Ad Creative
        public async Task<string> CreateAdCreative(Campaign campaign, FacebookCampaign facebookCampaign, List<CampaignContentCollection> campaignContentList, CampaignContent currentCampaignContents)
        {
            var accessToken = _configuration["Facebook:AccessToken"].ToString();
            var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            var accountId = _configuration["Facebook:AccountId"].ToString();
            //129212616937356
            var pageData = JObject.Parse(currentCampaignContents.PageName);
            var pageId = pageData["id"].ToString(); // Extract the page id
            //var pageId = "129212616937356"; // Extract the page id

            using (var client = new HttpClient())
            {
                var adCreativeUrl = $"{baseUrl}/{accountId}/adcreatives";
                if (facebookCampaign != null)
                {
                    adCreativeUrl = $"{baseUrl}/{accountId}/adcreatives/" + facebookCampaign.AdCreativeId;
                }

                var carouselCards = new JArray();
                var payload = new JObject { };

                if (currentCampaignContents.AdFormat == "Collection")
                {
                    carouselCards = new JArray(); // This will store all carousel items

                    // Loop through the content list to create each carousel card
                    foreach (var content in campaignContentList)
                    {
                        var card = currentCampaignContents.MediaType == "Video"
                            ? new JObject
                            {
                { "video_id", content.ImageHash }, // Each video_id here represents a separate video
                { "message", content.Description },
                { "image_hash", content.Heading ?? "548a9005670504a756c69d50cf52541c" }
                            }
                            : new JObject
                            {
                { "image_hash", content.ImageHash },
                { "link", content.WebUrl },
                { "message", content.Description },
                { "description", content.Description },
                { "name", content.Heading },
                { "call_to_action", new JObject
                    {
                        { "type", currentCampaignContents.CTA ?? "LEARN_MORE" },
                        { "value", new JObject
                            {
                                { "link", content.WebUrl }
                            }
                        }
                    }
                }
                            };

                        carouselCards.Add(card); // Add each card to the carousel
                    }

                    var webUrl = !string.IsNullOrEmpty(currentCampaignContents.WebUrl) ? currentCampaignContents.WebUrl : "https://www.google.com";

                    // For video ads
                    var mainData = new JObject
    {
        { "message", "Carousel ad message" },
        { "child_attachments", carouselCards } // This will be a carousel of video cards
    };

                    // Build the payload dynamically based on ad type (video or image)
                    payload = new JObject
    {
        { "name", campaign.Title }, // Campaign title
        { "object_story_spec", new JObject
            {
                { "page_id", pageId },
                { "link_data", mainData } // For carousel, use link_data, even for videos
            }
        },
        { "degrees_of_freedom_spec", new JObject
            {
                { "creative_features_spec", new JObject
                    {
                        { "standard_enhancements", new JObject { { "enroll_status", "OPT_OUT" } } }
                    }
                }
            }
        },
        { "access_token", accessToken }
    };
                }


                else
                {
                    var webUrl = "";

                    if (campaignContentList.Count > 0 && !string.IsNullOrEmpty(campaignContentList[0].WebUrl))
                    {
                        webUrl = campaignContentList[0].WebUrl;
                    }
                    else
                    {
                        webUrl = !string.IsNullOrEmpty(currentCampaignContents.WebUrl) ? currentCampaignContents.WebUrl : "https://www.google.com";
                    }
                    var type = string.IsNullOrEmpty(currentCampaignContents.CTA) ? "LEARN_MORE" : currentCampaignContents.CTA;

                    var mediaData = currentCampaignContents.MediaType == "Video"
                                    ? new JObject
                                    {
                                        { "video_id", campaignContentList.Count > 0 ? campaignContentList[0].ImageHash : "" }, // Use video_id here
                                        { "message", currentCampaignContents.Caption ?? (campaignContentList.Count > 0 ? campaignContentList[0].Description : "") },
                                         { "image_hash", campaignContentList.Count > 0 ? campaignContentList[0].Heading : "548a9005670504a756c69d50cf52541c"  }
                                    }
                                    : new JObject
                                    {
                                        { "image_hash", campaignContentList.Count > 0 ? campaignContentList[0].ImageHash : "" }, // Use image_hash here
                                        { "link", webUrl },
                                        { "description", campaignContentList.Count > 0 ? campaignContentList[0].Description : "" },
                                        { "name", campaignContentList.Count > 0 ? campaignContentList[0].Heading : "" },
                                        { "call_to_action", new JObject
                                            {
                                                { "type", type },
                                                { "value", new JObject { { "link", webUrl } } }
                                            }
                                        }
                                    };

                    payload = new JObject
                                    {
                                        { "name", campaign.Title },
                                        { "object_story_spec", new JObject
                                            {
                                                { "page_id", pageId },
                                                { currentCampaignContents.MediaType == "Video" ? "video_data" : "link_data", mediaData } // Use video_data or link_data
                                            }
                                        },
                                        { "degrees_of_freedom_spec", new JObject
                                            {
                                                { "creative_features_spec", new JObject
                                                    {
                                                        { "standard_enhancements", new JObject { { "enroll_status", "OPT_OUT" } } }
                                                    }
                                                }
                                            }
                                        },
                                        { "access_token", accessToken }
                                    };

                }


                var response = await PostRequest(client, adCreativeUrl, payload);
                _logger.LogInformation(response.ToString());
                string adCreativeId = response["id"].ToString();
                Console.WriteLine($"Ad Creative created with ID: {adCreativeId}");
                return adCreativeId;
            }
            //var accessToken = _configuration["Facebook:AccessToken"].ToString();
            //var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            //var accountId = _configuration["Facebook:AccountId"].ToString();

            //using (var client = new HttpClient())
            //{
            //    var adCreativeUrl = $"{baseUrl}/{accountId}/adcreatives";
            //    var payload = new JObject
            //    {
            //        { "name", campaign.Title },
            //        { "object_story_spec", new JObject
            //            {
            //                { "page_id", "129212616937356" },
            //                { "link_data", new JObject
            //                    {
            //                        { "description", "" },
            //                        { "image_hash", "imageHash" },
            //                        { "link", "www.google.com" },
            //                        { "message", "message" },
            //                        { "name", "headline" },
            //                        { "call_to_action", new JObject
            //                            {
            //                                { "type", "LEARN_MORE" },
            //                                { "value", new JObject { { "link", "www.google.com" } } }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        },
            //        { "degrees_of_freedom_spec", new JObject
            //            {
            //                { "creative_features_spec", new JObject
            //                    {
            //                        { "standard_enhancements", new JObject { { "enroll_status", "OPT_OUT" } } }
            //                    }
            //                }
            //            }
            //        },
            //        { "access_token", accessToken }
            //    };

            //    var response = await PostRequest(client, adCreativeUrl, payload);
            //    string adCreativeId = response["id"].ToString();
            //    Console.WriteLine($"Ad Creative created with ID: {adCreativeId}");
            //    return adCreativeId;
            //}

            //var accessToken = _configuration["Facebook:AccessToken"].ToString();
            //var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            //var accountId = _configuration["Facebook:AccountId"].ToString();

            //var adCreativeUrl = $"{baseUrl}/{accountId}/adcreatives";
            //if (facebookCampaign != null)
            //{
            //    adCreativeUrl = $"{baseUrl}/{accountId}/adcreatives/" + facebookCampaign.AdCreativeId;
            //}

            //// Initialize the JArray for child attachments
            //var contentAttachmentsArray = new JArray();

            //foreach (var attachment in campaignContentCollections)
            //{
            //    // Create a new JObject for each child attachment
            //    //{ "link", attachment.WebUrl },
            //    var childAttachment = new JObject
            //        {
            //            { "link", "https://www.example.com/product1" },
            //            { "image_hash", attachment.ImageHash },
            //            { "name", attachment.Image.Replace("Resources/Images/", "")    },
            //            { "description", attachment.Description },
            //            { "call_to_action", new JObject
            //                {
            //                    { "type", "SHOP_NOW" }
            //                }
            //            }
            //        };

            //    // Add the dynamically created JObject to the childAttachmentsArray
            //    contentAttachmentsArray.Add(childAttachment);
            //}
            ////{ "link", currentCampaignContents.WebUrl },
            //var linkData = new JObject
            //{
            //    { "message",currentCampaignContents.Headline  },
            //    { "link", "https://www.example.com" },
            //    { "child_attachments", contentAttachmentsArray },
            //};
            //// Conditionally add "multi_share_optimized" if contentAttachmentsArray count > 1
            //if (contentAttachmentsArray.Count > 1)
            //{
            //    linkData.Add("multi_share_optimized", true);
            //}

            //// Parse the currentCampaignContents.PageName JSON to extract the "id"
            //var pageData = JObject.Parse(currentCampaignContents.PageName);
            //var pageId = pageData["id"].ToString(); // Extract the page id

            //using (var client = new HttpClient())
            //{
            //    var payload = new JObject
            //{
            //    { "name", campaign.Title },
            //    { "object_story_spec", new JObject
            //        {
            //            { "page_id", "129212616937356" },
            //            { "link_data", linkData }
            //        }
            //    },
            //    { "degrees_of_freedom_spec", new JObject
            //        {
            //            { "creative_features_spec", new JObject
            //                {
            //                    { "standard_enhancements", new JObject { { "enroll_status", "OPT_OUT" } } }
            //                }
            //            }
            //        }
            //    },
            //    { "access_token", accessToken }
            //};

            //    var response = await PostRequest(client, adCreativeUrl, payload);
            //    string adCreativeId = response["id"].ToString();
            //    Console.WriteLine($"Ad Creative created with ID: {adCreativeId}");
            //    return adCreativeId;
            //}
        }

        // Step 5: Create Ad
        public async Task<string> CreateAd(string adSetId, string adCreativeId, Campaign campaign, FacebookCampaign facebookCampaign)
        {
            var accessToken = _configuration["Facebook:AccessToken"].ToString();
            var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            var accountId = _configuration["Facebook:AccountId"].ToString();

            var adUrl = $"{baseUrl}/{accountId}/ads";
            if (facebookCampaign != null)
            {
                adUrl = $"{baseUrl}/{accountId}/ads/" + facebookCampaign.AdId;

            }
            using (var client = new HttpClient())
            {
                var payload = new JObject
            {
                { "name", campaign.Title },
                { "adset_id", adSetId },
                { "creative", new JObject { { "creative_id", adCreativeId } } },
                { "status", "PAUSED" },
                { "access_token", accessToken }
            };

                var response = await PostRequest(client, adUrl, payload);
                _logger.LogInformation(response.ToString());
                string adId = response["id"].ToString();
                Console.WriteLine($"Ad created with ID: {adId}");
                return adId;
            }
        }

        // Helper method for making POST requests and parsing JSON responses
        public static async Task<JObject> PostRequest(HttpClient client, string url, object payload)
        {
            var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var resultContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(resultContent);
        }
        private async Task<AppResponse> SendHttpRequestAsync(HttpMethod method, string uri, object requestBody = null)
        {
            try
            {

                var requestMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(uri),
                };

                if (requestBody != null)
                {
                    // Serialize the requestBody while ignoring null values
                    var jsonContent = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                using (var response = await _httpClient.SendAsync(requestMessage))
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return _responseHelper.ErrorMessage(message: !string.IsNullOrEmpty(responseBody)
                            ? responseBody
                            : "Request failed with no response body.");
                    }

                    if (string.IsNullOrEmpty(responseBody))
                    {
                        return _responseHelper.ErrorMessage(message: "Response body is empty.");
                    }

                    return _responseHelper.SuccessMessage(data: responseBody, message: "Data retrieved successfully.");
                }
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> FacbookCampaignStatus(CampaignStatusDto data)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }
                if (Guid.TryParse(data.CampaignId, out Guid campaignId))
                {
                    var getIds = _context.FacebookCampaigns.Where(x => x.CampaignId == campaignId).FirstOrDefault();
                    var accessToken = _configuration["Facebook:AccessToken"].ToString();
                    var requestBody = new
                    {
                        status = data.Status,
                    };
                    using (var client = new HttpClient())
                    {
                        var campaignIdResponse = await PostRequest(client, $"https://graph.facebook.com/v20.0/{getIds.FbCampaignId}?access_token={accessToken}", requestBody);
                        var adSetIdResponse = await PostRequest(client, $"https://graph.facebook.com/v20.0/{getIds.AdSetId}?access_token={accessToken}", requestBody);
                        var adCreativeIdResponse = await PostRequest(client, $"https://graph.facebook.com/v20.0/{getIds.AdCreativeId}?access_token={accessToken}", requestBody);

                        getIds.Status = data.Status;
                        _context.SaveChanges();
                        return _responseHelper.SuccessMessage(data: getIds.Status, message: "Data retrieved successfully.");

                    }

                }
                return _responseHelper.ErrorMessage(message: $"Campaign Id not correct: {data.CampaignId}.");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }

        }
        public async Task<RootObject> PendingPages()
        {
            var accessToken = _configuration["Facebook:AccessToken"].ToString();
            var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            var accountId = _configuration["Facebook:PageId"].ToString();

            var pageUrl = $"https://graph.facebook.com/v21.0/{accountId}/pending_client_pages?access_token={accessToken}"; // Business ID of Balke Tech

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await _httpClient.GetAsync(pageUrl);

                    if (response.IsSuccessStatusCode)
                    {

                        var resultContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<RootObject>(resultContent);
                        return result;

                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return null;

                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> ClaimPageAsync(string pageId)
        {
            var accessToken = _configuration["Facebook:AccessToken"].ToString();
            var baseUrl = _configuration["Facebook:BaseUrl"].ToString();
            var accountId = _configuration["Facebook:PageId"].ToString();


            var claimPageUrl = $"https://graph.facebook.com/v20.0/{accountId}/client_pages"; // Business ID of Balke Tech

            var claimPageData = new
            {
                page_id = pageId, // User's Page ID
                permitted_tasks = new[] { "ADVERTISE", "ANALYZE", "CREATE_CONTENT", "MODERATE" },
                access_token = accessToken
            };
            _logger.LogInformation("---------------------------------- Claim Page Async Check ----------------------------------");
            _logger.LogInformation(pageId.ToString());
            var content = new StringContent(JsonConvert.SerializeObject(claimPageData), Encoding.UTF8, "application/json");

            _logger.LogInformation("---------------------------------- Claim Page Async Content ----------------------------------");
            string responseString1 = JsonConvert.SerializeObject(content, Formatting.Indented);
            _logger.LogInformation(responseString1.ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    //var campaignIdResponse = await PostRequest(client, claimPageUrl, content);
                    var response = await _httpClient.PostAsync(claimPageUrl, content);
                    _logger.LogInformation("---------------------------------- Claim Page Async response ----------------------------------");
                    string responseString2 = JsonConvert.SerializeObject(response, Formatting.Indented);
                    _logger.LogInformation(responseString2.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        // Success response handling
                        Console.WriteLine("Page successfully added to Balke Tech business.");
                        return true;
                    }
                    else
                    {
                        // Log the error response
                        _logger.LogInformation("---------------------------------- Claim Pages ----------------------------------");
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation(errorResponse.ToString());
                        //1752041 duplicate request. already send this request.
                        //1752044 Your business already has access to the object -- already access this page 1690061
                        // "1690061":"Asset already belongs to this Business Manager."
                        if (errorResponse.Contains("\"error_subcode\":1752044") || errorResponse.Contains("\"error_subcode\":1752041") || errorResponse.Contains("\"error_subcode\":1690061"))
                        {
                            Console.WriteLine("Business already has access to the page.");
                            return true; // Return true since the page is already claimed
                        }
                        Console.WriteLine($"Error claiming page: {errorResponse}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("---------------------------------- Claim Pages ----------------------------------");
                _logger.LogInformation(ex.Message.ToString());
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }
        public string ProcessAndSaveDataAsync(string pageId)
        {
            // Deserialize JSON into C# objects
            var root = JsonConvert.DeserializeObject<RootObject>(pageId);

            // Loop through the pages and extract required data
            var remainingPages = root.Data.Where(a => a.Id != pageId).ToList(); ;
            var newRoot = new
            {
                Data = remainingPages,
                root.Paging
            };

            // Serialize the new object back to a JSON string
            string remainingPagesJson = JsonConvert.SerializeObject(newRoot, Formatting.Indented);

            return remainingPagesJson;

        }
    }
}
