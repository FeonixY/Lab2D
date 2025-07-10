using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SDFMovement : MonoBehaviour
{
    [Header("GameObject")]
    public Transform Obstacles;
    public Tilemap RasterizedTilemap;
    public Tilemap EDTTilemap;
    
    [Header("Settings")]
    public int GridResolution = 256;
    public int ColorResolution = 16;
    public Vector2 WorldMin = new(-64, -64);
    public Vector2 WorldMax = new(64, 64);

    public static SDFMovement instance;

    private Tile blackTile;
    private Tile whiteTile;

    private readonly List<Tile> EDTTiles = new();
    private readonly List<PolygonCollider2D> colliders = new();
    private float[,] grid;
    private float[,] EDTgrid;

    private void OnValidate()
    {
        Initialize();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Initialize()
    {
        colliders.Clear();
        EDTTiles.Clear();
        for (int i = 0; i < Obstacles.childCount; i++)
        {
            colliders.Add(Obstacles.GetChild(i).GetComponent<PolygonCollider2D>());
        }

        whiteTile = ScriptableObject.CreateInstance<Tile>();
        whiteTile.color = Color.white;
        Texture2D whiteTexture = new(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Reinitialize(50, 50);
        whiteTexture.Apply();
        whiteTile.sprite = Sprite.Create(whiteTexture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));

        blackTile = ScriptableObject.CreateInstance<Tile>();
        blackTile.color = Color.black;
        Texture2D blackTexture = new(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Reinitialize(50, 50);
        blackTexture.Apply();
        blackTile.sprite = Sprite.Create(blackTexture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));

        for (int i = 0; i < ColorResolution; i++)
        {
            Tile SDFTile = ScriptableObject.CreateInstance<Tile>();
            Texture2D SDFTexture = new(50, 50);
            float color = i / (float)ColorResolution;
            for (int x = 0; x < SDFTexture.width; x++)
            {
                for (int y = 0; y < SDFTexture.height; y++)
                {
                    SDFTexture.SetPixel(x, y, new Color(color, color, color));
                }
            }
            SDFTexture.Apply();
            SDFTile.sprite = Sprite.Create(SDFTexture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));

            EDTTiles.Add(SDFTile);
        }

        grid = new float[GridResolution, GridResolution];
        EDTgrid = new float[GridResolution, GridResolution];
    }

    public void RunRasterization()
    {
        float cellWidth = (WorldMax.x - WorldMin.x) / GridResolution;
        float cellHeight = (WorldMax.y - WorldMin.y) / GridResolution;

        for (int x = 0; x < GridResolution; x++)
        {
            for (int y = 0; y < GridResolution; y++)
            {
                Vector2 cellCenter = new
                (
                    WorldMin.x + (x + 0.5f) * cellWidth,
                    WorldMin.y + (y + 0.5f) * cellHeight
                );

                bool hasCollider = false;
                foreach (PolygonCollider2D collider in colliders)
                {
                    if (collider.OverlapPoint(cellCenter))
                    {
                        hasCollider = true;
                        break;
                    }
                }

                Vector3Int tilePosition = new(x, y, 0);
                if (hasCollider)
                {
                    grid[x, y] = 1;
                    RasterizedTilemap.SetTile(tilePosition, blackTile);
                }
                else
                {
                    grid[x, y] = 0;
                    RasterizedTilemap.SetTile(tilePosition, whiteTile);
                }
            }
        }
    }

    public void EDT()
    {
        // 使用一维数组减少内存分配和提升缓存命中率
        int N = GridResolution;
        float[] G = new float[N * N];

        // 纵向距离变换
        for (int x = 0; x < N; x++)
        {
            float val = grid[x, 0] == 0 ? N * N + 1 : 0;
            G[x * N + 0] = val;
            for (int y = 1; y < N; y++)
            {
                val = (1 - grid[x, y]) * (1 + val);
                G[x * N + y] = val;
            }
            for (int y = N - 2; y >= 0; y--)
            {
                if (G[x * N + y] > G[x * N + y + 1] + 1)
                    G[x * N + y] = G[x * N + y + 1] + 1;
            }
        }

        // 横向距离变换
        for (int y = 0; y < N; y++)
        {
            // 1D距离变换
            float[] f = new float[N];
            for (int x = 0; x < N; x++)
                f[x] = G[x * N + y];

            int[] v = new int[N];
            float[] z = new float[N + 1];
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;

            for (int q = 1; q < N; q++)
            {
                float s;
                do
                {
                    s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2 * (q - v[k]));
                    if (s <= z[k]) k--;
                } while (s <= z[k]);
                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = float.PositiveInfinity;
            }

            k = 0;
            for (int x = 0; x < N; x++)
            {
                while (z[k + 1] < x) k++;
                float dist = (x - v[k]) * (x - v[k]) + f[v[k]];
                EDTgrid[x, y] = (float)Math.Sqrt(dist);

                int index = EDTgrid[x, y] == 0 ? 0 : Math.Clamp((int)(EDTgrid[x, y] / N * ColorResolution) + 1, 1, ColorResolution - 2);
                EDTTilemap.SetTile(new Vector3Int(x, y, 0), EDTTiles[index == 0 ? 0 : index + 1]);
            }
        }
    }

    public float Sample(Vector2 position)
    {
        int fx = Mathf.FloorToInt(position.x);
        int fy = Mathf.FloorToInt(position.y);
        float rx = position.x - fx;
        float ry = position.y - fy;

        int ix = fx + 64;
        int iy = fy + 64;
        int ix1 = Mathf.Clamp(ix + 1, 0, GridResolution - 1);
        int iy1 = Mathf.Clamp(iy + 1, 0, GridResolution - 1);
        ix = Mathf.Clamp(ix, 0, GridResolution - 1);
        iy = Mathf.Clamp(iy, 0, GridResolution - 1);

        return (EDTgrid[ix, iy] * (1 - rx) + EDTgrid[ix1, iy] * rx) * (1 - ry) +
               (EDTgrid[ix, iy1] * (1 - rx) + EDTgrid[ix1, iy1] * rx) * ry;
    }

    public Vector2 Gradient(Vector2 position)
    {
        float delta = 1f;
        return 0.5f * new Vector2
            (
            Sample(new Vector2(position.x + delta, position.y)) -
            Sample(new Vector2(position.x - delta, position.y)),
            Sample(new Vector2(position.x, position.y + delta)) -
            Sample(new Vector2(position.x, position.y - delta))
            );
    }

    public Vector2 GetValidPositionBySDF(Vector2 position, Vector2 direction, float speed, float playerRadius)
    {
        Vector2 newPosition = position + speed * Time.fixedDeltaTime * direction.normalized;
        float SD = Sample(newPosition);
        Debug.Log(SD);

        if (SD < playerRadius)
        {
            Vector2 gradient = Gradient(newPosition);
            Vector2 adjustPosition = direction - gradient * Vector2.Dot(gradient, position);
            newPosition = position + speed * Time.fixedDeltaTime * adjustPosition.normalized;
        }

        for (int i = 0; i < 4; i++)
        {
            SD = Sample(newPosition);
            if (SD >= playerRadius) break;
            newPosition += Gradient(newPosition) * (playerRadius - SD);
        }

        if (Vector2.Dot(newPosition - position, direction) < 0)
        {
            newPosition = position;
        }

        return newPosition;
    }
}
