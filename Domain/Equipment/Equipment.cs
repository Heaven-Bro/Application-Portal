namespace Domain.Equipment;

using Domain.Common.Base;
using Domain.Common.Enums;

public sealed class Equipment : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string EquipmentCode { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsAvailable { get; private set; }
    public EquipmentCondition Condition { get; private set; }

    private Equipment() { }

    public static Equipment Create(string name, string equipmentCode, string category, EquipmentCondition condition, string? description, long createdBy)
    {
        bool isAvailable = (condition == EquipmentCondition.Good || condition == EquipmentCondition.Fair);

        var equipment = new Equipment
        {
            Name = name,
            EquipmentCode = equipmentCode,
            Category = category,
            Description = description,
            IsAvailable = isAvailable,
            Condition = condition
        };
        equipment.MarkAsCreated(createdBy);
        return equipment;
    }

    public void UpdateDetails(string name, string equipmentCode, string category, EquipmentCondition condition, string? description, long modifiedBy)
    {
        Name = name;
        EquipmentCode = equipmentCode;
        Category = category;
        Condition = condition;
        Description = description;

        if (condition == EquipmentCondition.Good || condition == EquipmentCondition.Fair)
        {
            IsAvailable = true;
        }
        else
        {
            IsAvailable = false;
        }

        MarkAsModified(modifiedBy);
    }


    public void UpdateCondition(EquipmentCondition condition, long modifiedBy)
    {
        Condition = condition;
        MarkAsModified(modifiedBy);
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
