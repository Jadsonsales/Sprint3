using Sprint3.DTOs;

namespace Sprint3.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoResponseDto>> ListarTodosAsync(); // Método para listar todos os produtos
        Task<ProdutoResponseDto?> ObterPorIdAsync(int id); // Método para obter um produto por ID
        Task<ProdutoResponseDto> CriarAsync(ProdutoRegistroDto dto); // Método para criar um novo produto
        Task<bool> AtualizarAsync(int id, ProdutoRegistroDto dto); // Método para atualizar um produto existente
        Task<bool> RemoverAsync(int id); // Método para remover um produto
    }
}