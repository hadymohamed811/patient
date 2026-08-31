using Microsoft.AspNetCore.Identity;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty; 

   
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

   
    public List<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}