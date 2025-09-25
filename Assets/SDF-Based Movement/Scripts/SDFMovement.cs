using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SDFMovement : SingletonMonoBehaviour<SDFMovement>
{
    [Header("GameObject")]
    public Transform Obstacles;
    public Tilemap RasterizedTilemap;
    public Tilemap EDTTilemap;
    
    [Header("Settings")]
    public int GridResolution = 256;
    public int ColorResolution = 32;
    public Vector2 WorldMin = new(-64, -64);
    public Vector2 WorldMax = new(64, 64);

    [SerializeField]
    private Tile _blackTile;
    [SerializeField]
    private Tile _whiteTile;
    [SerializeField]
    private List<PolygonCollider2D> _colliders = new();
    [SerializeField]
    private List<Tile> _gradientTiles = new();
    
    private float[,] grid;
    private float[,] EDTgrid;

    private float cellWidth;
    private float cellHeight;

    private void Start()
    {
        Initialize();
        RunRasterization();
        EDT();
    }

    public void Initialize()
    {
        _colliders.Clear();
        foreach (Transform child in Obstacles)
        {
            if (child.TryGetComponent(out PolygonCollider2D polygonCollider)) _colliders.Add(polygonCollider);
        }
        
        if (_whiteTile == null || _blackTile == null)
        {
            _whiteTile = CreateSolidTile(Color.white);
            _blackTile = CreateSolidTile(Color.black);
        }

        if (_gradientTiles.Count != ColorResolution)
        {
            _gradientTiles.Clear();
            for (int i = 0; i < ColorResolution; i++)
            {
                float color = (float)i / (ColorResolution - 1);
                _gradientTiles.Add(CreateSolidTile(new (color, color, color, 1f)));
            }
        }

        grid = new float[GridResolution, GridResolution];
        EDTgrid = new float[GridResolution, GridResolution];
    }

    private Tile CreateSolidTile(Color color)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        Texture2D texture = new(50, 50);
        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                texture.SetPixel(x, y, color);
        texture.Apply();
        tile.sprite = Sprite.Create(texture, new(0f, 0f, 50f, 50f), new (0.5f, 0.5f));
        return tile;
    }

    public void RunRasterization()
    {
        cellWidth = (WorldMax.x - WorldMin.x) / GridResolution;
        cellHeight = (WorldMax.y - WorldMin.y) / GridResolution;

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
                foreach (PolygonCollider2D poly in _colliders)
                {
                    if (poly.OverlapPoint(cellCenter))
                    {
                        hasCollider = true;
                        break;
                    }
                }

                grid[x, y] = hasCollider ? 1f : 0f;
                RasterizedTilemap.SetTile(
                    RasterizedTilemap.WorldToCell(cellCenter),
                    hasCollider ? _blackTile : _whiteTile
                );
            }
        }
    }
    
    public static float[,] Compute(bool[,] mask)
    {
        if (mask == null) throw new ArgumentNullException(nameof(mask));
        int rows = mask.GetLength(0);
        int cols = mask.GetLength(1);
        float[,] distancesSquared = new float[rows, cols];

        // Choose a finite value that is larger than any possible squared distance in the image.
        float INF = float.PositiveInfinity;

        float[] f = new float[Math.Max(rows, cols)];
        float[] d = new float[Math.Max(rows, cols)];

        // Initialize: 0 for foreground, INF for background
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                distancesSquared[y, x] = mask[y, x] ? 0f : INF;
            }
        }

        // Horizontal pass
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++) f[x] = distancesSquared[y, x];
            OneDimensionalTransform(f, d, cols);
            for (int x = 0; x < cols; x++) distancesSquared[y, x] = d[x];
        }

        // Vertical pass
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++) f[y] = distancesSquared[y, x];
            OneDimensionalTransform(f, d, rows);
            for (int y = 0; y < rows; y++) distancesSquared[y, x] = d[y];
        }

        // Square root
        float[,] result = new float[rows, cols];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                float value = distancesSquared[y, x];
                result[y, x] = (float)(Mathf.Approximately(value, INF) ? float.PositiveInfinity : Math.Sqrt(value));
            }
        }
        return result;
    }

        private static void OneDimensionalTransform(float[] f, float[] d, int n)
        {
            int[] v = new int[n];
            float[] z = new float[n + 1];
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;

            float Intersection(int p, int q)
            {
                return (float)((f[q] + q * (float)q - (f[p] + p * (float)p)) / (2.0 * (q - p)));
            }

            // Forward scan to build lower hull
            for (int q = 1; q < n; q++)
            {
                float s = Intersection(v[k], q);
                while (k > 0 && s <= z[k])
                {
                    k--;
                    s = Intersection(v[k], q);
                }
                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = float.PositiveInfinity;
            }

            // Backward scan to compute distances
            k = 0;
            for (int q = 0; q < n; q++)
            {
                while (z[k + 1] < q) k++;
                int i = v[k];
                float dx = q - i;
                d[q] = dx * dx + f[i];
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

                int index = EDTgrid[x, y] == 0 ? 0 : Math.Clamp((int)(EDTgrid[x, y] / N * 16 * ColorResolution) + 1, 1, ColorResolution - 2);
                EDTTilemap.SetTile(new(x, y, 0), _gradientTiles[index == 0 ? 0 : index + 1]);
            }
        }
    }

    private float Sample(Vector2 position)
    {
        if (cellWidth <= 0f || cellHeight <= 0f)
            return float.MaxValue;

        float gx = (position.x - WorldMin.x) / cellWidth;
        float gy = (position.y - WorldMin.y) / cellHeight;

        int fx = Mathf.FloorToInt(gx);
        int fy = Mathf.FloorToInt(gy);
        float rx = gx - fx;
        float ry = gy - fy;

        int ix = Mathf.Clamp(fx, 0, GridResolution - 1);
        int iy = Mathf.Clamp(fy, 0, GridResolution - 1);
        int ix1 = Mathf.Clamp(fx + 1, 0, GridResolution - 1);
        int iy1 = Mathf.Clamp(fy + 1, 0, GridResolution - 1);

        float a = EDTgrid[ix, iy] * (1 - rx) + EDTgrid[ix1, iy] * rx;
        float b = EDTgrid[ix, iy1] * (1 - rx) + EDTgrid[ix1, iy1] * rx;
        float result = a * (1 - ry) + b * ry;

        return float.IsNaN(result) ? float.MaxValue : result;
    }

    private Vector2 Gradient(Vector2 position)
    {
        if (cellWidth <= 0f || cellHeight <= 0f)
            return Vector2.zero;

        float dx = (Sample(new (position.x + cellWidth, position.y)) -
                    Sample(new (position.x - cellWidth, position.y))) / (2f * cellWidth);
        float dy = (Sample(new (position.x, position.y + cellHeight)) -
                    Sample(new (position.x, position.y - cellHeight))) / (2f * cellHeight);

        Vector2 g = new (dx, dy);
        return (float.IsNaN(g.x) || float.IsNaN(g.y)) ? Vector2.zero : g;
    }

    public Vector2 GetValidPositionBySDF(Vector2 position, Vector2 direction, float speed, float playerRadius)
    {
        Vector2 dirNorm = direction.normalized;
        Vector2 newPosition = position + speed * Time.fixedDeltaTime * dirNorm;
        float sd = Sample(newPosition);

        if (sd < playerRadius)
        {
            Vector2 gradient = Gradient(newPosition);
            if (gradient.sqrMagnitude > 1e-6f)
            {
                Vector2 gradN = gradient.normalized;
                Vector2 adjustDirection = dirNorm - gradN * Vector2.Dot(dirNorm, gradN);
                if (adjustDirection.sqrMagnitude < 1e-6f)
                    adjustDirection = Vector2.Perpendicular(gradN);
                newPosition = position + speed * Time.fixedDeltaTime * adjustDirection.normalized;
            }
            else
            {
                newPosition = position;
            }
        }
        
        float maxPush = playerRadius * 4f;
        float totalPush = 0f;

        for (int i = 0; i < 4; i++)
        {
            sd = Sample(newPosition);
            if (sd >= playerRadius) break;

            Vector2 grad = Gradient(newPosition);
            if (grad.sqrMagnitude > 1e-6f)
            {
                float push = playerRadius - sd;
                totalPush += push;
                if (totalPush > maxPush) break;
                newPosition += grad.normalized * push;
            }
            else
            {
                newPosition -= dirNorm * ((playerRadius - sd) * 0.5f);
            }
        }

        if (Vector2.Dot(newPosition - position, dirNorm) < 0)
            newPosition = position;

        return newPosition;
    }
}
