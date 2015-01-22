using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NerdDinner.Web.Models
{
    /// <summary>
    /// Entity Class for Dinner 
    /// </summary>
    public class Dinner
    {
        /// <summary>
        /// Gets or sets DinnerID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public long HostedByUserId { get; set; }

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
        /// Gets or sets User
        /// </summary>
        public virtual User User { get; set; }
    }
}