﻿using System.Collections.Generic;

namespace Security.Common.Policy
{
    /// <summary>
    /// A simple in-memory storage of names of policies we registered to system.
    /// </summary>
    public static class KnownPolicies
    {
        private static readonly HashSet<string> _set = new HashSet<string>();

        public static void Add(string policyName_)
        {
            _set.Add(policyName_);
        }

        public static bool Contains(string policyName_)
        {
            return _set.Contains(policyName_);
        }
    }
}
