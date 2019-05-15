// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonTest;
using ExtCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using SoftinuxBase.Infrastructure.Interfaces;
using SoftinuxBase.Security.Data.Abstractions;
using SoftinuxBase.Security.Data.Entities;
using SoftinuxBase.Security.Tools;
using SoftinuxBase.Security.ViewModels.Permissions;
using SoftinuxBase.SeedDatabase;
using Xunit;
using Constants = SoftinuxBase.Common.Constants;
using Permission = SoftinuxBase.Security.Common.Enums.Permission;
using Role = SoftinuxBase.SeedDatabase.Role;

namespace SecurityTest
{
    /// <summary>
    /// Note: the extensions we want to work with must be added to this project's references so that the extension is found
    /// at runtime in unit test working directory (using ExtCore's ExtensionManager and custom path).
    /// So we did for the Chinook extension. Security was already referenced.
    /// </summary>
    [Collection("Database collection")]
    public class ReadGrantsTest : CommonTestWithDatabase
    {
        public ReadGrantsTest(DatabaseFixture databaseFixture_) : base(databaseFixture_)
        {
        }

        /// <summary>
        /// Create base permissions, roles, one additional role "Special User".
        /// Then create role-extension links as following:
        ///
        /// "Security" extension:
        ///
        /// Administrator role : Admin permission
        /// User role : Read permission
        /// Anonymous role : Never permission
        /// Special User role : Write permission
        ///
        /// "Chinook" extension:
        ///
        /// Administrator role : Admin permission
        /// User role : Write permission.
        ///
        /// </summary>
        [Fact]
        public async Task ReadAll()
        {
            var repo = DatabaseFixture.Storage.GetRepository<IRolePermissionRepository>();
            var permRepo = DatabaseFixture.Storage.GetRepository<IPermissionRepository>();
            try
            {
                // Arrange
                // 1. Create base roles
                await CreateBaseRolesIfNeeded();

                // 2. Create "Special User" role
                await CreateRoleIfNotExisting("Special User");

                // 3. Read roles to get their IDs
                var adminRole = await DatabaseFixture.RoleManager.FindByNameAsync(Role.Administrator.GetRoleName());
                var userRole = await DatabaseFixture.RoleManager.FindByNameAsync(Role.User.GetRoleName());
                var anonymousRole = await DatabaseFixture.RoleManager.FindByNameAsync(Role.Anonymous.GetRoleName());
                var specialUserRole = await DatabaseFixture.RoleManager.FindByNameAsync("Special User");

                // 4. Read permissions to get their IDs
                var adminPermissionId = permRepo.All().FirstOrDefault(p_ => p_.Name == Permission.Admin.GetPermissionName())?.Id;
                var writePermissionId = permRepo.All().FirstOrDefault(p_ => p_.Name == Permission.Write.GetPermissionName())?.Id;
                var readPermissionId = permRepo.All().FirstOrDefault(p_ => p_.Name == Permission.Read.GetPermissionName())?.Id;
                var neverPermissionId = permRepo.All().FirstOrDefault(p_ => p_.Name == Permission.Never.GetPermissionName())?.Id;
                Assert.NotNull(adminPermissionId);
                Assert.NotNull(writePermissionId);
                Assert.NotNull(readPermissionId);
                Assert.NotNull(neverPermissionId);

                // 5. Create role-extension links
                // Cleanup first
                repo.DeleteAll();
                DatabaseFixture.Storage.Save();

                repo.Create(new RolePermission
                    { RoleId = adminRole.Id, Extension = Constants.SoftinuxBaseSecurity, PermissionId = adminPermissionId });
                repo.Create(new RolePermission
                    { RoleId = userRole.Id, Extension = Constants.SoftinuxBaseSecurity, PermissionId = readPermissionId });
                repo.Create(new RolePermission
                {
                    RoleId = anonymousRole.Id, Extension = Constants.SoftinuxBaseSecurity, PermissionId = neverPermissionId
                });
                repo.Create(new RolePermission
                {
                    RoleId = specialUserRole.Id, Extension = Constants.SoftinuxBaseSecurity, PermissionId = writePermissionId
                });

                repo.Create(new RolePermission
                    { RoleId = adminRole.Id, Extension = "Chinook", PermissionId = adminPermissionId });
                repo.Create(new RolePermission
                    { RoleId = userRole.Id, Extension = "Chinook", PermissionId = writePermissionId });

                DatabaseFixture.Storage.Save();

                // 6. Build the dictionary that is used by the tool and created in GrantPermissionsController
                Dictionary<string, string> roleNameByRoleId = new Dictionary<string, string>();
                roleNameByRoleId.Add(adminRole.Id, adminRole.Name);
                roleNameByRoleId.Add(userRole.Id, userRole.Name);
                roleNameByRoleId.Add(anonymousRole.Id, anonymousRole.Name);
                roleNameByRoleId.Add(specialUserRole.Id, specialUserRole.Name);

                // Execute
                GrantViewModel model = ReadGrants.ReadAll(DatabaseFixture.RoleManager, DatabaseFixture.Storage, roleNameByRoleId);

                // Assert
                // 1. Number of keys: extensions
                Assert.Equal(ExtensionManager.GetInstances<IExtensionMetadata>().Count(), model.PermissionsByRoleAndExtension.Keys.Count);

                // 2. Number of roles for "Security" extension
                Assert.True(model.PermissionsByRoleAndExtension.ContainsKey(Constants.SoftinuxBaseSecurity));

                // We may have additional linked roles left by other tests...
                Assert.True(model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity].Keys.Count >= 4);

                // 3. Admin role
                Assert.True(model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity].ContainsKey(adminRole.Name));

                // Admin -> Admin, Write, Read, Never
                Assert.Equal(4, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][adminRole.Name].Count);
                Assert.Contains(Permission.Admin, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][adminRole.Name]);
                Assert.Contains(Permission.Write, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][adminRole.Name]);
                Assert.Contains(Permission.Read, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][adminRole.Name]);
                Assert.Contains(Permission.Never, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][adminRole.Name]);

                // 4. Special User role
                Assert.True(model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity].ContainsKey(specialUserRole.Name));

                // Write -> Write, Read, Never
                Assert.Equal(3, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][specialUserRole.Name].Count);
                Assert.Contains(Permission.Write, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][specialUserRole.Name]);
                Assert.Contains(Permission.Read, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][specialUserRole.Name]);
                Assert.Contains(Permission.Never, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][specialUserRole.Name]);

                // 5. User role
                Assert.True(model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity].ContainsKey(userRole.Name));

                // Read -> Read, Never
                Assert.Equal(2, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][userRole.Name].Count);
                Assert.Contains(Permission.Read, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][userRole.Name]);
                Assert.Contains(Permission.Never, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][userRole.Name]);

                // 6. Anonymous role
                Assert.True(model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity].ContainsKey(anonymousRole.Name));

                // Never -> Never
                Assert.Single(model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][anonymousRole.Name]);
                Assert.Contains(Permission.Never, model.PermissionsByRoleAndExtension[Constants.SoftinuxBaseSecurity][anonymousRole.Name]);

                // 7. Number of roles for Chinook extension
                // When the dll doesn't exist on disk, this is our case, no permissions should be found
                Assert.False(model.PermissionsByRoleAndExtension.ContainsKey("Chinook"));

                // No need to check the details for this extension

                // 8. SoftinuxBase.SeedDatabase extension was found, no permissions should be found
                Assert.True(model.PermissionsByRoleAndExtension.ContainsKey("SoftinuxBase.SeedDatabase"));
                Assert.Equal(0, model.PermissionsByRoleAndExtension["SoftinuxBase.SeedDatabase"].Keys.Count);
            }
            finally
            {
                // Cleanup created data
                repo.DeleteAll();
                DatabaseFixture.Storage.Save();

                var specialUserRole = await DatabaseFixture.RoleManager.FindByNameAsync("Special User");
                await DatabaseFixture.RoleManager.DeleteAsync(specialUserRole);
            }
        }

        /// <summary>
        /// A user is granted Admin permission for extension "X".
        /// A role is linked to the extension "X" with Write permission. It is linked to another user.
        /// IsLastAdmin check should return false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task IsLastAdmin_No_StillAnUserDirectAdminForThisExtension()
        {
            // Arrange
            var testAdminUser = new User {
                FirstName = "John",
                LastName = "Superman",
                Email = "johnsuperman@softinux.com",
                UserName = "johnsuperman"
            };
            var testRoleUser = new User
            {
                FirstName = "Henry",
                LastName = "Ford",
                Email = "henryford@softinux.com",
                UserName = "henryford"
            };
            string extensionName = "X";
            var testRole = new IdentityRole<string> {Name = "A great role"};
            var userPermissionRepo = DatabaseFixture.Storage.GetRepository<IUserPermissionRepository>();
            var rolePermissionRepo = DatabaseFixture.Storage.GetRepository<IRolePermissionRepository>();
            var permissionRepo = DatabaseFixture.Storage.GetRepository<IPermissionRepository>();
            var adminPermission = permissionRepo.Find(Permission.Admin);
            var writePermission = permissionRepo.Find(Permission.Write);

            try
            {
                // 1. Create a test user
                var userCreated = await DatabaseFixture.UserManager.CreateAsync(testAdminUser);
                if(!userCreated.Succeeded)
                    throw new Exception("Error creating admin test user");
                userCreated = await DatabaseFixture.UserManager.CreateAsync(testRoleUser);
                if (!userCreated.Succeeded)
                    throw new Exception("Error creating role test user");

                // 2. Assign Admin permission to test admin user for extension X
                userPermissionRepo.Create(new UserPermission { Extension = extensionName, UserId = testAdminUser.Id, PermissionId = adminPermission.Id });

                // 3. Record test role and assign it to test role user
                var roleCreated = await DatabaseFixture.RoleManager.CreateAsync(testRole);
                if (!roleCreated.Succeeded)
                    throw new Exception("Error creating test role");
                rolePermissionRepo.Create(new RolePermission {Extension = extensionName, PermissionId = writePermission.Id, RoleId = testRole.Id });
                var roleAddedToUser = await DatabaseFixture.UserManager.AddToRoleAsync(testRoleUser, testRole.Name);
                if (!roleAddedToUser.Succeeded)
                    throw new Exception("Error adding role to user");

                // 4. Commit the changes
                DatabaseFixture.Storage.Save();

                // Execute
                bool isLastAdmin = ReadGrants.IsLastAdmin(DatabaseFixture.RoleManager, DatabaseFixture.Storage, testRole.Name, extensionName);

                // Assert
                Assert.False(isLastAdmin);
            }
            finally
            {
                // Cleanup created data
                // role-permission record
                rolePermissionRepo.Delete(testRole.Id, extensionName);
                // role-user record
                await DatabaseFixture.UserManager.RemoveFromRoleAsync(testRoleUser, testRole.Name);
                // role
                await DatabaseFixture.RoleManager.DeleteAsync(testRole);
                // user-permission record
                userPermissionRepo.Delete(testAdminUser.Id, adminPermission.Id);
                // user
                await DatabaseFixture.UserManager.DeleteAsync(testRoleUser);
                await DatabaseFixture.UserManager.DeleteAsync(testAdminUser);
            }
        }

        [Fact]
        public async Task IsLastAdmin_No_StillAnotherAdminRoleWithUsersForThisExtension()
        {
            // TODO
            throw new NotImplementedException("To be coded");
        }

        [Fact]
        public async Task IsLastAdmin_Yes_StillAnotherRoleButWithoutUsersForThisExtension()
        {
            // TODO
            throw new NotImplementedException("To be coded");
        }

        [Fact]
        public async Task IsLastAdmin_Yes_NoOthertRoleWithUsersForThisExtension()
        {
            // TODO
            throw new NotImplementedException("To be coded");
        }
    }
}