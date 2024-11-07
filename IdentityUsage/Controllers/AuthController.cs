using IdentityUsage.Configuracoes;
using IdentityUsage.Models;
using IdentityUsage.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityUsage.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserPersonalization> _userManager;
        private readonly SignInManager<UserPersonalization> _signInManager;
        private readonly JwtConfiguration _jwtConfiguration;

        public AuthController(UserManager<UserPersonalization> userManager, SignInManager<UserPersonalization> signInManager, IOptions<JwtConfiguration> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtConfiguration = options.Value;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] Login login)
        {
            var resultado = await _signInManager.PasswordSignInAsync(login.Email, login.Senha, false, true);
            if (resultado.Succeeded)
            {
                var credenciais = await GerarCredenciais(login.Email);
                return Ok(credenciais);
            }

            return Unauthorized(resultado);
        }

        [HttpPost]
        [Route("Criar")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateUser([FromBody] User user)
        {
            var identityUser = new UserPersonalization
            {
                Name = user.Nome,
                UserName = user.Email,
                Email = user.Email,
                Cpf = user.Cpf
            };

            var result = await _userManager.CreateAsync(identityUser, user.Senha);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(identityUser, AppRoles.Consumer);
            var confirmationEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            var userId = await _userManager.GetUserIdAsync(identityUser);
            var msgFinal = $"https://localhost:7263/Auth/Confirmar?token={confirmationEmailToken}&id={userId}";

            return Ok(msgFinal);
        }

        [HttpGet]
        [Route("Confirmar")]
        [AllowAnonymous]
        public async Task<ActionResult> EmailConfirmation([FromQuery] string token, [FromQuery] int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<ActionResult> logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        private async Task<AuthResponse> GerarCredenciais(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await ObterClaims(user);
            var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtConfiguration.AccessTokenExpiration);
            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            return new AuthResponse(accessToken);
        }

        private async Task<IList<Claim>> ObterClaims(UserPersonalization user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(userClaims);

            foreach (var role in roles)
                claims.Add(new Claim("role", role));

            return claims;
        }

        private string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dataExpiracao,
                signingCredentials: _jwtConfiguration.SecurityKey);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
