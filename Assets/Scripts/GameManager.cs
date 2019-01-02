using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //Constantes
    const float spawnDelay = 0.2f; //delay para o spawn de uma peça

    //Grade onde ficam as peças
    private Grade grade;

    //Spawn de peças
    private SpawnPecas spawn;

    //objeto usado nos yields ao longo da corrotina LoopJogo
    private LoopTetris loopTetris = new LoopTetris(0);

    private void Awake()
    {
        //inicializa grade
        spawn = new SpawnPecas();
        grade = FindObjectOfType<Grade>();
        grade.spawn = spawn;
    }

    private void Start()
    {
        StartCoroutine(LoopJogo());
    }

    /// <summary>
    /// Cria outra peça caso a número de peças do loop ainda
    /// não tenha se esgotado
    /// </summary>
    public IEnumerator SegueLoopTetris()
    {
        loopTetris.numeroPecas--;

        if (loopTetris.numeroPecas > -1)
        {
            yield return new WaitForSeconds(spawnDelay);

            grade.CriaPeca();
        }
        else
        {
            yield return null;
        }
    }

    /// <summary>
    /// Envia nova mensagem para o chat
    /// </summary>
    private void EnviaMensagem()
    {
        print("Mensagem");
    }

    /// <summary>
    /// Altera o nivel no medidor de remédio
    /// </summary>
    private void RegulaMedidor()
    {
        print("Medidor");
    }
    
    /// <summary>
    /// Controla a ordem dos acontecimentos no jogo
    /// </summary>
    private IEnumerator LoopJogo()
    {
        loopTetris.numeroPecas = 5; StartCoroutine(SegueLoopTetris());
        yield return loopTetris;
        print("oi");
    }

    /// <summary>
    /// Mantém um loop no jogo de tetris até que um certo
    /// número de peças sejam usadas.
    /// </summary>
    private class LoopTetris: CustomYieldInstruction
    {
        public int numeroPecas;

        public override bool keepWaiting
        {
            get
            {
                return numeroPecas > -1;
            }
        }

        public LoopTetris(int numeroPecas)
        {
            this.numeroPecas = numeroPecas;
        }
    }

}
