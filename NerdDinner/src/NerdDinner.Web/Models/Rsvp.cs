using System.ComponentModel.DataAnnotations;

namespace NerdDinner.Web.Models
{
    /// <summary>
    /// Entity Class for Rsvp
    /// </summary>
    public class Rsvp
    {
        /// <summary>
        /// Gets or sets RSVP Id
        /// </summary>
        [Key]
        public long RsvpId { get; set; }

        /// <summary>
        /// Gets or sets Dinner Id
        /// </summary>
        [Required]
        public long DinnerId { get; set; }

        /// <summary>
        /// Gets or sets Attendee Id
        /// </summary>
        [Required]
        public long UserId { get; set; }
    }
}
