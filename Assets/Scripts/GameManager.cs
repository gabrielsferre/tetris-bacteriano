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

	public CaixaDeDialogo caixaDeDialogo;

	public MedidorDeRemedio medidor;

    public MeshRelogio relogio;

    private float incrementoRelogio = 0;    //quanto o relógio vai ser incrementado a cada peça posicionada
    private float incrementoMedidor = -0.05f;    //quanto o medidor vai ser incrementado a cada peça posicionada

    private void Awake()
    {
        //inicializações
        spawn = new SpawnPecas();
        grade = FindObjectOfType<Grade>();
        caixaDeDialogo = FindObjectOfType<CaixaDeDialogo>();
        medidor = FindObjectOfType<MedidorDeRemedio>();
        relogio = FindObjectOfType<MeshRelogio>();
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
        yield return new WaitForSeconds(spawnDelay);

        medidor.AdicionarRemedio(incrementoMedidor);
        relogio.Incrementa(incrementoRelogio);

        numeroPecas--;

        if (numeroPecas > -1)
        {
            grade.CriaPeca();
        }
    }

    /// <summary>
    /// Mantém um loop no jogo de tetris até que um certo
    /// número de peças seja usado.
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
            this.gameManager = gameManager;

            gameManager.numeroPecas = numeroPecas;
            gameManager.StartCoroutine(gameManager.SegueLoopTetris());
        }
    }

    /// <summary>
    /// Yield instruction que lança uma bactéria e depois espera um certo tempo.
    /// </summary>
    private class CriaBacteria : CustomYieldInstruction
    {
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

        public CriaBacteria(GameManager gameManager, TipoBacteria tipoBacteria, float tempo)
        {
            this.tempo = tempo;
            tempoInicial = Time.realtimeSinceStartup;

            if(tipoBacteria == TipoBacteria.Normal)
            {
                gameManager.grade.CriaBacteria();
            }
            else
            {
                gameManager.grade.CriaSuperBacteria();
            }
        }
    }

    /// <summary>
    /// Yield instruction que lança uma linha inteira de bactérias e depois espera um certo tempo.
    /// </summary>
    private class CriaLinhaBacteria : CustomYieldInstruction
    {
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

        public CriaLinhaBacteria(GameManager gameManager, TipoBacteria tipoBacteria, float tempo)
        {
            this.tempo = tempo;
            tempoInicial = Time.realtimeSinceStartup;

            if (tipoBacteria == TipoBacteria.Normal)
            {
                gameManager.grade.CriaLinhaBacteria();
            }
            else
            {
                gameManager.grade.CriaLinhaSuperBacteria();
            }
        }
    }

    /// <summary>
    /// Yield instruction que faz bactérias infectarem peças adjacentes.
    /// </summary>
    private class InfectaPecas : CustomYieldInstruction
    {
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
            this.tempo = tempo;
            tempoInicial = Time.realtimeSinceStartup;

            gameManager.grade.InfectaPecas();
        }
    }

    /// <summary>
    /// Yield instruction que enfraquece as bactérias especificadas.
    /// Importante: não espera as animações de eliminação/queda de linhas para continuar
    /// com a sequência
    /// </summary>
    private class EnfraqueceBacterias : CustomYieldInstruction
    {
        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada
        float tempoAdicional = 1.5f; //tempo acrescido para esperar a animação das linhas descendo
        int nivel;

        public override bool keepWaiting
        {
            get
            {
                if (Time.realtimeSinceStartup - tempoInicial >= tempo + tempoAdicional)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public EnfraqueceBacterias(GameManager gameManager, TipoBacteria tipoBacteria, float tempo)
        {
            this.tempo = tempo;
            tempoInicial = Time.realtimeSinceStartup;

            tempoAdicional = gameManager.grade.EnfraqueceBacterias(tipoBacteria) ? tempoAdicional : 0;
        }
    }

    /// <summary>
    /// Yield instruction que envia uma mensagem na tela do chat.
    /// </summary>
    private class EnviaMensagem : CustomYieldInstruction
    {
        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada

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
            this.tempo = tempo;
            tempoInicial = Time.time;

			gameManager.caixaDeDialogo.ImprimeTexto(tipo, mensagem);
        }
    }

    /// <summary>
    /// Yield instruction que regula o nível do medidor depois espera um certo tempo.
    /// </summary>
    private class RegulaMedidor: CustomYieldInstruction
    {
        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada

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

        public RegulaMedidor(GameManager gameManager, float tempo, float nivel)
        {
            this.tempo = tempo;
            tempoInicial = Time.time;

			gameManager.medidor.Quantidade = nivel;
        }
    }

    /// <summary>
    /// Yield instruction que inicia a contagem do relógio
    /// </summary>
    private class IniciaRelogio : CustomYieldInstruction
    {
        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada

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

        public IniciaRelogio(GameManager gameManager, float tempo, float incremento)
        {
            this.tempo = tempo;
            tempoInicial = Time.time;

            gameManager.incrementoRelogio = incremento;
        }
    }

    /// <summary>
    /// Yield instruction que zera a contagem do relógio
    /// </summary>
    private class ZeraRelogio : CustomYieldInstruction
    {
        float tempo;    //tempo que a yield instruction irá durar
        float tempoInicial; //momento em que a yield instruction é criada

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

        public ZeraRelogio(GameManager gameManager, float tempo)
        {
            this.tempo = tempo;
            tempoInicial = Time.time;
            
            gameManager.relogio.Zera();
        }
    }

    /// <summary>
    /// Controla a ordem dos acontecimentos no jogo
    /// </summary>
    private IEnumerator SequenciaJogo()
    {
        yield return new LoopTetris(this, 5);
        yield return new CriaLinhaBacteria(this, TipoBacteria.Normal, 1.5f);
        yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 1.5f);
        yield return new RegulaMedidor(this, 1, 2.5f);
        yield return new IniciaRelogio(this, 0, 0.01f);
        yield return new EnfraqueceBacterias(this, TipoBacteria.SuperBacteria, 0);
        yield return new LoopTetris(this, 5);
        yield return new CriaBacteria(this, TipoBacteria.Normal, 1);
        yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 1);
        yield return new InfectaPecas(this, 1);
        yield return new LoopTetris(this, 10);
    }
}
