using Microsoft.EntityFrameworkCore;

namespace Practicas_Oracle.Models
{
    // Heredamos de AcademiaContext para no tener que volver 
    // a escribir todos los DbSet (tablas)
    public class OracleAcademiaContext : AcademiaContext
    {
        public OracleAcademiaContext(DbContextOptions<OracleAcademiaContext> options)
        : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cargamos la configuración base de MySQL (nombres de tablas, llaves, etc.)
            base.OnModelCreating(modelBuilder);

            // 2. Ajustamos específicamente para Oracle 
            modelBuilder.Entity<Departamento>(entity =>
            {

                entity.HasKey(e => e.IdDepartamento).HasName("PK_DEPARTAMENTOS");

                // Cambiamos 'datetime' por 'DATE'
                entity.Property(e => e.FechaCreacion)
                      .HasColumnType("DATE")
                      .HasDefaultValueSql("CURRENT_DATE"); // Función estándar de Oracle

                // Oracle prefiere NUMBER para decimales
                entity.Property(e => e.Presupuesto)
                      .HasColumnType("NUMBER(12,2)")
                      .HasDefaultValueSql("0"); // Sin comillas
            });

            // 3. Ajuste para Estudiantes (Booleano)
            modelBuilder.Entity<Estudiante>(entity =>
            {

                entity.HasKey(e => e.IdEstudiante).HasName("PK_ESTUDIANTES");

                // Oracle no tiene bool nativo, usamos NUMBER(1) [0 o 1]
                entity.Property(e => e.Activo)
                      .HasColumnType("NUMBER(1)");
            });

            modelBuilder.Entity<Profesore>().HasKey(e => e.IdProfesor).HasName("PK_PROFESORES");
            modelBuilder.Entity<Curso>().HasKey(e => e.IdCurso).HasName("PK_CURSOS");
            modelBuilder.Entity<Inscripcione>().HasKey(e => new { e.IdEstudiante, e.IdCurso }).HasName("PK_INSCRIPCIONES");
        }

    }
}