// Copyright © 2017-2019 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using SoftinuxBase.Barebone.ViewModels.Shared.Script;

namespace SoftinuxBase.Barebone.ViewModels.Shared.Scripts
{
    public class ScriptsViewModel
    {
        public IEnumerable<ScriptViewModel> Scripts { get; set; }
    }
}