using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NerdDinner.Web.Models
{
    /// <summary>
    /// Entity Class for UserBase
    /// </summary>
    public class UserBase
    {
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