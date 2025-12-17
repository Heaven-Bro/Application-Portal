namespace Application.Features.Auth.Queries;

using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Repositories;

public record LoginQuery(string Email, string Password) : IRequest<Result<LoginResponse>>;

public record LoginResponse(string Token, UserDto User);

public class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtGenerator) : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return Result<LoginResponse>.Failure("Invalid email or password");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure("Invalid email or password");

        if (!user.EmailConfirmed)
            return Result<LoginResponse>.Failure("Email not verified");

        var token = jwtGenerator.GenerateToken(user.Id, user.Email, user.Role.ToString());

        var userDto = new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.EmailConfirmed,
            user.Role.ToString(),
            user.Status.ToString(),
            user.PhotoPath,
            user.Phone,
            user.Department
        );

        var response = new LoginResponse(token, userDto);
        return Result<LoginResponse>.Success(response);
    }
}
