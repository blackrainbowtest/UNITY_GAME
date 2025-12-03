/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/UI/SaveSlotController.cs                 */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:53:39 by UDA                                      */
/*   Updated: 2025/12/03 10:53:39 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

/**
 * slot brain
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class SaveSlotController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private SaveSlotView view;
	private SaveSlotData slot;

	private Vector2 startPos;
	private RectTransform rect;

	private const float dragThreshold = 80f; // 1/3 widths conditionally

	private bool deleteRevealed = false;
	private bool editRevealed = false;

	public void Init(SaveSlotData slot, SaveSlotView view)
	{
		this.slot = slot;
		this.view = view;
		rect = GetComponent<RectTransform>();

		RefreshView();
	}

	private void RefreshView()
	{
		if (!slot.exists)
		{
			view.SetHeader(Path.GetFileNameWithoutExtension(slot.path));
			view.SetEmpty();
			return;
		}

		SaveData data = SaveManager.Load(slot.index);
		if (data == null)
		{
			view.SetEmpty();
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
			RefreshView();
		}
		else
		{
			SaveData loaded = SaveService.RequestLoad(slot.index);
			if (loaded == null) return;

			TempSaveCache.pendingSave = loaded;
			SceneLoader.LoadScene(loaded.meta.sceneName);
		}
	}

	// ---------------- DRAG ----------------
	public void OnBeginDrag(PointerEventData eventData)
	{
		startPos = rect.anchoredPosition;
	}

	public void OnDrag(PointerEventData eventData)
	{
		float deltaX = eventData.position.x - eventData.pressPosition.x;

		rect.anchoredPosition = startPos + new Vector2(deltaX, 0);

		if (deltaX > dragThreshold && !slot.isAuto && slot.exists)
		{
			view.ShowEditPanel(true);
			editRevealed = true;
		}
		else if (deltaX < -dragThreshold && !slot.isAuto && slot.exists)
		{
			view.ShowDeletePanel(true);
			deleteRevealed = true;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		rect.anchoredPosition = startPos;

		if (deleteRevealed)
		{
			SaveService.RequestDelete(slot.index);
			deleteRevealed = false;
			RefreshView();
		}

		if (editRevealed)
		{
			Debug.Log("TODO: open rename dialog");
			editRevealed = false;
			view.ShowEditPanel(false);
		}

		view.ShowDeletePanel(false);
		view.ShowEditPanel(false);
	}
}
