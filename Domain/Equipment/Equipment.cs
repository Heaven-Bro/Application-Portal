namespace Domain.Equipment;

using Domain.Common.Base;

public sealed class Equipment : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string EquipmentCode { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; }

    private Equipment() { }

    public static Equipment Create(string name, string equipmentCode, string category, long createdBy)
    {
        var equipment = new Equipment
        {
            Name = name,
            EquipmentCode = equipmentCode,
            Category = category,
            IsAvailable = true
        };
        
        equipment.MarkAsCreated(createdBy);
        return equipment;
    }

    public void MarkAsAssigned(long modifiedBy)
    {
        IsAvailable = false;
        MarkAsModified(modifiedBy);
    }

    public void MarkAsAvailable(long modifiedBy)
    {
        IsAvailable = true;
        MarkAsModified(modifiedBy);
    }
}
