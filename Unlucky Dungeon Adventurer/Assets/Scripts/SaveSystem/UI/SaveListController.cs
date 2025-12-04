/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/UI/SaveListController.cs                 */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:42:10 by UDA                                      */
/*   Updated: 2025/12/03 10:42:10 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

/**
 * Asks the service for the number of slots
 * Creates UI elements
 * Inserts data into the slots
 * Displays the "Saving" / "Loading" header
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveListController : MonoBehaviour
{
	[Header("UI")]
	public Transform slotContainer;     // ScrollView/Content
	public GameObject slotPrefab;       // Prefab with SaveSlotView + SaveSlotController
	public TextMeshProUGUI titleText;

	private void Awake()
	{
		RefreshTitle();
		GenerateSlots();
	}

	private void RefreshTitle()
	{
		if (SaveLoadState.Mode == SaveLoadMode.Save)
			titleText.text = LanguageManager.Get("title_save");
		else
			titleText.text = LanguageManager.Get("title_load");
	}

	private void GenerateSlots()
	{
		// Clear old
		foreach (Transform child in slotContainer)
			Destroy(child.gameObject);

		// get data from service
		bool includeAuto = SaveLoadState.Mode == SaveLoadMode.Load;
		List<SaveSlotData> slots = SaveService.GetAllSlots(includeAuto);

		// Create slots from data
		foreach (var slotData in slots)
			CreateSlot(slotData);

		// Add empty slots in SAVE mode
		if (SaveLoadState.Mode == SaveLoadMode.Save)
			GenerateEmptySlots(slots);
	}

	private void CreateSlot(SaveSlotData data)
	{
		var obj = Instantiate(slotPrefab, slotContainer);

		var view = obj.GetComponent<SaveSlotView>();
		var controller = obj.GetComponent<SaveSlotController>();

		if (view == null)
		{
			Debug.LogError("[SaveListController] Slot prefab is missing SaveSlotView!");
			return;
		}

		// Add controller if missing
		if (controller == null)
		{
			controller = obj.AddComponent<SaveSlotController>();
		}

		// Add LayoutElement if missing
		var layoutElement = obj.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = obj.AddComponent<LayoutElement>();
		}
		layoutElement.preferredHeight = 180;
		layoutElement.minHeight = 180;

		// Create delete button if missing
		if (view.deleteButton == null)
		{
			CreateDeleteButton(obj.transform, view, controller);
		}

		controller.Init(data, view);
	}

	private void CreateDeleteButton(Transform parent, SaveSlotView view, SaveSlotController controller)
	{
		// Create button GameObject
		GameObject btnObj = new GameObject("DeleteButton");
		btnObj.transform.SetParent(parent, false);
		btnObj.layer = parent.gameObject.layer;

		// Add RectTransform
		RectTransform rt = btnObj.AddComponent<RectTransform>();
		rt.anchorMin = new Vector2(1, 0.5f);
		rt.anchorMax = new Vector2(1, 0.5f);
		rt.anchoredPosition = new Vector2(-50, 0);
		rt.sizeDelta = new Vector2(80, 80);

		// Add Image
		Image img = btnObj.AddComponent<Image>();
		img.color = new Color(0.8f, 0.2f, 0.2f, 1f);

		// Add Button
		Button btn = btnObj.AddComponent<Button>();
		btn.onClick.AddListener(() => controller.OnDeleteClicked());

		// Set reference
		view.deleteButton = btnObj;
	}

	private void GenerateEmptySlots(List<SaveSlotData> existingSlots)
	{
		const int targetSlots = 10;

		// Count non-auto slots
		int regularCount = 0;
		foreach (var s in existingSlots)
			if (!s.isAuto)
				regularCount++;

		// Add empty ones
		for (int i = regularCount; i < targetSlots; i++)
		{
			string path = SaveRepository.GetSlotPath(i);
			CreateSlot(new SaveSlotData(path, i, false, false));
		}
	}
}
