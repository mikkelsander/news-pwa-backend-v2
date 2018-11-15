using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]       
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string AuthenticationToken { get; set; }
        public DateTime? AuthenticationTokenExpiration { get; set; }
 
        public ICollection<Subscription> Subscriptions { get; set; }

        public bool AuthenticationTokenIsExpired()
        {
            return AuthenticationTokenExpiration < DateTime.UtcNow;
        }

    }
}
