using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySystemServer.Extantions
{
    public static class GeneralExtantions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return string.Empty;
            }

            else
            {
                return httpContext.User.Claims.Single(x => x.Type == "id").Value;
            }
        } 
    }
}
