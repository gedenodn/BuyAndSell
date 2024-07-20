using System;
namespace BuyAndSell.Contracts
{
	public class OrderRequest
	{
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string PaymentMethod { get; set; }

        public string PostOffice { get; set; }

        public int AdId { get; set; }
    }
}

