using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PecaReta : Peca
{
    //constroi peca a partir da posicao de seu centro
    protected override void montaPeca()
    {
        quadrados[0].move(centro[0], centro[1] - 1);
        quadrados[1].move(centro[0], centro[1]);
        quadrados[2].move(centro[0], centro[1] + 1);
        quadrados[3].move(centro[0], centro[1] + 2);
    }
}
