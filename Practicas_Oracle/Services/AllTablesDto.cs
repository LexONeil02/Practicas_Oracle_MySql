using System.Collections.Generic;
using Practicas_Oracle.Models;

public class AllTablesDto
{
    public List<Departamento> Departamentos { get; set; } = new();
    public List<Profesore> Profesores { get; set; } = new();
    public List<Curso> Cursos { get; set; } = new();
    public List<Estudiante> Estudiantes { get; set; } = new();
    public List<Inscripcione> Inscripciones { get; set; } = new();
}
