namespace Domain.Common.Enums;

public enum EquipmentAssignmentStatus
{
    CheckedOut = 0,
    ReturnRequested = 1,
    PendingDamageAcknowledgment = 2,
    DamageDisputed = 3,
    ReturnedGood = 4,
    ReturnedDamaged = 5,
    ReturnRejected = 6
}
