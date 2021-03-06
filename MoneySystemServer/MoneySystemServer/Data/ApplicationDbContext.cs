﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoneySystemServer.Models;

namespace MoneySystemServer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MoneyDetale> MoneyDetale { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
