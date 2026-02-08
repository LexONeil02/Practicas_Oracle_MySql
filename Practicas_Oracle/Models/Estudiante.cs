using System;
using System.Collections.Generic;

namespace Practicas_Oracle.Models;

public partial class Estudiante
{
    public int IdEstudiante { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
