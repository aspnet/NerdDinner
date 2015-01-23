using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NerdDinner.Web.Models
{
    /// <summary>
    /// Entity Class for User
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets UserId
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets ProviderId
        /// </summary>
        [Required]
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets ProviderName
        /// </summary>
        [Required]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets UserName
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Dinners
        /// </summary>
        public virtual ICollection<Dinner> Dinners { get; set; }
    }
}