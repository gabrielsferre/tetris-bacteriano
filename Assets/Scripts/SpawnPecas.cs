using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPecas {

    //número de peças que irão aparecer na sequencia das próximas peças
    public int numProximasPecas { get; private set; }

    //lista com sequencia das proximas peças que irão aparecer
    public List<TipoPeca> listaProximasPecas { get; private set; }
    
    //construtor que inicializa propriedades com valor padrão
    public SpawnPecas()
    {
        numProximasPecas = 3;
        InicializaLista(numProximasPecas);
    }

    /// <summary>
    /// Inicializa lista que possui a sequência das próximas peças atribuindo um dado tamanho.
    /// A sequência é preenchida com tipos aleatórios de peças não repetidos.
    /// O tamanho da lista é limitado também pela quantidade de tipos de peças.
    /// </summary>
    private void InicializaLista(int tamanho)
    {
        listaProximasPecas = new List<TipoPeca>();
        
        for( int i = 0; i < numProximasPecas && i < (int)TipoPeca.NUM_TIPOS; i++)
        {
            listaProximasPecas.Add(GeraPeca());
        }
    }

    /// <summary>
    /// Gera um tipo de peça aleatório que ainda não
    /// esteja na lista com as próximas peças.
    /// </summary>
    private TipoPeca GeraPeca()
    {
        //cria lista com tipos que não estão na lista com as próximas peças
        List<TipoPeca> listaExcluidos = new List<TipoPeca>();
        for(int i = 0; i < (int)TipoPeca.NUM_TIPOS; i++)
        {
            if( !listaProximasPecas.Contains((TipoPeca)i) )
            {
                listaExcluidos.Add((TipoPeca)i);
            }
        }
        
        //retorna tipo aleatória dentro de listaExcluidos
        return listaExcluidos[Random.Range(0, listaExcluidos.Count)];
    }

    /// <summary>
    /// Retorna a próxima peça da lista de peças, retirando
    /// ela da lista e inserindo uma nova no final
    /// </summary>
    /// <returns></returns>
    public TipoPeca ProximaPeca()
    {
        //salva peça
        TipoPeca proxima = listaProximasPecas[0];

        //remove peça da lista
        listaProximasPecas.RemoveAt(0);

        //insere nova peça no final da lista
        listaProximasPecas.Add(GeraPeca());

        return proxima;
    }
}
