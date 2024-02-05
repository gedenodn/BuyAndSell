using System;
using System.ComponentModel.DataAnnotations;

namespace BuyAndSell.Models
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        
        public string ImageUrl { get; set; } // URL-адрес изображения

    }


}

