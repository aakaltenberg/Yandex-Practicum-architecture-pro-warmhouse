using System.ComponentModel.DataAnnotations.Schema;

namespace GateService.Models;

public class Gate
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "closed"; // open, closed, error
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public string HouseId { get; set; }
    public string? Description { get; set; }
}