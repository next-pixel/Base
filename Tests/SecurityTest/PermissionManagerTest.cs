﻿using System;
using Security;
using SecurityTest.Util;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ExtCore.Infrastructure;
using Security.Data.Abstractions;
using Security.Data.Entities;
using Security.Enums;
using Xunit;
using Permission = Security.Data.Entities.Permission;

namespace SecurityTest
{
    [Collection("Database collection")]
    public class PermissionManagerTest : BaseTest
    {
        private const string CST_PERM_CODE_1 = "test_perm_1";
        private const string CST_PERM_CODE_2 = "test_perm_2";
        private const string CST_PERM_CODE_3 = "test_perm_3";
        private const string CST_ROLE_CODE_1 = "test_role_1";
        private const string CST_GROUP_CODE_1 = "test_group_1";
        private const string CST_GROUP_CODE_2 = "test_group_2";

        private static string _assembly = typeof(PermissionManagerTest).Assembly.FullName;

        public PermissionManagerTest(DatabaseFixture fixture_)
        {
            _fixture = fixture_;
        }

        private string FormatExpectedClaimValue(string permissionCode_, bool readWrite_)
        {
            return $"{permissionCode_}|{_assembly}|{(readWrite_ ? "RW" : "RO")}";
        }

        /// <summary>
        /// Test of GetFinalPermissions() passing permission-related data.
        /// </summary>
        [Fact]
        public void TestGetFinalPermissionsRwNoDatabase()
        {
            #region test data setup
            Permission roPerm = new Permission { Code = CST_PERM_CODE_1, OriginExtension = _assembly };
            Permission rwPerm = new Permission { Code = CST_PERM_CODE_1, OriginExtension = _assembly };
            Tuple<string, int> roTuple = new Tuple<string, int>(roPerm.UniqueIdentifier, (int)Security.Enums.Permission.PermissionLevelValue.ReadOnly);
            Tuple<string, int> rwTuple = new Tuple<string, int>(rwPerm.UniqueIdentifier, (int)Security.Enums.Permission.PermissionLevelValue.ReadWrite);

            #endregion
            IEnumerable<Claim> claims =
                new PermissionManager().GetFinalPermissions(new List<Tuple<string, int>> { roTuple, rwTuple });
            Assert.Equal(1, claims.Count());
            Assert.Equal(ClaimType.Permission, claims.First().Type);
            Assert.Equal(FormatExpectedClaimValue(rwPerm.Code, true), claims.First().Value);
        }

        /// <summary>
        /// Test of loading permissions from database, from roles and groups.
        /// </summary>
        [Fact]
        public void TestLoadPermissions()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup
                // Permission 1, Role 1, Permission 2, Permission 3, Group 1, Group 2, User 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly };
                Permission perm2 = new Permission { Code = CST_PERM_CODE_2, Label = "Perm 2", OriginExtension = _assembly };
                Permission perm3 = new Permission { Code = CST_PERM_CODE_3, Label = "Perm 3", OriginExtension = _assembly };

                Role role1 = new Role { Code = CST_ROLE_CODE_1, Label = "Role 1", OriginExtension = _assembly };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };
                Group group2 = new Group { Code = CST_GROUP_CODE_2, Label = "Group 2", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                IPermissionRepository permRepo = _fixture.GetRepository<IPermissionRepository>();
                permRepo.Create(perm1);
                permRepo.Create(perm2);
                permRepo.Create(perm3);

                _fixture.GetRepository<IRoleRepository>().Create(role1);

                IGroupRepository groupRepo = _fixture.GetRepository<IGroupRepository>();
                groupRepo.Create(group1);
                groupRepo.Create(group2);

                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Role 1, RW
                _fixture.GetRepository<IRolePermissionRepository>().Create(new RolePermission
                {
                    PermissionId = perm1.Id,
                    RoleId = role1.Id,
                    PermissionLevelId = (int)Security.Enums.Permission.PermissionLevelValue.ReadWrite
                });

                // Link Permission 2 to Group 1, RW
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm2.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = (int)Security.Enums.Permission.PermissionLevelValue.ReadWrite
                });

                // Link Permission 3 to Group 2, RW
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm3.Id,
                    GroupId = group2.Id,
                    PermissionLevelId = (int)Security.Enums.Permission.PermissionLevelValue.ReadWrite
                });

                // Link Role 1 and Group 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Tuple<string, int>> perms = new PermissionManager().LoadPermissionLevels(_fixture.DatabaseContext, user1);
                Assert.Equal(2, perms.Count());
                List<string> permCodes = new List<string>();
                foreach (Tuple<string, int> perm in perms)
                {
                    permCodes.Add(perm.Item1);
                }

                Assert.Contains(CST_PERM_CODE_1, permCodes);
                Assert.Contains(CST_PERM_CODE_2, permCodes);
            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

        /// <summary>
        /// Test of GetFinalPermissions() with permission loading and computation.
        /// </summary>
        [Fact]
        public void TestGetFinalPermissionsWithLevel()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup

                // Permission 1, Role 1, User 1, Group 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly };

                Role role1 = new Role { Code = CST_ROLE_CODE_1, Label = "Role 1", OriginExtension = _assembly };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                _fixture.GetRepository<IPermissionRepository>().Create(perm1);
                _fixture.GetRepository<IRoleRepository>().Create(role1);
                _fixture.GetRepository<IGroupRepository>().Create(group1);
                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();


                // Link Permission 1 to Role 1, RO
                _fixture.GetRepository<IRolePermissionRepository>().Create(new RolePermission
                {
                    PermissionId = perm1.Id,
                    RoleId = role1.Id,
                    PermissionLevelId = (int)Security.Enums.Permission.PermissionLevelValue.ReadOnly
                });


                // Link Permission 1 to User 1, Never
                _fixture.GetRepository<IUserPermissionRepository>().Create(new UserPermission
                {
                    PermissionId = perm1.Id,
                    UserId = user1.Id,
                    PermissionLevelId = (int)Security.Enums.Permission.PermissionLevelValue.Never
                });

                // Link Permission 1 to Group 1, RW
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm1.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = (int)Security.Enums.Permission.PermissionLevelValue.ReadWrite
                });

                // Link Role 1 and Group 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Claim> claims = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1);
                // Expected no permission because permission is denied
                Assert.Equal(0, claims.Count());
            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

    }
}
