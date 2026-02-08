using System;
using System.Collections.Generic;

namespace Practicas_Oracle.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal? Presupuesto { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Profesore> Profesores { get; set; } = new List<Profesore>();
}
