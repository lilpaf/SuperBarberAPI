﻿using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public required string? About { get; set; }
        
        public required TimeSpan TimeToExecute { get; set; }

        public required bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public ICollection<ServiceCategory> Categories { get; set; } = new HashSet<ServiceCategory>();
    }
}
