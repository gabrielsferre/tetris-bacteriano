using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quadrado : MonoBehaviour {

    //posicao do quadrado na grade
    //posicao.x é a linha e posicao.y, a coluna
    public Vector2Int posicao = new Vector2Int();

    protected Grade grade;

    protected void Awake()
    {
        grade = FindObjectOfType<Grade>();
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
