using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NerdDinner.Web.Models
{
    public class Dinner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DinnerId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; }

        [MaxLength(64)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(32)]
        public string ContactPhone { get; set; }

        [MaxLength(64)]
        public string Country { get; set; }

        [Required]
        [MaxLength(256)]
        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual ICollection<Rsvp> Rsvps { get; set; }

        public bool IsUserHost(string userName)
        {
            return (string.Equals(UserName, userName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsUserRegistered(string userName)
        {
            return Rsvps == null ? false : Rsvps.Any(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
        }
    }
}