// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.


using ExtCore.Data.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace SoftinuxBase.Security.Data.Entities
{
    /// <summary>
    /// This class wraps the IdentityRole so that we can query for it with ExtCore's repository feature.
    /// </summary>
    public class Role : IdentityRole, IEntity
    {
    }
}
