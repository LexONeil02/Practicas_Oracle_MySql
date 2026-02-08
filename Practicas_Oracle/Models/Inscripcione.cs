using System;
using System.Collections.Generic;

namespace Practicas_Oracle.Models;

public partial class Inscripcione
{
    public int IdEstudiante { get; set; }

    public int IdCurso { get; set; }

    public DateOnly? FechaInscripcion { get; set; }

    public decimal? NotaFinal { get; set; }

    public virtual Curso IdCursoNavigation { get; set; } = null!;

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;
}
