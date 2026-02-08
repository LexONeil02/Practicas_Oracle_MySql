using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Practicas_Oracle.Models;

public partial class AcademiaContext : DbContext
{
    public AcademiaContext()
    {
    }

    public AcademiaContext(DbContextOptions<AcademiaContext> options)
        : base(options)
    {
    }

    protected AcademiaContext(DbContextOptions options)
        : base(options)
    { }

    public virtual DbSet<Curso> Cursos { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<Profesore> Profesores { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.IdCurso).HasName("PRIMARY");

            entity.ToTable("cursos");

            entity.HasIndex(e => e.IdProfesor, "id_profesor");

            entity.Property(e => e.IdCurso).HasColumnName("id_curso");
            entity.Property(e => e.Creditos).HasColumnName("creditos");
            entity.Property(e => e.IdProfesor).HasColumnName("id_profesor");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.Cursos)
                .HasForeignKey(d => d.IdProfesor)
                .HasConstraintName("cursos_ibfk_1");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PRIMARY");

            entity.ToTable("departamentos");

            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Presupuesto)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("presupuesto");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.IdEstudiante).HasName("PRIMARY");

            entity.ToTable("estudiantes");

            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.Activo)
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => new { e.IdEstudiante, e.IdCurso })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("inscripciones");

            entity.HasIndex(e => e.IdCurso, "id_curso");

            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdCurso).HasColumnName("id_curso");
            entity.Property(e => e.FechaInscripcion).HasColumnName("fecha_inscripcion");
            entity.Property(e => e.NotaFinal)
                .HasPrecision(4, 2)
                .HasColumnName("nota_final");

            entity.HasOne(d => d.IdCursoNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdCurso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inscripciones_ibfk_2");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.IdEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inscripciones_ibfk_1");
        });

        modelBuilder.Entity<Profesore>(entity =>
        {
            entity.HasKey(e => e.IdProfesor).HasName("PRIMARY");

            entity.ToTable("profesores");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.IdDepartamento, "id_departamento");

            entity.Property(e => e.IdProfesor).HasColumnName("id_profesor");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Profesores)
                .HasForeignKey(d => d.IdDepartamento)
                .HasConstraintName("profesores_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
