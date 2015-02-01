using System.ComponentModel.DataAnnotations;

namespace NerdDinner.Web.Models
{
    /// <summary>
    /// Entity Class for ExternalUser
    /// </summary>
    public class ExternalUser : UserBase
    {
        /// <summary>
        /// Gets or sets Provider
        /// </summary>
        [Required]
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets ProviderKey
        /// </summary>
        [Required]
        public string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets External Access Token
        /// </summary>
        [Required]
        public string ExternalAccessToken { get; set; }
    }
}