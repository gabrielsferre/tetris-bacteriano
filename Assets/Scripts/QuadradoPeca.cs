using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quadrado de uma peca
public class QuadradoPeca : Quadrado
{ 
    //diz se o quadrado esta entre as colunas da grade
    public bool ChecaLimiteHorizontal()
    {
        return posicao.y >= 0 && posicao.y < Grade.colunas;
    }

    //diz se o quadrado esta entre as linhas da grade
    public bool ChecaLimiteVertical()
    {
        return posicao.x >= 0 && posicao.x < Grade.linhas;
    }

    //diz se o quadrado está dentro da grade
    public bool ChecaLimite()
    {
        return ChecaLimiteHorizontal() && ChecaLimiteVertical();
    }

    //diz se a posicao em que está o quadrado é permitida ou não
    public bool ValidaPosicao()
    {
        //se peça estiver fora da grade
        if( !ChecaLimite() )
        {
            return false;
        }

        //diz se a peça não está sobrepondo nenhuma outra
        return grade.quadrados[posicao.x][posicao.y].interior == Preenchimento.Livre;
    }
}
