// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Linq;
using SoftinuxBase.Security.Data.Abstractions;
using SoftinuxBase.Tests.Common;
using Xunit;

namespace SoftinuxBase.SecurityTests
{
    [Collection("Database collection")]
    public class BasicReadingTest : CommonTestWithDatabase
    {
        public BasicReadingTest(DatabaseFixture databaseFixture_) : base(databaseFixture_)
        {
        }

        /// <summary>
        /// Test that we are able to read data using Storage and ExtCore repository pattern.
        /// </summary>
        [Fact]
        public void TestReadDatabaseWithRepository()
        {
            var data = DatabaseFixture.Storage.GetRepository<IRoleToPermissionsRepository>().All();
            Assert.NotNull(data);
        }

        /// <summary>
        /// Test that we are able to read data using Identity.
        /// </summary>
        [Fact]
        public void ReadRoleWithIdentity()
        {
            var data = DatabaseFixture.RoleManager.Roles.ToList();
            Assert.NotNull(data);
        }
    }
}