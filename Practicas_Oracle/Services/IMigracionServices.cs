using System.Collections.Generic;
using System.Threading.Tasks;
using Practicas_Oracle.Models;

public interface IMigracionServices
{
    Task<bool> MigrarDepartamentosAsync();
    Task<bool> MigrarProfesoresAsync();
    Task<bool> MigrarCursosAsync();
    Task<bool> MigrarEstudiantesAsync();
    Task<bool> MigrarInscripcionesAsync();

    Task<bool> MigrarTodoAsync();

    // Lectura de tablas
    Task<List<Departamento>> GetDepartamentosAsync();
    Task<List<Profesore>> GetProfesoresAsync();
    Task<List<Curso>> GetCursosAsync();
    Task<List<Estudiante>> GetEstudiantesAsync();
    Task<List<Inscripcione>> GetInscripcionesAsync();
    Task<AllTablesDto> GetAllAsync();
}
