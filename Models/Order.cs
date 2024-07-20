using System;
using System.ComponentModel.DataAnnotations;

namespace BuyAndSell.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } 

        public string PhoneNumber { get; set; }

        public string PaymentMethod { get; set; } 

        public string PostOffice { get; set; } 

        public int AdId { get; set; }

        public string OwnerId { get; set; } 

        public string ProductName { get; set; } 

        public decimal ProductPrice { get; set; } 

        public DateTime CreatedAt { get; set; } 
    }
}
