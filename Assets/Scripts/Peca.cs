using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a classe deve ser filha de um objeto do tipo Grade
public class Peca : MonoBehaviour {

    //tempo entre as "quedas" sucessivas da peca em segundos
    public const float tempoQueda = 0.7f;
    //tempo de queda para quando o botão de descida estiver pressionado
    public const float tempoQuedaReduzido = 0.05f;

    //tempo para movimentação lateral quando o botão é pressionado
    public const float tempoSlide = 0.05f;

    //delay para o slide começar
    public const float slideDelay = 0.3f;

    //array com quadrados que compõe a peca
    public QuadradoPeca[] quadrados = new QuadradoPeca[4];

    //posicao do centro da peca, influencia em como ela vai girar
    //centro.x é a linha e centro.y, a coluna
    public Vector2Int centro = new Vector2Int();

    //linha e coluna de surgimento do centro da peca
    protected Vector2Int posicaoSurgimento = new Vector2Int(2,4);

    protected Grade grade;

    public QuadradoPeca prefabQuadrado;

    private PlayerKeys playerKeys;

    protected void Awake()
    {
        grade = FindObjectOfType<Grade>();

        playerKeys = GetComponentInParent<PlayerKeys>();

        centro = posicaoSurgimento;
    }

    protected void Start()
    {
        CriaPeca();

        //coroutine que faz peça cair
        StartCoroutine("DescePeca");
    }

    private void Update()
    {
        HandleInput();
    }

    //cria quadrados para a peca e define seu centro
    protected void CriaPeca()
    {
        for( int i = 0; i < quadrados.Length; i++ )
        {
            quadrados[i] = Instantiate(prefabQuadrado, grade.transform);
        }

        MontaPeca();
    }

    //monta a peça em seu formato padrão, varia para cada classe de peça
    //este método funciona como um tipo de método abstrato para as outras classes de peças
    protected virtual void MontaPeca()
    {
        Debug.Log("algo errado em MontaPeca");
    }


    //checa se algum quadrado da peca esta sobreposto a outro quadrado
    protected bool ChecaColisao()
    {
        foreach( QuadradoPeca quadrado in quadrados )
        {
            if (grade.quadrados[quadrado.posicao.x][quadrado.posicao.y].interior != Preenchimento.Livre) return true;
        }

        return false;
    }

    //move peca para esquerda caso possível
    protected void MoveEsquerda()
    {
        MovePecaCheck( new Vector2Int(0, -1) );
    }

    //move peca para a direita caso possivel
    protected void MoveDireita()
    {
        MovePecaCheck( new Vector2Int(0, 1) );
    }

    /// <summary>
    /// Coroutine que faz peça continuar movimento para esquerda botão direcional
    /// estiver pressionado. Se esquerda e direita estiverem pressionados juntos,
    /// peça para.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SlideEsquerda()
    {
        yield return new WaitForSeconds(slideDelay);

        while( playerKeys.GetLeft() )
        {
            yield return new WaitForSeconds(tempoSlide);

            MoveEsquerda();
        } 
    }

    /// <summary>
    /// Coroutine que faz peça continuar movimento para direita botão direcional
    /// estiver pressionado. Se esquerda e direita estiverem pressionados juntos,
    /// peça para.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SlideDireita()
    {
        yield return new WaitForSeconds(slideDelay);

        while( playerKeys.GetRight() )
        {
            yield return new WaitForSeconds(tempoSlide);

            MoveDireita();
        }
    }

    //move peca apenas se a nova posicao for válida
    //retorna se foi possível mover a peça ou não
    public bool MovePecaCheck( Vector2Int deslocamento )
    {
        VirtualMovePeca(deslocamento);

        //se nova posição for válida
        if( ValidaPosicao() )
        {
            MaterializaPeca();

            return true;
        }
        else
        {
            VirtualMovePeca(Vector2Int.zero - deslocamento);

            return false;
        }
    }

    //move a peca deslocamento.x quadrados para direita e deslocamento.y para baixo
    //x e y podem ser negativos
    //não move de fato os game objects, mas altera o resto das propriedades relacionadas à posição
    public void VirtualMovePeca(Vector2Int deslocamento)
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            quadrado.VirtualMove(quadrado.posicao + deslocamento);
        }
        centro += deslocamento;
    }

    //move todos os game objects(quadrados) que compõe a peça para suas respectivas posições
    public void MaterializaPeca()
    {
        //para cada quadrado que compõe a peça
        foreach (QuadradoPeca quadrado in quadrados)
        {
            //de fato move o objeto para a nova posição
            quadrado.Materializa();
        }
    }

    //checa se algum quadrado da peca esta em alguma borda da lateral da grade
    protected bool ChecaLimiteHorizontal(int limite)
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            if (quadrado.ChecaLimiteHorizontal()) return true;
        }
        return false;
    }

    //checa se algum quadrado da peca esta no fundo da grade
    protected bool ChecaLimiteVertical()
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            if (quadrado.ChecaLimiteVertical()) return true;
        }
        return false;
    }

    //faz a peca cair uma linha
    //é o método que identifica que se peça parou de cair
    protected void MoveBaixo()
    {
        //se não for possível mover peça para baixo
        if( !MovePecaCheck( new Vector2Int(1,0)) )
        {
            PecaPosicionada();
        }
    }

    /// <summary>
    /// Coroutine que faz peça descer periodicamente.
    /// A peça desce mais rápido caso o botão de descer esteja pressionado.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DescePeca()
    {
        while (gameObject != null)
        {
            //se botão de descida estiver pressionado
            if ( playerKeys.GetDown() )
            {
                yield return new WaitForSeconds(tempoQuedaReduzido);
            }
            else
            {
                yield return new WaitForSeconds(tempoQueda);
            }
            
            MoveBaixo();
        }
    }

    /// <summary>
    /// Chamado quando a peça acaba de cair
    /// </summary>
    private void PecaPosicionada()
    {
        int linhaMin = MinLin(); //menor linha ocupada pela peça
        int linhaMax = MaxLin(); //maior linha ocupada pela peça

        ApagaPeca();

        //Lida com a completude de linhas e spawna a próxima peça
        grade.AposQueda(linhaMin, linhaMax);
    }

    /// <summary>
    ///Deleta objeto peça e atualiza quadrados da grade.
    ///Os quadrados que ficarão na grade não estarão mais ligados à peça.
    ///Tambem checa se alguma linha da grade esta completa
    /// </summary>

    protected void ApagaPeca()
    {
        foreach( QuadradoPeca quadrado in quadrados )
        {
            //preenche quadrado da grade com quadrado da peça
            QuadradoGrade quadradoGrade = grade.quadrados[quadrado.posicao.x][quadrado.posicao.y];
            quadradoGrade.Preenche(quadrado);
        }

        Destroy(gameObject);
    }

    protected virtual void GiraPeca()
    {
        Rotacao.GiraPecaCheck(1, this, Rotacao.VirtualGiraPeca);
    }

    //retorna maior linha ocupada pela peca
    public int MaxLin()
    {
        int max = 0;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            max = Mathf.Max(max, quadrado.posicao.x);
        }

        return max;
    }

    //retorna menor coluna ocupada pela peca
    public int MinLin()
    {
        int min = Grade.linhas - 1;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            min = Mathf.Min(min, quadrado.posicao.x);
        }

        return min;
    }

    //retorna maior coluna ocupada pela peca
    public int MaxCol()
    {
        int max = 0;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            max = Mathf.Max(max, quadrado.posicao.y);
        }

        return max;
    }

    //retorna menor coluna ocupada pela peca
    public int MinCol()
    {
        int min = Grade.colunas - 1;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            min = Mathf.Min(min, quadrado.posicao.y);
        }

        return min;
    }

    /// <summary>
    /// Diz se a posicao em que está a peça é permitida ou não
    /// </summary>
    /// <returns></returns>
    public bool ValidaPosicao()
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            if( !quadrado.ValidaPosicao())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Faz com que a aceleração da queda seja instantânea ao se pressionar
    /// o botão de descida.
    /// </summary>
    /// <returns></returns>
    private void DownPressed()
    {
        StopCoroutine("DescePeca");
        StartCoroutine("DescePeca");
    }

    /// <summary>
    /// Move peça para a esquerda e garante que não vá se mover
    /// para a direita.
    /// </summary>
    private void LeftPressed()
    {
        MoveEsquerda();
        StartCoroutine("SlideEsquerda");
        StopCoroutine("SlideDireita");
    }


    /// <summary>
    /// Move peça para a direita e garante que não vá se mover
    /// para a esquerda.
    /// </summary>
    private void RightPressed()
    {
        MoveDireita();
        StartCoroutine("SlideDireita");
        StopCoroutine("SlideEsquerda");
    }

    private void HandleInput()
    {
        if( playerKeys.GetRawHorizontal() == 1)
        {
            RightPressed();
        }
        else if( playerKeys.GetRawHorizontal() == -1)
        {
            LeftPressed();
        }
        else if( playerKeys.GetRawVertical() == 1)
        {
            GiraPeca();
        }
        else if (playerKeys.GetRawVertical() == -1)
        {
            DownPressed();
        }
        else if (playerKeys.GetLeftReleased())
        {
            StopCoroutine("SlideEsquerda");
        }
        else if (playerKeys.GetRightReleased())
        {
            StopCoroutine("SlideDireita");
        }
    }
}