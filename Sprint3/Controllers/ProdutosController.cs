using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services.Interfaces;

namespace Sprint3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var produtos = await _produtoService.ListarTodosAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);
            if (produto == null)
                return NotFound(new { mensagem = "Produto não encontrado." });

            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ProdutoRegistroDto dto)
        {
            var criado = await _produtoService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoRegistroDto dto)
        {
            var atualizado = await _produtoService.AtualizarAsync(id, dto);
            if (!atualizado)
                return NotFound(new { mensagem = "Produto não encontrado." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            var removido = await _produtoService.RemoverAsync(id);
            if (!removido)
                return NotFound(new { mensagem = "Produto não encontrado." });

            return NoContent();
        }
    }
}