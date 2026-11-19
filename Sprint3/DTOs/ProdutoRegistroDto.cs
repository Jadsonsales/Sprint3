using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs
{
    public class ProdutoRegistroDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MaxLength(40, ErrorMessage = "A descrição deve ter o máximo de 40 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O estoque deve ser maior que zero.")]
        public int Estoque { get; set; }
    }
}