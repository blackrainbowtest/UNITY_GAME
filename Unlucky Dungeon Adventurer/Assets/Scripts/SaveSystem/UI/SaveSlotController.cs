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

public class SaveSlotController : MonoBehaviour, IPointerClickHandler,
	IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private SaveSlotView view;
	private SaveSlotData slot;

	// The movable content
	[SerializeField] private RectTransform mainContent;

	private Vector2 startDragPos;
	private Vector2 mainContentStartPos;
	private bool isDragging = false;

	private const float SWIPE_THRESHOLD = 80f;

	private bool deleteRevealed = false;
	private bool editRevealed = false;

	public void Init(SaveSlotData slot, SaveSlotView view, RectTransform mainContent)
	{
		this.slot = slot;
		this.view = view;
		this.mainContent = mainContent;

		RefreshView();
	}

	private void RefreshView()
	{
		deleteRevealed = false;
		editRevealed = false;

		view.ShowDeletePanel(false);
		view.ShowEditPanel(false);

		// Reset position
		mainContent.anchoredPosition = Vector2.zero;

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
		if (isDragging)
		{
			// Click is ignored if a swipe was detected
			return;
		}

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
		isDragging = false;
		startDragPos = eventData.position;
		mainContentStartPos = mainContent.anchoredPosition;
	}

	public void OnDrag(PointerEventData eventData)
	{
		float deltaX = eventData.position.x - startDragPos.x;

		if (Mathf.Abs(deltaX) > 20f)
			isDragging = true;

		if (!isDragging)
			return;

		// Move only MainContent
		mainContent.anchoredPosition = mainContentStartPos + new Vector2(deltaX, 0);

		// Reveal panels
		if (deltaX < -SWIPE_THRESHOLD && slot.exists && !slot.isAuto)
		{
			deleteRevealed = true;
			view.ShowDeletePanel(true);
			view.ShowEditPanel(false);
		}
		else if (deltaX > SWIPE_THRESHOLD && slot.exists && !slot.isAuto)
		{
			editRevealed = true;
			view.ShowEditPanel(true);
			view.ShowDeletePanel(false);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		// Always slide back to center
		mainContent.anchoredPosition = Vector2.zero;

		if (deleteRevealed)
		{
			SaveService.RequestDelete(slot.index);
			RefreshView();
		}
		else if (editRevealed)
		{
			Debug.Log("TODO: rename dialog");
		}

		// Reset
		deleteRevealed = false;
		editRevealed = false;

		view.ShowDeletePanel(false);
		view.ShowEditPanel(false);
	}
}
