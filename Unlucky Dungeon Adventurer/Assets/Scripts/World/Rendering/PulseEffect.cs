using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float speed = 3f;       // скорость пульсации
    public float scaleAmount = 0.15f; // величина увеличения

    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    private void Update()
    {
        float s = Mathf.Sin(Time.time * speed) * scaleAmount + 1f;
        transform.localScale = _baseScale * s;
    }
}
