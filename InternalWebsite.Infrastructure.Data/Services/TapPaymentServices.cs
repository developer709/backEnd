using InternalWebsite.Application.Utils;
using InternalWebsite.Application.ViewModels;
using InternalWebsite.Core.Entities;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Repositories;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using InternalWebsite.ViewModel.Enum;
using InternalWebsite.ViewModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class TapPaymentServices : GenericRepository<Core.Entities.IdentityRole, RoleDto>, ITapPaymentServices
    {
        private readonly IConfiguration _configuration;
        private readonly ResponseHelper _responseHelper;
        private readonly RoleManager<Core.Entities.IdentityRole> _roleManager;
        private readonly HttpClient _httpClient;

        public TapPaymentServices(ClCongDbContext context, IHttpContextAccessor httpContextAccessor,
            ClCongPrincipal clCongPrincipal,
            ResponseHelper responseHelper, RoleManager<Core.Entities.IdentityRole> roleManager,
            IConfiguration configuration, HttpClient httpClient) : base(context,
          httpContextAccessor, clCongPrincipal

          )
        {
            _responseHelper = responseHelper;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["TapPay:Secret_Key"].ToString());
        }

        private async Task<AppResponse> SendHttpRequestAsync(HttpMethod method, string uri, object requestBody = null)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

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

        public async Task<AppResponse> GetWallet()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                Wallet walletData = await _context.Wallets.Include(t => t.Transactions.Where(tx => tx.Status == TransactionStatus.Captured)).Where(a => a.UserId == userId).FirstOrDefaultAsync();

                return new ResponseHelper(_configuration).SuccessMessage(data: walletData, message: "Data retrived successfully.");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }
        public async Task<AppResponse> GetReferEarn()
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                Wallet walletData = await _context.Wallets.Include(t => t.Transactions.Where(tx => tx.TransactionType == TransactionType.Earned)).Where(a => a.UserId == userId).FirstOrDefaultAsync();

                return new ResponseHelper(_configuration).SuccessMessage(data: walletData, message: "Data retrived successfully.");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> CreateAuthorize(PaymentRequestDto request)
        {
            try
            {
                if (request.Amount == 0)
                {
                    return _responseHelper.ErrorMessage(message: "Amount should not be null or 0.");
                }
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                var userData = _context.ApplicationUsers.Where(a => a.Id == userId).FirstOrDefault();
                var requestBody = new
                {
                    amount = request.Amount,
                    currency = "USD",
                    customer_initiated = true,
                    threeDSecure = true,
                    save_card = request.SaveCard,
                    statement_descriptor = request.StatementDescriptor,
                    reference = new { transaction = GenerateUniqueId("txn"), order = GenerateUniqueId("ord") },
                    receipt = new { email = true, sms = true },
                    customer = new { id = "", first_name = userData.FirstName, middle_name = "", last_name = userData.LastName, email = userData.Email, phone = new { country_code = userData.PhoneNumber, number = userData.PhoneNumber } },
                    merchant = new { id = _configuration["TapPay:Marchant_Id"].ToString() },
                    source = new { id = "src_card" },
                    authorize_debit = false,
                    auto = new { type = "VOID", time = 100 },
                    post = new { url = request.PostUrl },
                    redirect = new { url = request.RedirectUrl }

                };

                return await SendHttpRequestAsync(HttpMethod.Post, "https://api.tap.company/v2/authorize/", requestBody);
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> RetrieveAuthorize(string authorizationId)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                if (string.IsNullOrEmpty(authorizationId))
                {
                    return _responseHelper.ErrorMessage(message: "Authorization Id could not be null or empty");
                }

                var responseBody = await SendHttpRequestAsync(HttpMethod.Get, $"https://api.tap.company/v2/authorize/{authorizationId}");

                if (!responseBody.SuccessFlag)
                {
                    return responseBody;
                }

                var jsonResponse = JsonDocument.Parse(responseBody.Data.ToString());

                string transactionId = jsonResponse.RootElement.GetProperty("id").GetString();
                if (_context.Transactions.Any(a => a.VerfiyId == transactionId))
                {
                    return _responseHelper.ErrorMessage(message: "Transaction already added. please make a new transaction");
                }

                string transactionStatus = jsonResponse.RootElement.GetProperty("status").GetString().ToUpper();
                string cardStatus = jsonResponse.RootElement.GetProperty("card_security").GetProperty("message").GetString().ToUpper();

                var transactionObj = CreateTransactionFromJsonResponse(jsonResponse, userId);
                Wallet walletObj;
                if (_context.Wallets.Any(a => a.UserId == userId))
                {
                    walletObj = _context.Wallets.First(a => a.UserId == userId);
                    if (transactionStatus == "AUTHORIZED" && cardStatus == "MATCH")
                    {
                        walletObj.Balance += transactionObj.Amount;
                        walletObj.EditOn = DateTime.Now;
                        walletObj.EditBy = userId;
                    }
                }
                else
                {
                    walletObj = CreateUpdateWalletFromJsonResponse(transactionStatus == "AUTHORIZED" && cardStatus == "MATCH" ? transactionObj.Amount : 0, transactionObj.Currency, userId);
                    _context.Wallets.Add(walletObj);
                }
                _context.SaveChanges();

                transactionObj.WalletId = walletObj.Id;
                _context.Transactions.Add(transactionObj);
                _context.SaveChanges();

                if (transactionStatus != "AUTHORIZED" || cardStatus != "MATCH")
                {
                    return _responseHelper.ErrorMessageWithData(data: responseBody, message: "Transaction Faild. Please try again latter");
                }
                Wallet walletData = await _context.Wallets.Include(t => t.Transactions.Where(tx => tx.Status == TransactionStatus.Captured)).Where(a => a.UserId == userId).FirstOrDefaultAsync();

                //var settings = new JsonSerializerSettings
                //{
                //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                //};
                //string walletObjJson = JsonConvert.SerializeObject(walletObj, settings);

                return _responseHelper.SuccessMessage(data: walletData, message: "Transaction Approved Successfull");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> CreateCharge(PaymentRequestDto request)
        {
            try
            {
                if (request.Amount == 0)
                {
                    return _responseHelper.ErrorMessage(message: "Amount should not be null or 0.");
                }
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return new ResponseHelper(_configuration).ErrorMessage(message: "userId is not Correct");
                }

                var userData = _context.ApplicationUsers.Where(a => a.Id == userId).FirstOrDefault();
                var requestBody = new
                {
                    amount = request.Amount,
                    currency = "USD",
                    customer_initiated = true,
                    threeDSecure = true,
                    save_card = request.SaveCard,
                    statement_descriptor = request.StatementDescriptor,
                    description = request.StatementDescriptor,
                    reference = new { transaction = GenerateUniqueId("txn"), order = GenerateUniqueId("ord") },
                    receipt = new { email = true, sms = true },
                    customer = new { id = "", first_name = userData.FirstName, middle_name = "", last_name = userData.LastName, email = userData.Email, phone = new { country_code = userData.PhoneNumber, number = userData.PhoneNumber } },
                    merchant = new { id = _configuration["TapPay:Marchant_Id"].ToString() },
                    source = new { id = "src_card" },
                    post = new { url = request.PostUrl },
                    redirect = new { url = request.RedirectUrl }

                };

                return await SendHttpRequestAsync(HttpMethod.Post, "https://api.tap.company/v2/charges/", requestBody);
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> RetrieveCharge(string chargeId)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                if (string.IsNullOrEmpty(chargeId))
                {
                    return _responseHelper.ErrorMessage(message: "Charge Id could not be null or empty");
                }

                var responseBody = await SendHttpRequestAsync(HttpMethod.Get, $"https://api.tap.company/v2/charges/{chargeId}");

                if (!responseBody.SuccessFlag)
                {
                    return responseBody;
                }

                var jsonResponse = JsonDocument.Parse(responseBody.Data.ToString());

                string transactionId = jsonResponse.RootElement.GetProperty("id").GetString();
                if (_context.Transactions.Any(a => a.VerfiyId == transactionId))
                {
                    return _responseHelper.ErrorMessage(message: "Transaction already added. please make a new transaction");
                }

                string transactionStatus = jsonResponse.RootElement.GetProperty("status").GetString().ToUpper();
                //string cardSecurity = jsonResponse.RootElement.GetProperty("card_security").GetString()?.ToUpper();
                //string cardStatus = "";

                //if (!string.IsNullOrEmpty(cardSecurity))
                //{
                //    cardStatus = jsonResponse.RootElement.GetProperty("card_security").GetProperty("message").GetString()?.ToUpper();
                //}

                var transactionObj = CreateTransactionFromJsonResponse(jsonResponse, userId);
                Wallet walletObj;
                if (_context.Wallets.Any(a => a.UserId == userId))
                {
                    walletObj = _context.Wallets.First(a => a.UserId == userId);
                    if (transactionStatus == "CAPTURED")
                    {
                        walletObj.Balance += transactionObj.Amount;
                        walletObj.EditOn = DateTime.Now;
                        walletObj.EditBy = userId;
                    }
                }
                else
                {
                    walletObj = CreateUpdateWalletFromJsonResponse(transactionStatus == "CAPTURED" ? transactionObj.Amount : 0, transactionObj.Currency, userId);
                    _context.Wallets.Add(walletObj);
                }
                _context.SaveChanges();

                transactionObj.WalletId = walletObj.Id;
                _context.Transactions.Add(transactionObj);
                _context.SaveChanges();

                if (transactionStatus != "CAPTURED")
                {
                    return _responseHelper.ErrorMessageWithData(data: responseBody.Data, message: "Transaction Faild. Please try again latter");
                }
                Wallet walletData = await _context.Wallets.Include(t => t.Transactions.Where(tx => tx.Status == TransactionStatus.Captured)).Where(a => a.UserId == userId).FirstOrDefaultAsync();

                //var settings = new JsonSerializerSettings
                //{
                //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                //};
                //string walletObjJson = JsonConvert.SerializeObject(walletObj, settings);

                return _responseHelper.SuccessMessage(data: walletData, message: "Transaction Approved Successfull");

            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> RetrieveChargeList(ChargeDto request)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                var requestBody = new
                {
                    period = new
                    {
                        date = new { from = request.From, to = request.To },
                        type = request.Type,
                    },
                    limit = request.Limit,
                    order_by = request.OrderBy
                };

                return await SendHttpRequestAsync(HttpMethod.Post, $"https://api.tap.company/v2/charges/list");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> DownloadCharge(ChargeDto request)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }

                var requestBody = new
                {
                    period = new
                    {
                        date = new { from = request.From, to = request.To },
                        type = request.Type,
                    },
                    limit = request.Limit,
                    order_by = request.OrderBy
                };

                return await SendHttpRequestAsync(HttpMethod.Post, $"https://api.tap.company/v2/charges/download");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        public async Task<AppResponse> CreateRefund(RefundDto request)
        {
            try
            {
                var userId = GetUserDetailId();
                if (userId == Guid.Empty)
                {
                    return _responseHelper.ErrorMessage(message: "User ID is not correct.");
                }
                var requestBody = new
                {
                    charge_id = request.ChargeId,
                    amount = request.Amount,
                    currency = request.Currency,
                    reason = request.Reason,
                    destinations = request.Destinations != null && request.Destinations.Count > 0 ?
                        new
                        {
                            destination = request.Destinations.Select(d => new
                            {
                                id = d.Id,
                                amount = d.Amount.ToString("0.##"),
                                currency = d.Currency
                            }).ToArray()
                        } : null,
                    post = new
                    {
                        url = request.PostUrl
                    }
                };

                return await SendHttpRequestAsync(HttpMethod.Post, $"https://api.tap.company/v2/refunds");
            }
            catch (Exception ex)
            {
                return _responseHelper.ErrorMessage(ex);
            }
        }

        private Wallet CreateUpdateWalletFromJsonResponse(decimal amount, string currency, Guid userId)
        {
            try
            {
                Wallet walletObj = new Wallet();
                walletObj.UserId = userId;
                walletObj.Currency = currency;
                walletObj.Balance = amount;
                walletObj.IsActive = true;
                walletObj.CreatedOn = DateTime.Now;
                walletObj.CreatedBy = userId;

                return walletObj;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating transaction: {ex.Message}");
                throw;
            }
        }

        private Transaction CreateTransactionFromJsonResponse(JsonDocument jsonResponse, Guid userId)
        {
            try
            {
                Transaction transactionObj = new Transaction();

                // Helper function to safely retrieve a string property
                string GetString(JsonElement element, string propertyName, string defaultValue = "")
                {
                    return element.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
                           ? property.GetString() ?? defaultValue
                           : defaultValue;
                }

                // Helper function to safely retrieve a decimal property
                decimal GetDecimal(JsonElement element, string propertyName, decimal defaultValue = 0)
                {
                    return element.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
                           ? property.GetDecimal()
                           : defaultValue;
                }

                // Helper function to safely retrieve a boolean property
                bool GetBool(JsonElement element, string propertyName, bool defaultValue = false)
                {
                    return element.TryGetProperty(propertyName, out var property) && property.ValueKind != JsonValueKind.Null
                           ? property.GetBoolean()
                           : defaultValue;
                }

                // Populate transaction properties using helper functions
                string transactionStatus = GetString(jsonResponse.RootElement, "status");
                transactionStatus = char.ToUpper(transactionStatus[0]) + transactionStatus.Substring(1).ToLower();

                string transactionType = GetString(jsonResponse.RootElement.GetProperty("source"), "payment_type");
                transactionType = char.ToUpper(transactionType[0]) + transactionType.Substring(1).ToLower();

                //string cardStatus = GetString(jsonResponse.RootElement.GetProperty("card_security"), "message").ToLower();

                if (Enum.TryParse(transactionType, out TransactionType type))
                {
                    transactionObj.TransactionType = type;
                }

                if (Enum.TryParse(transactionStatus, out TransactionStatus status))
                {
                    transactionObj.Status = status;
                }

                transactionObj.Amount = GetDecimal(jsonResponse.RootElement, "amount");
                transactionObj.Currency = GetString(jsonResponse.RootElement, "currency");
                transactionObj.Description = GetString(jsonResponse.RootElement, "statement_descriptor");
                transactionObj.PaymentMethod = GetString(jsonResponse.RootElement.GetProperty("source"), "payment_method");
                transactionObj.PaymentType = GetString(jsonResponse.RootElement.GetProperty("source"), "payment_type");

                string createdString = GetString(jsonResponse.RootElement.GetProperty("transaction"), "created");
                long createdTimestamp = long.Parse(createdString);
                transactionObj.TransactionDate = DateTimeOffset.FromUnixTimeMilliseconds(createdTimestamp).DateTime;

                transactionObj.CardBrand = GetString(jsonResponse.RootElement.GetProperty("card"), "brand");
                transactionObj.CardFirstEight = GetString(jsonResponse.RootElement.GetProperty("card"), "first_eight");
                transactionObj.CardLastFour = GetString(jsonResponse.RootElement.GetProperty("card"), "last_four");
                //transactionObj.CardSecurity = GetString(jsonResponse.RootElement.GetProperty("card_security"), "message");
                transactionObj.ReceiptId = GetString(jsonResponse.RootElement.GetProperty("receipt"), "id");
                transactionObj.ThreeDSecure = GetBool(jsonResponse.RootElement, "threeDSecure");
                transactionObj.ThreeDSecureId = GetString(jsonResponse.RootElement.GetProperty("security").GetProperty("threeDSecure"), "id");
                transactionObj.SourceId = GetString(jsonResponse.RootElement.GetProperty("source"), "id");
                transactionObj.SourceType = GetString(jsonResponse.RootElement.GetProperty("source"), "type");
                transactionObj.SourceChannel = GetString(jsonResponse.RootElement.GetProperty("source"), "channel");
                transactionObj.ReferenceAcquirer = GetString(jsonResponse.RootElement.GetProperty("reference"), "acquirer");
                transactionObj.ReferenceGateway = GetString(jsonResponse.RootElement.GetProperty("reference"), "gateway");
                transactionObj.ReferenceOrder = GetString(jsonResponse.RootElement.GetProperty("reference"), "order");
                transactionObj.ReferencePayment = GetString(jsonResponse.RootElement.GetProperty("reference"), "payment");
                transactionObj.ReferenceTraceId = GetString(jsonResponse.RootElement.GetProperty("reference"), "trace_id");
                transactionObj.ReferenceTrack = GetString(jsonResponse.RootElement.GetProperty("reference"), "track");
                transactionObj.ReferenceTransaction = GetString(jsonResponse.RootElement.GetProperty("reference"), "transaction");
                transactionObj.VerfiyId = GetString(jsonResponse.RootElement, "id");
                transactionObj.VerfiyOrderId = GetString(jsonResponse.RootElement.GetProperty("order"), "id");
                transactionObj.PayerAuthId = GetString(jsonResponse.RootElement.GetProperty("authentication"), "id");
                transactionObj.CustomerId = GetString(jsonResponse.RootElement.GetProperty("customer"), "id");
                transactionObj.LiveMode = GetBool(jsonResponse.RootElement, "live_mode");
                transactionObj.IsActive = true;
                transactionObj.CreatedOn = DateTime.Now;
                transactionObj.CreatedBy = userId;

                return transactionObj;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating transaction: {ex.Message}");
                throw;
            }
        }

        public string GenerateUniqueId(string prefix)
        {
            string timestamp = DateTime.Now.Ticks.ToString();
            string randomPart = new Random().Next(1000, 9999).ToString(); // 4-digit random number
            return $"{prefix}_{timestamp}_{randomPart}";
        }
    }
}
