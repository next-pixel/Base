﻿// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using ExtCore.Data.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace SoftinuxBase.Security.Data.Entities
{
    /// <summary>
    /// Links between roles and permissions: permissions assigned to the role.
    /// Default value for "Scope": "SoftinuxBase.Security".
    /// </summary>
    public class RolePermission : IEntity
    {
        //private string _name;
        public RolePermission()
        {
            Extension = "SoftinuxBase.Security";
        }

        /// <summary>
        /// Primary key: GUID.
        /// </summary>
        /// <value></value>
        public string Id { get; set; }

        public string RoleId { get; set; }

        public string Extension { get; set; }
        public string PermissionId { get; set; }

        public virtual IdentityRole<string> Role { get; set; }
        public virtual Permission Permission { get; set; }

    }
}
