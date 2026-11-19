using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository; // Repositório para operações relacionadas a usuários
        private readonly IConfiguration _configuration; // Interface para acessar configurações do aplicativo, como a chave secreta para JWT

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration) // Construtor que recebe o repositório de usuários e a configuração do aplicativo
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegistrarAsync(UsuarioRegistroDto dto) // Método para registrar um novo usuário
        {
            var existente = await _usuarioRepository.ObterPorEmailAsync(dto.Email);
            if (existente != null)
                return false;

            var usuario = new Usuario // Cria um novo objeto Usuario com os dados fornecidos no DTO
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = GerarHashSenha(dto.Senha)
            };

            await _usuarioRepository.CriarAsync(usuario); // Chama o método do repositório para criar o usuário no banco de dados
            return true;
        }

        public async Task<string?> LoginAsync(UsuarioLoginDto dto) // Método para autenticar um usuário e retornar um token JWT
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email);
            if (usuario == null || usuario.SenhaHash != GerarHashSenha(dto.Senha))
                return null;

            return GerarTokenJwt(usuario);
        }

        private static string GerarHashSenha(string senha) // Método para gerar um hash da senha usando SHA256
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(bytes);
        }

        private string GerarTokenJwt(Usuario usuario) // Método para gerar um token JWT para o usuário autenticado
        {
            var chaveSecreta = _configuration["Jwt:Key"] // Obtém a chave secreta para JWT do arquivo de configuração (appsettings.json)
                ?? throw new InvalidOperationException("Chave JWT não configurada no appsettings.json.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta)); // Cria uma chave simétrica para assinar o token JWT
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Cria as credenciais de assinatura usando a chave e o algoritmo HMAC SHA256

            var claims = new[] // Define as reivindicações (claims) que serão incluídas no token JWT
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()), // Adiciona o ID do usuário como uma reivindicação (claim) no token JWT
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email), // Adiciona o email do usuário como uma reivindicação (claim) no token JWT
                new Claim(ClaimTypes.Name, usuario.Nome) // 
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            ); // Cria o token JWT com as reivindicações, data de expiração e credenciais de assinatura

            return new JwtSecurityTokenHandler().WriteToken(token); // Retorna o token JWT como uma string
        }
    }
}