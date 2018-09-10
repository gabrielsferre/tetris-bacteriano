using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotacao {


    //retorna vetor com as distancias horizontal(x) e vertical(y) do quadrado ao centro
    //caso a distância horizontal seja positiva, significa que o quadrado está a direita do centro
    //caso a distância vertical seja positiva, significa que o quadrado está abaixo do centro
    private static int[] DistanciaCentro(Peca peca, QuadradoPeca quadrado)
    {
        int[] distancia = new int[2];

        distancia[0] = quadrado.coluna - peca.centro[1];
        distancia[1] = quadrado.linha - peca.centro[0];

        return distancia;
    }

    //retorna posicao do quadrado girada de 90 graus em torno do centro
    //se sentido for 1, gira no sentido horário
    //se sentido for -1, gira no sentido anti-horário
    private static int[] GiraQuadrado( int sentido, Peca peca,  QuadradoPeca quadrado)
    {
        int[] distancia = DistanciaCentro(peca, quadrado); //distancia do quadrado ao centro da peca

        int[] novaPosicao = new int[2];
        novaPosicao[0] = peca.centro[0] + sentido * distancia[0];
        novaPosicao[1] = peca.centro[1] - sentido * distancia[1];
        
        return novaPosicao;
    }

    public static void GiraPeca( int sentido, Peca peca )
    {
        foreach( QuadradoPeca quadrado in peca.quadrados )
        {
            int[] novaPosicao = GiraQuadrado(1, peca, quadrado);
            quadrado.Move(novaPosicao[0], novaPosicao[1]);
        }
    }

    //metodo usado especificamente para girar a peca reta
    public static void GiraPecaReta( int sentido, Peca peca )
    {
        int[] distancia = DistanciaCentro(peca, peca.quadrados[0]); //distancia ao centro do quadrado que é separado do resto da peça pelo centro

        MonoBehaviour.print(distancia[0] + " " + distancia[1]);

        peca.MovePeca(-distancia[1], -distancia[0]);

        foreach (QuadradoPeca quadrado in peca.quadrados)
        {
            int[] novaPosicao = GiraQuadrado(1, peca, quadrado);
            quadrado.Move(novaPosicao[0], novaPosicao[1]);
        }
    }
}
