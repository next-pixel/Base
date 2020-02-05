﻿using System.ComponentModel.DataAnnotations;
using SoftinuxBase.Security.RefreshClaims;

namespace SoftinuxBase.Security.Data.Entities
{
    [NoQueryFilterNeeded]
    public class TimeStore
    {
        [Key]
        [Required]
        [MaxLength(AuthChangesConsts.CacheKeyMaxSize)]
        public string Key { get; set; }

        public long LastUpdatedTicks { get; set; }
    }
}