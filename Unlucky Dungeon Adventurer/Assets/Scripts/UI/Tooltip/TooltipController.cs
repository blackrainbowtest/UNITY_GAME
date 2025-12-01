/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   TooltipController.cs                                 /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 14:26:45 by UDA                                      */
/*   Updated: 2025/12/01 14:26:45 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    private RectTransform _rect;

    private void Awake()
    {
        Instance = this;
        _rect = panel.GetComponent<RectTransform>();
        Hide();
    }

    public void Show(string title, string desc, Vector3 worldPosition)
    {
        titleText.text = title;
        descText.text = desc;

        panel.SetActive(true);

        // фиксированное смещение на экране
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPosition);
        _rect.position = screenPos + new Vector2(80, -80);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
