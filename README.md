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
Para fazer as configurações vou criar uma classe separada, mas você pode cria-las diretamente  no startup / program. Vou criar um método de extensão que posa ser usado como extensão do `IServiceCollection`, nele vamos colocar todo o código relacionado as configurações da gestão de Identidade com o JWT. Você pode olhar: `IdentityUsage.Configuracoes.IdentityConfiguration`, eu apenas vou listar aqui as configurações:

- **Bind do Objeto JwtConfiguration** - Esse Bind permite obter as propriedades gravadas no Json para ativar o tipo JwtConfiguration que tem os mesmo campos, e configurar assim a sua Injeção para o código. 
- **Geração da chave assimétrica** - Gera novo objeto para a chave assimétrica do Json baseado na SecurityKey do json. Por isso que é tão importante armazenar de forma segura.
- **Configurar a Autenticação** - O Identity permite que algumas coisas sejam configuradas, você pode encontar todas elas aqui: [IdentityOptions](https://learn.microsoft.com/pt-br/dotnet/api/microsoft.aspnetcore.identity.identityoptions?view=aspnetcore-8.0). Para esse caso estamos usando:

    - **Senha** (Password): Requer dígitos(números) nas senha. Requer Letras mininúsculas. Requer Caracteres especiais. Requer Letras maiúsculas. Requer tamnho mínimo de 8 caracteres. 
    - **Bloqueio** : Permitir novos usuários. Tempo padrão de bloqueio de 2 minutos. Quantidade máxima de erros de senha de 3 tentativas.
    - **Usuário** : Requer email único entre os cadastrados.
    - **SignIn** : Requer que o email seja confirmado.

- **Criar configuração do JWT**:  O Jwt pode ter algumas informações no seu corpo, algumas delas são padrões, e outras podem ser adicionadas, quando usamos o ojeto TokenValidationsParameters podemos configurar quais dados do corpo do token Jwt precisam ser validados como uma camada adicional de segurança. Em meu caso estou usando todas as informações que estamos adicionando: Issuer, Audience, ChaveAssimétrica e Expiração.

- **Configurar a Autenticação** : Finalmente adicionamos a autenticação como serviço usando os parametros acima criados. 

- **Configurar os demais serviços** : Para que tudo não virasse um macarrão eu separei a adição dos outros serviços em um outro método de extensão em: `IdentityUsage.Configuracoes.Services`. Nele estou adicionando o Context para o Entity, e adicionando as configurações do Identity indicando novamente as classes `UserPersonalization` e `RolePersonalization`.

E como usar esse métodos? Este metodos são de extessão por isso eles recebem `this` na frente do primeiro parâmetro da assinatura, o que significa que podemos chama-los de qualquer propriedade ou método que retorne o tipo `IServiceColletion`. Por sorte o `builder.Services` que usamos na program é exatamente desse tipo rs :D. Então apenes chame os métodos que criamos agora na startup / program. 

    builder.Services.RegisterServices(builder.Configuration);
    builder.Services.ConfigureAuthentication(builder.Configuration);

além disso é importante adicionar nessa classe também o uso do a Autenticação e Autorização no framework para isso adicione: 

    app.UseAuthentication();
    app.UseAuthorization();

É importante que estes sejam usados na ordem aqui descrita para que o serviços funcionem corretamente, e que como pode parecer intuitivo que isso seja chamado antes da `app.Run()`.

## Gerar e rodar as migrations
A apartir desse ponto já poderiamos rodar as migrations e inicializar o projeto, não veriamos nada de diferente na execução da App, mas ao olhar o banco deveriamos ver várias tabelas que o Identity criou automaticamente, com foco principalmente na tabela de usuário que deve ter adicionalmente as propriedades que colocamos na Classe `UserPersonalization`. Existem  muitas formas de rodar e aplicar as migrations, geralmente prefiro adiciona-las pelo package-manager via: 

    Add-migration [name]

E usar o `Database.Migrate` no startup / program, para garantir que toda execução da App aplique a ultima versão do banco.

## Não esquecer
- Autenticação : Verificar que alguém é quem diz ser.
- Autorização : Verificar que alguém tem acesso a um conteúdo.
