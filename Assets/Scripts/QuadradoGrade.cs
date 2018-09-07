using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quadrado da grade
public class QuadradoGrade : MonoBehaviour {

    //se o quadrado esta preenchido por alguma peca ou nao
    public bool ocupado = false;

    //peca que quadrado de peca que o preenche
    public QuadradoPeca quadradoPeca = null;

    public void preenche(QuadradoPeca quadrado)
    {
        quadradoPeca = quadrado;
        ocupado = true;
    }

    public void esvazia()
    {
        if( quadradoPeca != null)
        {
            Destroy(quadradoPeca);
        }
        ocupado = false;
    }
}