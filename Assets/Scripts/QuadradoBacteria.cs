using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadradoBacteria : Quadrado {

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Desce bactéria em dada coluna até ela encontrar um compartimento da grade já ocupado.
    /// Se o compartimento da linha superior da grade já estiver preenchido, deleta bactéria.
    /// </summary>    
    public void desceBacteria(int coluna)
    {
        //pega posição acima do compartimento da grade ocupado
        int linha = primeiraLinhaOcupada(coluna) - 1;

        //se linha superior estava ocupada
        if (linha < 0)
        {
            Destroy(gameObject);
        }

        else
        {
            grade.quadrados[linha][coluna].Preenche(this);
        }
    }

    /// <summary>
    /// Retorna a primeira linha (a linhas mais em cima) que está ocupada
    /// por alguma peça ou bactéria.
    /// </summary>
    /// <param name="coluna"></param>
    /// <returns></returns>
    private int primeiraLinhaOcupada(int coluna)
    {
        int i;

        //checa qual é a primeira linha ocupada
        for (i = 0; i < Grade.linhas; i++)
        {
            if (grade.quadrados[i][coluna].interior != Preenchimento.Livre)
            {
                break;
            }
        }

        return i;
    }

    /// <summary>
    /// Transforma peças adjacentes à bactéria em bactérias
    /// </summary>
    /// <returns></returns>
    private void TransformaAdjacentes()
    {
        SubstituiPeca(posicao + new Vector2Int(1, 0));
        SubstituiPeca(posicao + new Vector2Int(-1, 0));
        SubstituiPeca(posicao + new Vector2Int(0, 1));
        SubstituiPeca(posicao + new Vector2Int(0, -1));
    }

    /// <summary>
    /// Substitui peça em certa posição por bactéria.
    /// Não faz nada se nenhuma peça estiver naquela posição.
    /// </summary>
    private void SubstituiPeca(Vector2Int posicao)
    {
        //se posição for válida
        if (ValidaPosicao(posicao))
        {
            QuadradoGrade quadradoGrade = grade.quadrados[posicao.x][posicao.y];

            quadradoGrade.Esvazia();
            quadradoGrade.Preenche(Instantiate(this, transform.parent));
        }
    }

    /// <summary>
    /// Retorna 'true' se posição não estiver fora dos limites da grade
    /// e corresponder a um compartimento da grade preenchido por alguma 
    /// peça.
    /// </summary>
    /// <returns></returns>
    private bool ValidaPosicao(Vector2Int posicao)
    {
        //checa se posição está fora da grade
        if (!ChecaLimite(posicao))
        {
            return false;
        }
        
        //diz se posição está ocupada por alguma peça
        return grade.quadrados[posicao.x][posicao.y].interior == Preenchimento.Peca;
    }

    /// <summary>
    /// Retorna 'true' se posição estiver dentro da grade
    /// </summary>
    /// <param name="posicao"></param>
    /// <returns></returns>
    private bool ChecaLimite(Vector2Int posicao)
    {
        bool checaLinha = (posicao.x >= 0 && posicao.x < Grade.linhas);
        bool checaColuna = (posicao.y >= 0 && posicao.y < Grade.colunas);
        return checaLinha && checaColuna;
    }


    //código temporário para testes
    private void HandleInput()
    {
        PlayerKeys playerKeys = GetComponentInParent<PlayerKeys>();

        if (playerKeys.GetX())
        {
            TransformaAdjacentes();
        }
    }
}
