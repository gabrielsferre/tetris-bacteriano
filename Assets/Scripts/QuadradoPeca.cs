using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quadrado de uma peca
public class QuadradoPeca : MonoBehaviour
{
    //posicao do quadrado na grade
    //posicao.x é a linha e posicao.y, a coluna
    public Vector2Int posicao = new Vector2Int();

    protected Grade grade;

    protected void Awake()
    {
        grade = FindObjectOfType<Grade>();
    }

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

    //move o quadrado para uma dada posicao da grade
    public void Move(Vector2Int novaPosicao)
    {
        VirtualMove(novaPosicao);
        Materializa();
    }

    //muda o atributo 'posicao' mas não desloca o quadrado de fato
    public void VirtualMove(Vector2Int novaPosicao)
    {
        posicao = novaPosicao;
    }

    //desloca o quadrado para local correspondente ao atributo 'posição'
    public void Materializa()
    {
        transform.position = grade.quadrados[posicao.x][posicao.y].transform.position + new Vector3(0, 0, -1);
    }
}
