// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace SoftinuxBase.Barebone.Actions
{
    public class UseRewriteAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            var options = new RewriteOptions()
                .AddRewrite(@"/#dashboard", "/", skipRemainingRules: true);

            applicationBuilder.UseRewriter(options);
        }

        public int Priority { get { return 5000; } }
    }
}