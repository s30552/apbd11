using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apbd11.Data;
using Apbd11.DTOs;
using Apbd11.Models;

namespace Apbd11.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly PrescriptionContext _context;
        public PrescriptionsController(PrescriptionContext context) 
            => _context = context;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrescriptionDtos dto)
        {
            if (dto.Medicaments == null || !dto.Medicaments.Any())
                return BadRequest("Prescription must contain at least one medicament.");
            if (dto.Medicaments.Count > 10)
                return BadRequest("Prescription can contain max 10 medicaments.");
            if (dto.DueDate < dto.Date)
                return BadRequest("DueDate must be on or after Date.");

            Patient patient = dto.Patient.IdPatient.HasValue
                ? await _context.Patients.FindAsync(dto.Patient.IdPatient.Value)
                : null;

            if (patient == null)
            {
                patient = new Patient {
                    FirstName = dto.Patient.FirstName,
                    LastName  = dto.Patient.LastName,
                    BirthDate = dto.Patient.BirthDate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var doctor = await _context.Doctors.FindAsync(dto.IdDoctor);
            if (doctor == null)
                return BadRequest("Doctor not found.");

            var medIds = dto.Medicaments.Select(m => m.IdMedicament).ToList();
            var meds   = await _context.Medicaments
                                      .Where(m => medIds.Contains(m.IdMedicament))
                                      .ToListAsync();
            if (meds.Count != medIds.Count)
                return BadRequest("One or more medicaments not found.");

            var pres = new Prescription {
                Date                    = dto.Date,
                DueDate                 = dto.DueDate,
                Patient                 = patient,
                Doctor                  = doctor,
                PrescriptionMedicaments = dto.Medicaments
                    .Select(m => new PrescriptionMedicament {
                        IdMedicament = m.IdMedicament,
                        Dose         = m.Dose,
                        Description  = m.Description
                    }).ToList()
            };
            _context.Prescriptions.Add(pres);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.IdPatient }, null);
        }

        [HttpGet("patient/{id}")]
        public async Task<ActionResult<PatientDetailsDto>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.Medicament)
                .FirstOrDefaultAsync(p => p.IdPatient == id);

            if (patient == null)
                return NotFound();

            var dto = new PatientDetailsDto {
                IdPatient    = patient.IdPatient,
                FirstName    = patient.FirstName,
                LastName     = patient.LastName,
                BirthDate    = patient.BirthDate,
                Prescriptions = patient.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionDto {
                        IdPrescription = pr.IdPrescription,
                        Date           = pr.Date,
                        DueDate        = pr.DueDate,
                        Doctor         = new DoctorDto {
                            IdDoctor  = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName,
                            LastName  = pr.Doctor.LastName
                        },
                        Medicaments = pr.PrescriptionMedicaments
                            .Select(pm => new MedicamentInfoDto {
                                IdMedicament = pm.IdMedicament,
                                Name         = pm.Medicament.Name,
                                Dose         = pm.Dose,
                                Description  = pm.Description
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return Ok(dto);
        }
    }
}
