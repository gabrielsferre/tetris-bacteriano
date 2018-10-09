using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grade : MonoBehaviour {

    //numero de linhas e colunas da grade
    public const int linhas = 20;
    public const int colunas = 10;

    //lado dos quadrados que compõem a grade
    public const float lado = 0.75f;

    //matriz com quadrados
    public QuadradoGrade[][] quadrados = new QuadradoGrade[linhas][];

    //prefab de um quadrado
    public QuadradoGrade quadrado;

    //dicionario com os prefabs de cada tipo de peça
    public Peca[] prefabs = new Peca[(int)TipoPeca.NUM_TIPOS];

    private void Awake()
    {
        MontaGrade();
    }

    private void Start()
    {
        CriaPeca();
    }

    /// <summary>
    /// Posiciona os quadrados da grade
    /// </summary>
    private void MontaGrade()
    {
        //define tamanho dos quadrados
        quadrado.transform.localScale = new Vector3(lado, lado, 0);
        float tamanho = quadrado.GetComponent<SpriteRenderer>().bounds.size.x;

        //posiciona quadrados
        for ( int i = 0; i < linhas; i++ )
        {
            quadrados[i] = new QuadradoGrade[colunas];
            for( int j = 0; j < colunas; j++)
            {
                QuadradoGrade novoQuadrado = Instantiate(quadrado, transform.position + new Vector3(tamanho * j, -tamanho * i, 0), Quaternion.identity, transform);
                novoQuadrado.Esvazia();
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

        //começa animação de queda das peças depois que as linhas completas desaparecerem
        sequenciaFade.OnComplete(() => sequenciaDesce.Play());

        //apaga linhas que foram completadas pela peça
        int naoApagadas = ApagaCompletasDesceIncompletas(linhaMin, linhaMax, sequenciaFade, sequenciaDesce); //número de linhas que não foram apagadas
        
        //desce as linhas que estão em cima das peças apagadas
        DesceLinhas(linhaMax - naoApagadas, linhaMin, sequenciaDesce);

        sequenciaFade.Pause();
        sequenciaDesce.Pause();

        sequenciaFade.Play();
    }

    /// <summary>
    /// Apaga as linhas dentro do intervalo [linhaMin,linhaMax] que estiverem completas e desce as linhas que não estiverem.
    /// Retorna o número de linhas que não foram completadas pela peça (dentre as linhas em que a peça ocupou).
    /// Tudo isso após o término de tweens
    /// </summary>
    private int ApagaCompletasDesceIncompletas(int linhaMin, int linhaMax, Sequence sequenciaFade, Sequence sequenciaDesce)
    {

        int naoApagadas = 0;  //número de linhas que não puderam ser apagadas

        //apaga linhas que estiverem completas
        for( int i = linhaMax; i >= linhaMin; i--)
        {
            if (LinhaCompleta(i))
            {
                int iNotClosure = i;    //variável para driblar o closure ao usar ApagaLinha na expressão labda

                //cria sequencia para animar as peças sumindo
                Sequence novaSequencia = Efeitos.FadeLinha(quadrados[i]);
                //chama método que apaga as peças quando a sequencia acabar
                novaSequencia.OnComplete(() => ApagaLinha(iNotClosure));
                //adiciona sequencia na sequencia com todas as animações de peças sumindo
                sequenciaFade.Insert(0, novaSequencia);
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
                    Sequence novaSequencia = Efeitos.MoveLinha(quadrados[i], quadrados[linhaMax - naoApagadas]);
                    //chama método que move as peças quando a sequencia acabar
                    novaSequencia.OnComplete(() => MoveLinha(iNotClosure, linhaMax - naoApagadasNotClosure));
                    //adiciona sequencia na sequencia com todas as animações de queda
                    sequenciaDesce.Insert(0, novaSequencia);
                }
                naoApagadas++;
            }
        }

        return naoApagadas;
    }

    /// <summary>
    /// Checa se linha está completa
    /// </summary>
    /// <param name="linha"></param>
    /// <returns></returns>
    private bool LinhaCompleta(int linha)
    {
        for( int j = 0; j < colunas; j++)
        {
            //caso campo da grade não esteja preenchido por uma peça
            if( quadrados[linha][j].interior != Preenchimento.Peca)
            {
                return false;
            }
        }
        return true;
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
                    quadradoDestino.Preenche(Instantiate(quadradoOrigem.quadradoPeca));
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
    private void DesceLinhas(int linhaBase, int linhaTopo, Sequence sequenciaDesce)
    {
        //caso a base esteja embaixo ou junto do topo
        if (linhaBase >= linhaTopo)
        {
            for (int i = linhaTopo - 1; i >= 0; i--)
            {
                int iNotClosure = i;    //variável para driblar o closure

                int linhaFinal = linhaBase + iNotClosure - linhaTopo + 1; //linha em que a linha de quadrados atual deve terminar a queda

                //cria sequencia para animar queda da linha
                Sequence novaSequencia = Efeitos.MoveLinha(quadrados[i], quadrados[linhaFinal]);
                //chama método que move as peças quando a sequencia acabar
                novaSequencia.OnComplete(() => MoveLinha(iNotClosure, linhaFinal));
                //adiciona sequencia na sequencia com todas as animações de queda
                sequenciaDesce.Insert(0, novaSequencia);
            }
        }
    }

    /// <summary>
    /// função temporária
    /// </summary>
    public void CriaPeca()
    {
        TipoPeca tipo = (TipoPeca) Random.Range(0, (int)TipoPeca.NUM_TIPOS);
        Instantiate(prefabs[(int)tipo], transform);
    }
}
