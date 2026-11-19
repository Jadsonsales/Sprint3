using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services.Interfaces;

namespace Sprint3.Controllers
{
    [ApiController] // Indica que esta classe é um controlador de API
    [Route("api/[controller]")] // Define a rota base para este controlador, onde [controller] será substituído pelo nome do controlador (Auth)
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; // Interface para o serviço de autenticação, que será injetada via dependência

        public AuthController(IAuthService authService) // Construtor que recebe o serviço de autenticação via injeção de dependência
        {
            _authService = authService;
        }

        [HttpPost("registrar")] // Define a rota para o endpoint de registro de usuário, que será acessível via POST em /api/auth/registrar
        public async Task<IActionResult> Registrar([FromBody] UsuarioRegistroDto dto)
        {
            var sucesso = await _authService.RegistrarAsync(dto);
            if (!sucesso)
                return BadRequest(new { mensagem = "E-mail já cadastrado." });

            return StatusCode(201, new { mensagem = "Usuário cadastrado com sucesso." });
        }

        [HttpPost("login")] // Define a rota para o endpoint de login, que será acessível via POST em /api/auth/login
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

            return Ok(new { token });
        }
    }
}