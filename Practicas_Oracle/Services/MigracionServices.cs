using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Practicas_Oracle.Models;
using Microsoft.Extensions.Logging;

public class MigracionServices : IMigracionServices
{
    private readonly OracleAcademiaContext _oracle;
    private readonly AcademiaContext _mysql;
    private readonly ILogger<MigracionServices> _logger;

    public MigracionServices(AcademiaContext mysql, OracleAcademiaContext oracle, ILogger<MigracionServices> logger)
    {
        _mysql = mysql;
        _oracle = oracle;
        _logger = logger;
    }

    public async Task<bool> MigrarCursosAsync()
    {
        _logger.LogInformation("Inicio: MigrarCursosAsync");

        using var transacciones =
            await _oracle.Database
            .BeginTransactionAsync();

        try
        {
            var listaCursosOracle = await _oracle.Cursos.Select(c => c.IdCurso).ToListAsync();

            var listaIdsFiltrada = await _mysql.Cursos
                .AsNoTracking()
                .Where(e => !listaCursosOracle.Contains(e.IdCurso))
                .ToListAsync();

            _logger.LogInformation("Cursos a migrar: {Count}", listaIdsFiltrada.Count);

            if (listaIdsFiltrada.Any())
            {
                // Evitar referencias a entidades navegacionales que pertenezcan al contexto MySQL
                foreach (var curso in listaIdsFiltrada)
                {
                    curso.IdProfesorNavigation = null;
                }

                await _oracle.Cursos.AddRangeAsync(listaIdsFiltrada);

                await _oracle.SaveChangesAsync();
                await transacciones.CommitAsync();

                _logger.LogInformation("Migración de cursos completada. Insertados: {Count}", listaIdsFiltrada.Count);
                return true;
            }

            _logger.LogInformation("Migración de cursos: no hay cursos nuevos.");
            return false;
        }
        catch
        {
            await transacciones.RollbackAsync();
            _logger.LogInformation("Rollback en MigrarCursosAsync");
            throw;
        }
    }

    public async Task<bool> MigrarDepartamentosAsync()
    {
        _logger.LogInformation("Inicio: MigrarDepartamentosAsync");

        using var transacciones = await _oracle.Database.BeginTransactionAsync();
        try
        {
            var listaDepartamenos = await _mysql.Departamentos.AsNoTracking().ToListAsync();

            var idsEnOracle = await _oracle.Departamentos.Select(departamento => departamento.IdDepartamento).ToListAsync();

            var nuevosDepartamentos = listaDepartamenos.Where(id => !idsEnOracle.Contains(id.IdDepartamento)).ToList();

            _logger.LogInformation("Departamentos a migrar: {Count}", nuevosDepartamentos.Count);

            if (nuevosDepartamentos.Any())
            {
                await _oracle.Departamentos.AddRangeAsync(nuevosDepartamentos);
                await _oracle.SaveChangesAsync();
                await transacciones.CommitAsync();

                _logger.LogInformation("Migración de departamentos completada. Insertados: {Count}", nuevosDepartamentos.Count);
                return true;
            }

            _logger.LogInformation("Migración de departamentos: no hay datos nuevos.");
            return false;
        }
        catch
        {
            await transacciones.RollbackAsync();
            _logger.LogInformation("Rollback en MigrarDepartamentosAsync");
            throw;
        }
    }

    public async Task<bool> MigrarEstudiantesAsync()
    {
        _logger.LogInformation("Inicio: MigrarEstudiantesAsync");

        using var transacciones = await _oracle.Database.BeginTransactionAsync();

        try
        {
            var idsEstudiantesOracle =
                await _oracle.Estudiantes
                .Select(e => e.IdEstudiante)
                .ToListAsync();

            var estudiantesValidos =
                 await _mysql.Estudiantes
                .AsNoTracking()
                .Where(e => !idsEstudiantesOracle.Contains(e.IdEstudiante))
                .ToListAsync();

            _logger.LogInformation("Estudiantes a migrar: {Count}", estudiantesValidos.Count);

            if (estudiantesValidos.Any())
            {
               await _oracle.Estudiantes.AddRangeAsync(estudiantesValidos);
               await _oracle.SaveChangesAsync();
               await transacciones.CommitAsync();

               _logger.LogInformation("Migración de estudiantes completada. Insertados: {Count}", estudiantesValidos.Count);
                return true;
            }

            _logger.LogInformation("Migración de estudiantes: no hay datos nuevos.");
            return false;
        }
        catch
        {
           await transacciones.RollbackAsync();
           _logger.LogInformation("Rollback en MigrarEstudiantesAsync");
           throw;
        }
    }

    public async Task<bool> MigrarInscripcionesAsync()
    {
        _logger.LogInformation("Inicio: MigrarInscripcionesAsync");

        using var transacciones = await _oracle.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Se inicia el proceso de inserción de inscripciones");

            //Obtener los ids de los estudiantes que ya hay en oracle
            HashSet<int> estudiantesPermitidos = await _oracle.Estudiantes.Select(e => (int)e.IdEstudiante).ToHashSetAsync();

            //Obtener los ids de los cursos que ya hay en oracle
            HashSet<int> cursosPermitidos = await _oracle.Cursos.Select(e => (int)e.IdCurso).ToHashSetAsync();

            //Recoger las inscripciones que hay almacenadas en oracle
            var inscripcionesOracle =
                 await _oracle.Inscripciones
                .Select(i => i.IdEstudiante + "-" + i.IdCurso)
                .ToHashSetAsync();

            //Obtener las Inscripciones de mysql
            var listaObjetosMysql =
                await _mysql.Inscripciones
                .AsNoTracking()
                .ToListAsync();

            /* Comprobar que los datos a migrar de mysql a oracle no existan ya 
             * Y además comprobar que el curso y el estudiante existe en oracle
             */
            var inscripcionesValidas =
                listaObjetosMysql.Where(e =>
                !inscripcionesOracle
                .Contains(e.IdEstudiante + "-" + e.IdCurso) &&
                estudiantesPermitidos.Contains(e.IdEstudiante) &&
                cursosPermitidos.Contains(e.IdCurso))
                .ToList();

            _logger.LogInformation("Inscripciones válidas a migrar: {Count}", inscripcionesValidas.Count);

            if (inscripcionesValidas.Any())
            {
                foreach (var inscripcion in inscripcionesValidas)
                {
                    inscripcion.IdCursoNavigation = null;
                    inscripcion.IdEstudianteNavigation = null;
                }

                await _oracle.Inscripciones.AddRangeAsync(inscripcionesValidas);
                await _oracle.SaveChangesAsync();
                await transacciones.CommitAsync();

                _logger.LogInformation("Se han migrado {Count} inscripciones", inscripcionesValidas.Count);

                return true;
            }

            _logger.LogInformation("Migración de inscripciones: no hay inscripciones válidas para migrar.");
            return false;
        }
        catch
        {
            await transacciones.RollbackAsync();
            _logger.LogInformation("Rollback en MigrarInscripcionesAsync");
            throw;
        }
    }

    public async Task<bool> MigrarProfesoresAsync()
    {
        _logger.LogInformation("Inicio: MigrarProfesoresAsync");

        using var transacciones = await _oracle.Database.BeginTransactionAsync();

        try
        {
            var listaIdsDepartamentosSql =
                await _mysql.Departamentos
                .Select(d => d.IdDepartamento)
                .ToHashSetAsync();

            var listaDepartamentosOracle =
                await _oracle.Departamentos
                .Select(d => d.IdDepartamento)
                .ToHashSetAsync();

            var totalIds =
                listaIdsDepartamentosSql
                .Union(listaDepartamentosOracle)
                .ToList();

            var listaProfesores =
               await _mysql.Profesores
                .AsNoTracking()
                .ToListAsync();

            var listaProfesoresOracle =
                await _oracle.Profesores
                .Select(p => p.IdProfesor)
                .ToListAsync();

            var listaProfesoresValida = 
                listaProfesores
                .Where(prof => !listaProfesoresOracle.Contains(prof.IdProfesor))
                .ToList();

            var listaProfesoresFinal = 
                listaProfesoresValida
                .Where(prof => prof.IdDepartamento.HasValue
                ? totalIds.Contains(prof.IdDepartamento.Value)
                : false).ToList();

            _logger.LogInformation("Profesores a migrar: {Count}", listaProfesoresFinal.Count);

            if (listaProfesoresFinal.Any())
            {
                // Evitar referencias a entidades navegacionales que pertenezcan al contexto MySQL
                foreach (var prof in listaProfesoresFinal)
                {
                    prof.IdDepartamentoNavigation = null;
                }

                await _oracle.Profesores.AddRangeAsync(listaProfesoresFinal);
                await _oracle.SaveChangesAsync();
                await transacciones.CommitAsync();

                _logger.LogInformation("Migración de profesores completada. Insertados: {Count}", listaProfesoresFinal.Count);
                return true;
            }

            _logger.LogInformation("Migración de profesores: no hay profesores nuevos.");
            return false;
        }
        catch
        {
            await transacciones.RollbackAsync();
            _logger.LogInformation("Rollback en MigrarProfesoresAsync");
            throw;
        }
    }

    public async Task<bool> MigrarDepartamentos()
    {
        _logger.LogInformation("Inicio: MigrarDepartamentos (sin sufijo Async)");

        using var transacciones =
            await _oracle.Database
            .BeginTransactionAsync();

        try
        {
            var listaDepartamenos = 
                await _mysql.Departamentos
                .AsNoTracking()
                .ToListAsync();

            var idsEnOracle =
                await _oracle.Departamentos
                .Select(departamento => departamento.IdDepartamento)
                .ToListAsync();

            var nuevosDepartamentos =
                listaDepartamenos
                .Where(id => !idsEnOracle.Contains(id.IdDepartamento))
                .ToList();

            _logger.LogInformation("Departamentos a migrar: {Count}", nuevosDepartamentos.Count);

            if (nuevosDepartamentos.Any())
            {
               await _oracle.Departamentos.AddRangeAsync(nuevosDepartamentos);
               await _oracle.SaveChangesAsync();
               await transacciones.CommitAsync();

               _logger.LogInformation("Migración de departamentos (sin sufijo) completada. Insertados: {Count}", nuevosDepartamentos.Count);
                return true;
            }

            _logger.LogInformation("Migración de departamentos (sin sufijo): no hay datos nuevos.");
            return false;
        }
        catch
        {
            await transacciones.RollbackAsync();
            _logger.LogInformation("Rollback en MigrarDepartamentos (sin sufijo)");
            throw;
        }
    }

    public async Task<bool> MigrarTodoAsync()
    {
        _logger.LogInformation("Inicio: MigrarTodoAsync");
        try
        {
            // 1. Departamentos (No dependen de nadie)
            await MigrarDepartamentosAsync();

            // 2. Profesores (Dependen de Departamentos)
            await MigrarProfesoresAsync();

            // 3. Cursos (Dependen de Profesores)
            await MigrarCursosAsync();

            // 4. Estudiantes
            await MigrarEstudiantesAsync();

            // 5. Inscripciones (Dependen de Estudiantes y Cursos)
            await MigrarInscripcionesAsync();

            _logger.LogInformation("Migración completa: MigrarTodoAsync finalizada correctamente");
            return true;
        }
        catch
        {
            _logger.LogInformation("Migracion completa: se produjo un error y la excepción será propagada");
            throw;
        }
    }

    public async Task<List<Departamento>> GetDepartamentosAsync()
    {
        return await _oracle.Departamentos.AsNoTracking().ToListAsync();
    }

    public async Task<List<Profesore>> GetProfesoresAsync()
    {
        return await _oracle.Profesores.AsNoTracking().ToListAsync();
    }

    public async Task<List<Curso>> GetCursosAsync()
    {
        return await _oracle.Cursos.AsNoTracking().ToListAsync();
    }

    public async Task<List<Estudiante>> GetEstudiantesAsync()
    {
        return await _oracle.Estudiantes.AsNoTracking().ToListAsync();
    }

    public async Task<List<Inscripcione>> GetInscripcionesAsync()
    {
        return await _oracle.Inscripciones.AsNoTracking().ToListAsync();
    }

    public async Task<AllTablesDto> GetAllAsync()
    {
        var dto = new AllTablesDto
        {
            Departamentos = await _oracle.Departamentos.AsNoTracking().ToListAsync(),
            Profesores = await _oracle.Profesores.AsNoTracking().ToListAsync(),
            Cursos = await _oracle.Cursos.AsNoTracking().ToListAsync(),
            Estudiantes = await _oracle.Estudiantes.AsNoTracking().ToListAsync(),
            Inscripciones = await _oracle.Inscripciones.AsNoTracking().ToListAsync()
        };

        return dto;
    }
}
