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

    //qual eh o preenchimento do quadrado
    public Preenchimento interior { get; private set;}

    //quadrado de peca que o preenche o espaço
    public QuadradoPeca quadradoPeca { get; private set; }

    /// <summary>
    /// Preenche o quadrado com uma certa peça e atualiza
    /// o preenchimento
    /// </summary>
    /// <param name="quadrado"></param>
    public void Preenche(QuadradoPeca quadrado)
    {
        quadradoPeca = quadrado;
        interior = Preenchimento.Peca;
        quadrado.transform.position = transform.position - Vector3.forward;
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