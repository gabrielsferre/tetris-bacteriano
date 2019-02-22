using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

/// <summary>
/// Um game object com esta classe deve ser filho do objeto relógio.
/// </summary>
public class MeshRelogio : MonoBehaviour
{
    private Vector3[] mVertices;    //o vértice do meio possui index 0
    private Vector3[] mNormals;
    private Vector2[] mUVs;
    private Color[] mColors;
    private int[] mTriangles;

    private Vector3 mNormal = Vector3.forward;  //normal para todos os vértices

    private Color mColorBackground = Color.white;  //cor de fundo do relógio
    private Color mColorFill = Color.red;  //cor de preenchimento do relógio

    public float radius { get; private set; }
    private int subdivisions = 100;   //igual ao número de triângulos

    [Range(0,1)]
    private float pointerPosition = 0;  //quantos % do relógio estará preenchido

    private MeshRenderer meshRenderer;

    [SerializeField]
    private GameObject pointer;
    
    private void Start()
    {
        radius = GetComponentInParent<SpriteRenderer>().sprite.bounds.extents.x * 
            0.95f;  //redução do raio para que o fundo não apareça fora da moldura

        //Constrói o círculo
        mVertices = new Vector3[3 * subdivisions];
        mNormals = new Vector3[3 * subdivisions];
        mUVs = new Vector2[3 * subdivisions];
        mColors = new Color[3 * subdivisions];
        mTriangles = new int[3 * subdivisions];
        
        float angulo = Mathf.PI / 2;    //ângulo inicial
        float incremento_angulo = -2 * Mathf.PI / subdivisions; //círculo é construido no sentido horário
        for (int i = 0; i < subdivisions; i++, angulo += incremento_angulo)
        {
            //indice dos vértices de uma subdivisão
            int index_centro = 3 * i;   //vértice no centro do círculo
            int index_circ1 = 3 * i + 1;    //vértice na circunferência
            int index_circ2 = 3 * i + 2;    //vértice na circunferência

            //vértices
            float x1 = radius * Mathf.Cos(angulo);
            float y1= radius * Mathf.Sin(angulo);
            float x2 = radius * Mathf.Cos(angulo + incremento_angulo);
            float y2 = radius * Mathf.Sin(angulo + incremento_angulo);
            float z = transform.position.z;

            mVertices[index_centro] = new Vector3(0, 0, z);    //vértice do meio
            mVertices[index_circ1] = new Vector3(x1, y1, z);  //vértice na circunferência
            mVertices[index_circ2] = new Vector3(x2, y2, z);  //vértice na circunferência

            //normais
            mNormals[index_centro] = mNormal;
            mNormals[index_circ1] = mNormal;
            mNormals[index_circ2] = mNormal;

            //texturas
            mUVs[index_centro] = new Vector2(0, 0);
            mUVs[index_circ1] = new Vector2(x1, y1);
            mUVs[index_circ2] = new Vector2(x2, y2);

            //cor
            mColors[index_centro] = mColorBackground;
            mColors[index_circ1] = mColorBackground;
            mColors[index_circ2] = mColorBackground;

            //triângulos
            mTriangles[index_centro] = index_centro;
            mTriangles[index_circ1] = index_circ1;
            mTriangles[index_circ2] = index_circ2;
        }

        //Cria e atualiza mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = mVertices;
        mesh.normals = mNormals;
        mesh.uv = mUVs;
        mesh.colors = mColors;
        mesh.triangles = mTriangles;

        //Ponteiro
        pointer.transform.eulerAngles = new Vector3(0, 0, 90);

    }

    /// <summary>
    /// Incrementa a marcação do relógio de uma certa porcentagem relativa à área total do mesmo.
    /// </summary>
    public void Incrementa(float aumento)
    {
        PointerPosition += aumento;
    }

    /// <summary>
    /// Faz com que a marcação do relógio volte ao início.
    /// </summary>
    public void Zera()
    {
        PointerPosition = 0;
    }

    //Propriedades

    public Color MColorBackground
    {
        get { return mColorBackground; }
        set { mColorBackground = value; }
    }

    public Color MColorFill
    {
        get { return mColorFill; }
        set { mColorFill = value; }
    }

    public float PointerPosition
    {
        get { return pointerPosition; }
        set
        {
            if (value < 0) value = 0;
            else if (value > 1) value = 1;
            pointerPosition = value;

            //Preenche subdivisões
            int filled_sub = Mathf.RoundToInt(subdivisions * value);

            for(int i = 0; i < filled_sub; i++)
            {
                //index dos vértices de uma subdivisão
                int index_centro = 3 * i;   //vértice no centro do círculo
                int index_circ1 = 3 * i + 1;    //vértice na circunferência
                int index_circ2 = 3 * i + 2;    //vértice na circunferência

                //preenche subdivisão
                mColors[index_centro] = mColorFill;
                mColors[index_circ1] = mColorFill;
                mColors[index_circ2] = mColorFill;
            }

            GetComponent<MeshFilter>().mesh.colors = mColors;

            pointer.transform.eulerAngles = new Vector3(0, 0, -360 * value + 90);
            //negativo por causa do sentido horário e +90 pois o relógio começa com o ponteiro para cima
        }
    }
}
