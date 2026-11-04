# 🔒 Sistema de Autenticação Seguro com Verificação por E-mail (ASP.NET Core MVC) (.NET 10)

Este projeto foi desenvolvido para aplicar de forma prática os conceitos de **Segurança da Informação (SI)** e **Defesa Cibernética** estudados na faculdade de Análise e Desenvolvimento de Sistemas (ADS). O objetivo é criar um sistema de autenticação que seja seguro, resiliente a falhas e protegido contra ataques comuns.

---

## 🌐 Demonstração Online (Como Testar)

A aplicação está publicada e pronta para testes práticos de segurança. Você pode interagir com os mecanismos de defesa ativos acessando o link abaixo:
➡️ https://authorizationsytem.runasp.net/

## 🧪 Cenários Sugeridos para Testes de Invasão (Pentest)

Para validar as defesas implementadas, tente simular os seguintes comportamentos no ambiente ativo:
* **Teste de Força Bruta (Rate Limiting):** Tente realizar requisições de login ou cadastro seguidas de forma rápida. O sistema interceptará o tráfego a nível de aplicação, gerará o código HTTP 429 e redirecionará seu navegador automaticamente para uma página customizada de bloqueio por IP.

* **Teste de Enumeração de Usuários:** Tente criar uma conta com um e-mail já existente. O sistema adotará a técnica do Sucesso Falso, avançando no fluxo lógico sem dar pistas textuais na tela se o e-mail possui ou não cadastro na base de dados.

---

## 🚀 Etapas de Desenvolvimento do Sistema

Acompanhamento do progresso de implementação dos requisitos de segurança do sistema:

- [x] Adoção de padrões de senhas fortes.
- [x] Uso de algoritmos de Hashing (BCrypt) para persistência segura de senhas no banco de dados.
- [x] Gerenciamento confidencial de credenciais do servidor através de variáveis de ambiente (`.env`).
- [x] Proteção ativa contra Engenharia Social e Enumeração de Usuários (*User Enumeration*).
- [x] Verificação e ativação de novas contas através do código enviado por e-mail (*MFA*).
- [x] Controle de acesso por Autorização em rotas protegidas (*Authorize*).
- [x] Mecanismo de bloqueio contra ataques de força bruta (*Rate Limiting*).
- [x] Proteção ativa a nível de aplicação contra DoS e DDoS.

---

## 🛡️ Conceitos de Segurança da Informação Aplicada

Abaixo estão detalhados os pilares de segurança implementados no sistema e a justificativa técnica por trás de cada defesa:

### 1. Mecanismo de Defesa a Nível de Aplicação (Rate Limiting)
Para garantir a disponibilidade da aplicação contra abusos e ataques de Negação de Serviço, foi adotada uma estratégia de **Defesa em Camadas** no pipeline do ASP.NET Core:

* **Proteção Global (Anti-DoS) - *Sliding Window*:** Implementado globalmente na raiz da aplicação baseado no IP do cliente. A Janela Deslizante calcula o tráfego em tempo real de forma contínua, impedindo que atacantes gerem picos artificiais de requisições na virada de blocos de tempo (vulnerabilidade comum da Janela Fixa).
* **Proteção de Autenticação (Anti-Brute Force) - *Token Bucket*:** Aplicado especificamente nos endpoints críticos de autenticação (`/Login` e `/RegisterAccount`). O Balde de Tokens permite a flexibilidade de navegação de um usuário humano real que pode errar a senha algumas vezes rápido, mas restringe drasticamente scripts automatizados a um ritmo controlado de recarga assim que o balde esvazia.

### 2. Criptografia e Gerenciamento de Identidades
* **Gerenciamento de Senhas:** Padronização de senhas fortes através de expressões regulares (Regex), visando a mitigação de dicionários de força bruta simples e impedindo que usuários criem credenciais fracas.
* **Criptografia de Senhas (Hashing):** Armazenamento de senhas no banco de dados utilizando criptografia forte com o algoritmo **BCrypt**, garantindo que as credenciais reais nunca fiquem legíveis, mesmo em caso de vazamento da base de dados.
* **Autenticação Multifator (MFA):** Implementação de código de confirmação de 6 dígitos via serviço de e-mail (SMTP) nos fluxos de Registro e Recuperação de Senha, garantindo o princípio da autenticidade.

### 3. Proteções Lógicas e de Código
* **Proteção contra Enumeração de Usuários (*User Enumeration*):** Se um usuário tentar cadastrar um e-mail que já existe, o sistema adota a técnica do *Sucesso Falso* e avança sem dar pistas visuais. Isso impede que atacantes descubram quais e-mails possuem conta no sistema através de mensagens de erros específicos na tela.
* **Controle de Acesso (*Access Control*):** Implementação de defesa em profundidade para restringir o acesso a endpoints internos via ferramentas como Postman, garantindo que as rotas só fiquem acessíveis se o fluxo lógico anterior for concluído com sucesso.
* **Confidencialidade das Credenciais:** Uso de variáveis de ambiente (`.env`) para isolar chaves secretas e credenciais do servidor de e-mail (SMTP), garantindo que dados sensíveis nunca fiquem expostos no código-fonte ou em repositórios públicos.

---

## 🎯 Por que eu desenvolvi este projeto?

Este projeto foi desenvolvido para aplicar na prática os conceitos de **Segurança da Informação e Defesa Cibernética** com o desenvolvimento de **software seguro** e resiliente. Os esforços no desenvolvimento concentraram-se em mitigar ataques comuns e conhecidos, como *Brute Force*, *DoS* e *User Enumeration*, através da adoção de boas práticas relacionadas ao desenvolvimento de software.

---

## 🏛️ Por que escolhi a Arquitetura MVC?

A escolha pelo padrão **MVC (Model-View-Controller)** foi baseada levando em consideração o escopo do projeto, possibilitando direcionar o foco na lógica do **Backend** aplicada aos fluxos principais de *login*, *registro de novas contas* e *recuperação de senhas*, e, posteriormente, facilitando o deploy por se tratar de uma aplicação monólitica.

---

## 📐 Padrões de Design e Clean Code aplicados
Inicialmente, alguns fluxos da camada de serviços foram desenvolvidos em métodos **lineares** (inline) para validar a lógica de negócio de ponta a ponta. Posteriormente, foram aplicados conceitos de **Clean Code**, como **Prrincípio da Reponsabilidade Única/Segregação de Interface** e refatoração para extrair a lógica em métodos privados estáticos e helpers. Isso melhorou a legibilidade da orquestração principal, removeu 'Magic Numbers' usando constantes centralizadas e garantiu o Princípio da Responsabilidade Única dentro da classe de serviço.

--- 


## 📖 Documentação Completa

Mais detalhes sobre a documentação desse projeto você pode acompanhar através do meu perfl profisional no linkedin.
🔗 https://www.linkedin.com/in/hilton-camara-developer

---


## 🤝 Sugestões de Melhorias e Contato

Este projeto está em constante evolução e aberto a feedbacks voltados para Engenharia de Segurança e Otimização de Código. Sinta-se à vontade para abrir uma Issue, enviar um Pull Request com melhorias ou entrar em contato através dos canais abaixo:
• LinkedIn: https://www.linkedin.com/in/hilton-camara-developer
• E-mail: [kalebeyuuzaki@gmail.com]

---