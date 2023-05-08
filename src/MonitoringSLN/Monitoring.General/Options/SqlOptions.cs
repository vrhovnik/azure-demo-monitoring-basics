using System.ComponentModel.DataAnnotations;

namespace Monitoring.General.Options;

public class SqlOptions
{
    [Required(ErrorMessage = "Connection string is required")]
    public string ConnectionString { get; set; }
}