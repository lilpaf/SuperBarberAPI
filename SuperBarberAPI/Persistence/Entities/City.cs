﻿using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } 
    }
}
