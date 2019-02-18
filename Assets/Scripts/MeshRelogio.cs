using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float radius = 1;
    private int subdivisions = 100;   //igual ao número de triângulos

    private float pointer = 0;  //quantos % do relógio estará preenchido

    private MeshRenderer meshRenderer;

    private void Start()
    {
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
            //index dos vértices de uma subdivisão
            int index_centro = 3 * i;   //vértice no centro do círculo
            int index_circ1 = 3 * i + 1;    //vértice na circunferência
            int index_circ2 = 3 * i + 2;    //vértice na circunferência

            //posiciona vértices
            float x1 = radius * Mathf.Cos(angulo);
            float y1= radius * Mathf.Sin(angulo);
            float x2 = radius * Mathf.Cos(angulo + incremento_angulo);
            float y2 = radius * Mathf.Sin(angulo + incremento_angulo);

            mVertices[index_centro] = new Vector3(0, 0, 0);    //vértice do meio
            mVertices[index_circ1] = new Vector3(x1, y1, 0);  //vértice na circunferência
            mVertices[index_circ2] = new Vector3(x2, y2, 0);  //vértice na circunferência

            //posiciona normais
            mNormals[index_centro] = mNormal;
            mNormals[index_circ1] = mNormal;
            mNormals[index_circ2] = mNormal;

            //posiciona texturas
            mUVs[index_centro] = new Vector2(0, 0);
            mUVs[index_circ1] = new Vector2(x1, y1);
            mUVs[index_circ2] = new Vector2(x2, y2);

            //seleciona cor
            mColors[index_centro] = mColorBackground;
            mColors[index_circ1] = mColorBackground;
            mColors[index_circ2] = mColorBackground;

            //posiciona triângulos
            mTriangles[index_centro] = index_centro;
            mTriangles[index_circ1] = index_circ1;
            mTriangles[index_circ2] = index_circ2;
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = mVertices;
        mesh.normals = mNormals;
        mesh.uv = mUVs;
        mesh.colors = mColors;
        mesh.triangles = mTriangles;
    }

    /// <summary>
    /// Incrementa a marcação do relógio de uma certa porcentagem relativa à área total do mesmo.
    /// </summary>
    public void Incrementa(float aumento)
    {
        Pointer += aumento;
    }

    /// <summary>
    /// Faz com que a marcação do relógio volte ao início.
    /// </summary>
    public void Zera()
    {
        Pointer = 0;
    }

    //Properties

    public float Radius
    {
        get { return radius; }
        private set { radius = value; }
    }
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
    public float Pointer
    {
        get { return pointer; }
        set
        {
            if (value < 0) value = 0;
            else if (value > 1) value = 1;
            pointer = value;

            //Preenche subdivisões
            int filled_sub = Mathf.RoundToInt(subdivisions * value);

            for(int i = 0; i < filled_sub; i++)
            {
                //index dos vértices de uma subdivisão
                int index_centro = 3 * i;   //vértice no centro do círculo
                int index_circ1 = 3 * i + 1;    //vértice na circunferência
                int index_circ2 = 3 * i + 2;    //vértice na circunferência

                //seleciona cor
                mColors[index_centro] = mColorFill;
                mColors[index_circ1] = mColorFill;
                mColors[index_circ2] = mColorFill;
            }

            GetComponent<MeshFilter>().mesh.colors = mColors;
        }
    }
}
