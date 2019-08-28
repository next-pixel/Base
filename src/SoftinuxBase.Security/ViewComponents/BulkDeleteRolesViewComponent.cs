﻿// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExtCore.Data.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using SoftinuxBase.Barebone.ViewComponents;

namespace SoftinuxBase.Security.ViewComponents
{
    public class BulkDeleteRolesViewComponent : ViewComponentBase
    {
        private readonly RoleManager<IdentityRole<string>> _roleManager;

        public BulkDeleteRolesViewComponent(IStorage storage_, RoleManager<IdentityRole<string>> roleManager_) : base(storage_)
        {
            _roleManager = roleManager_;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View("_BulkDeleteRoles", _roleManager));
        }
    }
}