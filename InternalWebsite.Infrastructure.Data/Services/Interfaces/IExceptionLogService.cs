using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.Infrastructure.Data.Services.Interfaces
{
    public interface IExceptionLogService
    {
        string ExceptionLog(Exception ex = null, LogLevel level = 0, string comments = null);
    }
}
