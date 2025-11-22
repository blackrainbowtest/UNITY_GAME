using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float dragSpeed = 0.005f;
    public float inertiaDamp = 0.9f;
    public float minInertia = 0.001f;

    private Camera cam;
    private Vector3 lastScreenPos;
    private Vector3 inertia;

	private bool autoCenter = false;
	private Vector3 autoTarget;
	public float autoSpeed = 5f;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        bool dragging = false;
        Vector3 deltaPixels = Vector3.zero;

		// === Автоцентрирование камеры ===
		if (autoCenter)
		{
			// Плавное движение к игроку
			transform.position = Vector3.Lerp(transform.position, autoTarget, Time.deltaTime * autoSpeed);

			// Если камера почти дошла — выключаем режим автоприцепа
			if (Vector3.Distance(transform.position, autoTarget) < 0.01f)
			{
				transform.position = autoTarget;
				autoCenter = false;
			}

			return; // Блокируем ручное перемещение, пока камера "летит"
		}
		
        // === PC ===
        if (Input.GetMouseButtonDown(0))
        {
            lastScreenPos = Input.mousePosition;
            inertia = Vector3.zero;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 current = Input.mousePosition;
            deltaPixels = current - lastScreenPos;
            lastScreenPos = current;
            dragging = true;

            cam.transform.position -= cam.ScreenToWorldPoint(deltaPixels) - cam.ScreenToWorldPoint(Vector3.zero);
        }

        // === Mobile ===
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                lastScreenPos = t.position;
                inertia = Vector3.zero;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector3 current = t.position;
                deltaPixels = current - lastScreenPos;
                lastScreenPos = current;
                dragging = true;

                cam.transform.position -= cam.ScreenToWorldPoint(deltaPixels) - cam.ScreenToWorldPoint(Vector3.zero);
            }
        }

        // === Инерция ===
        if (dragging)
        {
            inertia = deltaPixels;
        }
        else
        {
            if (inertia.magnitude > minInertia)
            {
                Vector3 deltaWorld = cam.ScreenToWorldPoint(inertia) - cam.ScreenToWorldPoint(Vector3.zero);
                cam.transform.position -= deltaWorld;
                inertia *= inertiaDamp;
            }
        }
    }

	public void CenterToPlayer(Vector3 playerPos)
	{
		autoTarget = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		autoCenter = true;
	}
}
