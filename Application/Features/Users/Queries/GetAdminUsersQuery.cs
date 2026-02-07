namespace Application.Features.Users.Queries;

using MediatR;
using Domain.Repositories;
using Domain.Common.Enums;

public record GetAdminUsersQuery(
    string? Search = null,
    string? Status = null,
    string? Role = null
) : IRequest<List<AdminUserListItemDto>>;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, List<AdminUserListItemDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAdminUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<AdminUserListItemDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            users = users.Where(u =>
                (!string.IsNullOrWhiteSpace(u.Name) && u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(u.Department) && u.Department.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(u.Session) && u.Session.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<UserStatus>(request.Status, true, out var status))
        {
            users = users.Where(u => u.Status == status).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Role) &&
            Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            users = users.Where(u => u.Role == role).ToList();
        }

        return users
            .OrderBy(u => u.Name)
            .ThenBy(u => u.Username)
            .Select(u => new AdminUserListItemDto(
                u.Id,
                u.Name,
                u.Username,
                u.Email,
                u.Role.ToString(),
                u.Status.ToString(),
                u.Department,
                u.Session,
                u.CreatedAt))
            .ToList();
    }
}

public record AdminUserListItemDto(
    long Id,
    string Name,
    string Username,
    string Email,
    string Role,
    string Status,
    string? Department,
    string? Session,
    DateTime CreatedAt
);
