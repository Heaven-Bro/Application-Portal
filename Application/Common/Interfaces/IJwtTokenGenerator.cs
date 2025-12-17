namespace Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(long userId, string email, string role);
}
