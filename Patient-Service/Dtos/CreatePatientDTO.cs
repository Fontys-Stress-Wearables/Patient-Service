namespace Patient_Service.Dtos;

public class CreatePatientDTO
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime Birthdate { get; set; }
}