using UnityEngine;

public class WorldMapController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[WorldMap] WorldMapController запущен");

        // если пришли после создания персонажа — создаём автосейв
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
