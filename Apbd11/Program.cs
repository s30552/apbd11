using Apbd11.Data;
using Apbd11.Models;
using Microsoft.EntityFrameworkCore;

namespace Apbd11;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<PrescriptionContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddControllers();
        
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<PrescriptionContext>();
            ctx.Database.Migrate();

        
            if (!ctx.Doctors.Any())
            {
                ctx.Doctors.Add(new Doctor { FirstName = "Anna", LastName = "Kowalska" });
                ctx.SaveChanges();
            }

            if (!ctx.Medicaments.Any())
            {
                ctx.Medicaments.AddRange(
                    new Medicament { Name = "Ibuprofen" },
                    new Medicament { Name = "Paracetamol" }
                );
                ctx.SaveChanges();
            }
        }
        app.MapControllers();
        app.Run();
    }
}