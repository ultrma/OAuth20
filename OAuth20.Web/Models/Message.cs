using System.ComponentModel.DataAnnotations;

namespace OAuth20.Web.Models
{
    public class Message
    {
        [Required]
        public string Content { get; set; }
    }
}
