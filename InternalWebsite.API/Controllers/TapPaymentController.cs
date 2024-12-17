using InternalWebsite.API.Extensions;
using InternalWebsite.Application.Utils;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace InternalWebsite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TapPaymentController : ClCongController
    {
        private readonly ITapPaymentServices _tapPayment;
        public TapPaymentController(ILogger<AccountController> logger,
         UserManager<tblUser> userManager,
         IConfiguration config, ITapPaymentServices tapPayment) : base(logger, config, userManager)
        {
            _tapPayment = tapPayment;
        }

        [HttpGet]
        [Authorize]
        [Route("GetWallet")]
        public async Task<IActionResult> GetWallet()
        {
            try
            {
                return Ok(await _tapPayment.GetWallet());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }

        }

        [HttpGet]
        [Authorize]
        [Route("GetReferEarn")]
        public async Task<IActionResult> GetReferEarn()
        {
            try
            {
                return Ok(await _tapPayment.GetReferEarn());
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }

        }

        [HttpGet]
        [Authorize]
        [Route("RetrieveAuthorize")]
        public async Task<IActionResult> RetrieveAuthorize(string id)
        {
            try
            {
                return Ok(await _tapPayment.RetrieveAuthorize(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }

        }

        [HttpPost]
        [Authorize]
        [Route("CreateAuthorize")]
        public async Task<IActionResult> CreateAuthorize(PaymentRequestDto paymentRequest)
        {
            try
            {
                return Ok(await _tapPayment.CreateAuthorize(paymentRequest));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("CreateCharge")]
        public async Task<IActionResult> CreateCharge(PaymentRequestDto paymentRequest)
        {
            try
            {
                return Ok(await _tapPayment.CreateCharge(paymentRequest));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpGet]
        [Authorize]
        [Route("RetrieveCharge")]
        public async Task<IActionResult> RetrieveCharge(string id)
        {
            try
            {
                return Ok(await _tapPayment.RetrieveCharge(id));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetChargeList")]
        public async Task<IActionResult> RetrieveChargeList(ChargeDto requestList)
        {
            try
            {
                return Ok(await _tapPayment.RetrieveChargeList(requestList));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("DownloadCharge")]
        public async Task<IActionResult> DownloadCharge(ChargeDto requestList)
        {
            try
            {
                return Ok(await _tapPayment.DownloadCharge(requestList));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("CreateRefund")]
        public async Task<IActionResult> CreateRefund(RefundDto requestData)
        {
            try
            {
                return Ok(await _tapPayment.CreateRefund(requestData));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseHelper(Config).ErrorMessage(ex));
            }
        }

        //[HttpPost("CreateTokenCard")]
        //public async Task<IActionResult> CreateTokenCard([FromBody] CardTokenDto request)
        //{
        //    var cardDetails = new
        //    {
        //        number = request.CardNumber.ToString(),
        //        exp_month = request.ExpirationMonth.ToString(),
        //        exp_year = request.ExpirationYear.ToString(),
        //        cvc = request.Cvc.ToString(),
        //        name = request.CardHolderName
        //    };

        //    // Add address if provided
        //    var requestBody = new
        //    {
        //        card = cardDetails,
        //        address = !string.IsNullOrEmpty(request.Country) &&
        //          !string.IsNullOrEmpty(request.AddressLine1) &&
        //          !string.IsNullOrEmpty(request.AddressCity) ?
        //          new
        //          {
        //              country = request.Country,
        //              line1 = request.AddressLine1,
        //              city = request.AddressCity,
        //              street = request.AddressStreet,
        //              avenue = request.AddressAvenue
        //          } : null,
        //        client_ip = !string.IsNullOrEmpty(request.ClientIp) ? request.ClientIp : null
        //    };

        //    var json = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    });

        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var requestMessage = new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Post,
        //        RequestUri = new Uri("https://api.tap.company/v2/tokens"),
        //        Headers =
        //        {
        //            { "accept", "application/json" },
        //            { "Authorization", $"Bearer {secretKey}" },
        //        },
        //        Content = content
        //    };

        //    using (var response = await _httpClient.SendAsync(requestMessage))
        //    {
        //        response.EnsureSuccessStatusCode();
        //        var responseBody = await response.Content.ReadAsStringAsync();
        //        return Ok(responseBody);
        //    }
        //}
    }
}
