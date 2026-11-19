using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository; // Repositório para operações relacionadas a produtos

        public ProdutoService(IProdutoRepository produtoRepository) // Construtor que recebe o repositório de produtos
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<ProdutoResponseDto>> ListarTodosAsync() // Método para listar todos os produtos
        {
            var produtos = await _produtoRepository.ListarTodosAsync();
            return produtos.Select(p => new ProdutoResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                Estoque = p.Estoque
            });
        }

        public async Task<ProdutoResponseDto?> ObterPorIdAsync(int id) // Método para obter um produto por ID
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
                return null;

            return new ProdutoResponseDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Estoque = produto.Estoque
            };
        }

        public async Task<ProdutoResponseDto> CriarAsync(ProdutoRegistroDto dto) // Método para criar um novo produto
        {
            var produto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                Estoque = dto.Estoque
            };

            var criado = await _produtoRepository.CriarAsync(produto);

            return new ProdutoResponseDto
            {
                Id = criado.Id,
                Nome = criado.Nome,
                Descricao = criado.Descricao,
                Preco = criado.Preco,
                Estoque = criado.Estoque
            };
        }

        public async Task<bool> AtualizarAsync(int id, ProdutoRegistroDto dto) // Método para atualizar um produto existente
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
                return false;

            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.Preco = dto.Preco;
            produto.Estoque = dto.Estoque;

            await _produtoRepository.AtualizarAsync(produto);
            return true;
        }

        public async Task<bool> RemoverAsync(int id) // Método para remover um produto existente
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
                return false;

            await _produtoRepository.RemoverAsync(produto);
            return true;
        }
    }
}