namespace GateService.Controllers
{
    public class GateCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string HouseId { get; set; }
    }
}
