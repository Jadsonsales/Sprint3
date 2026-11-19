using Sprint3.Models;

namespace Sprint3.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario> CriarAsync(Usuario usuario);
    }
}