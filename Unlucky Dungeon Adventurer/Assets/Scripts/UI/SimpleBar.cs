using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SimpleBar : MonoBehaviour
{
    [SerializeField] private Color fillColor = new Color32(0xFF, 0x44, 0x44, 0xFF);
    [SerializeField] private float initialFill = 1f;

    private Image _image;

    void Reset()
    {
        // Этот метод вызывается когда добавляешь компонент – подставим что-то по умолчанию
        _image = GetComponent<Image>();
        _image.type = Image.Type.Filled;
        _image.fillMethod = Image.FillMethod.Horizontal;
        _image.fillOrigin = (int)Image.OriginHorizontal.Left;
        _image.fillAmount = 1f;
        _image.color = fillColor;
    }

    void Awake()
    {
        _image = GetComponent<Image>();
        // защищаем от случайного оставления другого типа
        _image.type = Image.Type.Filled;
        _image.fillMethod = Image.FillMethod.Horizontal;
        _image.fillOrigin = (int)Image.OriginHorizontal.Left;
        _image.fillAmount = initialFill;
        _image.color = fillColor;
    }

    /// <summary>
    /// Установить текущее значение бара (от 0 до 1)
    /// </summary>
    public void SetFraction(float frac)
    {
        _image.fillAmount = Mathf.Clamp01(frac);
    }
}
