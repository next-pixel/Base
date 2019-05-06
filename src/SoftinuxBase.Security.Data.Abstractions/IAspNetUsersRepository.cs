// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using ExtCore.Data.Abstractions;

namespace SoftinuxBase.Security.Data.Abstractions
{
    /// <summary>
    /// Additional repository to access WIF objects in an optimized way.
    /// </summary>
    public interface IAspNetUsersRepository : IRepository
    {
        bool FindByNormalizedUserNameOrEmail(string value_);
    }
}