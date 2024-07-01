using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using WebApiUserManagement.Context;
using WebApiUserManagement.Models;

namespace WebApiUserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                var result = await _context.Users.ToListAsync();
                if (result == null || result.Count == 0)
                    return NotFound("No se encontraron usuarios.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
                if (result == null)
                    return NotFound($"Usuario con Id {id} no encontrado.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            try
            {
                //Genero un hash para la password
                string hashedPassword = HashPassword(user.PasswordHash);
                user.PasswordHash = hashedPassword;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        //Metodo para generar una contraseña hasheada
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        // Método para verificar la contraseña
        private bool VerifyPassword(string password, string hashedPassword)
        {
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return passwordMatch;
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] User user)
        {
            try
            {
                var userInfo = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
                if (userInfo == null)
                    return NotFound($"Usuario con Id {id} no encontrado.");

                userInfo.Username = user.Username;
                userInfo.Role = user.Role;
                userInfo.CreationTime = DateTime.Now;

                _context.Attach(userInfo);
                await _context.SaveChangesAsync();

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userInfo = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
                if (userInfo == null)
                    return NotFound($"Usuario con Id {id} no encontrado.");

                userInfo.IsDeleted = true;
                userInfo.DeleteTime = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Login de usuario
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == request.Username);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                    return BadRequest("Usuario o contraseña incorrectos.");

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        public class AuthenticationRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        //Chekeo el role del usuario
        [HttpGet("isAdmin/{username}")]
        public async Task<IActionResult> isAdmin(string username)
        {
            try
            {
                var userInfo = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
                if (userInfo == null)
                    return NotFound($"Usuario '{username}' no encontrado.");

                return Ok(userInfo.Role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
