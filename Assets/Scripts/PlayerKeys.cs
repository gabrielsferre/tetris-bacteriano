using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeys : MonoBehaviour {

    public KeyCode direita;
    public KeyCode esquerda;
    public KeyCode baixo;
    public KeyCode cima;

	public int GetRawVertical()
    {
        if(Input.GetKeyDown(cima)) return 1;
        if (Input.GetKeyDown(baixo)) return -1;
        return 0;
    }

    public int GetRawHorizontal()
    {
        if (Input.GetKeyDown(direita)) return 1;
        if (Input.GetKeyDown(esquerda)) return -1;
        return 0;
    }

    public bool GetDown()
    {
        return Input.GetKey(baixo);
    }

    public bool GetLeft()
    {
        return Input.GetKey(esquerda);
    }

    public bool GetRight()
    {
        return Input.GetKey(direita);
    }

    public bool GetLeftReleased()
    {
        return Input.GetKeyUp(esquerda);
    }

    public bool GetRightReleased()
    {
        return Input.GetKeyUp(direita);
    }

    //código temporário para testes
    public bool GetZ()
    {
        return Input.GetKeyDown("z");
    }
    public bool GetX()
    {
        return Input.GetKeyDown("x");
    }
    public bool GetR()
    {
        return Input.GetKeyDown("r");
    }
}
