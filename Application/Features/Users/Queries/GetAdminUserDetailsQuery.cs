namespace Application.Features.Users.Queries;

using MediatR;
using Application.Common;
using Domain.Repositories;

public record GetAdminUserDetailsQuery(long UserId) : IRequest<Result<AdminUserDetailsDto>>;

public class GetAdminUserDetailsQueryHandler : IRequestHandler<GetAdminUserDetailsQuery, Result<AdminUserDetailsDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAdminUserDetailsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<AdminUserDetailsDto>> Handle(GetAdminUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result<AdminUserDetailsDto>.Failure("User not found");

        var dto = new AdminUserDetailsDto(
            user.Id,
            user.Name,
            user.Username,
            user.Email,
            user.Role.ToString(),
            user.Status.ToString(),
            user.EmailConfirmed,
            user.PhotoPath,
            user.Phone,
            user.PermanentAddress,
            user.CurrentAddress,
            user.Year,
            user.Session,
            user.Semester,
            user.Faculty,
            user.Department,
            user.CreatedAt,
            user.ModifiedAt
        );

        return Result<AdminUserDetailsDto>.Success(dto);
    }
}

public record AdminUserDetailsDto(
    long Id,
    string Name,
    string Username,
    string Email,
    string Role,
    string Status,
    bool EmailConfirmed,
    string? PhotoPath,
    string? Phone,
    string? PermanentAddress,
    string? CurrentAddress,
    string? Year,
    string? Session,
    string? Semester,
    string? Faculty,
    string? Department,
    DateTime CreatedAt,
    DateTime? ModifiedAt
);
