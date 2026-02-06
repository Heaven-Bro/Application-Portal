namespace Application.Features.Auth.Commands;

using Application.Common;
using Application.Common.Interfaces;
using Domain.Identity;
using Domain.Repositories;
using MediatR;

public record RegisterCommand(
    string Email,
    string Password,
    string Name
) : IRequest<Result<long>>;


public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<long>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<long>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            return Result<long>.Failure("Email already registered");

        var username = GenerateUsername(request.Email, request.Name);

        var existingUsername = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (existingUsername != null)
            return Result<long>.Failure("Username already taken");

        var passwordHash = _passwordHasher.Hash(request.Password);

				var user = User.CreateApplicant(username, request.Name, request.Email, passwordHash, createdBy: 0);
        
        user.GenerateEmailVerificationToken();

        await _userRepository.AddAsync(user, cancellationToken);

        await _emailService.SendEmailVerificationAsync(
            user.Email,
            user.EmailVerificationToken!,
            cancellationToken);

        return Result<long>.Success(user.Id);
    }

    private string GenerateUsername(string email, string? name)
    {
        var emailLower = email.ToLower();
        
        if (emailLower.Contains("@student.just.edu.bd"))
        {
            var localPart = emailLower.Split('@')[0];
            var rollNumber = localPart.Split('.')[0];
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                return $"{name.ToLower().Replace(" ", "")}-{rollNumber}";
            }
            
            return localPart.Replace(".", "-");
        }
        
        return emailLower.Split('@')[0];
    }
}







