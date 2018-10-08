using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum que define os diferentes formatos de peças
/// </summary>
public enum TipoPeca
{
    L_esq,
    L_dir,
    T,
    I,
    QUADRADO,
    S,
    Z,
    NUM_TIPOS
};

public static class DicionarioDeTipos
{
    public static Dictionary<TipoPeca, Peca> CriaDicionario()
    {
        Dictionary<TipoPeca, Peca> dicionario = new Dictionary<TipoPeca, Peca>();
        dicionario[TipoPeca.L_esq] = null;
        dicionario[TipoPeca.L_dir] = null;
        dicionario[TipoPeca.T] = null;
        dicionario[TipoPeca.I] = null;
        dicionario[TipoPeca.QUADRADO] = null;
        dicionario[TipoPeca.S] = null;
        dicionario[TipoPeca.Z] = null;

        return dicionario;
    }
}

//comentário para lembrar das cores de cada tipo de peça
/**
case TipoPeca.I: return new Color32(0,255,255,255);
case TipoPeca.L_esq: return new Color32(0, 0, 255, 255);
case TipoPeca.L_dir: return new Color32(255, 165, 0, 255);
case TipoPeca.QUADRADO: return new Color32(255, 255, 0, 255);
case TipoPeca.T: return new Color32(170, 0, 255, 255);
case TipoPeca.S: return new Color32(0, 255, 0, 255);
case TipoPeca.Z: return new Color32(255, 0, 0, 255);
**/
        













