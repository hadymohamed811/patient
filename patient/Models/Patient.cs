
using Microsoft.AspNetCore.Identity;
using patient.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; }

    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    public List<Appointment> Appointments { get; set; } = new List<Appointment>();
}
