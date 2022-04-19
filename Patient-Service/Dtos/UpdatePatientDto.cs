namespace Patient_Service.Dtos;

public class UpdatePatientDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? Birthdate { get; set; }
}