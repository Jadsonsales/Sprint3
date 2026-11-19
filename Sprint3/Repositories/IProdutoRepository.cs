using Sprint3.Models;

namespace Sprint3.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> ListarTodosAsync();
        Task<Produto?> ObterPorIdAsync(int id);
        Task<Produto> CriarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task RemoverAsync(Produto produto);
    }
}