﻿// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SoftinuxBase.Security.ViewModels.Account
{
    public class SignInViewModel
    {
        /// <summary>
        /// Gets or sets the user id, if any match.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets user name or e-mail field.
        /// </summary>
        [Display(Name = "Username or e-mail")]
        [Required]
        [StringLength(64)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets password field.
        /// </summary>
        [Display(Name = "Password")]
        [Required]
        [StringLength(64)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use persistent cookie if desired.
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }
    }
}
