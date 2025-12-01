using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    public static PathRenderer Instance;

    public GameObject ballPrefab;
    public Transform ballParent;
    public float tileSize = 10f;

    private readonly List<GameObject> _balls = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public static void Show(List<Vector2Int> path)
    {
        if (Instance == null) return;
        Instance.Render(path);
    }

    public static void ClearAll()
    {
        if (Instance == null) return;
        Instance.ClearBalls();
    }

    public static void ConsumeFirst()
    {
        if (Instance == null) return;
        Instance.Consume();
    }

    private void Render(List<Vector2Int> path)
    {
        ClearBalls();

        // не ставим шарик на стартовом тайле (i = 1)
        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int p = path[i];
            Vector3 pos = new Vector3(p.x * tileSize, p.y * tileSize, 0f);
            GameObject ball = Instantiate(ballPrefab, pos, Quaternion.identity, ballParent);
            _balls.Add(ball);
        }
    }

    private void ClearBalls()
    {
        foreach (var b in _balls)
            if (b != null) Destroy(b);
        _balls.Clear();
    }

    private void Consume()
    {
        if (_balls.Count == 0) return;

        GameObject first = _balls[0];
        _balls.RemoveAt(0);
        if (first != null) Destroy(first);
    }
}
