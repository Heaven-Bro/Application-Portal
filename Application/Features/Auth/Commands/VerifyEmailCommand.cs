namespace Application.Features.Auth.Commands;

using MediatR;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;

public record VerifyEmailCommand(string Email, string Token) : IRequest<Result>;

public class VerifyEmailCommandHandler(
    IUserRepository userRepository) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        if (!user.VerifyEmail(request.Token))
            return Result.Failure("Invalid or expired verification token");

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}







