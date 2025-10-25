﻿
using IdentityService.Application.Features.Users.Commands;

namespace eShop.Application.Features.Users.Commands
{
    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationCommandValidator()
        {
            RuleFor(x => x.UserRegistration.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.UserRegistration.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.UserRegistration.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                // Optional stronger rules — uncomment if you want complexity checks:
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.")
                ;

            RuleFor(x => x.UserRegistration.ComfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.UserRegistration.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.UserRegistration.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^[0-9+\-\s]+$").WithMessage("Phone number contains invalid characters.")
                .MinimumLength(11).WithMessage("Phone number must be at least 11 digits long.");

            // Booleans: optional, but you can enforce defaults or business logic if needed
            RuleFor(x => x.UserRegistration.AutoConfirmEmail)
                .NotNull().WithMessage("AutoConfirmEmail flag is required.");

            RuleFor(x => x.UserRegistration.ActivateUser)
                .NotNull().WithMessage("ActivateUser flag is required.");
        }
    }
}
