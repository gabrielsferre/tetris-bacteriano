using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grade : MonoBehaviour {

    //numero de linhas e colunas da grade
    public const int linhas = 20;
    public const int colunas = 10;

    //lado dos quadrados que compõem a grade
    public const float lado = 0.75f;

    //matriz com quadrados
    public QuadradoGrade[,] quadrados = new QuadradoGrade[linhas, colunas];

    //prefab de um quadrado
    public QuadradoGrade quadrado;

    //prefabde uma peca
    public Peca peca;

    private void Awake()
    {
        montaGrade();
    }

    private void Start()
    {

        Instantiate(peca, transform);
    }

    private void montaGrade()
    {

        //define tamanho dos quadrados
        quadrado.transform.localScale = new Vector3(lado, lado, 0);
        float tamanho = quadrado.GetComponent<SpriteRenderer>().bounds.size.x;

        //posiciona quadrados
        for ( int i = 0; i < linhas; i++ )
        {
            for( int j = 0; j < colunas; j++)
            {
                QuadradoGrade novoQuadrado = Instantiate(quadrado, transform.position + new Vector3(tamanho * j, -tamanho * i, 0), Quaternion.identity, transform);
                quadrados[i, j] = novoQuadrado;
            }
        }
    }
}
