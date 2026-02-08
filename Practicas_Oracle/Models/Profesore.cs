using System;
using System.Collections.Generic;

namespace Practicas_Oracle.Models;

public partial class Profesore
{
    public int IdProfesor { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Email { get; set; }

    public int? IdDepartamento { get; set; }

    public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();

    public virtual Departamento? IdDepartamentoNavigation { get; set; }
}
