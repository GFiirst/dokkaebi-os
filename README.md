# Dokkaebi OS

Dokkaebi OS é uma aplicação desktop para descoberta e, futuramente, monitoramento de dispositivos em uma rede local. O projeto foi inspirado na personagem Dokkaebi, de *Tom Clancy's Rainbow Six Siege*. No jogo, o gadget da operadora invade os celulares adversários e os faz tocar, revelando sua presença. Como reproduzir esse comportamento em aparelhos reais não é o objetivo do projeto, a ideia foi reinterpretada de forma segura: localizar os dispositivos conectados à mesma rede e apresentar informações úteis sobre eles.

Na versão atual, o Dokkaebi OS funciona como o primeiro estágio de um analisador de rede inspirado no Wireshark. Ele identifica hosts conhecidos pela rede local e exibe seus endereços IP e MAC, além do fabricante associado ao prefixo do MAC. A próxima grande etapa planejada é permitir a observação do tráfego de cada IP, transformando a interface atual em uma ferramenta mais completa de diagnóstico e aprendizado sobre redes.

> Use o projeto somente em redes próprias ou em ambientes nos quais você tenha autorização para realizar varreduras e capturas de tráfego.

## Estado atual

O projeto implementa:

- detecção automática da rede IPv4 ativa;
- varredura dos endereços do segmento local por meio de `ping`;
- consulta à tabela de vizinhos ARP do sistema operacional;
- suporte a descoberta de dispositivos no Windows e no Linux;
- exibição de endereço IP, endereço MAC, status e fabricante;
- identificação do fabricante por uma base local de prefixos MAC (OUI);
- interface desktop temática construída com Avalonia UI.

O fluxo atual da descoberta é:

1. A aplicação encontra a primeira interface Ethernet ou Wi-Fi ativa com um endereço IPv4.
2. O segmento `/24` correspondente é percorrido nos endereços de final `2` a `255`.
3. Uma tentativa curta de `ping` é enviada a cada endereço para estimular o preenchimento da tabela ARP.
4. A tabela ARP é consultada com `ip neigh` no Linux ou `arp -a` no Windows.
5. Os dispositivos encontrados são relacionados aos fabricantes cadastrados em `Data/mac-vendors.csv` e apresentados na interface.

Um dispositivo pode aparecer mesmo sem responder diretamente ao `ping`, desde que esteja presente na tabela ARP. Da mesma forma, equipamentos fora dessa tabela não serão apresentados.

## Tecnologias

- C# e .NET 10;
- Avalonia UI 12;
- CommunityToolkit.Mvvm;
- padrão de apresentação MVVM;
- comandos nativos de rede do Windows e do Linux.

## Como executar

### Pré-requisitos

- SDK do .NET 10 instalado;
- Windows ou Linux;
- no Linux, o comando `ip` disponível no sistema (normalmente fornecido pelo pacote `iproute2`);
- conexão a uma rede local por Ethernet ou Wi-Fi.

Na raiz do repositório, execute:

```bash
dotnet restore
dotnet run
```

A varredura começa automaticamente quando a lista de rede é carregada. O tempo e a quantidade de dispositivos encontrados dependem da rede, das regras de firewall e do estado da tabela ARP do computador.

## Estrutura do projeto

```text
Models/       Modelos exibidos pela aplicação
Services/     Descoberta de rede, leitura de ARP e identificação de fabricantes
ViewModels/   Estado e lógica de apresentação
Views/        Janela e componentes da interface Avalonia
Assets/       Imagens e identidade visual
Data/         Base local de fabricantes por prefixo MAC
Installer/    Configuração do instalador para Windows
```

## Limitações conhecidas

- Ainda não há captura ou inspeção de pacotes de rede.
- A aplicação assume uma rede IPv4 `/24`; outras máscaras de sub-rede ainda não são consideradas.
- Apenas a primeira interface Ethernet ou Wi-Fi ativa encontrada é utilizada.
- macOS e outros sistemas operacionais ainda não são suportados pelo serviço de ARP.
- Os controles de ordenação, favoritos e dispositivos confiáveis presentes na interface ainda não possuem comportamento implementado.
- A identificação do fabricante depende da cobertura e da atualização do arquivo local de fabricantes.
- A varredura atual não deve ser interpretada como um inventário completo da rede.

## Próximos passos

A evolução principal planejada é adicionar uma visualização de tráfego por dispositivo/IP. Para chegar a esse objetivo, o projeto poderá evoluir em etapas:

- capturar pacotes de uma interface de rede escolhida pelo usuário;
- agrupar o tráfego por IP de origem e destino;
- exibir protocolo, portas, volume de dados e horário dos pacotes;
- permitir selecionar um dispositivo para acompanhar somente seu tráfego;
- implementar filtros funcionais e pesquisa;
- adicionar atualização periódica e cancelamento da varredura;
- detectar corretamente a máscara e o intervalo de cada sub-rede;
- oferecer exportação dos resultados para análise posterior.

Capturar tráfego pode exigir permissões elevadas e uma biblioteca ou driver específico de captura, dependendo do sistema operacional. Essa funcionalidade deve respeitar os limites da rede local e a autorização de seus responsáveis.

## Objetivo educacional

O Dokkaebi OS está sendo desenvolvido como um projeto de estudo sobre C#, aplicações desktop multiplataforma, arquitetura MVVM e fundamentos de redes. Ele não tenta substituir ferramentas maduras como o Wireshark; sua proposta é construir, passo a passo, uma interface acessível para entender quais dispositivos estão presentes em uma rede e como eles se comunicam.

Este é um projeto de fã e não possui afiliação com a Ubisoft. *Tom Clancy's Rainbow Six Siege*, Dokkaebi e seus elementos relacionados pertencem aos respectivos titulares de direitos.
