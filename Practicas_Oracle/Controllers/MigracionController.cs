using Microsoft.AspNetCore.Mvc;
namespace Practicas_Oracle.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class MigracionController : ControllerBase
    {
        private readonly IMigracionServices _migracionServices;

        public MigracionController(IMigracionServices migracionServices)
        {
            _migracionServices = migracionServices;
        }

        [HttpGet("departamentos")]
        public async Task<IActionResult> GetDepartamentos()
        {
            var lista = await _migracionServices.GetDepartamentosAsync();
            return Ok(lista);
        }

        [HttpGet("profesores")]
        public async Task<IActionResult> GetProfesores()
        {
            var lista = await _migracionServices.GetProfesoresAsync();
            return Ok(lista);
        }

        [HttpGet("cursos")]
        public async Task<IActionResult> GetCursos()
        {
            var lista = await _migracionServices.GetCursosAsync();
            return Ok(lista);
        }

        [HttpGet("estudiantes")]
        public async Task<IActionResult> GetEstudiantes()
        {
            var lista = await _migracionServices.GetEstudiantesAsync();
            return Ok(lista);
        }

        [HttpGet("inscripciones")]
        public async Task<IActionResult> GetInscripciones()
        {
            var lista = await _migracionServices.GetInscripcionesAsync();
            return Ok(lista);
        }

        [HttpGet("todas")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _migracionServices.GetAllAsync();
            return Ok(all);
        }

        [HttpPost("migrar-todo")]
        public async Task<IActionResult> MigrarTodo()
        {

            bool exito = await _migracionServices.MigrarTodoAsync(); 


            return exito ? Ok("Migración completa exitosa") : StatusCode(500, "Error durante la migración");


        }

    }
}
