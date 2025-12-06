using System.Collections.Generic;
using UnityEngine;

namespace WorldLogic
{
    public class UniqueLocationOverlayRenderer : MonoBehaviour
    {
        public static UniqueLocationOverlayRenderer Instance { get; private set; }

        [Header("Icon prefab")]
        public GameObject iconPrefab;

        private Dictionary<WorldTilePos, GameObject> activeIcons = new();
        private UniqueLocationManager manager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            manager = FindFirstObjectByType<UniqueLocationManager>();

            if (manager == null)
            {
                Debug.LogError("[UniqueLocationOverlay] Manager not found");
                return;
            }

            if (iconPrefab == null)
                iconPrefab = Resources.Load<GameObject>("Prefabs/World/UniqueLocationIcon");

            if (iconPrefab == null)
                Debug.LogError("[UniqueLocationOverlay] iconPrefab missing!");
            else
                Debug.Log("[UniqueLocationOverlay] Renderer initialized!");
        }

        public void OnTileSpawned(WorldTilePos pos, GameObject tileObj)
        {
            if (manager == null || manager.Locations == null)
                return;

            var loc = manager.GetAtTile(pos);
            if (loc == null)
                return;

            if (activeIcons.ContainsKey(pos))
                return;

            SpawnIcon(loc, pos, tileObj);
        }

        public void OnTileDespawned(WorldTilePos pos)
        {
            if (activeIcons.TryGetValue(pos, out var obj))
            {
                Destroy(obj);
                activeIcons.Remove(pos);
            }
        }

        private void SpawnIcon(UniqueLocationInstance loc, WorldTilePos pos, GameObject tileObj)
        {
            if (iconPrefab == null)
            {
                Debug.LogError("[UniqueLocationOverlay] iconPrefab is null!");
                return;
            }

            var icon = Instantiate(iconPrefab, tileObj.transform);
            icon.transform.localPosition = new Vector3(0, 0, -0.1f);

            var sr = icon.GetComponent<SpriteRenderer>();
            if (sr != null && loc.Def != null && loc.Def.icon != null)
            {
                sr.sprite = loc.Def.icon;
            }

            activeIcons[pos] = icon;
        }
    }
}
