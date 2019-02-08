using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Grade : MonoBehaviour {

    //numero de linhas e colunas da grade
    public const int linhas = 20;
    public const int colunas = 10;

    //delay entre bacterias serem enfraquecidas e elas poderem apagar
    public const float delayEnfraquecimento = 1f;

    //matriz com quadrados
    public QuadradoGrade[][] quadrados = new QuadradoGrade[linhas][];

    //prefab de um quadrado
    public QuadradoGrade quadrado;

    //dicionario com os prefabs de cada tipo de peça
    public Peca[] prefabs = new Peca[(int)TipoPeca.NUM_TIPOS];

    //prefab de bactéria
    public QuadradoBacteria quadradoBacteria;

    //objeto que spawna peças
    public SpawnPecas spawn { get; set; }

    //instancia do game manager
    private GameManager gameManager;

    private void Awake()
    {
        MontaGrade();

        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Posiciona os quadrados da grade
    /// </summary>
    private void MontaGrade()
    {
        //define tamanho dos quadrados
        float tamanhoX = quadrado.GetComponent<SpriteRenderer>().bounds.size.x * transform.localScale.x;
        float tamanhoY = quadrado.GetComponent<SpriteRenderer>().bounds.size.y * transform.localScale.y;

        //posiciona quadrados
        for ( int i = 0; i < linhas; i++ )
        {
            quadrados[i] = new QuadradoGrade[colunas];
            for( int j = 0; j < colunas; j++)
            {
                QuadradoGrade novoQuadrado = Instantiate(quadrado, transform.position + new Vector3(tamanhoX * j, -tamanhoY * i, 0), Quaternion.identity, transform);
                novoQuadrado.Esvazia();
                novoQuadrado.posicao = new Vector2Int(i, j);
                quadrados[i][j] = novoQuadrado;
            }
        }
    }

    /// <summary>
    /// Executa tarefas que devem ser feitas após uma peça terminar de cair.
    /// A peça vai de linhaMin até linhaMax
    /// </summary>
    /// <param name="peca"></param>
    public void AposQueda(int linhaMin, int linhaMax)
    {
        Sequence sequenciaFade = DOTween.Sequence();    //sequencia usada para dar o fade nas linhas apagadas
        Sequence sequenciaDesce = DOTween.Sequence();   //sequencia usada para fazer as linhas caírem

        //Fila de métodos que serão executados após fim das animações de queda das peças
        //São métodos que apagam as peças que devem ser pagadas e descem as peças que devem cair
        List<Action> listaMetodos = new List<Action>();

        //começa animação de queda das peças depois que as linhas completas desaparecerem
        sequenciaFade.OnComplete(() => sequenciaDesce.Play());

        //executa métodos da fila após fim da animação de queda
        sequenciaDesce.OnComplete(() => listaMetodos.ForEach(action => action()));

        //apaga linhas que foram completadas pela peça
        int naoApagadas = ApagaCompletasDesceIncompletas(linhaMin, linhaMax, sequenciaFade, sequenciaDesce, listaMetodos); //número de linhas que não foram apagadas
        
        //desce as linhas que estão em cima das peças apagadas
        DesceLinhas(linhaMax - naoApagadas, linhaMin, sequenciaDesce, listaMetodos);

        //adiciona criação da próxima peça no final da lista de metodos
        listaMetodos.Add(() => StartCoroutine(gameManager.SegueLoopTetris()));

        sequenciaFade.Pause();
        sequenciaDesce.Pause();

        sequenciaFade.Play();
    }

    /// <summary>
    /// Apaga as linhas dentro do intervalo [linhaMin,linhaMax] que estiverem completas e desce as linhas que não estiverem.
    /// Retorna o número de linhas que não foram completadas pela peça (dentre as linhas em que a peça ocupou).
    /// Tudo isso após o término de tweens
    /// </summary>
    private int ApagaCompletasDesceIncompletas(int linhaMin, int linhaMax, Sequence sequenciaFade, Sequence sequenciaDesce, List<Action> listaMetodos)
    {

        int naoApagadas = 0;  //número de linhas que não puderam ser apagadas

        //apaga linhas que estiverem completas
        for( int i = linhaMax; i >= linhaMin; i--)
        {
            if (LinhaCompleta(i))
            {
                int iNotClosure = i;    //variável para driblar o closure ao usar ApagaLinha na expressão labda

                //cria sequencia para animar as peças sumindo
                Sequence novaSequencia = Efeitos.FadeLinha(quadrados[iNotClosure]);
                //adiciona sequencia na sequencia com todas as animações de peças sumindo
                sequenciaFade.Insert(0, novaSequencia);

                //adiciona na lista método que apaga as peças quando a sequencia acabar
                listaMetodos.Add(() => ApagaLinha(iNotClosure));
            }
            else
            {
                //caso alguma linha embaixo tenha sido apagada
                //garante que a animação só seja feita se a linha realmente for cair
                if (naoApagadas != i)
                {
                    int iNotClosure = i;    //variável para driblar o closure
                    int naoApagadasNotClosure = naoApagadas;    //variável para driblar o closure

                    //cria sequencia para animar queda da linha
                    Sequence novaSequencia = Efeitos.MoveLinha(quadrados[iNotClosure], quadrados[linhaMax - naoApagadas]);
                    //adiciona sequencia na sequencia com todas as animações de queda
                    sequenciaDesce.Insert(0, novaSequencia);

                    //sem esse comando a sequencia para de funcionar misteriosamente
                    novaSequencia.OnComplete(() => { });

                    //adiciona na lista método que move as peças
                    listaMetodos.Add(() => MoveLinha(iNotClosure, linhaMax - naoApagadasNotClosure));
                }
                naoApagadas++;
            }
        }

        return naoApagadas;
    }

    /// <summary>
    /// Checa se linha está completa com peças e/ou bactérias engraquecidas.
    /// </summary>
    /// <param name="linha"></param>
    /// <returns></returns>
    private bool LinhaCompleta(int linha)
    {
        for( int j = 0; j < colunas; j++)
        {
            QuadradoGrade quadrado = quadrados[linha][j];

            //caso campo da grade esteja vazio
            if (quadrado.interior == Preenchimento.Livre)
            {
                return false;
            }
            
            //caso campo esteja preenchido por uma bactéria
            if (quadrado.interior == Preenchimento.Bacteria)
            {
                //caso bactéria não esteja enfraquecida
                if (!((QuadradoBacteria)quadrado.quadradoPeca).bacteriaEnfraquecida)
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    /// <summary>
    /// Checa se existe algum quadrado de peça ou bactéria na linha superior de um certa coluna.
    /// </summary>
    /// <returns></returns>
    private bool ColunaCheia(int coluna)
    {
        return quadrados[0][coluna].interior != Preenchimento.Livre;
    }

    /// <summary>
    /// Esvazia linha, deixando todos os espaços livres, e
    /// apaga as peças que estão na linha
    /// </summary>
    /// <param name="linha"></param>
    private void ApagaLinha(int linha)
    {
        for (int j = 0; j < colunas; j++)
        {
            QuadradoGrade quadrado = quadrados[linha][j];

            //esvazia espaço
            quadrado.Esvazia();
        }
    }

    /// <summary>
    /// Diz qual é a primeira linha da grade preenchida por alguma peça ou bactéria.
    /// Retorna índice inválido caso nenhuma linha esteja preenchida.
    /// Aparentemente não é utilizada
    /// </summary>
    /// <returns></returns>
    private int PrimeiraLinhaPreenchida()
    {
        for( int i = 0; i < linhas; i++)
        {
            for( int j = 0; j < colunas; j++)
            {
                if( quadrados[i][j].interior != Preenchimento.Livre)
                {
                    return i;
                }
            }
        }

        Debug.Log("Nenhuma linha preenchida, está função não deveria estar sendo chamada");
        return -1;
    }

    /// <summary>
    /// Move peças da linha 'origem' para a linha 'destino'
    /// </summary>
    /// <param name="origem"></param>
    /// <param name="destino"></param>
    private void MoveLinha(int origem, int destino)
    {
        if (origem != destino)
        {
            for (int j = 0; j < colunas; j++)
            {
                QuadradoGrade quadradoOrigem = quadrados[origem][j];
                QuadradoGrade quadradoDestino = quadrados[destino][j];

                if (quadradoOrigem.quadradoPeca != null)
                {
                    //copia peça do quadrado de origem para o quadrado destino
                    quadradoDestino.Preenche(Instantiate(quadradoOrigem.quadradoPeca, transform));
                }

                //apaga peça do quadrado de origem
                quadradoOrigem.Esvazia();
            }
        }
    }

    /// <summary>
    /// Faz o conjunto de linhas acima de 'linhaTopo' cair até 'linhaBase'
    /// </summary>
    /// <param name="linhaBase"></param>
    /// <param name="linhaTopo"></param>
    private void DesceLinhas(int linhaBase, int linhaTopo, Sequence sequenciaDesce, List<Action> listaMetodos)
    {
        //caso a base esteja embaixo ou junto do topo
        if (linhaBase >= linhaTopo)
        {
            for (int i = linhaTopo - 1; i >= 0; i--)
            {
                int iNotClosure = i;    //variável para driblar o closure

                int linhaFinal = linhaBase + iNotClosure - linhaTopo + 1; //linha em que a linha de quadrados atual deve terminar a queda

                //cria sequencia para animar queda da linha
                Sequence novaSequencia = Efeitos.MoveLinha(quadrados[iNotClosure], quadrados[linhaFinal]);
                //adiciona sequencia na sequencia com todas as animações de queda
                sequenciaDesce.Insert(0, novaSequencia);

                //sem esse comando a sequencia para de funcionar misteriosamente
                novaSequencia.OnComplete(() => { });

                //adiciona na lista método que move as peças
                listaMetodos.Add(() => MoveLinha(iNotClosure, linhaFinal));
            }
        }
    }

    /// <summary>
    /// Instancia a próxima peça da fila
    /// </summary>
    public void CriaPeca()
    {   
        TipoPeca tipo = spawn.ProximaPeca();
        //TipoPeca tipo = TipoPeca.I;
        Instantiate(prefabs[(int)tipo], transform);
    }

    /// <summary>
    /// Cria bactéria fora da grade e faz ela cair até
    /// encontrar outro bloco ou o fim da grade.
    /// </summary>
    public void CriaBacteria()
    {
        int coluna =  UnityEngine.Random.Range(0,colunas);
        QuadradoBacteria bacteria = Instantiate(quadradoBacteria, transform);

        bacteria.DesceBacteria(coluna);
    }

    /// <summary>
    /// Cria bactéria fora da grade e faz ela cair até
    /// encontrar outro bloco ou o fim da grade.
    /// </summary>
    public void CriaSuperBacteria()
    {
        int coluna = UnityEngine.Random.Range(0, colunas);
        QuadradoBacteria bacteria = Instantiate(quadradoBacteria, transform);
        bacteria.ViraSuperBacteria();

        bacteria.DesceBacteria(coluna);
    }

    /// <summary>
    /// Cria linha de bactérias normais que caem ao longo da grade
    /// </summary>
    public void CriaLinhaBacteria()
    {
        for( int coluna = 0; coluna < colunas; coluna++ )
        {
            QuadradoBacteria bacteria = Instantiate(quadradoBacteria, transform);
            bacteria.DesceBacteria(coluna);
        }
    }

    /// <summary>
    /// Cria linha de super bactérias que caem ao longo da grade
    /// </summary>
    public void CriaLinhaSuperBacteria()
    {
        for (int coluna = 0; coluna < colunas; coluna++)
        {
            QuadradoBacteria bacteria = Instantiate(quadradoBacteria, transform);
            bacteria.ViraSuperBacteria();
            bacteria.DesceBacteria(coluna);
        }
    }

    /// <summary>
    /// Transforma em bactéria peças que estejam do lado de bactérias.
    /// </summary>
    public void InfectaPecas()
    {
        foreach( QuadradoBacteria bacteria in FindObjectsOfType<QuadradoBacteria>())
        {
            bacteria.TransformaAdjacentes();
        }
    }

    /// <summary>
    /// Faz com que determinadas bactérias possam ser eliminadas e
    /// apaga linhas que tenham se tornado completas.
    /// Retorna 'true' se alguma linha for apagada.
    /// </summary>
    public bool EnfraqueceBacterias(TipoBacteria tipoBacteria)
    {
        //transforma bactérias
        for(int linha = 0; linha < linhas; linha++)
        {
            for(int coluna = 0; coluna < colunas; coluna++)
            {
                QuadradoGrade quadradoGrade = quadrados[linha][coluna];

                //se celula contiver uma bactéria
                if( quadradoGrade.interior == Preenchimento.Bacteria)
                {
                    QuadradoBacteria bacteria = (QuadradoBacteria)quadradoGrade.quadradoPeca;

                    //se bactérica for menos ou igualmente forte ao tipo especificado
                    if( tipoBacteria == TipoBacteria.SuperBacteria || tipoBacteria == bacteria.tipoBacteria)
                    {
                        bacteria.EnfraqueceBacteria();
                    }
                }
            }
        }

        //completa linhas

        Sequence sequenciaFade = DOTween.Sequence();    //sequencia usada para dar o fade nas linhas apagadas
        Sequence sequenciaDesce = DOTween.Sequence();   //sequencia usada para fazer as linhas caírem

        //Fila de métodos que serão executados após fim das animações de queda das peças
        //São métodos que apagam as peças que devem ser pagadas e descem as peças que devem cair
        List<Action> listaMetodos = new List<Action>();

        //começa animação de queda das peças depois que as linhas completas desaparecerem
        sequenciaFade.OnComplete(() => sequenciaDesce.Play());

        //executa métodos da fila após fim da animação de queda
        sequenciaDesce.OnComplete(() => listaMetodos.ForEach(action => action()));

        //apaga linhas completas
        int naoApagadas = ApagaCompletasDesceIncompletas(0, linhas-1, sequenciaFade, sequenciaDesce, listaMetodos); //número de linhas que não foram apagadas

        //desce as linhas que estão em cima das peças apagadas
        DesceLinhas(linhas - 1 - naoApagadas, 0, sequenciaDesce, listaMetodos);

        sequenciaFade.Pause();
        sequenciaDesce.Pause();

        sequenciaFade.Play();

        return naoApagadas < linhas;
    }

    //código temporário para testes
    private void HandleInput()
    {
        PlayerKeys playerKeys = GetComponent<PlayerKeys>();

        if (playerKeys.GetZ()) CriaBacteria();
        if (playerKeys.GetC()) CriaSuperBacteria();
        if (playerKeys.GetX()) InfectaPecas();
        if (playerKeys.GetR()) LimpaTela();
    }

    private void LimpaTela()
    {
        //posiciona quadrados
        for (int i = 0; i < linhas; i++)
        {
            for (int j = 0; j < colunas; j++)
            {
                quadrados[i][j].Esvazia();
            }
        }
    }
}
