using patient.Models;

public class TimeSlot
{
    public int Id { get; set; }

    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public DateTime SlotDate { get; set; } 
    public bool IsBooked { get; set; } = false; 

    public Appointment? Appointment { get; set; }
}