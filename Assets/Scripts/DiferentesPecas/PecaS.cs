using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PecaS : Peca
{
    //constroi peca a partir da posicao de seu centro
    protected override void MontaPeca()
    {
        quadrados[0].Move(centro + new Vector2Int(0, -1));
        quadrados[1].Move(centro);
        quadrados[2].Move(centro + new Vector2Int(-1, 0));
        quadrados[3].Move(centro + new Vector2Int(-1, 1));
    }
}
