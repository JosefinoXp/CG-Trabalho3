# Trabalho 3 CG (VR Shooter Narrativo)

Um projeto/jogo shooter de XR desenvolvido em Unity focado em Narrativa, Interações e Armas.

![Banner ou Screenshot Principal do Projeto](caminho/para/imagem.png)

## Alunos

Aliana Wakassugui de Paula e Silva
José Lucas Hoppe Macedo

## 📄 Sobre o Projeto

Este projeto foi desenvolvido como parte da matéria de Computação Gráfica (CG) para fazer um jogo narrativo e com vídeos. O objetivo principal é criar uma experiência imersiva que integra sistemas complexos de Armamento com Sockets, habilidades especiais e narrativas.

O ambiente utiliza o **Built-in Render Pipeline** para garantir compatibilidade com assets específicos e shaders customizados.

## ✨ Funcionalidades Principais

### 🧠 Sistema IA NavMesh
Uma arquitetura de Inteligência Artificial unificada para inimigos e NPCs:
* **Comportamento Unificado:** Script `Enemy` centralizado que gerencia estados.
* **Módulos de Detecção:** Sistema de visão para identificar o jogador em diferentes condições.
* **Combate e Patrulha:** Estados de patrulha aleatória e engajamento de combate dinâmico.

### 🕶️ Interações VR
Utilizando o **XR Interaction Toolkit**:
* **Manipulação de Objetos:** Mecânicas de levitação para o jogador e interação física via Sockets.
* **Mãos Virtuais:** Animações responsivas baseadas no input do controle.

### 🔄 Sistema de Progressão
* **Gerenciamento de Cenas:** Transição automática de estágios baseada na eliminação de grupos de inimigos.
* **Uso de Timelines:** Uso de narrativas como vídeos e progressão de cenários através de interações físicas.

## 🛠️ Tecnologias e Assets

* **Engine:** Unity 2021.3 (Built-in Render Pipeline)
* **XR:** Unity XR Interaction Toolkit
* **Fotogrametria** Uso de cenários com fotogramétria para maior imersão
* **Linguagem:** C#

## 🚀 Como Executar

1.  Clone este repositório:
    ```bash
    git clone [https://github.com/seu-usuario/seu-projeto.git](https://github.com/seu-usuario/seu-projeto.git)
    ```
2.  Abra o projeto via **Unity Hub** (Certifique-se de ter a versão `2021.3` instalada).
3.  Abra a cena `Prologue` em `Assets/Scenes` e dê Play.
4.  Caso quiser ver a Demo, procure por `DevScene`