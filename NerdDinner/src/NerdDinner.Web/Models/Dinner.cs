using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NerdDinner.Web.Models
{
    public class Dinner
    {
        [Key]
        public long DinnerId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(1028)]
        public string Description { get; set; }

        [Required]
        [MaxLength(64)]
        public long UserId { get; set; }

        [Required]
        [MaxLength(32)]
        public string ContactPhone { get; set; }

        [Required]
        [MaxLength(64)]
        public string Country { get; set; }

        [Required]
        [MaxLength(256)]
        public string Address { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        [MaxLength(64)]
        public string HostedByName { get; set; }

        public ICollection<Rsvp> Rsvps { get; set; }

        public bool IsUserRegistered(int userId)
        {
            return Rsvps.Any(r => r.UserId == userId);
        }
    }
}