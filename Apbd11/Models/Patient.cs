namespace Apbd11.Models
{
    public class Patient
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }

    public class Doctor
    {
        public int IdDoctor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }

    public class Medicament
    {
        public int IdMedicament { get; set; }
        public string Name { get; set; }
        public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    }

    public class Prescription
    {
        public int IdPrescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int IdPatient { get; set; }
        public Patient Patient { get; set; }
        public int IdDoctor { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    }

    public class PrescriptionMedicament
    {
        public int IdPrescription { get; set; }
        public Prescription Prescription { get; set; }
        public int IdMedicament { get; set; }
        public Medicament Medicament { get; set; }
        public int Dose { get; set; }
        public string Description { get; set; }
    }

    
}


