using BuyAndSell.ViewModels;
using FluentValidation;

namespace BuyAndSell.Validators.UserModelValidator
{
	public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
	{
		public LoginViewModelValidator()
		{
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                ;
        }
	}
}

