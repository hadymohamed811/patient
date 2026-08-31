public class Appointment
{
    public int Id { get; set; }

    public int TimeSlotId { get; set; }
    public TimeSlot? TimeSlot { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Confirmed"; 
}