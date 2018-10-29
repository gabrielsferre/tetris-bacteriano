using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum usado para dizer se o espaço está sendo ocupado por uma peça, uma bactéria, ou se está livre
public enum Preenchimento
{
    Peca,
    Bacteria,
    Livre
};

//Quadrado da grade
public class QuadradoGrade : MonoBehaviour {

    string nomeQuadradoPeca = "QuadradoPeca";
    string nomeQuadradoBacteria = "QuadradoBacteria";

    //qual eh o preenchimento do quadrado
    public Preenchimento interior { get; private set;}

    //quadrado de peca que o preenche o espaço
    public Quadrado quadradoPeca { get; private set; }

    //vetor que indica em que linha (coordenada x) e em que coluna (coordenada y) o quadrado se encontra
    public Vector2Int posicao;

    /// <summary>
    /// Preenche o quadrado com uma certa peça ou bactéria e atualiza
    /// o preenchimento. Também atualiza a posicao da peça ou bactéria
    /// com a posição do quadrado.
    /// </summary>
    /// <param name="quadrado"></param>
    public void Preenche(Quadrado quadrado)
    {
        quadrado.posicao = posicao;
        quadradoPeca = quadrado;
        quadrado.transform.position = transform.position - Vector3.forward;

        //se quadrado for de uma peça
        if (quadrado.GetType().Name == nomeQuadradoPeca)
        {
            interior = Preenchimento.Peca;
        }

        //se quadrado for de uma bactéria
        else if(quadrado.GetType().Name == nomeQuadradoBacteria)
        {
            interior = Preenchimento.Bacteria;
        }

        else
        {
            Debug.Log("Erro: quadrado não é de nenhum tipo reconhecido");
        }
    }

    /// <summary>
    /// Apaga a peça que estiver preenchendo o quadrado e atualiza
    /// o preenchimento.
    /// </summary>
    public void Esvazia()
    {
        if( quadradoPeca != null)
        {
            Destroy(quadradoPeca.gameObject);
        }
        quadradoPeca = null;
        interior = Preenchimento.Livre;
    }
}