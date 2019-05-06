// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using ExtCore.Data.Abstractions;
using Microsoft.AspNetCore.Identity;
using SoftinuxBase.Security.Common.Enums;

namespace SoftinuxBase.Security.Data.Abstractions
{
    /// <summary>
    /// Additional repository to access WIF objects in an optimized way.
    /// </summary>
    public interface IAspNetRolesRepository : IRepository
    {
        /// <summary>
        /// Find the roles that are linked to at least one user and that are also linked to an extension with a permission level.
        /// </summary>
        /// <param name="permissionLevel_"></param>
        /// <param name="extensionName_"></param>
        /// <returns></returns>
        IEnumerable<IdentityRole> FindHavingUsers(Permission permissionLevel_, string extensionName_);
    }
}
