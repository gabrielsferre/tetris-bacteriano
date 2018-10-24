using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    //Grade onde ficam as peças
    public Grade grade;

    //Spawn de peças
    private SpawnPecas spawn;

    private void Awake()
    {
        spawn = new SpawnPecas();

        Init();
    }

    /// <summary>
    /// Inicializa propriedades
    /// </summary>
    private void Init()
    {
        InitGrade();
    }

    /// <summary>
    /// Inicializa propriedade que corresponde a grade
    /// </summary>
    private void InitGrade()
    {
        grade.spawn = spawn;
    }
}
