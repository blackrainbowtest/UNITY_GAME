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

public class SaveListController : MonoBehaviour
{
	[Header("UI")]
	public Transform slotContainer;     // Content from ScrollView
	public GameObject slotPrefab;       // Prefab SaveSlotUI
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
		// Cleaning old slots
		foreach (Transform child in slotContainer)
			Destroy(child.gameObject);

		// We get a list of slots from the service
		List<SaveSlotData> slots = SaveService.GetAllSlots(
			includeAutoSave: SaveLoadState.Mode == SaveLoadMode.Load
		);

		// Generating UI elements
		foreach (var slot in slots)
		{
			var obj = Instantiate(slotPrefab, slotContainer);
			var ui = obj.GetComponent<SaveSlotUI>();
			ui.Init(slot.path, slot.isAuto);
		}

		// In SAVE mode, always show empty slots first.
		if (SaveLoadState.Mode == SaveLoadMode.Save)
		{
			const int targetSlotCount = 10;
			int currentCount = 0;

			foreach (var s in slots)
				if (!s.isAuto) currentCount++;

			// Adding empty slots
			for (int i = currentCount; i < targetSlotCount; i++)
			{
				string path = SaveRepository.GetSlotPath(i);
				var obj = Instantiate(slotPrefab, slotContainer);
				var ui = obj.GetComponent<SaveSlotUI>();
				ui.Init(path, false);
			}
		}
	}
}
