using FluentValidation;
using BuyAndSell.Models.ViewModel;

namespace BuyAndSell.Validators.UserModelValidator
{
    public class EditUserValidator : AbstractValidator<ChangeUserInformationModel>
    {
        public EditUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.NewPassword).NotEmpty().When(x => !string.IsNullOrEmpty(x.NewPassword)).WithMessage("New password is required.");
            RuleFor(x => x.NewPasswordConfirm).Equal(x => x.NewPassword).When(x => !string.IsNullOrEmpty(x.NewPassword)).WithMessage("Passwords do not match.");

            RuleFor(x => x.NewEmail).NotEmpty().When(x => !string.IsNullOrEmpty(x.NewEmail)).WithMessage("New email is required.");
            RuleFor(x => x.NewEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.NewEmail)).WithMessage("Invalid email address.");
        }
    }
}
