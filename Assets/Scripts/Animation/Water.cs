using UnityEngine;
using System.Collections;

public struct WaterColumn
{
    public GameObject surfaceParticle;
    public Transform transform;
    public Rigidbody2D body;

    public void Update(float targetHeight, float dampening, float tension)
    {
        float x = targetHeight - transform.position.y;
        Vector2 speed = body.velocity;
        speed.y += tension * x - speed.y * dampening;
        body.velocity = speed;
    }
}

public class Water : MonoBehaviour 
{
    public int vertexCount;
    public int propagationPasses;

	[Range(0, 10)]
    public float springConstant;

    public float dampening;

	[Range(-10, 0)]
    public float spread;

    public GameObject moleculePrefab;

    WaterColumn[] molecules;

    Mesh waterMesh;

    LineRenderer waterLine;

    float yBottom;
    float yTop;
    float xLeft;

    float width;
    float height;

	void Start () 
    {
        waterLine = GetComponent<LineRenderer>();
        waterLine.SetVertexCount(vertexCount);
        FindEquilibrium();
        SpawnWaterMolecules();
        SpawnWaterMesh();
	}

    void FindEquilibrium()
    {
        BoxCollider2D waterBox = GetComponent<BoxCollider2D>();
        yBottom = transform.position.y - waterBox.bounds.extents.y;
        yTop = transform.position.y + waterBox.bounds.extents.y;
        xLeft = transform.position.x - waterBox.bounds.extents.x;
        width = waterBox.bounds.size.x;
        height = waterBox.bounds.size.y;
    }

    void SpawnWaterMolecules()
    {
        molecules = new WaterColumn[vertexCount+2];
        float particlePadding = (float)width / vertexCount;
        float particleHalfPadding = particlePadding / 2.0f;

        SpawnMolecule(0, new Vector2(xLeft, yTop));

        Vector2 spawnPos = new Vector2(0.0f, yTop);
        for (int i = 1; i < vertexCount+2; i++)
        {
            spawnPos.x = xLeft + particlePadding * i + particleHalfPadding;
            SpawnMolecule(i, spawnPos);
        }

    }

    void SpawnMolecule(int i, Vector2 spawnPos)
    {
        molecules[i] = new WaterColumn();
        molecules[i].surfaceParticle = (GameObject)Instantiate(moleculePrefab, spawnPos, transform.rotation);
        molecules[i].transform = molecules[i].surfaceParticle.transform;
        molecules[i].transform.parent = transform;
        molecules[i].body = molecules[i].surfaceParticle.GetComponent<Rigidbody2D>();
    }

    void SpawnWaterMesh()
    {
        waterMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = waterMesh;
        waterMesh.vertices = GetMeshVertices();
        waterMesh.triangles = GetMeshTriangles();
    }

    private Vector3[] GetMeshVertices()
    {
        Vector3[] vertices = new Vector3[2 * molecules.Length];

        float scale = width / (molecules.Length - 1f);
        for (int i = 0; i < molecules.Length; i++ )
        {
            vertices[i] = molecules[i].transform.localPosition;
            vertices[i + molecules.Length] =
                new Vector2(molecules[i].transform.localPosition.x,
                            molecules[i].transform.localPosition.y - height);
        }

        return vertices;
    }
    
    private int[] GetMeshTriangles()
    {
        int[] triangles = new int[6 * (molecules.Length - 1)];

        for (int i = 0; i < molecules.Length - 1; i++)
        {
            triangles[6 * i] = i;
            triangles[6 * i + 1] = i + 1;
            triangles[6 * i + 2] = molecules.Length + i + 1;

            triangles[6 * i + 3] = i;
            triangles[6 * i + 4] = molecules.Length + i + 1;
            triangles[6 * i + 5] = molecules.Length + i;
        }

        return triangles;
    }

    void FixedUpdate()
    {
		try {
	        ApplySpringForces();
	        PropagateWaves();
	        DrawWaterSurface();
	        RepositionMesh();
		} catch(UnityException ex) {
			Debug.Log (ex.Message);
			Debug.Log (ex.StackTrace);
		}
    }

    void ApplySpringForces()
    {
        foreach(WaterColumn molecule in molecules) 
        {
            molecule.Update(yTop, dampening, springConstant);
        }
    }

    void PropagateWaves()
    {
        float[] leftDeltas = new float[molecules.Length];
        float[] rightDeltas = new float[molecules.Length];

        for (int j = 0; j < propagationPasses; j++)
        {
            for (int i = 0; i < molecules.Length; i++)
            {
                float k = yTop - molecules[i].transform.position.y;
                if (i > 0)
                {
                    float k2 = yTop - molecules[i - 1].transform.position.y;
                    leftDeltas[i] = spread * (k - k2);
                    molecules[i - 1].body.velocity +=
                        molecules[i - 1].body.velocity.normalized * leftDeltas[i] * Time.deltaTime;
                }

                if (i < molecules.Length - 1)
                {
                    float k2 = yTop - molecules[i + 1].transform.position.y;
                    rightDeltas[i] = spread * (k - k2);
                    molecules[i + 1].body.velocity +=
                        molecules[i + 1].body.velocity.normalized * rightDeltas[i] * Time.deltaTime;
                }
            }

            for (int i = 0; i < molecules.Length; i++)
            {
                float height = yTop - molecules[i].transform.position.y;

                if (i > 0)
                {
                    Vector3 pos = molecules[i - 1].transform.position;
                    pos.y += leftDeltas[i] * Time.fixedDeltaTime;
                   	molecules[i - 1].transform.position = pos;
                }

                if (i < molecules.Length - 1)
                {
                    Vector3 pos = molecules[i + 1].transform.position;
                    pos.y += rightDeltas[i] * Time.fixedDeltaTime;
                    molecules[i + 1].transform.position = pos;
                }
            }
        }
    }

    void DrawWaterSurface()
    {
        for (int i = 0; i < vertexCount; i++) {
			waterLine.SetPosition (i, molecules [i].transform.position);
		}
    }

    void RepositionMesh()
    {
        Vector3[] vertices = waterMesh.vertices;
        for (int i = 0; i < molecules.Length; i++)
            vertices[i] = molecules[i].transform.localPosition;
        waterMesh.vertices = vertices;
    }

}
