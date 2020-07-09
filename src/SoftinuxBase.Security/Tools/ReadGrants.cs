// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ExtCore.Data.Abstractions;
using ExtCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using SoftinuxBase.Infrastructure.Interfaces;
using SoftinuxBase.Security.Data.Abstractions;
using SoftinuxBase.Security.Data.Entities;
using SoftinuxBase.Security.Permissions;
using SoftinuxBase.Security.ViewModels.Permissions;

namespace SoftinuxBase.Security.Tools
{
    /*
        The main ReadGrants class.
        Contains all methods for reading grants permissions.
    */

    /// <summary>
    /// The main ReadGrants class.
    ///
    /// contains all methods for reading grants permissions.
    /// </summary>
    public static class ReadGrants
    {
        /// <summary>
        /// Read all grants:
        ///
        /// - to have a global view of permissions granting:
        ///
        /// -- for a role or a user, what kind of permission is granted, for every extension.
        /// </summary>
        /// <param name="storage_">Storage interface provided by services container.</param>
        /// <returns>Return a GrantViewModel model object.</returns>
        public static GrantViewModel ReadAll(IStorage storage_)
        {
            GrantViewModel model = new GrantViewModel();

            // key : extension name,
            // value : extension's permission enum type, since the enum can be in another assembly.
            var extensionNameAndEnumDict = new Dictionary<string, Type>();

            // 1. Get all extension names from loaded extensions, create initial dictionaries
            foreach (IExtensionMetadata extensionMetadata in ExtensionManager.GetInstances<IExtensionMetadata>())
            {
                if(extensionMetadata.Permissions == null)
                {
                    // Ignore extension when it doesn't define its permissions
                    continue;
                }
                model.RolesWithPermissions.Add(extensionMetadata.Name, new Dictionary<PermissionDisplay, List<string>>());
                extensionNameAndEnumDict.Add(extensionMetadata.Name, extensionMetadata.Permissions);
            }

            // 2. Read role/permission/extension settings (RoleToPermissions table)
            List<RoleToPermissions> rolesToPermissions = storage_.GetRepository<IRoleToPermissionsRepository>().All().ToList();

            foreach (RoleToPermissions roleToPermission in rolesToPermissions)
            {
                model.RoleNames.Add(roleToPermission.RoleName);

                var permissions = roleToPermission.PermissionsForRole;
                PermissionsDisplayDictionary permissionsDisplayDictionary = new PermissionsDisplayDictionary(extensionNameAndEnumDict, permissions);
                foreach (var extensionName in permissionsDisplayDictionary.Dictionary.Keys)
                {
                    var permissionDisplays = permissionsDisplayDictionary.Get(extensionName);
                    foreach (var permissionDisplay in permissionDisplays)
                    {
                        model.RolesWithPermissions[extensionName].TryGetValue(permissionDisplay, out var roleNames);
                        if (roleNames == null)
                        {
                            roleNames = new List<string>();
                            model.RolesWithPermissions[extensionName].Add(permissionDisplay, roleNames);
                        }

                        roleNames.Add(roleToPermission.RoleName);
                    }
                }
            }

            return model;
        }

        // TODO REWRITE for new permissions if useful

        /// <summary>
        /// Get the list of all extensions associated to a role, with corresponding permissions,
        /// and also the list of extensions not linked to the role.
        /// </summary>
        /// <param name="roleId_">Id of a role.</param>
        /// <param name="storage_">Storage interface provided by services container.</param>
        /// <param name="availableExtensions_">Output a list of available extensions.</param>
        /// <param name="selectedExtensions_">Output a list of selected extensions.</param>
        public static void GetExtensions(string roleId_, IStorage storage_, out IList<string> availableExtensions_, out IList<SelectedExtension> selectedExtensions_)
        {
            // selectedExtensions_ = storage_.GetRepository<IRolePermissionRepository>().FilteredByRoleId(roleId_).Select(
            //         rp_ => new SelectedExtension { ExtensionName = rp_.Extension, PermissionName = rp_.Permission.Name, PermissionId = rp_.PermissionId })
            //     .ToList();
            //
            // IEnumerable<string> selectedExtensionsNames = selectedExtensions_.Select(se_ => se_.ExtensionName).ToList();
            //
            // availableExtensions_ = new List<string>();
            // foreach (IExtensionMetadata extensionMetadata in ExtensionManager.GetInstances<IExtensionMetadata>())
            // {
            //     var extensionName = extensionMetadata.Name;
            //     if (!selectedExtensionsNames.Contains(extensionName) && extensionMetadata.Type != null)
            //     {
            //         availableExtensions_.Add(extensionName);
            //     }
            // }
            throw new NotImplementedException();
        }
     }
}