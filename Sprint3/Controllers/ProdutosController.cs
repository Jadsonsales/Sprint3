using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;

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

        // Endpoint para gerar relatório em PDF
        [HttpGet("relatorio-pdf")]
        public async Task<IActionResult> GerarRelatorioPdf()
        {
            var produtos = await _produtoService.ListarTodosAsync();

            var usuarioLogado = User.Identity?.Name
                ?? User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.FindFirst("unique_name")?.Value
                ?? User.FindFirst("name")?.Value
                ?? User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? "Usuário Autenticado";

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Relatório de Estoque - Sprint 3")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                        col.Item().PaddingTop(2).Text($"Emitido por: {usuarioLogado}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Nome
                            columns.RelativeColumn(4);   // Descrição
                            columns.RelativeColumn(2);   // Preço
                            columns.ConstantColumn(65);  // Estoque
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Produto").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Descrição").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Preço").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Estoque").Bold();
                        });

                        foreach (var p in produtos)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Id.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Nome);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Descricao);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"R$ {p.Preco:N2}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Estoque.ToString());
                        }
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Gerado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | Página ");
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "Relatorio_Estoque.pdf");
        }
    }
}