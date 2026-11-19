using Sprint3.DTOs;

namespace Sprint3.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegistrarAsync(UsuarioRegistroDto dto); // Método para registrar um novo usuário
        Task<string?> LoginAsync(UsuarioLoginDto dto); // Método para autenticar um usuário e retornar um token JWT
    }
}