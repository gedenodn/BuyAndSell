using BuyAndSell.Models;
using FluentValidation;

namespace BuyAndSell.Validators
{
    public class AdValidator : AbstractValidator<Ad>
    {
        public AdValidator()
        {
            RuleFor(ad => ad.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(ad => ad.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(ad => ad.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .Must(BeAValidUrl).WithMessage("ImageUrl must be a valid URL.");
        }

        private bool BeAValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
