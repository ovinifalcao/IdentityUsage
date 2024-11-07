# IdentityUsage
Este é um projeto de estudo que visa somente ser um guia para lembrar-me de como usar essa biblioteca em projetos reais. Visa ser "step-by-step" prático para implementação no futuro. 

# Quais passos devem ser seguidos?

## Instalações
 - **Aspnet Core Identity**  : Contém todas as ferramentas da MS para autenticação e autorização.
 - **Apsnet Core Authentication JwtBearer** : Contém as ferramentas necessárias para gerar um token JWT.
  - **Apsnet Core Identity Entity Framework Core** : Contém as ferramentas para identificação no EF core.
 - **Entity Framework Core** : Ferramentas do EF Core.
 - **Entity Framework Core Design** : Ferramentas do EF Core para logica em "tempo de design".
 - **Pomelo Entity Framework Core MySql** : Ferramenta do EF Core para conexão com MySql.

 ## Configurações

 - Criar o projeto da API
 - Remover o Weather Forescast

### Adicionar ao Json
É necessário adicionar ao Json os parametros para as configurações do Projeto, neste caso teremos as configurações de banco de Dados, a string de conexão e as configurações do JWT, que vou especificar mais para frente.


    {
        "ConnectionStrings": {
            "database": ""
        },
        "JwtOptions": {
            "Issuer": "",
            "Audience": "",
            "SecurityKey": "NAO_DEIXE_ISSO_AQUI_PFVR_HEIN",
            "AccessTokenExpiration": 0,
            "RefreshTokenExpiration": 0
        }
    }


**OBS:** Este projeto tem compose para docker que configura um banco MySql local necessário para rodar tudo que será descrito  aqui abaixo. A partir desse momento vou considerar que o  `docker compose up` já tenha sido rodado com suceso e que você preencheu esse Appsettings :D.

### Presonalizar o Usuário
Por padrão o Entity já tem um modelo para usuário que vai ter toda a lógica já implementada para salvar senha de uma forma segura, já contem campos como verificação de duplo fator, e temos a possibilidade de estender esse usuário herdando `IdentityUser<T>` onde T é o tipo da chave primária do Id desse modelo. Então olha que lindo, eu posso pegar uma classe que já tem lógica de identidade já estruturada e testada e apenas colocar as propriedades core do meu negócio:

    public class UserPersonalization : IdentityUser<int>
    {
        public string Cpf { get; set; }

        public string Name { get; set; }
    }

### Presonalizar Roles
Uma das coisas interessantes a respeito de Autorização é que você pode seguimentar quem tem acesso a um recurso usando diversas aboradgens, uma delas são as Roles (que eu gosto de traduzir como Papéis). Então um usuário pode ter um ou mais papéis, Ex: Meu usuário que é leitor e escritor de um Blog, eu vou poder decorar os métodos das controlers com quais Roles podem acessar cada um deles e garantir Autorização efetiva. Dado o mesmo caso acima a Identity já provê um objeto para implementar essas Roles que também são personalizáveis caso queira, mas não vou querer rs:

    public class RolePersonalization : IdentityRole<int>
    {
    }

### Criar um Context
Geralmente criamos uma classe de contexto que herda de DbContext, nesse caso vamos herdar de `IdentityContext`. Essa é a classe base para criar os itens do entity Framwork com base no servidor de identidade. Essa classe vai receber três tipos, <TipoDoUsuário, TipoDaRole, TipoDaChave> da seguinte maneira:

    class DataContext : IdentityDbContext<UserPersonalization, RolePersonalization, int>

Existem algumas personalizações que podem ser feitas nos métodos forneceidos para serem sobscritos, mas se você não precisar pode usar apenas o construtor para preencher o atributo da herança. Eu utilizei também o OnModelCreating para criar um seeding das roles que queria adicionar ao sistema.

### Fazer as configurações


## Não esquecer
- Autenticação : Verificar que alguém é quem diz ser.
- Autorização : Verificar que alguém tem acesso a um conteúdo.
