using BuyAndSell.Models;
using BuyAndSell.Models.ViewModel;
using BuyAndSell.ViewModels;

namespace BuyAndSell.Services
{
	public interface IUserService
	{
		Task AddUserAsync(RegisterViewModel viewModel);
		Task<bool> LoginUserAsync(LoginViewModel viewModel);
		Task EditUser(ChangeUserInformationModel model);

    }
}

