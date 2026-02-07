namespace Domain.Settings;

using Domain.Common.Base;

public sealed class ChairmanAvailabilitySlot : Entity
{
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    private ChairmanAvailabilitySlot() { }

    public static ChairmanAvailabilitySlot Create(DateTime startTime, DateTime endTime, long createdBy)
    {
        var slot = new ChairmanAvailabilitySlot
        {
            StartTime = startTime,
            EndTime = endTime
        };

        slot.MarkAsCreated(createdBy);
        return slot;
    }

    public void Update(DateTime startTime, DateTime endTime, long modifiedBy)
    {
        StartTime = startTime;
        EndTime = endTime;
        MarkAsModified(modifiedBy);
    }
}
