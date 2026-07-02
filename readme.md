# MicMute 🎙️

[![Release](https://img.shields.io/github/v/release/AllvesMatteus/MicMute?style=flat-flat&color=32d74b)](https://github.com/AllvesMatteus/MicMute/releases)
[![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg)](LICENSE)

O **MicMute** é um aplicativo leve e de alta performance para Windows que permite silenciar e ativar instantaneamente o seu microfone padrão (ou um dispositivo específico) com um clique na bandeja do sistema (System Tray) ou através de teclas de atalho personalizáveis globais.

Projetado com uma interface escura (Dark Theme) moderna no estilo Windows 11, o MicMute se integra perfeitamente ao sistema operacional e oferece feedback visual e sonoro imediato a cada alteração de estado.

---

## 📸 Demonstração da Interface

![Interface do MicMute](assets/icons/micon.png) *(Substitua pelo print do aplicativo em execução)*

---

## ✨ Funcionalidades Principais

- 🎙️ **Alternador de Mudo Global**: Silencie seu microfone em qualquer lugar através de atalhos globais configuráveis.
- 🎛️ **Seleção Dinâmica de Dispositivo**: Escolha qual microfone controlar diretamente dentro das configurações do aplicativo.
- 🔊 **Feedback Sonoro Personalizado**: Sons curtos e agradáveis tocam ao silenciar e ao reativar o microfone (carregados dinamicamente).
- 🚀 **Iniciar com o Windows**: Opção nativa (estilo Windows 11 Toggle Switch) para iniciar o aplicativo automaticamente no logon do sistema.
- 🎨 **Interface Dark Windows 11**: Janela e menu da bandeja com visual moderno, cantos arredondados e cores integradas ao DWM do sistema.
- 💻 **Instalador Robusto**: Empacotado em um instalador padrão do Windows (.exe) com suporte a arquitetura de 64 bits e seleção livre de pastas.

---

## 🛠️ Como Instalar e Executar

1. **Baixe o Instalador**: Acesse a aba de [Releases](https://github.com/AllvesMatteus/MicMute/releases) e faça o download do `MicMute_Setup.exe`.
2. **Requisitos de Sistema**: O aplicativo roda em qualquer versão do Windows (Windows 7 SP1, 8, 8.1, 10, 11) com o **Microsoft .NET Framework 4.8**. O instalador verificará a presença dele automaticamente. Todas as dependências adicionais (.DLLs) estão totalmente embutidas no executável principal.
3. **Execute a Instalação**: Siga os passos na tela e selecione a pasta de instalação desejada (o padrão é `C:\Program Files (x86)\MicMute`).
4. **Inicie o Aplicativo**: O MicMute iniciará minimizado diretamente na sua bandeja do sistema (ao lado do relógio).

---

## ⌨️ Atalhos de Teclado (Hotkeys)

Você pode registrar três tipos de atalhos globais diferentes dentro do app:
1. **Alternar (Toggle)**: Uma mesma tecla atalho para mutar/desmutar ciclicamente.
2. **Mutar (Mute)**: Atalho específico apenas para silenciar.
3. **Desmutar (Unmute)**: Atalho específico apenas para ativar.

*Basta clicar no campo correspondente, digitar a combinação desejada e ela será salva automaticamente. Os nomes das teclas e o status "Nenhum" adaptam-se automaticamente ao idioma inglês se selecionado.*

---

## 🔊 Customização de Sons de Feedback

O aplicativo busca os arquivos de som nas seguintes pastas relativas ao executável:
- Mutado: `assets/sounds/muted.mp3`
- Ativado: `assets/sounds/unmuted.mp3`

Você pode substituir esses arquivos por quaisquer sons de sua preferência no formato `.wav` ou `.mp3` para personalizar o feedback sonoro do seu sistema.

---

## 🔧 Solução de Problemas

### Os atalhos não funcionam dentro de jogos ou programas em tela cheia
- **Causa**: Programas que executam com privilégios de administrador (Admin) bloqueiam a escuta de teclas por aplicativos que rodam no modo padrão do usuário.
- **Solução**: Execute o MicMute como Administrador. Para tornar permanente:
  1. Clique com o botão direito sobre o atalho ou executável do `MicMute.exe`.
  2. Acesse **Propriedades** > aba **Compatibilidade**.
  3. Marque a opção **"Executar este programa como administrador"** e clique em **Aplicar**.

---

## 💻 Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: .NET Framework 4.8 (Windows Forms)
- **Áudio**: [AudioSwitcher](https://github.com/xenolightning/AudioSwitcher) para integração com dispositivos de áudio CoreAudio do Windows.
- **Atalhos**: [Shortcut](https://github.com/AlexArchive/Shortcut) para registro de Hotkeys de baixo nível.
- **Instalador**: [Inno Setup 6](https://www.innosetup.com) para empacotamento.

---

## 👤 Autor

Desenvolvido com carinho por **Mateus Alves**. Conecte-se comigo!

- 💻 **GitHub**: [@AllvesMatteus](https://github.com/AllvesMatteus)
- 👔 **LinkedIn**: [Mateus Alves](https://www.linkedin.com/in/allves-matteus/)

---

## 📄 Licença

Este projeto está licenciado sob a licença MIT - consulte o arquivo [LICENSE](LICENSE) para obter mais detalhes.
