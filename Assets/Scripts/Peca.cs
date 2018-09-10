using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a classe deve ser filha de um objeto do tipo Grade
public class Peca : MonoBehaviour {

    //tempo entre as "quedas" sucessivas da peca em segundos
    public const float tempoQueda = 0.5f;

    //array com quadrados que compõe a peca
    public QuadradoPeca[] quadrados = new QuadradoPeca[4];

    //posicao do centro da peca, influencia em como ela vai girar
    //centro[0] é a linha e centro[1], a coluna
    public int[] centro = new int[2];

    //linha e coluna de surgimento do centro da peca
    protected int[] posicaoSurgimento = { 6, 4 };

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

        InvokeRepeating("moveBaixo", tempoQueda, tempoQueda);
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
            quadrados[i] = Instantiate(prefabQuadrado);
        }

        MontaPeca();
    }

    //monta a peça em seu formato padrão, varia para cada classe de peça
    //este método funciona como um tipo de método abstrato para as outras classes de peças
    protected virtual void MontaPeca()
    {
        print("algo errado em montaPeca");
    }


    //checa se algum quadrado da peca esta sobreposto a outro quadrado
    protected bool ChecaColisao()
    {
        foreach( QuadradoPeca quadrado in quadrados )
        {
            if (grade.quadrados[quadrado.linha, quadrado.coluna].ocupado) return true;
        }

        return false;
    }

    //move peca para esquerda caso possivel
    protected void MoveEsquerda()
    {
        //checa se algum quadrado da peca esta em alguma borda da lateral da grade
        if (ChecaLimiteHorizontal(0)) return;

        //move quadrados da peca para a esquerda
        MovePeca(0, -1);

        //se nova posicao nao colidir
        if (!ChecaColisao()) return;

        //se colidir, move tudo de volta
        MovePeca(0, 1);
    }

    //move peca para a direita caso possivel
    protected void MoveDireita()
    {
        //checa se algum quadrado da peca esta em alguma borda da lateral da grade
        if (ChecaLimiteHorizontal(Grade.colunas - 1)) return;

        //move quadrados da peca para a direita
        MovePeca(0, 1);

        //se nova posicao nao colidir com outra peca
        if (!ChecaColisao()) return;

        //se colidir, move tudo de volta
        MovePeca(0, -1);
    }

    //move a peca i quadrados para direita e j para baixo
    //i e j podem ser negativos
    public void MovePeca( int i, int j )
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            quadrado.Move(quadrado.linha + i, quadrado.coluna + j);
        }
        centro[0] += i;
        centro[1] += j;
    }

    //checa se algum quadrado da peca esta em alguma borda da lateral da grade
    protected bool ChecaLimiteHorizontal(int limite)
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            if (quadrado.ChecaLimiteHorizontal(limite)) return true;
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
    protected void moveBaixo()
    {
        /**
        //se a peca estiver no fundo da grade
        if(checaLimiteVertical())
        {
            apagaPeca();
            return;
        }

        //move quadrados da peca para baixo
        movePeca(1, 0);

        //se nova posicao nao colidir com outra peca
        if (!checaColisao()) return;

        //se colidir, move tudo de volta
        movePeca(-1, 0);

        apagaPeca();
        **/
    }

    //deleta objeto peça e atualiza quadrados da grade
    //os quadrados que ficarão na grade não estarão mais ligados à peça
    //tambem checa se alguma linha da grade esta completa
    protected void ApagaPeca()
    {
        foreach( QuadradoPeca quadrado in quadrados )
        {
            //preenche quadrado da grade com quadrado da peça
            QuadradoGrade quadradoGrade = grade.quadrados[quadrado.linha, quadrado.coluna];
            quadradoGrade.Preenche( quadrado );
        }

        Destroy(gameObject);
    }

    protected virtual void GiraPeca()
    {
        Rotacao.GiraPeca(1, this);
    }

    //retorna maior linha ocupada pela peca
    public int MaxLin()
    {
        int max = 0;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            max = Mathf.Max(max, quadrado.linha);
        }

        return max;
    }

    //retorna maior coluna ocupada pela peca
    public int MaxCol()
    {
        int max = 0;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            max = Mathf.Max(max, quadrado.coluna);
        }

        return max;
    }

    //retorna menor coluna ocupada pela peca
    public int MinCol()
    {
        int min = Grade.colunas - 1;

        foreach (QuadradoPeca quadrado in quadrados)
        {
            min = Mathf.Min(min, quadrado.coluna);
        }

        return min;
    }

    private void HandleInput()
    {
        if( playerKeys.GetRawHorizontal() == 1)
        {
            MoveDireita();
        }
        else if( playerKeys.GetRawHorizontal() == -1)
        {
            MoveEsquerda();
        }
        else if( playerKeys.GetRawVertical() == 1)
        {
            GiraPeca();
        }
    }
}