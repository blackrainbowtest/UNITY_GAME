/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Rendering/UniqueLocationOverlayRenderer.cs*/
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

namespace WorldLogic
{
    /// <summary>
    /// Overlay renderer for unique locations.
    /// Spawns small icons above tiles when they appear on screen.
    /// Removes icons when tiles disappear.
    /// Reacts to discovered/cleared state.
    /// </summary>
    public class UniqueLocationOverlayRenderer : MonoBehaviour
    {
        public static UniqueLocationOverlayRenderer Instance { get; private set; }

        private GameObject iconPrefab;

        // Active icons: tile position → icon GameObject
        private Dictionary<WorldTilePos, GameObject> activeIcons = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Load prefab from Resources
            iconPrefab = Resources.Load<GameObject>("Prefabs/World/UniqueLocationIcon");

            if (iconPrefab == null)
                Debug.LogError("[UniqueLocationOverlay] Icon prefab not found at Prefabs/World/UniqueLocationIcon!");
        }

        // =====================================================================
        //  When a tile appears in world (created by WorldMapController)
        // =====================================================================
        public void OnTileSpawned(WorldTilePos pos, Transform tileTransform)
        {
            var mgr = UniqueLocationManager.Instance;
            if (mgr == null)
                return;

            var loc = mgr.GetAtTile(pos);
            if (loc == null)
                return;

            // Undiscovered locations are not shown yet
            if (!loc.IsDiscovered)
                return;

            SpawnIcon(loc, tileTransform);
        }

        // =====================================================================
        //  When tile disappears (camera moved)
        // =====================================================================
        public void OnTileDespawned(WorldTilePos pos)
        {
            if (activeIcons.TryGetValue(pos, out var obj))
            {
                Destroy(obj);
                activeIcons.Remove(pos);
            }
        }

        // =====================================================================
        //  Spawn icon over tile
        // =====================================================================
        private void SpawnIcon(UniqueLocationInstance loc, Transform tile)
        {
            var pos = loc.Position;

            // Already spawned
            if (activeIcons.ContainsKey(pos))
                return;

            var icon = Instantiate(iconPrefab, tile);
            icon.transform.localPosition = new Vector3(0, 0, -0.1f);

            // Choose sprite based on state
            var sr = icon.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = loc.IsCleared ? loc.Def.iconCleared : loc.Def.icon;
            }

            activeIcons[pos] = icon;
        }

        // =====================================================================
        //  When state changes (e.g., location cleared)
        // =====================================================================
        public void RefreshIcon(UniqueLocationInstance loc)
        {
            if (!activeIcons.TryGetValue(loc.Position, out var icon))
                return;

            var sr = icon.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = loc.IsCleared ? loc.Def.iconCleared : loc.Def.icon;
            }
        }
    }
}
