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
    /// Yield instruction que lança uma bactéria e depois espera um certo tempo.
    /// </summary>
    private class CriaBacteria : CustomYieldInstruction
    {
        GameManager gameManager;

        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada
        int nivel;

        public override bool keepWaiting
        {
            get
            {
                if (Time.realtimeSinceStartup - tempoInicial >= tempo)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public CriaBacteria(GameManager gameManager, float tempo)
        {
            this.gameManager = gameManager;
            this.tempo = tempo;
            tempoInicial = Time.realtimeSinceStartup;

            gameManager.grade.CriaBacteria();
        }
    }

    /// <summary>
    /// Yield instruction que faz bactérias infectarem peças adjacentes.
    /// </summary>
    private class InfectaPecas : CustomYieldInstruction
    {
        GameManager gameManager;

        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada
        int nivel;

        public override bool keepWaiting
        {
            get
            {
                if (Time.realtimeSinceStartup - tempoInicial >= tempo)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public InfectaPecas(GameManager gameManager, float tempo)
        {
            this.gameManager = gameManager;
            this.tempo = tempo;
            tempoInicial = Time.realtimeSinceStartup;

            gameManager.grade.InfectaPecas();
        }
    }

    /// <summary>
    /// Yield instruction que envia uma mensagem na tela do chat.
    /// </summary>
    private class EnviaMensagem : CustomYieldInstruction
    {
        GameManager gameManager;

        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada
        TipoDeTexto tipo;
        string mensagem;

        public override bool keepWaiting
        {
            get
            {
                if (Time.time - tempoInicial >= tempo)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public EnviaMensagem(GameManager gameManager, float tempo, TipoDeTexto tipo, string mensagem)
        {
            this.gameManager = gameManager;
            this.tempo = tempo;
            this.tipo = tipo;
            this.mensagem = mensagem;
            tempoInicial = Time.time;

            if (tipo == TipoDeTexto.FALA)
            {
                print("Fala: " + mensagem);
            }
            if (tipo == TipoDeTexto.RESPOSTA)
            {
                print("Resposta: " + mensagem);
            }
        }
    }

    /// <summary>
    /// Yield instruction que regula o nível do medidor depois espera um certo tempo.
    /// </summary>
    private class RegulaMedidor: CustomYieldInstruction
    {
        GameManager gameManager;

        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada
        int nivel;

        public override bool keepWaiting
        {
            get
            {
                if( Time.time - tempoInicial >= tempo )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public RegulaMedidor(GameManager gameManager, float tempo, int nivel)
        {
            this.gameManager = gameManager;
            this.tempo = tempo;
            this.nivel = nivel;
            tempoInicial = Time.time;

            print("Medidor regulado");
        }
    }

    /// <summary>
    /// Controla a ordem dos acontecimentos no jogo
    /// </summary>
    private IEnumerator SequenciaJogo()
    {
        yield return new LoopTetris(this, 2);
        yield return new EnviaMensagem(this, 1,TipoDeTexto.FALA, "Estou passando mal");
        yield return new EnviaMensagem(this, 1, TipoDeTexto.RESPOSTA, "Toma antibiótico");
        yield return new CriaBacteria(this, 1);
        yield return new InfectaPecas(this, 1);
        yield return new LoopTetris(this, 4);
    }
}
