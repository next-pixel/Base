﻿// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using ExtCore.Data.EntityFramework;
using Microsoft.AspNetCore.Identity;
using SoftinuxBase.Security.Data.Abstractions;
using SoftinuxBase.Security.Data.Entities;
using System.Linq;

namespace SoftinuxBase.Security.Data.EntityFramework
{
    /// <summary>
    /// Additional repository to access WIF objects in an optimized way.
    /// </summary>
    public class AspNetRolesRepository : RepositoryBase<Role>, IAspNetRolesRepository
    {
        /// <summary>
        /// Find the roles that are linked to at least one user and that are also linked to an extension with a permission level.
        /// </summary>
        /// <param name="permissionLevel_"></param>
        /// <param name="extensionName_"></param>
        /// <returns></returns>
        public IEnumerable<IdentityRole> FindHavingUsers(Common.Enums.Permission permissionLevel_, string extensionName_)
        {
            IEnumerable<IdentityRole> roles =
                from p in storageContext.Set<Permission>()
                join rp in storageContext.Set<RolePermission>() on p.Id equals rp.PermissionId
                join r in storageContext.Set<IdentityRole>() on rp.RoleId equals r.Id
                join ur in storageContext.Set<IdentityUserRole<string>>() on r.Id equals ur.RoleId
                join u in storageContext.Set<User>() on ur.UserId equals u.Id
                where rp.Extension == extensionName_ && p.Name == extensionName_
                select r;

            return roles;
        }
    }
}