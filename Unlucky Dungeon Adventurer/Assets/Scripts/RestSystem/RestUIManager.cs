using UnityEngine;
using UnityEngine.UI;

public class RestUIManager : MonoBehaviour
{
    public static RestUIManager Instance;

    private RestUIController _restUI;

    private void Awake()
    {
        Instance = this;
        _restUI = FindFirstObjectByType<RestUIController>();
        
        if (_restUI == null)
        {
            Debug.LogError("[RestUIManager] RestUIController not found in scene!");
        }

        // Ensure RestController exists
        if (RestController.Instance == null)
        {
            var go = new GameObject("RestController");
            go.AddComponent<RestController>();
            Debug.Log("[RestUIManager] Created RestController automatically.");
        }
    }

    public void OpenRestMenu(RestEnvironment env)
    {
        if (_restUI == null)
        {
            Debug.LogError("[RestUIManager] RestUIController not initialized!");
            return;
        }

        _restUI.Open(env);
    }
}
