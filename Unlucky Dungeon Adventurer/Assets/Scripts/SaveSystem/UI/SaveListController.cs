/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/UI/SaveListController.cs                 */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using System.Collections.Generic;

public class SaveListController : MonoBehaviour
{
	[Header("UI")]
	public RectTransform content;
	public GameObject slotPrefab;

	private const float SLOT_HEIGHT = 180f;
	private const float SLOT_SPACING = 20f;

	void Start()
	{
		RefreshList();
	}

	public void RefreshList()
	{
		// Удаляем старые слоты
		foreach (Transform child in content)
			Destroy(child.gameObject);

		// Получаем список слотов
		List<SaveSlotData> slots = SaveService.GetAllSlots(includeAutoSave: true);

		// Добавляем пустые слоты до 10
		int regularCount = 0;
		foreach (var s in slots)
			if (!s.isAuto)
				regularCount++;

		for (int i = regularCount; i < 10; i++)
		{
			string path = SaveRepository.GetSlotPath(i);
			slots.Add(new SaveSlotData(path, i, false, false));
		}

		// Изменяем размер контента
		float totalHeight = slots.Count * (SLOT_HEIGHT + SLOT_SPACING);
		content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

		// Создаём слоты
		for (int i = 0; i < slots.Count; i++)
		{
			var obj = Instantiate(slotPrefab, content);
			var rt = obj.GetComponent<RectTransform>();

			// Позиционирование
			rt.anchorMin = new Vector2(0, 1);
			rt.anchorMax = new Vector2(1, 1);
			rt.pivot = new Vector2(0.5f, 1);
			rt.sizeDelta = new Vector2(0, SLOT_HEIGHT);
			rt.anchoredPosition = new Vector2(0, -i * (SLOT_HEIGHT + SLOT_SPACING));

			// Инициализация
			var view = obj.GetComponent<SaveSlotView>();
			var controller = obj.GetComponent<SaveSlotController>();
			
			if (controller == null)
				controller = obj.AddComponent<SaveSlotController>();
			
			controller.Init(slots[i], view);
		}
	}
}
