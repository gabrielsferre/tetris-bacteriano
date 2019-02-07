using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadradoBacteria : Quadrado {

    /// <summary>
    /// Desce bactéria em dada coluna até ela encontrar um compartimento da grade já ocupado.
    /// Se o compartimento da linha superior da grade já estiver preenchido, deleta bactéria.
    /// </summary>    
    public void DesceBacteria(int coluna)
    {
        QuadradoGrade quadrado = QuadradoDesceBacteria(coluna);
        
        if(quadrado == null)
        {
            Destroy(this);
        }
        else
        {
            //coloca bactéria na grade
            quadrado.Preenche(this);

            //tamanho de uma célula da grade
            float tamanho = quadrado.GetComponent<SpriteRenderer>().bounds.size.y;

            //posição do acima da grade na dada coluna
            Vector3 posicaoInicial = grade.quadrados[0][coluna].transform.position + new Vector3(0, tamanho, -1);

            //coloca bactéria acima da tela
            transform.position = posicaoInicial;

            //cria animação de queda da bactéria
            Efeitos.MoveBacteria(this, transform.position.y, quadrado.transform.position.y);
        }
    }

    /// <summary>
    /// Da o quadrado da grade que será preenchido por DesceBactéria
    /// </summary>
    public QuadradoGrade QuadradoDesceBacteria(int coluna)
    {
        //pega posição acima do compartimento da grade ocupado
        int linha = PrimeiraLinhaOcupada(coluna) - 1;

        //se linha superior estava ocupada
        if (linha < 0)
        {
            return null;
        }

        else
        {
            return grade.quadrados[linha][coluna];
        }
    }

    /// <summary>
    /// Retorna a primeira linha (a linhas mais em cima) que está ocupada
    /// por alguma peça ou bactéria.
    /// </summary>
    /// <param name="coluna"></param>
    /// <returns></returns>
    private int PrimeiraLinhaOcupada(int coluna)
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
    public void TransformaAdjacentes()
    {
        //float aleatório entre 0 e 1
        float sorteado = Random.Range(0, 1f);

        if (sorteado < 0.5)
        {
            //se não for possível substituir a peça
            if (!SubstituiPeca(posicao + new Vector2Int(0, 1)))
            {
                SubstituiPeca(posicao + new Vector2Int(0, -1));
            }
        }
        else
        {
            //se não for possível substituir a peça
            if(!SubstituiPeca(posicao + new Vector2Int(0, -1)))
            {
                SubstituiPeca(posicao + new Vector2Int(0, 1));
            }
        }
    }

    /// <summary>
    /// Substitui peça em certa posição por bactéria.
    /// Não faz nada se nenhuma peça estiver naquela posição.
    /// Retorna se foi capaz de substituir alguma peça ou não.
    /// </summary>
    private bool SubstituiPeca(Vector2Int posicao)
    {
        //se posição for válida
        if (ValidaPosicao(posicao))
        {
            QuadradoGrade quadradoGrade = grade.quadrados[posicao.x][posicao.y];

            quadradoGrade.Esvazia();
            quadradoGrade.Preenche(Instantiate(this, transform.parent));

            return true;
        }

        return false;
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
}
