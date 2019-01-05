// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Diagnostics;
using System.Threading.Tasks;
using ExtCore.Data.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoftinuxBase.Barebone.ViewModels.Barebone;

namespace SoftinuxBase.Barebone.Controllers
{
    public class BareboneController : Infrastructure.ControllerBase
    {
        private readonly string _corporateName, _corporateLogo;

        public BareboneController(IStorage storage_, IConfiguration configuration_) : base(storage_)
        {
            _corporateName = configuration_["Corporate:Name"];
            _corporateLogo = configuration_["Corporate:BrandLogo"];
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.CorporateName = _corporateName;
            ViewBag.CorporateLogo = _corporateLogo;
            return await Task.Run(() => View(new IndexViewModelFactory().Create()));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public async Task<IActionResult> Error()
        {
            return await Task.Run(() => View(new Barebone.ViewModels.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }));
        }
    }
}