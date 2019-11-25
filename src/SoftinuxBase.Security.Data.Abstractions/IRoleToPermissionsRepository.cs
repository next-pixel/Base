﻿// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.
using System.Collections.Generic;
using ExtCore.Data.Abstractions;
using SoftinuxBase.Security.DataLayer.ExtraAuthClasses;

namespace SoftinuxBase.Security.Data.Abstractions
{
    public interface IRoleToPermissionsRepository : IRepository
    {
        /// <summary>
        /// Delete all roles to permissions records.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Find a record by role name.
        /// </summary>
        /// <param name="roleName_">Name of role.</param>
        /// <returns><see cref="RoleToPermissions"/> if any.</returns>
        RoleToPermissions FindBy(string roleName_);

        /// <summary>
        /// Create a role to permissions record.
        /// </summary>
        /// <param name="entity_">role to permissions data.</param>
        void Create(RoleToPermissions entity_);

        /// <summary>
        /// Set new permissions to a role, erasing old values.
        /// </summary>
        /// <param name="roleName_">Name of role.</param>
        /// <param name="permissions_">New permissions.</param>
        void SetPermissions(string roleName_, ICollection<PermissionParts.Permissions> permissions_);
    }
}
