using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Renders a visual path on the map using ball prefabs.
/// Each path node is represented by a PathBall with pulse animation.
/// </summary>
public class PathRenderer : MonoBehaviour
{
    [Header("Path Visualization")]
    public GameObject ballPrefab; // Drag PathBall prefab here

    [Header("Settings")]
    public Transform pathContainer; // Optional: parent for organizing path balls

    private List<GameObject> activeBalls = new List<GameObject>();

    /// <summary>
    /// Displays the path by instantiating ball prefabs at each node position.
    /// </summary>
    /// <param name="path">List of tile positions representing the path</param>
    public void ShowPath(List<Vector2Int> path)
    {
        ClearPath();

        if (path == null || path.Count == 0 || ballPrefab == null)
            return;

        foreach (var tilePos in path)
        {
            Vector3 worldPos = new Vector3(tilePos.x, tilePos.y, 0f);
            GameObject ball = Instantiate(ballPrefab, worldPos, Quaternion.identity);
            
            if (pathContainer != null)
                ball.transform.SetParent(pathContainer);

            activeBalls.Add(ball);
        }
    }

    /// <summary>
    /// Removes all active path balls from the scene.
    /// </summary>
    public void ClearPath()
    {
        foreach (var ball in activeBalls)
        {
            if (ball != null)
                Destroy(ball);
        }

        activeBalls.Clear();
    }

    private void OnDestroy()
    {
        ClearPath();
    }
}
