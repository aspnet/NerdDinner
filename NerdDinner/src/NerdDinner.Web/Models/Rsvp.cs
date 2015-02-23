﻿using System.ComponentModel.DataAnnotations;

namespace NerdDinner.Web.Models
{
    public class Rsvp
    {
        [Key]
        public long RsvpId { get; set; }

        [Required]
        public long DinnerId { get; set; }

        [Required]
        public long UserId { get; set; }
    }
}
