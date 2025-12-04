/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/UI/SaveSlotView.cs                       */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:53:06 by UDA                                      */
/*   Updated: 2025/12/03 10:53:06 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotView : MonoBehaviour
{
	[Header("UI Elements")]
	public TextMeshProUGUI slotTitle;
	public TextMeshProUGUI slotInfo;
	public TextMeshProUGUI playerNameText;
	public TextMeshProUGUI levelText;
	public Image background;

	[Header("Buttons")]
	public GameObject deleteButton;

	public void SetEmpty()
	{
		if (slotInfo != null)
			slotInfo.text = LanguageManager.Get("empty_slot");

		if (playerNameText != null)
			playerNameText.text = "---";

		if (levelText != null)
			levelText.text = "";

		if (background)
			background.color = new Color(1, 1, 1, 0.25f);
	}

	public void SetHeader(string title)
	{
		if (slotTitle != null)
			slotTitle.text = title;
	}

	public void SetPlayerInfo(string name, int level, int gold)
	{
		if (playerNameText != null)
			playerNameText.text = name;

		if (levelText != null)
			levelText.text = $"Lvl {level} | Gold: {gold}";
	}

	public void SetSlotInfo(string text)
	{
		if (slotInfo != null)
			slotInfo.text = text;
	}

	public void SetFilled()
	{
		if (background)
			background.color = Color.white;
	}

	public void ShowDeleteButton(bool state)
	{
		if (deleteButton != null)
			deleteButton.SetActive(state);
	}

	public void SetInteractable(bool interactable)
	{
		if (background)
			background.color = interactable ? new Color(1, 1, 1, 0.25f) : new Color(0.5f, 0.5f, 0.5f, 0.15f);
	}
}
