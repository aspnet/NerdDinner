using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NerdDinner.Web.Models
{
    /// <summary>
    /// Entity Class for Dinner 
    /// </summary>
    public class Dinner
    {
        /// <summary>
        /// Gets or sets DinnerId
        /// </summary>
        [Key]
        public long DinnerId { get; set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets EventDate
        /// </summary>
        [Required]
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets HostedByUserId
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets ContactPhone
        /// </summary>
        [Required]
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets Country
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets Address
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Latitude
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets Longitude
        /// </summary>
        [Required]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets HostedByName
        /// </summary>
        [Required]
        public string HostedByName { get; set; }

        /// <summary>
        /// Gets or sets Rsvps
        /// </summary>
        public ICollection<Rsvp> Rsvps { get; set; }

        /// <summary>
        /// Checks if user is registered for the dinner
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns>true if registered, false otherwise</returns>
        public bool IsUserRegistered(int userId)
        {
            return Rsvps.Any(r => r.UserId == userId);
        }
    }
}