using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotacao {


    //retorna vetor com as distancias horizontal(x) e vertical(y) do quadrado ao centro
    //caso a distância horizontal seja positiva, significa que o quadrado está a direita do centro
    //caso a distância vertical seja positiva, significa que o quadrado está abaixo do centro
    private static Vector2Int DistanciaCentro(Peca peca, QuadradoPeca quadrado)
    {
        Vector2Int distancia = new Vector2Int();

        distancia = quadrado.posicao - peca.centro;

        return distancia;
    }

    //retorna posicao do quadrado girada de 90 graus em torno do centro
    //se sentido for 1, gira no sentido horário
    //se sentido for -1, gira no sentido anti-horário
    private static Vector2Int GiraQuadrado( int sentido, Peca peca,  QuadradoPeca quadrado)
    {
        Vector2Int distancia = DistanciaCentro(peca, quadrado); //distancia do quadrado ao centro da peca

        Vector2Int novaPosicao = new Vector2Int();
        novaPosicao.x = peca.centro.x + sentido * distancia.y;
        novaPosicao.y = peca.centro.y - sentido * distancia.x;

        return novaPosicao;
    }

    //gira a peça sem mover o objeto peça
    //muda apenas o atributo 'posicao' dos quadrados que compõe a peça
    public static void VirtualGiraPeca( int sentido, Peca peca )
    {
        foreach( QuadradoPeca quadrado in peca.quadrados )
        {
            Vector2Int novaPosicao = GiraQuadrado(sentido, peca, quadrado);
            quadrado.VirtualMove(novaPosicao);
        }
    }

    //gira a peça sem mover o objeto peça
    //muda apenas o atributo 'posicao' dos quadrados que compõe a peça
    public static void VirtualGiraPecaReta( int sentido, Peca peca )
    {
        Vector2Int distancia = DistanciaCentro(peca, peca.quadrados[0]); //distancia ao centro do quadrado que é separado do resto da peça pelo centro

        //sentido horário
        if (sentido > 0)
        {
            peca.VirtualMovePeca(new Vector2Int(-distancia.x, -distancia.y));
        }
        else
        {
            peca.VirtualMovePeca(new Vector2Int(-distancia.y, distancia.x));
        }

        foreach (QuadradoPeca quadrado in peca.quadrados)
        {
            Vector2Int novaPosicao = GiraQuadrado(sentido, peca, quadrado);
            quadrado.VirtualMove(novaPosicao);
        }
    }
}
