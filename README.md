# IdentityUsage
Este é um projeto de estudo que visa somente ser um guia para lembrar-me de como usar essa biblioteca em projetos reais. Visa ser "step-by-step" prático para implementação no futuro. 

# Quais passos devem ser seguidos?

## Instalações
 - **Aspnet Core Identity**  : Contém todas as ferramentas da MS para autenticação e autorização.
 - **Apsnet Core Authentication JwtBearer** : Contém as ferramentas necessárias para gerar um token JWT.
 - **Entity Framework Core** : Ferramentas do EF Core.
 - **Entity Framework Core Design** : Ferramentas do EF Core para logica em "tempo de design".
 - **Pomelo Entity Framework Core MySql** : Ferramenta do EF Core para conexão com MySql.

 ## Configurações
**OBS:** Este projeto tem compose para docker que configura um banco MySql local necessário para rodar tudo que será descrito  aqui abaixo. A partir desse momento vou considerar que o  `docker compose up` já tenha sido rodado com suceso :D.

### Criar um Context
Geralmente criamos uma classe de contexto que herda de DbContext, nesse caso vamos herdar de `IdentityContext`.



## Não esquecer
- Autenticação : Verificar que alguém é quem diz ser.
- Autorização : Verificar que alguém tem acesso a um conteúdo.
