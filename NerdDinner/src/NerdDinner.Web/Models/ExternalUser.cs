using System.ComponentModel.DataAnnotations;

namespace NerdDinner.Web.Models
{
    public class ExternalUser : UserBase
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string ProviderKey { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }
    }
}