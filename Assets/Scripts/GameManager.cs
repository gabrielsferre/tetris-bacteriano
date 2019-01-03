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
    private int numeroPecas = 0;

    private void Awake()
    {
        //inicializa grade
        spawn = new SpawnPecas();
        grade = FindObjectOfType<Grade>();
        grade.spawn = spawn;
    }

    private void Start()
    {
        StartCoroutine(SequenciaJogo());
    }

    /// <summary>
    /// Cria outra peça caso a número de peças do loop ainda
    /// não tenha se esgotado
    /// </summary>
    public IEnumerator SegueLoopTetris()
    {
        numeroPecas--;

        if (numeroPecas > -1)
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
    /// Joga bactéria no campo de tetris.
    /// O argumento 'tempo' define o tempo de espera para que alguma outra
    /// ação seja feita após a bactéria ser enviada.
    /// </summary>
    /// <param name="tempo"></param>
    /// <returns></returns>
    private IEnumerator CriaBacteria(float tempo)
    {
        grade.CriaBacteria();
        yield return new WaitForSeconds(1+tempo);
    }

    /// <summary>
    /// Transforma em bactéria peças que estejam do lado de bactérias.
    /// O argumento 'tempo' define o tempo de espera para que alguma outra
    /// ação seja feita após as peças serem infectadas.
    /// </summary>
    /// <param name="tempo"></param>
    /// <returns></returns>
    private IEnumerator InfectaPecas(float tempo)
    {
        grade.InfectaPecas();
        yield return new WaitForSeconds(tempo);
    }

    /// <summary>
    /// Envia mensagem de um certo tipo (fala ou resposta) para o chat.
    /// O argumento 'tempo' define o tempo de espera para que alguma outra
    /// ação seja feita após a mensagem ser enviada.
    /// </summary>
    private IEnumerator EnviaMensagem(string mensagem, TipoDeTexto tipo, float tempo)
    {
        if( tipo == TipoDeTexto.FALA )
        {
            print("Mensagem de fala");
        }
        if( tipo == TipoDeTexto.RESPOSTA )
        {
            print("Mensagem de resposta");
        }
        yield return new WaitForSeconds(tempo);
    }

    /// <summary>
    /// Regula nível do medidor de remédio.
    /// O argumento 'tempo' define o tempo de espera para que alguma outra
    /// ação seja feita após o medidor ser regulado.
    /// </summary>
    private IEnumerator RegulaMedidor(int nivel, float tempo)
    {
        print("Medidor regulado");
        yield return new WaitForSeconds(tempo);
    }

    /// <summary>
    /// Mantém um loop no jogo de tetris até que um certo
    /// número de peças sejam usadas.
    /// </summary>
    private class LoopTetris: CustomYieldInstruction
    {
        GameManager gameManager;

        public override bool keepWaiting
        {
            get
            {
                return gameManager.numeroPecas > -1;
            }
        }

        /// <summary>
        /// Recebe a instancia do objeto GameManager e o
        /// número de peças que a rodada de tetris terá.
        /// </summary>
        /// <param name="gameManager"></param>
        /// <param name="numeroPecas"></param>
        public LoopTetris(GameManager gameManager, int numeroPecas)
        {
            gameManager.numeroPecas = numeroPecas;
            gameManager.StartCoroutine(gameManager.SegueLoopTetris());
            this.gameManager = gameManager;
        }
    }

    /// <summary>
    /// Controla a ordem dos acontecimentos no jogo
    /// </summary>
    private IEnumerator SequenciaJogo()
    {
        yield return new LoopTetris(this, 5);
        print("oi");
        yield return new LoopTetris(this, 4);
    }
}
