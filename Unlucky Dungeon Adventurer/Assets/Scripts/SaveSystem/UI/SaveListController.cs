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
using TMPro;

using System.Collections.Generic;
using UnityEngine;
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

		if (view == null || controller == null)
		{
			Debug.LogError("[SaveListController] Slot prefab is missing SaveSlotView / SaveSlotController!");
			return;
		}

		controller.Init(data, view);
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
