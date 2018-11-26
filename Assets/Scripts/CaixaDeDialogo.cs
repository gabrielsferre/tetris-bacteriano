using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TipoDeTexto { FALA, RESPOSTA};

public class CaixaDeDialogo : MonoBehaviour {

    public GameObject balaoDeTextoPrefab;
    public GameObject balaoDeRespostaPrefab;

    private List<GameObject> lista;

    private void Awake()
    {
        lista = new List<GameObject>();
    }

    public void ImprimeTexto(TipoDeTexto tipo, string mensagem)
    {
        GameObject obj;

        switch (tipo) {
            case TipoDeTexto.FALA :
                obj = Instantiate(balaoDeTextoPrefab, transform);
                break;
            case TipoDeTexto.RESPOSTA:
                obj = Instantiate(balaoDeRespostaPrefab, transform);
                break;
            default:
                obj = Instantiate(balaoDeTextoPrefab, transform);
                break;
        }

        

        BalaoDeTexto novoBalao = obj.GetComponent<BalaoDeTexto>();        

        novoBalao.Text = mensagem;
        novoBalao.GetComponent<RectTransform>().localScale = new Vector3(3.0f, 3.0f, 3.0f);

        switch (tipo)
        {
            case TipoDeTexto.FALA:
                novoBalao.GetComponent<RectTransform>().position.Set(-2.5f, 4.3f, 0);
                break;
            case TipoDeTexto.RESPOSTA:
                novoBalao.GetComponent<RectTransform>().position.Set(2.5f, 4.3f, 0);
                break;
            default:
                novoBalao.GetComponent<RectTransform>().position.Set(-2.5f, 4.3f, 0);
                break;
        }        

        if (lista.Count != 0)
        {
            StartCoroutine(SobeTextoRotina(obj));            
        }
        else
        {
            lista.Add(obj);
        }
        
    }

    public void ImprimeTexto(string mensagem)
    {
        ImprimeTexto(TipoDeTexto.FALA, mensagem);
    }

    IEnumerator SobeTextoRotina(GameObject balao)
    {
        yield return new WaitForEndOfFrame();
        SobeTexto(balao.GetComponent<BalaoDeTexto>().tmp.GetComponent<RectTransform>().sizeDelta.y * 0.05f + (0.25f));
        lista.Add(balao);
    }

    private void SobeTexto(float distancia)
    {
        
        float tempoDeSubida = 0.6f;

        foreach(GameObject balao in lista)
        {
            balao.transform.DOMoveY(balao.transform.position.y + distancia, tempoDeSubida);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            ImprimeTexto("Lorem This is not a bug, that's how C# (or any object oriented language) is supposed to work. Its intended behavior, in fact Transform logs a warning when you attempt to do the same things for it's properties like position rotation");
        }

        if (Input.GetKeyDown("w"))
        {
            ImprimeTexto("Lorem This is not a bug, that's how C# (or any object oriented language)");
        }

        if (Input.GetKeyDown("a"))
        {
            ImprimeTexto(TipoDeTexto.RESPOSTA,"Lorem This is not a bug, that's how C# (or any object oriented language) is supposed to work. Its intended behavior, in fact Transform logs a warning when you attempt to do the same things for it's properties like position rotation");
        }

        if (Input.GetKeyDown("s"))
        {
            ImprimeTexto(TipoDeTexto.RESPOSTA ,"Lorem This is not a bug, that's how C# (or any object oriented language)");
        }

    }

}
