// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using ExtCore.Data.Abstractions;
using ExtCore.Data.EntityFramework;
using Infrastructure.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.Data.Entities;

namespace BaseTest.Common
{
    /// <summary>
    /// Provider-specific storage context to use, with sensitive data logging enabled for unit tests debugging.
    /// This code is identical to Webapp's ApplicationDbContext.
    /// </summary>
    public partial class BaseTestDbContext : IdentityDbContext<User, IdentityRole<string>, string>, IStorageContext
    {
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }
        public BaseTestDbContext(DbContextOptions<BaseTestDbContext> options_) : base(options_)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder_)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new EfLoggerProvider());
            base.OnConfiguring(optionsBuilder_.EnableSensitiveDataLogging().UseLoggerFactory(loggerFactory));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder_)
        {
            base.OnModelCreating(modelBuilder_);
            this.RegisterEntities(modelBuilder_);
        }
    }
}