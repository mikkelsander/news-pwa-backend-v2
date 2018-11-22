using System;
using System.ComponentModel.DataAnnotations;

namespace PWANews.Models.DomainModels
{
    public class Article : IEquatable<Article>
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string PublisherId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string Author { get; set; }
        public string Description { get; set; }
        public string UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Content { get; set; }
         
        public Publisher Publisher { get; set; }

        public bool Equals(Article other)
        {
            //using Object.Equals instead of string.Equals to account for null values. If both strings are null they are still considered equal.

            return  Equals(PublisherId, other.PublisherId) &&
                    Equals(Title, other.Title) &&
                    Equals(Author, other.Author) &&
                    Equals(Description, other.Description) &&
                    Equals(Content, other.Content);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PublisherId, Title, Author, Description, Content);
        }


    
    }
}
