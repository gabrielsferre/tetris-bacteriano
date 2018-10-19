using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Efeitos {

    const float tempoFade = 0.5f;
    const float velocidadeQueda = 2f;

    /// <summary>
    /// Faz o quadrado brilhar e desaparecer gradativamente
    /// </summary>
    /// <param name="quadrado"></param>
	private static Tweener FadeQuadrado( QuadradoPeca quadrado)
    {
        if (quadrado != null)
        {
            SpriteRenderer renderer = quadrado.GetComponent<SpriteRenderer>();
            
            return renderer.DOColor(new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0), tempoFade) ;
        }
        else
        {
            Debug.Log("Quadrado nulo");
            return null;
        }
        
    }

    /// <summary>
    /// Cria sequencia com fades para todos os quadrados da linha
    /// </summary>
    /// <param name="linha"></param>
    /// <returns></returns>
    public static Sequence FadeLinha( QuadradoGrade[] linha )
    {
        Sequence sequencia = DOTween.Sequence();

        foreach( QuadradoGrade quadrado in linha)
        {
            if (quadrado.quadradoPeca != null)
            {
                sequencia.Insert(0, FadeQuadrado(quadrado.quadradoPeca));
            }
        }

        return sequencia;
    }

    public static Sequence FadeLinhas(List<QuadradoGrade[]> linhas)
    {
        Sequence sequencia = DOTween.Sequence();

        foreach (QuadradoGrade[] linha in linhas)
        {
            sequencia.Append(FadeLinha(linha));
        }
        
        return sequencia;
    }

    /// <summary>
    /// Cria tween que move o quadrado de "posInicial" para "posFinal"
    /// </summary>
    /// <param name="quadrado"></param>
    /// <param name="posInicial"></param>
    /// <param name="posFinal"></param>
    /// <returns></returns>
    public static Tweener MoveQuadrado( QuadradoPeca quadrado, float posInicial, float posFinal )
    {
        float tempo = Mathf.Abs(posFinal - posInicial) / velocidadeQueda;
        return quadrado.transform.DOMoveY(posFinal, tempo);
    }

    /// <summary>
    /// Cria sequencia que move a linha da "linhaInicial" para "linhaFinal" 
    /// </summary>
    /// <param name="linhaInicial"></param>
    /// <param name="linhaFinal"></param>
    /// <returns></returns>
    public static Sequence MoveLinha( QuadradoGrade[] linhaInicial, QuadradoGrade[] linhaFinal)
    {
        Sequence sequencia = DOTween.Sequence();

        if (linhaInicial != linhaFinal)
        {
            for (int j = 0; j < Grade.colunas; j++)
            {
                if (linhaInicial[j].quadradoPeca != null)
                {
                    float posInicial = linhaInicial[j].transform.position.y;
                    float posFinal = linhaFinal[j].transform.position.y;

                    sequencia.Insert(0, MoveQuadrado(linhaInicial[j].quadradoPeca, posInicial, posFinal));
                }

            }
            
        }

        return sequencia;
    }
}
