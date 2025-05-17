using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apbd11.Controllers;
using Apbd11.Data;
using Apbd11.DTOs;
using Apbd11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Apbd11.Tests
{
    public class PrescriptionsControllerTests
    {
        private PrescriptionContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<PrescriptionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new PrescriptionContext(options);
            // Seed one doctor and one medicament
            context.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "Doc", LastName = "Tor" });
            context.Medicaments.Add(new Medicament { IdMedicament = 1, Name = "Med1" });
            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task Create_Returns_CreatedResult_And_Persists()
        {
            var context = GetInMemoryContext();
            var controller = new PrescriptionsController(context);
            var dto = new PrescriptionDtos
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(5),
                IdDoctor = 1,
                Patient = new PatientDto
                {
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { IdMedicament = 1, Dose = 2, Description = "Twice daily" }
                }
            };

            var result = await controller.Create(dto);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetPatient", createdAt.ActionName);

            // Check persisted
            var patients = context.Patients.Include(p => p.Prescriptions).ThenInclude(pr => pr.PrescriptionMedicaments).ToList();
            Assert.Single(patients);
            var patient = patients.First();
            Assert.Equal("John", patient.FirstName);
            Assert.Single(patient.Prescriptions);
            var prescription = patient.Prescriptions.First();
            Assert.Equal(dto.DueDate, prescription.DueDate);
            Assert.Single(prescription.PrescriptionMedicaments);
            Assert.Equal(2, prescription.PrescriptionMedicaments.First().Dose);
        }

        [Fact]
        public async Task GetPatient_Returns_NotFound_When_NoPatient()
        {
            var context = GetInMemoryContext();
            var controller = new PrescriptionsController(context);

            var result = await controller.GetPatient(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPatient_Returns_PatientDetails()
        {
            var context = GetInMemoryContext();
            var patient = new Patient { FirstName = "A", LastName = "B", BirthDate = DateTime.Today };
            context.Patients.Add(patient);
            context.SaveChanges();

            var prescription = new Prescription
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                Patient = patient,
                Doctor = context.Doctors.First(),
                PrescriptionMedicaments = new List<PrescriptionMedicament>
                {
                    new PrescriptionMedicament { Medicament = context.Medicaments.First(), Dose = 1, Description = "Desc" }
                }
            };
            context.Prescriptions.Add(prescription);
            context.SaveChanges();

            var controller = new PrescriptionsController(context);
            var actionResult = await controller.GetPatient(patient.IdPatient);
            var okResult = Assert.IsType<ActionResult<PatientDetailsDto>>(actionResult);
            var dto = Assert.IsType<PatientDetailsDto>(okResult.Value);

            Assert.Equal(patient.IdPatient, dto.IdPatient);
            Assert.Single(dto.Prescriptions);
            Assert.Equal(1, dto.Prescriptions.First().Medicaments.Count);
        }
    }
}
