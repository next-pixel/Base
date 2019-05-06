﻿// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using ExtCore.Data.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace SoftinuxBase.Security.Data.Entities
{
    /// <summary>
    /// This class extends the IdentityUser with custom fields.
    /// It also wraps it with  so that we can query for it with ExtCore's repository feature.
    /// </summary>
    public class User : IdentityUser, IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime FirstConnection { get; set; }
        public DateTime LastConnection { get; set; }
    }
}
