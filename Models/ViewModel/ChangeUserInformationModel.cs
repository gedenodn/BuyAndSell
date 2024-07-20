using System;
namespace BuyAndSell.Models.ViewModel
{
	public class ChangeUserInformationModel
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
        public string NewEmail { get; set; }
    }
}

