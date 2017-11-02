﻿using System.Collections.Generic;
using Infrastructure;

namespace Extension1
{
    public class ExtensionMetadata : IExtensionMetadata
    {
        public IEnumerable<StyleSheet> StyleSheets => new StyleSheet[] {};
        public IEnumerable<Script> Scripts => new Script[] {};
        public IEnumerable<MenuGroup> MenuGroups => new MenuGroup[]
        {
            new MenuGroup(
                "Content",
                1000,
                new MenuItem[]
                {
                    new MenuItem("/extension1", "Extension 1", 100)
                }
            )
        };
    }
}