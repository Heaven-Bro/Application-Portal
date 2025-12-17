namespace Domain.Identity;

using Domain.Common.Base;
using Domain.Common.Enums;

public sealed class User : Entity
{
    public string Username { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool EmailConfirmed { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public string? PhotoPath { get; private set; }
    public string? Phone { get; private set; }
    public string? PermanentAddress { get; private set; }
    public string? CurrentAddress { get; private set; }
    public string? Year { get; private set; }
    public string? Session { get; private set; }
    public string? Semester { get; private set; }
    public string? Faculty { get; private set; }
    public string? Department { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiry { get; private set; }

    private User() { }

    public static User CreateApplicant(string username, string name, string email, string passwordHash, long createdBy)
    {
        var user = new User
        {
            Username = username,
            Name = name,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = UserRole.Applicant,
            Status = UserStatus.Active,
            EmailConfirmed = false
        };
        
        user.MarkAsCreated(createdBy);
        return user;
    }

    public static User CreateAdmin(string username, string name, string email, string passwordHash, long createdBy)
    {
        var user = new User
        {
            Username = username,
            Name = name,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            EmailConfirmed = true
        };
        
        user.MarkAsCreated(createdBy);
        return user;
    }

    public void GenerateEmailVerificationToken()
    {
        EmailVerificationToken = Guid.NewGuid().ToString("N");
        EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
    }

    public bool VerifyEmail(string token)
    {
        if (EmailConfirmed)
            return false;

        if (EmailVerificationToken != token)
            return false;

        if (EmailVerificationTokenExpiry == null || EmailVerificationTokenExpiry < DateTime.UtcNow)
            return false;

        EmailConfirmed = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiry = null;
        return true;
    }

    public void UpdateProfile(string? photoPath, string? phone, string? permanentAddress, 
        string? currentAddress, string? year, string? session, string? semester, 
        string? faculty, string? department, long modifiedBy)
    {
        PhotoPath = photoPath;
        Phone = phone;
        PermanentAddress = permanentAddress;
        CurrentAddress = currentAddress;
        Year = year;
        Session = session;
        Semester = semester;
        Faculty = faculty;
        Department = department;
        MarkAsModified(modifiedBy);
    }

    public void UpdatePassword(string newPasswordHash, long modifiedBy)
    {
        PasswordHash = newPasswordHash;
        MarkAsModified(modifiedBy);
    }

    public void Enable(long modifiedBy)
    {
        Status = UserStatus.Active;
        MarkAsModified(modifiedBy);
    }

    public void Disable(long modifiedBy)
    {
        Status = UserStatus.Disabled;
        MarkAsModified(modifiedBy);
    }
}

