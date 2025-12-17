namespace Domain.Common.Base;

public abstract class Entity
{
    public long Id { get; protected set; }
    public int Version { get; protected set; } = 1;
    public long CreatedBy { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public long? ModifiedBy { get; protected set; }
    public DateTime? ModifiedAt { get; protected set; }

    protected void MarkAsCreated(long userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    protected void MarkAsModified(long userId)
    {
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
        Version++;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id == 0 || other.Id == 0)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
