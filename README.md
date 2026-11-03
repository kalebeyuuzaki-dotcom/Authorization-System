# 🔒 Sistema de Autenticação Seguro com Verificação por E-mail (ASP.NET Core MVC) (.NET 10)

Este projeto foi desenvolvido para aplicar de forma prática os conceitos de **Segurança da Informação (SI)** e **Defesa Cibernética** estudados na faculdade de Análise e Desenvolvimento de Sistemas (ADS). O objetivo é criar um sistema de autenticação que seja seguro, resiliente a falhas e protegido contra ataques comuns.

## 🚀 Etapas de Desenvolvimento do Sistema

Acompanhamento do progresso de implementação dos requisitos de segurança do sistema:

- [x] Adoção de padrões de senhas fortes.
- [x] Uso de algoritmos de Hashing (BCrypt) para persistência segura de senhas no banco de dados.
- [x] Gerenciamento confidencial de credenciais do servidor através de variáveis de ambiente (`.env`).
- [x] Proteção ativa contra Engenharia Social e Enumeração de Usuários (*User Enumeration*).
- [x] Verificação e ativação de novas contas através do código enviado por e-mail (*MFA*).
- [x] Controle de acesso por Autorização em rotas protegidas (*Authorize*).
- [x] Mecanismo de bloqueio contra ataques de força bruta (*Rate Limiting*).
- [ ] Prevenção de ataques de negação de serviço (*DoS e DDoS*).


## 🛡️ Conceitos de Segurança da Informação Aplicados
* **Gerenciamento de Senhas:** Padronização de senhas fortes através de expressões Regex, visando a mitigação de ataques de força fruta, impedindo que usuários criem senhas muito curtas e fáceis.
* **Criptografia de Senhas (Hashing):** Armazenamento de senhas no banco de dados utilizando criptografia forte com o algoritmo BCrypt, garantindo que as credenciais reais nunca fiquem legíveis, mesmo em caso de vazamento da base de dados.
* **Proteção contra Enumeração de Usuários (User Enumeration):** Se um usuário tentar cadastrar um e-mail que já existe, o sistema adota a técnica do *Sucesso Falso* e avança sem dar pistas visuais. Isso impede que atacantes descubram quais e-mails possuem conta no sistema através de mensagens de erros específicos na tela.
* **Autenticação Multifator (MFA):** Implementação de código de confirmação de 6 dígitos via serviço de e-mail (SMTP) nos fluxos de Registro e Recuperação de Senha, garantindo a autenticidade das informações.
* **Limites de tentativas (Rate Limiting):** Limite de até 3 tentativas que podem ser feitas na tela de confirmação de código. Após esse limite, é necessário o usuário solicitar um novo código.
* **Controle de acesso a rotas protegidas (Access Control):** Implementação de defesa em profundidade para restringir o acesso a endpoints internos via Postman, garantindo que as rotas só fiquem acessíveis se o fluxo lógico anterior for concluído com sucesso.
* **Confidencialidade (Proteção de Credenciais/Hardcoded):** Uso de variáveis de ambiente (`.env`) para isolar chaves secretas e credenciais do servidor de e-mail (SMTP), garantindo que dados sensíveis nunca fiquem expostos no código-fonte ou em repositórios públicos.

## 📐 Padrões de Design e Clean Code aplicados
Inicialmente, o fluxo de registro foi desenvolvido em métodos **lineares** (inline) para validar a lógica de negócio de ponta a ponta. Posteriormente, apliquei conceitos de **Clean Code** e refatoração para extrair a lógica em métodos privados estáticos e helpers. Isso melhorou a legibilidade da orquestração principal, removeu 'Magic Numbers' usando constantes centralizadas e garantiu o Princípio da Responsabilidade Única dentro da classe de serviço.