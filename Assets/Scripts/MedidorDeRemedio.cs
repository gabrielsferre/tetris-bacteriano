using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedidorDeRemedio : MonoBehaviour {

    public Image marcador;
    public Image barra;

    [Range(0,1)]
    public float quantidadeInicial;
        
    [SerializeField]
    private float quantidade;
    [Range(0,1)]
    public float quantidadeMinima;

    public float Quantidade
    {
        get
        {
            return quantidade;
        }

        set
        {
            quantidade = value;
            if(quantidade < 0)
            {
                quantidade = 0;
            }
            if(quantidade > 1)
            {
                quantidade = 1;
            }
            StartCoroutine(AnimacaoBarra(quantidade));
        }
    }

    private void Awake()
    {
        PosicionarBarra();
        Quantidade = quantidadeInicial;
        barra.fillAmount = quantidadeInicial;
    }

    void PosicionarBarra()
    {
        marcador.rectTransform.anchorMin = new Vector2(0f, quantidadeMinima);
        marcador.rectTransform.anchorMax = new Vector2(1f, quantidadeMinima);
    }

    public void AdicionarRemedio(float valor)
    {
        Quantidade = Quantidade + valor;
    }

    public void SetRemedio(float novoValor)
    {
        Quantidade = novoValor;
    }

    public void SetToOk()
    {
        Quantidade = quantidadeMinima;
    }

    IEnumerator AnimacaoBarra(float novoValor)
    {
        float velocidadeAnimacao = 0.2f;

        while(Mathf.Abs(barra.fillAmount - novoValor) >= 0.005)
        {
            if(barra.fillAmount > novoValor)
            {
                barra.fillAmount -= Time.deltaTime * velocidadeAnimacao;
            }
            else
            {
                barra.fillAmount += Time.deltaTime * velocidadeAnimacao;
            }

            yield return null;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown("5"))
        {
            SetRemedio(0.5f);
        }

        if (Input.GetKeyDown("9"))
        {
            AdicionarRemedio(-0.1f);
        }

        if (Input.GetKeyDown("0"))
        {
            AdicionarRemedio(0.1f);
        }

        if (Input.GetKeyDown("1"))
        {
            SetToOk();
        }

    }

}
