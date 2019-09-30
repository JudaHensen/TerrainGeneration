using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    [SerializeField]
    [Header("Maximum terrain size.")]
    private Vector2Int terrainSize = new Vector2Int(30,30);

    [SerializeField]
    [Header("Distance between points.")]
    private int pointDistance = 10;

    [SerializeField]
    [Header("Maximum terrain height")]
    private float terrainHeight = 10;

    [SerializeField]
    [Header("Detail increases amount of points, terrain width, depth and height will stay the same")]
    private int detail = 1;

    [SerializeField]
    [Header("Increment of perlin noise.")]
    private float perlinIncrement = .25f;

    [SerializeField]
    [Header("Perlin noise x & y offset multiplier")]
    private float perlinOffsetMultiplier = 1024;

    [SerializeField]
    [Header("Parent object")]
    private GameObject parent;

    private Seeding seeding;

    // Perlin values
    private float[,] perlinValues;

    [SerializeField]
    private Camera camera;
    
    [SerializeField]
    [Header("Camera rotation speed")]
    private float rotationSpeed = .01f;
    private float rotation = 0;
    private float cameraHeight;
    private float distance = 1;

    void Start () {
        seeding = GameObject.FindObjectOfType<Seeding>();
        Generate();
    }

    public void Generate()
    {
        perlinValues = new float[terrainSize.x * detail, terrainSize.y * detail];

        GeneratePerlinNoise();
        CreateMesh();

        if (terrainSize.x > terrainSize.y) distance = terrainSize.x * pointDistance;
        else distance = terrainSize.y * pointDistance;

        cameraHeight = 245 * (pointDistance / 10);

        parent.transform.position = new Vector3((-terrainSize.x / 2) * pointDistance, 0, (-terrainSize.y / 2) * pointDistance);
    }

    public void SetDistanceBetween(float value)
    {
        pointDistance = (int)value;
    }
    public void SetDetail(float value)
    {
        detail = (int)value;
    }
    public void SetHeight(float value)
    {
        terrainHeight = (float)value;
    }
    public void SetPerlinIncrement(float value)
    {
        perlinIncrement = value;
    }


    public void Update()
    {
        // Update Camera
        Vector3 position = new Vector3();
        position.x = 0 + distance * Mathf.Cos(rotation * Mathf.PI / 180);
        position.y = cameraHeight;
        position.z = 0 + distance * Mathf.Sin(rotation * Mathf.PI / 180);

        Vector3 rotations = new Vector3(50, -90 - rotation, 0);

        camera.transform.position = position;
        camera.transform.eulerAngles = rotations;

        rotation += rotationSpeed * Time.deltaTime;
        rotation %= 360;
    }

    // Generates a perlin noise map
    private void GeneratePerlinNoise()
    {
        // perlin noise x & y offset
        float offsetX, offsetY;
        offsetX = seeding.NextValue() * perlinOffsetMultiplier;
        offsetY = seeding.NextValue() * perlinOffsetMultiplier;

        // Go through 2Dimensional array
        for (int i = 0; i < terrainSize.x * detail; i++)
        {
            for (int j = 0; j < terrainSize.y * detail; j++)
            {
                perlinValues[i, j] = Mathf.PerlinNoise(offsetX + ((i * perlinIncrement) / detail), offsetY + ((j * perlinIncrement) / detail));
            }
        }
    }

    // Create mesh
    private void CreateMesh()
    {
        Mesh terrain = new Mesh();
        parent.GetComponent<MeshFilter>().mesh = terrain;

        Vector3[] vertices;
        int[] triangles;

        vertices = new Vector3[ (terrainSize.x * detail)*(terrainSize.y * detail) ];
        triangles = new int[ ((terrainSize.x) * detail) * ((terrainSize.y) * detail) * 2 * 3];

        // create vertices
        int count = 0;
        for(int y = 0; y < terrainSize.y * detail; y++)
        {
            for (int x = 0; x < terrainSize.x * detail; x++)
            {
                vertices[count] = new Vector3( (x * pointDistance)/detail, perlinValues[x,y] * terrainHeight, (y * pointDistance)/detail );
                count++;
            }
        }

        // create triangles
        count = 0;
        for(int y = 0; y < (terrainSize.y * detail)-1; y++)
        {
            for (int x = 0; x < (terrainSize.x * detail)-1; x++)
            {

                triangles[count] = MultiDimensionToSingle(x + 1, y + 1);
                triangles[count + 1] = MultiDimensionToSingle(x + 1, y);
                triangles[count + 2] = MultiDimensionToSingle(x, y);

                count += 3;

                triangles[count] = MultiDimensionToSingle(x, y);
                triangles[count + 1] = MultiDimensionToSingle(x, y + 1);
                triangles[count + 2] = MultiDimensionToSingle(x + 1, y + 1);
                count += 3;

            }
        }

        // finish mesh
        terrain.vertices = vertices;
        terrain.triangles = triangles;

        terrain.RecalculateBounds();
        terrain.RecalculateNormals();
    }

    private int MultiDimensionToSingle(int x, int y)
    {
        int totalWidth = terrainSize.x * detail;
        return y*totalWidth + x;
    }

}
