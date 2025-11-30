using UnityEngine;

public class PathBallPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;      // скорость пульсации
    public float pulseScale = 0.15f;   // насколько увеличивается
    public float glowStrength = 0.25f; // насколько увеличивается яркость

    private Vector3 baseScale;
    private SpriteRenderer sr;
    private Color baseColor;

    void Start()
    {
        baseScale = transform.localScale;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            baseColor = sr.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        // масштаб
        transform.localScale = baseScale * (1f + t * pulseScale);

        // свечение через яркость цвета
        if (sr != null)
        {
            float a = baseColor.a + t * glowStrength;
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
        }
    }
}
