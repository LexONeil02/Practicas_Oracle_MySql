using Microsoft.AspNetCore.Mvc;
using Practicas_Oracle.Controllers;
using Microsoft.EntityFrameworkCore;
using Practicas_Oracle.Models;
namespace Practicas_Oracle.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class MigracionController : ControllerBase
    {
        private readonly AcademiaContext _mysql;
        private readonly OracleAcademiaContext _oracle;

        public MigracionController(AcademiaContext mysql, OracleAcademiaContext oracle)
        {
            _mysql = mysql;
            _oracle = oracle;
        }


        [HttpPost("migrar-departamentos")]
        public IActionResult MigrarDepartamentos()
        {
            var listaDepartamenos = _mysql.Departamentos.AsNoTracking().ToList();

            var idsEnOracle = _oracle.Departamentos.Select(departamento => departamento.IdDepartamento).ToList();

            var nuevosDepartamentos = listaDepartamenos.Where(id => !idsEnOracle.Contains(id.IdDepartamento)).ToList();

            if (nuevosDepartamentos.Any())
            {
             _oracle.Departamentos.AddRange(nuevosDepartamentos);

             _oracle.SaveChanges();

                return Ok("Migracios completada");
            }


           

            return Ok("No hay datos nuevos");

        }

        [HttpPost("migrar-profesores")]
        public IActionResult MigrarProfesores()
        {

            var listaIdsDepartamentosSql = _mysql.Departamentos.Select(d => d.IdDepartamento).ToList();

            var listaDepartamentosOracle = _oracle.Departamentos.Select(d => d.IdDepartamento).ToList();

            var totalIds = listaIdsDepartamentosSql.Union(listaDepartamentosOracle).ToList();

            var listaProfesores = _mysql.Profesores.AsNoTracking().ToList();

            var listaProfesoresOracle = _oracle.Profesores.Select(p => p.IdProfesor).ToList(); 

            var listaProfesoresValida = listaProfesores.Where(prof => !listaProfesoresOracle.Contains(prof.IdProfesor)).ToList();

            var listaProfesoresFinal = listaProfesoresValida.Where(prof =>
            prof.IdDepartamento.HasValue ? totalIds.Contains(prof.IdDepartamento.Value) : false).ToList();

            if (listaProfesoresFinal.Any())
            {
                _oracle.Profesores.AddRange(listaProfesoresFinal);
                _oracle.SaveChanges();
                return Ok("Profesores Guardados");
            }

            return Ok("No se han podido guardar los profesores");




        }

        [HttpPost("migrar-cursos")]
        public IActionResult migrar_cursos()
        {

            var listaCursosOracle = _oracle.Cursos.Select(c => c.IdCurso).ToList();

            var listaIdsFiltrada = _mysql.Cursos.AsNoTracking().Where(e => !listaCursosOracle.Contains(e.IdCurso));

            if (listaIdsFiltrada.Any())
            {
                _oracle.Cursos.AddRange(listaIdsFiltrada);

                _oracle.SaveChanges();

                return Ok("Lista de Cursos : " + listaIdsFiltrada);

            }

            return Ok("No se encuentran cursos");


        }

        [HttpPost("migrar-estudiantes")]
        public IActionResult migrar_estudiantes()
        {

            var idsEstudiantesOracle = 
                _oracle.Estudiantes
                .Select(e => e.IdEstudiante)
                .ToList();


            var estudiantesValidos =
                _mysql.Estudiantes
                .AsNoTracking()
                .Where(e =>
                !idsEstudiantesOracle.Contains(e.IdEstudiante));


            if (estudiantesValidos.Any())
            {

                _oracle.Estudiantes.AddRange(estudiantesValidos);
                _oracle.SaveChanges();

                return Ok("Total de objetos insertados : " + estudiantesValidos.Count());

            }

            return BadRequest("No se han insertado datos");
        }


        [HttpPost("migrar-inscripciones")]

        public IActionResult migrar_inscripciones()
        {
            //Obtener los ids de los estudiantes que ya hay en oracle
            HashSet<int> estudiantesPermitidos = _oracle.Estudiantes.Select(e => (int) e.IdEstudiante).ToHashSet();

            //Obtener los ids de los cursos que ya hay en oracle
            HashSet<int> cursosPermitidos = _oracle.Cursos.Select(e => (int) e.IdCurso).ToHashSet();


            //Recoger las inscripciones que hay almacenadas en oracle
            var inscripcionesOracle = 
                _oracle.Inscripciones
                .Select(i => i.IdEstudiante + "-" + i.IdCurso).
                ToHashSet();

            //Obtener las Inscripciones de mysql
            var listaObjetosMysql = _mysql.Inscripciones
                .AsNoTracking()
                .ToList();

            /* Comprobar que los datos a migrar de mysql a oracle no existan ya 
             * Y además comprobar que el curso y el estudiante existe en oracle
             */
            var inscripcionesValidas =
                listaObjetosMysql.Where(e =>
                !inscripcionesOracle
                .Contains(e.IdEstudiante + "-" + e.IdCurso) && 
                estudiantesPermitidos.Contains(e.IdEstudiante) &&
                cursosPermitidos.Contains(e.IdCurso))
                .ToList(); ;




            if (inscripcionesValidas.Any())
            {
                foreach (var inscripcion in inscripcionesValidas)
                {
                    inscripcion.IdCursoNavigation = null;
                    inscripcion.IdEstudianteNavigation = null;
                }

                _oracle.Inscripciones.AddRange(inscripcionesValidas);
                _oracle.SaveChanges();

                return Ok("Inscripciones insertadas de forma valida : " + inscripcionesValidas);

            }

            return BadRequest("No se han podido insertar" + inscripcionesValidas.Count());
        }



        [HttpGet("ver-profesores")]
        public IActionResult verProfesores()
        {

            return Ok(_oracle.Profesores.ToList());




        }


        [HttpGet("ver-cursos")]
        public IActionResult verCursos()
        {

            return Ok(_oracle.Cursos.ToList());
        }

        [HttpGet("ver-inscripciones")]
        public IActionResult verInscripciones()
        {

            return Ok(_oracle.Inscripciones.ToList());
        }

        [HttpGet("ver-estudiantes")]
        public IActionResult verEstudiantes()
        {

            return Ok(_oracle.Estudiantes.ToList());
        }
    }
}
