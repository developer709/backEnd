using InternalWebsite.Application.Utils;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class ExceptionLogService : ResponseHelper, IExceptionLogService
    {
        //private readonly IExceptionLogService _userRepository;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        protected ClCongDbContext _context;

        public ExceptionLogService(Microsoft.Extensions.Configuration.IConfiguration configuration, ClCongDbContext context) : base(configuration)
        {
            _configuration = configuration;
            _context = context;
        }
        private SqlConnection SqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }
        private int GetErorCode(Exception ex)
        {
            int errCode = 0;
            var w32ex = ex as Win32Exception;

            if (ex != null)
            {
                if (w32ex == null)
                {
                    w32ex = ex.InnerException as Win32Exception;
                }

                if (w32ex != null)
                {
                    errCode = w32ex.ErrorCode;
                }
            }

            return errCode;
        }

        public String FindErrMsgByCode(int errcode)
        {
            string errmessage = "";

            switch (errcode)
            {
                case 0:
                    errmessage = _configuration["ErrorCodeMessage:0"].ToString();
                    break;
                case int n when (n > 99 && n < 200):
                    errmessage = _configuration["ErrorCodeMessage:100"].ToString();
                    break;
                case int n when (n >= 200 && n < 300):
                    errmessage = _configuration["ErrorCodeMessage:200"].ToString();
                    break;
                case int n when (n >= 300 && n < 400):
                    errmessage = _configuration["ErrorCodeMessage:300"].ToString();
                    break;
                case int n when (n >= 400 && n < 500):
                    errmessage = _configuration["ErrorCodeMessage:400"].ToString();
                    break;
                case int n when (n >= 400 && n < 500):
                    errmessage = _configuration["ErrorCodeMessage:500"].ToString();
                    break;
                default:
                    errmessage = _configuration["ErrorCodeMessage:-1"].ToString();
                    break;
            }

            return errmessage;
        }

        public string ExceptionLog(Exception ex = null, LogLevel level = 0, string comments = null)
        {

            string userErrMsg = "NoErrFound";

            try
            {
                if (ex != null)
                {
                    //using (var db = new ClCongDbContext())
                    //{
                    //    db.AuditLogs.Add(new AuditLog
                    //    {
                    //        Exception = ex.StackTrace,
                    //        Message = ex.Message,
                    //        InnerException = ex.InnerException == null ? "" : ex.InnerException.ToString(),
                    //        InnerMessage = ex.InnerException == null ? "" : ex.InnerException.Message.ToString(),
                    //        Comments = comments,
                    //        LogLevel = level,
                    //        CreatedDate = DateTime.Now
                    //    });
                    //    db.SaveChanges();
                    //}

                    userErrMsg = FindErrMsgByCode(GetErorCode(ex));
                }
            }
            catch (Exception e)
            {
                userErrMsg = FindErrMsgByCode(GetErorCode(e));
            }

            return userErrMsg;
        }
    }
}
