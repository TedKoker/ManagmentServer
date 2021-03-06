﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneySystemServer.Models;

namespace MoneySystemServer.Services
{
    public interface IIdentityService
    {
        Task<AouthenticationResoult> RegisterAsync(string email, string password);
        Task<AouthenticationResoult> LoginAsync(string email, string paswoord);
        Task<AouthenticationResoult> RefreshTokenAsync(string token, string refreshToken);
    }
}
