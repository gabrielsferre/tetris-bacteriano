using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a classe deve ser filha de um objeto do tipo Grade
public class Peca : MonoBehaviour {

    //tempo entre as "quedas" sucessivas da peca em segundos
    public const float tempoQueda = 0.5f;

    //array com quadrados que compõe a peca
    protected QuadradoPeca[] quadrados = new QuadradoPeca[4];

    //posicao do centro da peca, influencia em como ela vai girar
    protected int[] centro = new int[2];

    //linha e coluna de surgimento do centro da peca
    protected int[] posicaoSurgimento = { 1, 4 };

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
        criaPeca();

        InvokeRepeating("moveBaixo", tempoQueda, tempoQueda);
    }

    private void Update()
    {
        handleInput();
    }

    //cria quadrados para a peca e define seu centro
    protected void criaPeca()
    {
        for( int i = 0; i < quadrados.Length; i++ )
        {
            quadrados[i] = Instantiate(prefabQuadrado);
        }

        montaPeca();
    }

    protected virtual void montaPeca()
    {
        print("algo errado");
    }


    //checa se algum quadrado da peca esta sobreposto a outro quadrado
    protected bool checaColisao()
    {
        foreach( QuadradoPeca quadrado in quadrados )
        {
            if (grade.quadrados[quadrado.linha, quadrado.coluna].ocupado) return true;
        }

        return false;
    }

    //move peca para esquerda caso possivel
    protected void moveEsquerda()
    {
        //checa se algum quadrado da peca esta em alguma borda da lateral da grade
        if (checaLimiteHorizontal(0)) return;

        //move quadrados da peca para a esquerda
        movePeca(0, -1);

        //se nova posicao nao colidir
        if (!checaColisao()) return;

        //se colidir, move tudo de volta
        movePeca(0, 1);
    }

    //move peca para a direita caso possivel
    protected void moveDireita()
    {
        //checa se algum quadrado da peca esta em alguma borda da lateral da grade
        if (checaLimiteHorizontal(Grade.colunas - 1)) return;

        //move quadrados da peca para a direita
        movePeca(0, 1);

        //se nova posicao nao colidir com outra peca
        if (!checaColisao()) return;

        //se colidir, move tudo de volta
        movePeca(0, -1);
    }

    //move a peca i quadrados para direita e j para baixo
    //i e j podem ser negativos
    protected void movePeca( int i, int j )
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            quadrado.move(quadrado.linha + i, quadrado.coluna + j);
        }
        centro[0] += i;
        centro[1] += j;
    }

    //checa se algum quadrado da peca esta em alguma borda da lateral da grade
    protected bool checaLimiteHorizontal(int limite)
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            if (quadrado.checaLimiteHorizontal(limite)) return true;
        }
        return false;
    }

    //checa se algum quadrado da peca esta no fundo da grade
    protected bool checaLimiteVertical()
    {
        foreach (QuadradoPeca quadrado in quadrados)
        {
            if (quadrado.checaLimiteVertical()) return true;
        }
        return false;
    }

    //faz a peca cair uma linha
    protected void moveBaixo()
    {
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
    }

    //deleta objeto peça e atualiza quadrados da grade
    //os quadrados que ficarão na grade não estarão mais ligados à peça
    //tambem checa se alguma linha da grade esta completa
    protected void apagaPeca()
    {
        foreach( QuadradoPeca quadrado in quadrados )
        {
            //preenche quadrado da grade com quadrado da peça
            QuadradoGrade quadradoGrade = grade.quadrados[quadrado.linha, quadrado.coluna];
            quadradoGrade.preenche( quadrado );
        }

        Destroy(gameObject);
    }

    private void handleInput()
    {
        if( playerKeys.getRawHorizontal() == 1)
        {
            moveDireita();
        }
        else if( playerKeys.getRawHorizontal() == -1)
        {
            moveEsquerda();
        }
    }
}