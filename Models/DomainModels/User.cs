using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PWANews.Models.DomainModels
{
    public class User
    {
        public int Id { get; set; }

        [Required]       
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string AuthenticationToken { get; set; }
        public DateTime? AuthenticationTokenExpiration { get; set; }
 
        public ICollection<Subscription> Subscriptions { get; set; }

        public bool AuthenticationTokenIsExpired()
        {
            return AuthenticationTokenExpiration < DateTime.UtcNow;
        }

    }
}
