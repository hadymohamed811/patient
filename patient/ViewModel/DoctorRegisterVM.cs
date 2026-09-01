using System.ComponentModel.DataAnnotations;

namespace patient.ViewModel
{
    public class DoctorRegisterVM
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Specialization { get; set; }
    }
}