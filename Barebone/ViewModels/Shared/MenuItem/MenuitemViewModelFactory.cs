// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Infrastructure.Interfaces;

namespace Barebone.ViewModels.Shared.MenuItem
{
    public class MenuItemViewModelFactory : ViewModelFactoryBase
    {
        public MenuItemViewModelFactory(IRequestHandler requestHandler)
            : base(requestHandler)
        {
        }
        public MenuItemViewModel Create(Infrastructure.MenuItem menuItem_)
        {
            return new MenuItemViewModel()
            {
                Url = menuItem_.Url,
                Name = menuItem_.Name,
                Position = menuItem_.Position
            };
        }
    }
}