/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/UI/SaveSlotController.cs                 */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class SaveSlotController : MonoBehaviour, IPointerClickHandler
{
	private SaveSlotView view;
	private SaveSlotData slot;

	public void Init(SaveSlotData slot, SaveSlotView view)
	{
		this.slot = slot;
		this.view = view;

		RefreshView();
	}

	private void RefreshView()
	{
		// Check if file actually exists on disk
		bool fileExists = File.Exists(slot.path);
		
		if (!fileExists)
		{
			view.SetHeader(Path.GetFileNameWithoutExtension(slot.path));
			view.SetEmpty();
			
			// Update slot state
			slot.exists = false;
			
			// Show delete button for filled slots only
			view.ShowDeleteButton(false);
			
			// Disable in Load mode
			bool interactable = SaveLoadState.Mode == SaveLoadMode.Save;
			view.SetInteractable(interactable);
			
			return;
		}

		SaveData data = SaveManager.Load(slot.index);
		if (data == null)
		{
			view.SetEmpty();
			slot.exists = false;
			view.ShowDeleteButton(false);
			view.SetInteractable(false);
			return;
		}

		string header = slot.isAuto
			? LanguageManager.Get("auto_save")
			: Path.GetFileNameWithoutExtension(slot.path);

		view.SetHeader(header);
		view.SetPlayerInfo(data.player.name, data.player.level, data.player.gold);

		string date = File.GetLastWriteTime(slot.path).ToString("dd.MM.yyyy HH:mm");
		string label = SaveLoadState.Mode == SaveLoadMode.Save
			? $"{LanguageManager.Get("button_save")}: {date}"
			: $"{LanguageManager.Get("button_load")}: {date}";

		view.SetSlotInfo(label);
		view.SetFilled();
		slot.exists = true;
		
		// Show delete button only for non-auto saves
		view.ShowDeleteButton(slot.exists && !slot.isAuto);
		
		// Always interactable when filled
		view.SetInteractable(true);
	}

	// ---------------- CLICK ----------------
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!slot.exists && SaveLoadState.Mode == SaveLoadMode.Load)
			return;

		if (SaveLoadState.Mode == SaveLoadMode.Save)
		{
			var data = GameManager.Instance.GetCurrentGameData();
			SaveService.RequestSave(slot.index, data);
			
			// Close save window
			var closeHandler = FindFirstObjectByType<CloseButtonHandler>();
			if (closeHandler != null)
				closeHandler.CloseSaveWindow();
		}
		else
		{
			SaveData loaded = SaveService.RequestLoad(slot.index);
			if (loaded == null) return;

			TempSaveCache.pendingSave = loaded;
			
			// Resume time before loading
			Time.timeScale = 1;
			
			// Start fade transition
			var fade = FindFirstObjectByType<SaveLoadFade>();
			if (fade != null)
			{
				fade.StartCoroutine(fade.FadeOutAndLoad(loaded.meta.sceneName));
			}
			else
			{
				// Fallback: direct load
				SceneLoader.LoadScene(loaded.meta.sceneName);
			}
		}
	}

	// Called by delete button
	public void OnDeleteClicked()
	{
		if (!slot.exists || slot.isAuto)
			return;

		SaveService.RequestDelete(slot.index);
		RefreshView();
	}
}
