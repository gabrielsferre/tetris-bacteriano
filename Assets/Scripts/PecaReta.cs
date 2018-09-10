using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PecaReta : Peca
{
    //constroi peca a partir da posicao de seu centro
    protected override void MontaPeca()
    {
        quadrados[0].Move(centro[0], centro[1] - 1);
        quadrados[1].Move(centro[0], centro[1]);
        quadrados[2].Move(centro[0], centro[1] + 1);
        quadrados[3].Move(centro[0], centro[1] + 2);
    }

    protected override void GiraPeca()
    {
        Rotacao.GiraPecaReta(1, this);
    }
}
