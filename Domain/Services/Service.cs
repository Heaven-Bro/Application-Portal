namespace Domain.Services;

using Domain.Common.Base;
using Domain.Common.Exceptions;

public sealed class Service : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int ServiceVersion { get; private set; }

    private readonly List<ServiceStep> _steps = new();
    public IReadOnlyCollection<ServiceStep> Steps => _steps.AsReadOnly();

    private Service() { }

    public static Service Create(string name, string description, long createdBy)
    {
        var service = new Service
        {
            Name = name,
            Description = description,
            IsActive = true,
            ServiceVersion = 1
        };
        
        service.MarkAsCreated(createdBy);
        return service;
    }

    public void AddStep(string name, string description, int order, bool requiresFileUpload, 
        bool requiresTextInput, bool isEquipmentAssignment)
    {
        var step = new ServiceStep(Id, name, description, order, requiresFileUpload, 
            requiresTextInput, isEquipmentAssignment);
        _steps.Add(step);
    }

    public void UpdateDescription(string newDescription, long modifiedBy)
    {
        Description = newDescription;
        MarkAsModified(modifiedBy);
    }

    public void UpdateSteps(List<ServiceStep> newSteps, long modifiedBy, bool hasPaidApplications)
    {
        if (hasPaidApplications)
        {
            throw new BusinessRuleViolationException(
                "Cannot modify service steps while applications with paid fees exist.");
        }

        _steps.Clear();
        _steps.AddRange(newSteps);
        ServiceVersion++;
        MarkAsModified(modifiedBy);
    }

    public void Activate(long modifiedBy)
    {
        IsActive = true;
        MarkAsModified(modifiedBy);
    }

    public void Deactivate(long modifiedBy)
    {
        IsActive = false;
        MarkAsModified(modifiedBy);
    }
}
