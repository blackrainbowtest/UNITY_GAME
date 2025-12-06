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

        if (RestController.Instance == null)
        {
            var go = new GameObject("RestController");
            go.AddComponent<RestController>();
        }
    }

    public void OpenRestMenu(RestEnvironment env)
    {
        if (_restUI == null) return;

        _restUI.Open(env);
    }
}
