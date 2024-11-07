using System.Text.Json.Serialization;

namespace IdentityUsage.Models.Request
{
    public class AuthResponse
    {
        public bool Sucesso => Erros.Count == 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccessToken { get; private set; }

        public List<string> Erros { get; private set; } = new List<string>();

        public AuthResponse() => Erros = new List<string>();

        public AuthResponse(string token) => AccessToken = token;

        public void AdicionarErro(string erro) => Erros.Add(erro);

        public void AdicionarErros(IEnumerable<string> erros) => Erros.AddRange(erros);
    }
}
