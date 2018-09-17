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

    //checa se o quadrado esta na coluna 'limite'
    public bool ChecaLimiteHorizontal(int limite)
    {
        return posicao.y == limite;
    }

    //checa se o quadrado esta no fundo da grade
    public bool ChecaLimiteVertical()
    {
        return posicao.x < 0 || posicao.x >= (Grade.linhas - 1);
    }

    //retorna o quanto deve ser adicionado à posição dada para que ela talvez torne-se válida
    //não checa se a posicão retornada está ocupada por outra peça
    public Vector2Int ValidaPosicao( Vector2Int posicao )
    {
        Vector2Int novaPosicao = new Vector2Int();

        if (novaPosicao.y < 0) ;

        return novaPosicao;
    }

    //move o quadrado para uma dada posicao da grade
    public void Move( Vector2Int novaPosicao)
    {
        posicao = novaPosicao;
        transform.position = grade.quadrados[posicao.x, posicao.y].transform.position + new Vector3(0, 0, -1);
    }
}
