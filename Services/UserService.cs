using BuyAndSell.Data;
using BuyAndSell.Models;
using BuyAndSell.Models.ViewModel;
using BuyAndSell.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BuyAndSell.Services
{
	public class UserService : IUserService
	{
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
		}

        public async Task AddUserAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", errors)}");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
        }
        public async Task<bool> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                var errors = result.IsLockedOut ? "User account locked out." :
                             result.IsNotAllowed ? "User account not allowed to login." :
                             "Invalid login attempt.";
                throw new InvalidOperationException($"Failed login attempt for user with email: {model.Email}. Reason: {errors}");
            }

            return true;
        }
        public async Task EditUser(ChangeUserInformationModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.NewEmail);

            if (user == null)
            {
                throw new Exception($"User with email {model.NewEmail} not found.");
            }
            var currentUser = await _userManager.FindByIdAsync(user.Id);

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                currentUser.FirstName = model.FirstName;
            }
            if (!string.IsNullOrEmpty(model.LastName))
            {
                currentUser.LastName = model.LastName;
            }
            if (!string.IsNullOrEmpty(model.NewEmail))
            {
                currentUser.Email = model.NewEmail;
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(currentUser);
                await _userManager.ResetPasswordAsync(currentUser, token, model.NewPassword);
            }

            var result = await _userManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors);
                throw new Exception($"Failed to update user profile: {errors}");
            }
        }



       
    }
}

