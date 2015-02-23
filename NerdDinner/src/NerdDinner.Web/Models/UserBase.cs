using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NerdDinner.Web.Models
{
    public class UserBase
    {
        [Required]
        public string UserName { get; set; }

        public virtual ICollection<Dinner> Dinners { get; set; }
    }
}