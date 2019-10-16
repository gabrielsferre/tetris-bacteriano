using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotaoSeta : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool segurado = false;  //botão mantem-se pressionado
    private bool pressionado = false;   //botão foi pressionado uma vez
    private bool solto = false;     //botão foi solto

    private bool consumirSolto = true; //flag usada para 'solto' valer 'false' na hora certa
    private bool consumirPressionado = true; //flag usada para 'pressionado' valer 'false' na hora certa

    public void OnPointerDown(PointerEventData eventData)
    {
        Segurado = true;
        Pressionado = true;
        consumirPressionado = false;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Segurado = false;
        Solto = true;
        consumirSolto = false;
    }

	private void LateUpdate()
	{
		
	}

	private void Update()
    {
		//faz com que 'solto' funcione como um tipo de Input.GetKeyUp()
		if (!consumirSolto && Solto)
        {
            consumirSolto= true;
        }
        if (consumirSolto)
        {
            Solto = false;
        }

        //faz com que 'pressionado' funcione como um tipo de Input.GetKeyDown()
        if (!consumirPressionado && Pressionado)
        {
            consumirPressionado = true;
        }
        else if(consumirPressionado)
        {
            Pressionado = false;
        }
    }

    public bool Pressionado
    {
        get { return pressionado; }
        private set { pressionado = value; }
    }

    public bool Solto
    {
        get { return solto; }
        private set { solto = value; }
    }
    public bool Segurado
    {
        get { return segurado; }
        private set { segurado = value; }
    }
}
