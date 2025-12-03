using System;

/// <summary>
/// Root save file container.
/// This object is serialized by SaveManager and stored on disk.
/// All modules below are stored in separate files for clarity.
/// </summary>
[Serializable]
public class SaveData
{
    public PlayerSaveData player = new PlayerSaveData();
    public WorldSaveData world = new WorldSaveData();
    public InventorySaveData inventory = new InventorySaveData();
    public QuestSaveData quests = new QuestSaveData();
    public MetaSaveData meta = new MetaSaveData();
}
