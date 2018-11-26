using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BalaoDeTexto : MonoBehaviour {

    public TextMeshProUGUI tmp;
    private string text;
    public string Text
    {
        get
        {
            return text;
        }

        set
        {
            text = value;
            if(tmp != null)
            {
                tmp.text = value;
            }            
        }
    }

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = text;
    }


}
