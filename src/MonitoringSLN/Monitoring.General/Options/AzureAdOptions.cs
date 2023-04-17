using System.ComponentModel.DataAnnotations;

namespace Monitoring.General.Options;

public class AzureAdOptions
{
    public string Instance { get; set; } = "https://login.microsoftonline.com/";
    [Required]
    public string Domain { get; set; }
    [Required]
    public string TenantId { get; set; }
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string CallbackPath { get; set; }
    public string SignedOutCallbackPath { get; set; }
    [Required]
    public string Secret { get; set; }
    [Required]
    public string SubscriptionId { get; set; }
}