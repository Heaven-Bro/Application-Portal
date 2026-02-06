namespace Domain.Services;

using Domain.Common.Base;

public sealed class ServiceStep : Entity
{
    public long ServiceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool RequiresFileUpload { get; private set; }
    public bool RequiresTextInput { get; private set; }
    public string? Instructions { get; private set; }
    public string? DownloadableFormUrl { get; private set; }
    public string? UploadConfig { get; private set; }
    public bool IsOptional { get; private set; }
    public bool RequiresApproval { get; private set; }  

    private ServiceStep() { }
    
    internal ServiceStep(
        long serviceId, 
        string name, 
        int order,
        bool requiresFileUpload, 
        bool requiresTextInput,
        string? instructions = null,           
        string? downloadableFormUrl = null,
        string? uploadConfig = null,
        bool isOptional = false,
        bool requiresApproval = true)  
    {
        ServiceId = serviceId;
        Name = name;
        Order = order;
        RequiresFileUpload = requiresFileUpload;
        RequiresTextInput = requiresTextInput;
        Instructions = instructions;
        DownloadableFormUrl = downloadableFormUrl;
        UploadConfig = uploadConfig;
        IsOptional = isOptional;
        RequiresApproval = requiresApproval;  
    }

    public void UpdateDetails(
        string name,
        int order,
        bool requiresFileUpload,
        bool requiresTextInput,
        string? instructions,
        string? downloadableFormUrl,
        bool isOptional,
        string? uploadConfig,
        bool requiresApproval)  
    {
        Name = name;
        Order = order;
        RequiresFileUpload = requiresFileUpload;
        RequiresTextInput = requiresTextInput;
        Instructions = instructions;
        DownloadableFormUrl = downloadableFormUrl;
        IsOptional = isOptional;
        UploadConfig = uploadConfig;
        RequiresApproval = requiresApproval;  
    }
}
