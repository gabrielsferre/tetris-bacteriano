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
    public Preenchimento interior = Preenchimento.Livre;

    //quadrado de peca que o preenche o espaço
    public QuadradoPeca quadradoPeca = null;

    public void Preenche(QuadradoPeca quadrado)
    {
        quadradoPeca = quadrado;
        interior = Preenchimento.Peca;
    }

    public void Esvazia()
    {
        if( quadradoPeca != null)
        {
            Destroy(quadradoPeca);
        }
        interior = Preenchimento.Livre;
    }
}