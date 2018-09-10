using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quadrado de uma peca
public class QuadradoPeca : MonoBehaviour
{
    //indices da linha e coluna em que se encontra o quadrado
    public int linha;
    public int coluna;

    protected Grade grade;

    protected void Awake()
    {
        grade = FindObjectOfType<Grade>();
    }

    //checa se o quadrado esta na coluna 'limite'
    public bool ChecaLimiteHorizontal(int limite)
    {
        return coluna == limite;
    }

    //checa se o quadrado esta no fundo da grade
    public bool ChecaLimiteVertical()
    {
        return linha < 0 || linha >= (Grade.linhas - 1);
    }

    //retorna o quanto deve ser adicionado à posição dada para que ela talvez torne-se válida
    //não checa se a posicão retornada está ocupada por outra peça
    public int[] ValidaPosicao( int[] posicao )
    {
        int[] novaPosicao = new int[2];

        if (novaPosicao[1] < 0) ;

        return novaPosicao;
    }

    //move o quadrado para uma dada posicao da grade
    public void Move(int novaLinha, int novaColuna)
    {
        linha = novaLinha;
        coluna = novaColuna;
        transform.position = grade.quadrados[linha, coluna].transform.position + new Vector3(0, 0, -1);
    }
}
