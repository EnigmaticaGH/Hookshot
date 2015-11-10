using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour 
{
    public int vertexCount;
    public GameObject moleculePrefab;

    GameObject[] molecules;

    LineRenderer waterLine;

    float yBottom;
    float yTop;
    float xLeft;

    float width;

	void Start () 
    {
        waterLine = GetComponent<LineRenderer>();
        waterLine.SetVertexCount(vertexCount);
        FindEquilibrium();
        SpawnWaterMolecules();
	}

    void FindEquilibrium()
    {
        BoxCollider2D waterBox = GetComponent<BoxCollider2D>();
        yBottom = waterBox.offset.y - waterBox.bounds.extents.y;
        yTop = waterBox.offset.y + waterBox.bounds.extents.y;
        xLeft = waterBox.offset.x - waterBox.bounds.extents.x;
        width = waterBox.bounds.size.x;
    }

    void SpawnWaterMolecules()
    {
        Vector2 spawnPos = new Vector2(0.0f, yTop);
        molecules = new GameObject[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            spawnPos.x = xLeft + (float)width / vertexCount * i;
            molecules[i] = (GameObject)Instantiate(moleculePrefab, spawnPos, transform.rotation);
        }
    }

    void Update()
    {
        DrawWater();
        DrawWaterLine();
    }
	
    void FixedUpdate()
    {
        PullMolecules();
    }

    private void PullMolecules()
    {
    }

    private Material lineMaterial;

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void DrawWater()
    {
        CreateLineMaterial();

        lineMaterial.SetPass(0);

        GL.PushMatrix();

        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.TRIANGLES);

        Color midnightBlue = new Color(0, 15, 40) * 0.9f;
        Color lightBlue = new Color(0.2f, 0.5f, 1f) * 0.8f;

        float scale = width / (molecules.Length - 1f);


        for (int i = 1; i < molecules.Length; i++ )
        {
            Vector3 mol_i = molecules[i-1].transform.position;
            Vector3 mol_i2 = molecules[i].transform.position;

            Vector2 p1 = new Vector2(mol_i.x, mol_i.y);
            Vector2 p2 = new Vector2(mol_i2.x, mol_i2.y);
            Vector2 p3 = new Vector2(p2.x, yBottom);
            Vector2 p4 = new Vector2(p1.x, yBottom);



            GL.Color(lightBlue);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.Color(midnightBlue);
            GL.Vertex(p3);

            GL.Color(lightBlue);
            GL.Vertex(p1);
            GL.Color(midnightBlue);
            GL.Vertex(p3);
            GL.Vertex(p4);
        }

        GL.End();

        GL.PopMatrix();
    }

    void DrawWaterLine()
    {
        for (int i = 0; i < vertexCount; i++)
            waterLine.SetPosition(i, molecules[i].transform.position);
    }
}
