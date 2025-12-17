namespace Domain.Services;

using Domain.Common.Base;

public sealed class ServiceStep : Entity
{
    public long ServiceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool RequiresFileUpload { get; private set; }
    public bool RequiresTextInput { get; private set; }
    public bool IsEquipmentAssignment { get; private set; }

    private ServiceStep() { }

    internal ServiceStep(long serviceId, string name, string description, int order, 
        bool requiresFileUpload, bool requiresTextInput, bool isEquipmentAssignment)
    {
        ServiceId = serviceId;
        Name = name;
        Description = description;
        Order = order;
        RequiresFileUpload = requiresFileUpload;
        RequiresTextInput = requiresTextInput;
        IsEquipmentAssignment = isEquipmentAssignment;
    }
}
