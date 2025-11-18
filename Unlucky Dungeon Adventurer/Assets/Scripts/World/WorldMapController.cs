using UnityEngine;

public class WorldMapController : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[WorldMap] GameManager отсутствует — загружаю Preloader");
            SceneLoader.LoadScene("Preloader");
            return;
        }

        if (GameData.CurrentPlayer == null)
        {
            Debug.LogWarning("[WorldMap] CurrentPlayer отсутствует — загружаю сохранение");
            if (TempSaveCache.pendingSave != null)
            {
                GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
                TempSaveCache.pendingSave = null;
            }
        }

        TryAutoSaveOnEnter();
    }

    private void TryAutoSaveOnEnter()
    {
        // если игрок загружен корректно
        if (GameData.CurrentPlayer != null)
        {
            Debug.Log("[WorldMap] Создаю автосейв при входе в WorldMap");

            SaveManager.SaveAuto(GameManager.Instance.GetCurrentGameData());
        }
        else
        {
            Debug.LogWarning("[WorldMap] CurrentPlayer == NULL — автосейв НЕ создан");
        }
    }
}
