﻿// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using ExtCore.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using SoftinuxBase.Security.Common;
using SoftinuxBase.Security.Data.Abstractions;
using SoftinuxBase.Security.Data.Entities;

// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SoftinuxBase.Security")]
// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SecurityTest")]
// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SoftinuxBase.SeedDatabase")]

namespace SoftinuxBase.Security.Data.EntityFramework
{
    public class UserPermissionRepository : RepositoryBase<UserPermission>, IUserPermissionRepository
    {
        public UserPermission FindBy(string userId_, string permissionId_)
        {
            return dbSet.FirstOrDefault(e_ => e_.UserId == userId_ && e_.PermissionId == permissionId_);
        }

        /// <inheritdoc />
        public IEnumerable<UserPermission> FindBy(string extensionName_, Common.Enums.Permission level_)
        {
            var data = from userPermission in storageContext.Set<UserPermission>()
                   join permission in storageContext.Set<Permission>() on userPermission.PermissionId equals permission.Id
                   where userPermission.Extension == extensionName_ && permission.Name == level_.GetPermissionName()
                   select userPermission;
            return data.ToList();
        }

        public IEnumerable<UserPermission> FilteredByUserId(string userId_)
        {
            return dbSet.Where(e_ => e_.UserId == userId_).ToList();
        }

        public virtual void Create(UserPermission entity_)
        {
            dbSet.Add(entity_);
        }

        public virtual void Edit(UserPermission entity_)
        {
            storageContext.Entry(entity_).State = EntityState.Modified;
        }

        public void Delete(string userId_, string permissionId_)
        {
            var entity = FindBy(userId_, permissionId_);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public void DeleteAll()
        {
            dbSet.RemoveRange(dbSet.ToArray());
        }
    }
}
