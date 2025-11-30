using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    // только 4 направления
    private static readonly Vector2Int[] dirs = {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        // Открытый список — мин-куча
        var open = new PriorityQueue<Node>();
        // Закрытый список
        var all = new Dictionary<Vector2Int, Node>();

        Node startNode = new Node(start, null, 0, Heuristic(start, goal));
        open.Enqueue(startNode);
        all[start] = startNode;

        while (open.Count > 0)
        {
            Node current = open.Dequeue();

            // Путь найден
            if (current.pos == goal)
                return ReconstructPath(current);

            foreach (var d in dirs)
            {
                Vector2Int np = current.pos + d;

                float cost = TileGenerator.GetTileMoveCost(np.x, np.y);
                if (cost <= 0) continue;  // нет тайла или недоступно

                float newG = current.g + cost;

                if (all.TryGetValue(np, out var existing))
                {
                    if (newG < existing.g)
                    {
                        existing.g = newG;
                        existing.f = newG + Heuristic(np, goal);
                        existing.parent = current;
                        open.UpdatePriority(existing);
                    }
                }
                else
                {
                    Node node = new Node(np, current, newG, newG + Heuristic(np, goal));
                    all[np] = node;
                    open.Enqueue(node);
                }
            }
        }

        return null; // путь нет
    }

    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        // Manhattan distance (только 4 направления)
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> ReconstructPath(Node end)
    {
        var result = new List<Vector2Int>();

        Node current = end;
        while (current != null)
        {
            result.Add(current.pos);
            current = current.parent;
        }

        result.Reverse();
        return result;
    }

    private class Node : IHasPriority
    {
        public Vector2Int pos;
        public Node parent;
        public float g; // стоимость пути
        public float f; // g + h

        public Node(Vector2Int pos, Node parent, float g, float f)
        {
            this.pos = pos;
            this.parent = parent;
            this.g = g;
            this.f = f;
        }

        public float Priority => f;
    }
}
