using System;
using InternalWebsite.Application.ViewModels;
using Microsoft.Extensions.Configuration;

namespace InternalWebsite.Application.Utils
{
    public class ResponseHelper
    {
        private readonly IConfiguration _config;

        public ResponseHelper(IConfiguration config)
        {
            _config = config;
        }

        public AppResponse ErrorMessage(string message = null)
        {
            return new AppResponse
            {
                Message = string.IsNullOrEmpty(message)
                    ? _config["ResponseMessage:ErrorMessage"].ToString()
                    : message,
                SuccessFlag = false
            };
        }

        public AppResponse SuccessMessage(string message = "", object data = null, object miscData = null,
            int totalCount = 0)
        {
            return new AppResponse
            {
                Data = data,
                MiscData = miscData,
                Message = string.IsNullOrEmpty(message)
                    ? _config["ResponseMessage:SuccessMessage"].ToString()
                    : message,
                SuccessFlag = true,
                TotalCount = totalCount
            };
        }

        public AppResponse ErrorMessageWithData(string message = "", object data = null, object miscData = null,
             int totalCount = 0)
        {
            return new AppResponse
            {
                Data = data,
                MiscData = miscData,
                Message = string.IsNullOrEmpty(message)
                    ? _config["ResponseMessage:SuccessMessage"].ToString()
                    : message,
                SuccessFlag = false,
                TotalCount = totalCount
            };
        }

        public AppResponse ErrorMessage(Exception exception)
        {
            return new AppResponse
            {
                Message = string.IsNullOrEmpty(exception.Message)
                    ? _config["ResponseMessage:ErrorMessage"].ToString()
                    : exception.Message,
                SuccessFlag = false
            };
        }
    }
}