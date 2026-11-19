using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage = "O nome é obrigátorio.")]
        public string Nome {get; set;} = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O formato de gigitado é inválido")]
        public string Email {get; set;} = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string Senha {get; set;} = string.Empty; 

    }
}