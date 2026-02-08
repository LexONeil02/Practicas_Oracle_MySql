using System;
using System.Collections.Generic;

namespace Practicas_Oracle.Models;

public partial class Curso
{
    public int IdCurso { get; set; }

    public string Titulo { get; set; } = null!;

    public int? Creditos { get; set; }

    public int? IdProfesor { get; set; }

    public virtual Profesore? IdProfesorNavigation { get; set; }

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
