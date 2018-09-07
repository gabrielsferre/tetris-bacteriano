using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeys : MonoBehaviour {

    public KeyCode direita;
    public KeyCode esquerda;
    public KeyCode baixo;
    public KeyCode cima;

	public int getRawVertical()
    {
        if(Input.GetKeyDown(cima)) return 1;
        if (Input.GetKeyDown(baixo)) return -1;
        return 0;
    }

    public int getRawHorizontal()
    {
        if (Input.GetKeyDown(direita)) return 1;
        if (Input.GetKeyDown(esquerda)) return -1;
        return 0;
    }
}
