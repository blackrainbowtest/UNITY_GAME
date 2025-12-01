using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PathBallPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseScale = 0.15f;
    public float glowStrength = 0.25f;

    private Vector3 _baseScale;
    private SpriteRenderer _sr;
    private Color _baseColor;

    private void Start()
    {
        _baseScale = transform.localScale;
        _sr = GetComponent<SpriteRenderer>();
        _baseColor = _sr.color;
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        transform.localScale = _baseScale * (1f + t * pulseScale);

        float a = _baseColor.a + t * glowStrength;
        _sr.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, a);
    }
}
