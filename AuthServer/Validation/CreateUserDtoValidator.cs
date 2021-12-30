using AuthServer.Core.DTOs;
using FluentValidation;

namespace AuthServer.API.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Cant be empty").EmailAddress().WithMessage("This is not email");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Cant be empty");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Cant be empty");
        }
    }
}