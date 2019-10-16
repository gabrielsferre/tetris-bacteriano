using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKeys : MonoBehaviour {

    public KeyCode direita;
    public KeyCode esquerda;
    public KeyCode baixo;
    public KeyCode cima;

    public Button bDireita;
    public Button bEsquerda;
    public Button bRotacao;
    public Button bBaixo;

    private BotaoSeta scriptDireita;
    private BotaoSeta scriptEsquerda;
    private BotaoSeta scriptBaixo;
    private BotaoSeta scriptRotacao;

    private void Awake()
    {
        scriptDireita = bDireita.GetComponent<BotaoSeta>();
        scriptEsquerda = bEsquerda.GetComponent<BotaoSeta>();
        scriptBaixo = bBaixo.GetComponent<BotaoSeta>();
        scriptRotacao = bRotacao.GetComponent<BotaoSeta>();
    }

    public bool GetRight()
    {
        return Input.GetKey(direita) || scriptDireita.Segurado;
    }

    public bool GetLeft()
    {
        return Input.GetKey(esquerda) || scriptEsquerda.Segurado;
    }

    public bool GetDown()
    {
        return Input.GetKey(baixo) || scriptBaixo.Segurado;
    }

    public bool GetUpPressed()
    {
        return Input.GetKeyDown(cima) || scriptRotacao.Pressionado;
    }

    public bool GetDownPressed()
    {
        return Input.GetKeyDown(baixo) || scriptBaixo.Pressionado;
    }

    public bool GetLeftPressed()
    {
        return Input.GetKeyDown(esquerda) || scriptEsquerda.Pressionado;
    }

    public bool GetRightPressed()
    {
        return Input.GetKeyDown(direita) || scriptDireita.Pressionado;
    }

    public bool GetLeftReleased()
    {
        return Input.GetKeyUp(esquerda) || scriptEsquerda.Solto;
    }

    public bool GetRightReleased()
    {
        return Input.GetKeyUp(direita) || scriptDireita.Solto;
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
    public bool GetC()
    {
        return Input.GetKeyDown("c");
    }
}
