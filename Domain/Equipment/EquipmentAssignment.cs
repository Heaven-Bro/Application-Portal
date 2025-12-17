namespace Domain.Equipment;

using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Exceptions;

public sealed class EquipmentAssignment : Entity
{
    public long ApplicationId { get; private set; }
    public long EquipmentId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? ExpectedReturnDate { get; private set; }
    public DateTime? ReturnRequestedAt { get; private set; }
    public DateTime? ReturnVerifiedAt { get; private set; }
    public long? ReturnVerifiedBy { get; private set; }
    public EquipmentAssignmentStatus Status { get; private set; }
    public string? AdminNotes { get; private set; }
    public string? ApplicantResponse { get; private set; }
    public DateTime? DamageAcknowledgedAt { get; private set; }

    private EquipmentAssignment() { }

    public static EquipmentAssignment Create(long applicationId, long equipmentId, 
        DateTime? expectedReturnDate, long assignedBy)
    {
        var assignment = new EquipmentAssignment
        {
            ApplicationId = applicationId,
            EquipmentId = equipmentId,
            AssignedAt = DateTime.UtcNow,
            ExpectedReturnDate = expectedReturnDate,
            Status = EquipmentAssignmentStatus.CheckedOut
        };
        
        assignment.MarkAsCreated(assignedBy);
        return assignment;
    }

    public void RequestReturn(long applicantId)
    {
        if (Status != EquipmentAssignmentStatus.CheckedOut)
            throw new InvalidStateTransitionException(Status.ToString(), "RequestReturn");

        Status = EquipmentAssignmentStatus.ReturnRequested;
        ReturnRequestedAt = DateTime.UtcNow;
        MarkAsModified(applicantId);
    }

    public void ApproveReturnAsGood(long adminId)
    {
        if (Status != EquipmentAssignmentStatus.ReturnRequested)
            throw new InvalidStateTransitionException(Status.ToString(), "ApproveReturnAsGood");

        Status = EquipmentAssignmentStatus.ReturnedGood;
        ReturnVerifiedBy = adminId;
        ReturnVerifiedAt = DateTime.UtcNow;
        MarkAsModified(adminId);
    }

    public void MarkAsDamaged(string adminNotes, long adminId)
    {
        if (Status != EquipmentAssignmentStatus.ReturnRequested)
            throw new InvalidStateTransitionException(Status.ToString(), "MarkAsDamaged");

        Status = EquipmentAssignmentStatus.PendingDamageAcknowledgment;
        AdminNotes = adminNotes;
        ReturnVerifiedBy = adminId;
        ReturnVerifiedAt = DateTime.UtcNow;
        MarkAsModified(adminId);
    }

    public void AcknowledgeDamage(long applicantId)
    {
        if (Status != EquipmentAssignmentStatus.PendingDamageAcknowledgment)
            throw new InvalidStateTransitionException(Status.ToString(), "AcknowledgeDamage");

        Status = EquipmentAssignmentStatus.ReturnedDamaged;
        DamageAcknowledgedAt = DateTime.UtcNow;
        MarkAsModified(applicantId);
    }

    public void DisputeDamage(string applicantResponse, long applicantId)
    {
        if (Status != EquipmentAssignmentStatus.PendingDamageAcknowledgment)
            throw new InvalidStateTransitionException(Status.ToString(), "DisputeDamage");

        Status = EquipmentAssignmentStatus.DamageDisputed;
        ApplicantResponse = applicantResponse;
        MarkAsModified(applicantId);
    }

    public void RejectReturn(string adminNotes, long adminId)
    {
        if (Status != EquipmentAssignmentStatus.ReturnRequested)
            throw new InvalidStateTransitionException(Status.ToString(), "RejectReturn");

        Status = EquipmentAssignmentStatus.ReturnRejected;
        AdminNotes = adminNotes;
        ReturnVerifiedBy = adminId;
        ReturnVerifiedAt = DateTime.UtcNow;
        MarkAsModified(adminId);
    }

    public void ResolveManually(EquipmentAssignmentStatus finalStatus, string notes, long adminId)
    {
        if (finalStatus != EquipmentAssignmentStatus.ReturnedGood && 
            finalStatus != EquipmentAssignmentStatus.ReturnedDamaged)
            throw new BusinessRuleViolationException("Can only resolve to ReturnedGood or ReturnedDamaged");

        Status = finalStatus;
        AdminNotes = notes;
        ReturnVerifiedBy = adminId;
        ReturnVerifiedAt = DateTime.UtcNow;
        MarkAsModified(adminId);
    }

    public bool IsReturned()
    {
        return Status == EquipmentAssignmentStatus.ReturnedGood || 
               Status == EquipmentAssignmentStatus.ReturnedDamaged;
    }
}
