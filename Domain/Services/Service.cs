namespace Domain.Services;

using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Exceptions;

public sealed class Service : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public ServiceType ServiceType { get; private set; }


    private readonly List<ServiceStep> _steps = new();
    public IReadOnlyCollection<ServiceStep> Steps => _steps.AsReadOnly();

    private Service() { }

    public static Service Create(string name, string description, ServiceType serviceType, long createdBy)
    {
        var service = new Service
        {
            Name = name,
            Description = description,
            IsActive = true,
            ServiceType = serviceType
        };
        service.MarkAsCreated(createdBy);
        return service;
    }

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void AddStep(
    string name,
    int order,
    bool requiresFileUpload,
    bool requiresTextInput,
    string? instructions = null,
    string? downloadableFormUrl = null,
    bool isOptional = false,
    string? uploadConfig = null,
    bool requiresApproval = true)  
{
    var step = new ServiceStep(
        Id,
        name,
        order,
        requiresFileUpload,
        requiresTextInput,
        instructions,
        downloadableFormUrl,
        uploadConfig,
        isOptional,
        requiresApproval  
    );

    _steps.Add(step);
}


    public void RemoveStep(ServiceStep step)
    {
        _steps.Remove(step);
    }

    public void UpdateDescription(string newDescription, long modifiedBy)
    {
        Description = newDescription;
        MarkAsModified(modifiedBy);
    }

    public void UpdateSteps(List<ServiceStep> newSteps, long modifiedBy, bool hasPendingApplications)
    {
        if (hasPendingApplications)
        {
            throw new BusinessRuleViolationException(
                "Cannot modify service steps while pending applications exist. Resolve or cancel them first.");
        }

        _steps.Clear();
        _steps.AddRange(newSteps);
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
