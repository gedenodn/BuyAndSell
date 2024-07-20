
namespace BuyAndSell.Contracts
{
	public class AdRequest
	{
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; } // URL-адрес изображения

        public string UserId { get; set; } // Идентификатор пользователя

        public decimal Price { get; set; }

    }
}

