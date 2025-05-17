using System.ComponentModel.DataAnnotations;

namespace Apbd11.DTOs;

public class PrescriptionDtos
{
    [Required] public DateTime Date { get; set; }
    [Required] public DateTime DueDate { get; set; }
    [Required] public int IdDoctor { get; set; }
    [Required] public PatientDto Patient { get; set; }
    [Required] public List<MedicamentDto> Medicaments { get; set; }
}

public class PatientDto
{
    public int? IdPatient { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public DateTime BirthDate { get; set; }
}

public class MedicamentDto
{
    [Required] public int IdMedicament { get; set; }
    [Required] public int Dose { get; set; }
    [Required] public string Description { get; set; }
}

public class PatientDetailsDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public List<PrescriptionDto> Prescriptions { get; set; }
}

public class PrescriptionDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public DoctorDto Doctor { get; set; }
    public List<MedicamentInfoDto> Medicaments { get; set; }
}

public class DoctorDto
{
    public int IdDoctor { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class MedicamentInfoDto
{
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public int Dose { get; set; }
    public string Description { get; set; }
}