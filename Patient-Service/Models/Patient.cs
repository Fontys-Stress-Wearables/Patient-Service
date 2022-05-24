namespace Patient_Service.Models;

public class Patient
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime Birthdate { get; set; }
    public bool IsActive { get; set; }
    public string Tenant { get; set; } = "";
    public string? ProfileImageUrl { get; set; } = "";
}