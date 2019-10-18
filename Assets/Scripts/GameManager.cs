using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //Constantes
    const float spawnDelay = 0.2f; //delay para o spawn de uma peça
    public const float tempoDeEnfraquecimento = 1.5f; //tempo que leva para a bactéria se enfraquecer

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
            yield return StartCoroutine(grade.CriaPeca());
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
        float tempoDescida = 1.5f; //tempo adicional para linhas completar descerem

        public override bool keepWaiting
        {
            get
            {
                if (Time.realtimeSinceStartup - tempoInicial >= tempo + tempoDeEnfraquecimento + tempoDescida)
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

            gameManager.StartCoroutine(gameManager.grade.EnfraqueceBacterias(tipoBacteria));
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
            gameManager.incrementoRelogio = 0;
        }
    }

    /// <summary>
    /// Controla a ordem dos acontecimentos no jogo
    /// </summary>
    private IEnumerator SequenciaJogo()
    {
		//yield return new LoopTetris(this, 5);
		//yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "chuva de bactéria!");
		//yield return new EnviaMensagem(this, 2, TipoDeTexto.RESPOSTA, "ai droga");
		//yield return new CriaLinhaBacteria(this, TipoBacteria.Normal, 1.5f);
		//yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 1.5f);
		//yield return new EnviaMensagem(this, 1, TipoDeTexto.FALA, "tomando remédio");
		//yield return new EnviaMensagem(this, 1, TipoDeTexto.FALA, "relógio do remédio iniciado");
		//yield return new RegulaMedidor(this, 3, 2.5f);
		//yield return new IniciaRelogio(this, 0, 0.01f);
		//yield return new LoopTetris(this, 5);
		//yield return new EnviaMensagem(this, 1, TipoDeTexto.FALA, "enfraquecendo bactérias");
		//yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2);
		//yield return new LoopTetris(this, 3);
		//yield return new CriaBacteria(this, TipoBacteria.Normal, 1);
		//yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 1);
		//yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "bactérias se multiplicando");
		//yield return new InfectaPecas(this, 0.5f);
		//yield return new InfectaPecas(this, 0.5f);
		//yield return new InfectaPecas(this, 2);
		//yield return new LoopTetris(this, 10);
		//yield return new EnfraqueceBacterias(this, TipoBacteria.SuperBacteria, 2);
		//yield return new LoopTetris(this, 5);
		//yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "fim");

		yield return new WaitForSeconds(2.5f);
		yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "Olá");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Nesse jogo você vai simular o funcionamento de um órgão do seu corpo");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Forme linhas com os blocos para processá-los e garantir o bom funcionamento do organismo");
		yield return new LoopTetris(this, 8);
		yield return new CriaLinhaBacteria(this, TipoBacteria.Normal, 2);
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Oh não! Parece que algumas bactérias começaram a surgir");
		yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "Bactérias impedem que você processe uma linha de blocos, e de tempos em tempos vão se multiplicar");
		yield return new LoopTetris(this, 3);
		yield return new InfectaPecas(this, 0.5f);
		yield return new LoopTetris(this, 3);
		yield return new CriaBacteria(this, TipoBacteria.Normal, 0.1f);
		yield return new CriaBacteria(this, TipoBacteria.Normal, 1.5f);
		yield return new LoopTetris(this, 3);
		yield return new InfectaPecas(this, 0.5f);

		yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "Hum...");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Parece que o efeito das bactérias está começando a ser sentido pelo corpo");
		yield return new EnviaMensagem(this, 2, TipoDeTexto.RESPOSTA, "<sprite=0> GLUP <sprite=0>");
		yield return new RegulaMedidor(this, 3.5f, 0.8f);
		yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2.0f);
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Ah! Agora nosso corpo está devidamente medicado");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Enquanto o nível de remédio estiver acima da marca as bactérias serão enfraquecidas e podem ser eliminadas");
		yield return new EnviaMensagem(this, 5, TipoDeTexto.FALA, "O nível naturalmente cai com o tempo, mas o resto do corpo vai lembrar de tomar o remédio nas horas certas");
		yield return new RegulaMedidor(this, 3.5f, 0.8f);
		yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2.0f);

		for (int i = 0; i < 5; i++)
		{
			if (medidor.Quantidade < 0.5f)
			{
				yield return new EnviaMensagem(this, 2, TipoDeTexto.RESPOSTA, "<sprite=0> GLUP <sprite=0>");
				yield return new RegulaMedidor(this, 3.5f, 0.8f);
				yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2.0f);
			}

			yield return new LoopTetris(this, 2);
			yield return new CriaBacteria(this, TipoBacteria.Normal, 0.1f);
			yield return new CriaBacteria(this, TipoBacteria.Normal, 0.1f);
			yield return new CriaBacteria(this, TipoBacteria.Normal, 1.5f);
			
			yield return new InfectaPecas(this, 0.5f);
		}

		yield return new LoopTetris(this, 2);
		yield return new CriaBacteria(this, TipoBacteria.Normal, 0.1f);
		yield return new CriaBacteria(this, TipoBacteria.Normal, 1.5f);

		yield return new InfectaPecas(this, 0.5f);

		yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "Hum...");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Estranho parece que o corpo não está mais tomando o remédio");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Talvez ele esteja se sentindo melhor, mas não percebeu que ainda temos bactérias aqui!");

		for (int i = 0; i < 4; i++)
		{

			yield return new LoopTetris(this, 2);

			if (i >= 1)
				yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 0.1f);
			else
				yield return new CriaBacteria(this, TipoBacteria.Normal, 0.1f);

			yield return new CriaBacteria(this, TipoBacteria.Normal, 0.1f);
			yield return new CriaBacteria(this, TipoBacteria.Normal, 1.5f);

			if (medidor.CheckQuantidadeMinima())
			{
				yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2.0f);
			}
			else
			{
				yield return new InfectaPecas(this, 0.5f);
			}

		}

		yield return new EnviaMensagem(this, 2, TipoDeTexto.RESPOSTA, "<sprite=0> GLUP <sprite=0>");
		yield return new RegulaMedidor(this, 3.5f, 0.8f);

		yield return new EnviaMensagem(this, 2, TipoDeTexto.FALA, "Ufa!");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Parece que o corpo voltou a tomar sua medicação");
		
		yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2.0f);

		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Hum...");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Ah não! Parece que algumas bactérias adquiriram resistência à medicação convencional!");
		
		for (int i = 0; i < 4; i++)
		{
			if (medidor.Quantidade < 0.5f)
			{
				yield return new EnviaMensagem(this, 2, TipoDeTexto.RESPOSTA, "<sprite=0> GLUP <sprite=0>");
				yield return new RegulaMedidor(this, 3.5f, 0.8f);
			}

			yield return new LoopTetris(this, 1);			
			yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 0.1f);
			yield return new CriaBacteria(this, TipoBacteria.SuperBacteria, 1.5f);

			if (medidor.CheckQuantidadeMinima())
			{
				yield return new EnfraqueceBacterias(this, TipoBacteria.Normal, 2.0f);
			}
			else
			{
				yield return new InfectaPecas(this, 0.5f);
			}

		}

		yield return new EnviaMensagem(this, 2, TipoDeTexto.RESPOSTA, "<sprite=1> GLUP <sprite=1>");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Ok, parece que agora a medicação foi trocada");
		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Vamos torcer para que as superbactérias não resistam a essas também");
		yield return new RegulaMedidor(this, 3.5f, 0.8f);
		yield return new EnfraqueceBacterias(this, TipoBacteria.SuperBacteria, 2.0f);

		yield return new LoopTetris(this, 10);

		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Ufa! Parece que tudo voltou a correr bem!");

		yield return new LoopTetris(this, 1);

		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Mas cada vez que bactérias adquirem resistência a um tipo de medicação se torna mais difícil combatê-las");

		yield return new LoopTetris(this, 1);

		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Lembre-se de sempre respeitar os horários e doses do seu tratamento");

		yield return new LoopTetris(this, 1);

		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "E sempre consulte um médico antes de tomar qualquer medicação");

		yield return new LoopTetris(this, 1);

		yield return new EnviaMensagem(this, 4, TipoDeTexto.FALA, "Até a próxima e muito obrigado por jogar!");

		SceneManager.LoadScene(0);

	}
}
