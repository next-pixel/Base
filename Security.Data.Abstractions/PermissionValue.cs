﻿namespace Security.Data.EntityFramework
{
    /// <summary>
    /// Class to hold the unique id and permission level value of an attributed permission.
    /// </summary>
    public class PermissionValue
    {
        public string UniqueId { get; set; }

        public byte Level { get; set; }
    }
}