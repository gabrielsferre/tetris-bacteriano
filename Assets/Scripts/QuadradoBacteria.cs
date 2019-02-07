using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoBacteria
{
    Normal,
    SuperBacteria
};

public class QuadradoBacteria : Quadrado {


    public TipoBacteria tipoBacteria = TipoBacteria.Normal;
    public Sprite spriteSuperBacteria;

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
        Vector2Int esquerda = posicao + new Vector2Int(0, -1);
        Vector2Int direita = posicao + new Vector2Int(0, 1);
        Vector2Int baixo = posicao + new Vector2Int(1, 0);
        Vector2Int cima = posicao + new Vector2Int(-1, 0);

        if (SubstituiPeca(direita)) return;
        else if (SubstituiPeca(esquerda)) return;
        else if (SubstituiPeca(cima)) return;
        else SubstituiPeca(baixo);
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

            QuadradoBacteria novaBacteria = Instantiate(this, transform.parent);
            
            if( tipoBacteria == TipoBacteria.SuperBacteria )
            {
                novaBacteria.ViraSuperBacteria();
            }

            quadradoGrade.Preenche(novaBacteria);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Retorna true se a posição for passível de ser infectada.
    /// </summary>
    /// <returns></returns>
    private bool ValidaPosicao(Vector2Int posicao)
    {
        //checa se posição está fora da grade
        if (!ChecaLimite(posicao))
        {
            return false;
        }


        QuadradoGrade quadradoGrade = grade.quadrados[posicao.x][posicao.y];

        //posição não está ocupada por nada
        if (quadradoGrade.interior == Preenchimento.Livre)
        {
            return false;
        }

        //posição está ocupada por alguma bacteria
        if (quadradoGrade.interior == Preenchimento.Bacteria)
        {
            QuadradoBacteria bacteria = (QuadradoBacteria)quadradoGrade.quadradoPeca;

            //se a bactéria nesta posicao for uma super bactéria
            //ou bactéria que está tentando infectar esta posição for uma bactéria normal
            if (bacteria.tipoBacteria == TipoBacteria.SuperBacteria || tipoBacteria == TipoBacteria.Normal)
            {
                return false;
            }
        }

        //posição esta ocupada por 
        return true;
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

    /// <summary>
    /// Transforma bacteria em superbacteria
    /// </summary>
    public void ViraSuperBacteria()
    {
        GetComponent<SpriteRenderer>().sprite = spriteSuperBacteria;
        tipoBacteria = TipoBacteria.SuperBacteria;
    }
}
