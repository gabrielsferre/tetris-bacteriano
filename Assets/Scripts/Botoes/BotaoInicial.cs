using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotaoInicial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Transicao);
    }

    private void Transicao()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("JogoPrincipal");
    }
}
