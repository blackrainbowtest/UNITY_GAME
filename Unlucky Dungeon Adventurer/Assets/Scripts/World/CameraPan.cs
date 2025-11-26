using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float dragSpeed = 0.005f;
    public float inertiaDamp = 0.9f;
    public float minInertia = 0.001f;
    // Дискретные уровни масштаба (size): 10, 12, 14, 16, 18, 20
    public int[] zoomSteps = new int[] { 10, 12, 14, 16, 18, 20 };
    public int zoomIndex = 0; // текущий индекс в zoomSteps
    public float pinchThreshold = 10f; // чувствительность для смены шага на мобильном

    private Camera cam;
    private Vector3 lastScreenPos;
    private Vector3 inertia;
    private float lastPinchDistance;

	private bool autoCenter = false;
	private Vector3 autoTarget;
	public float autoSpeed = 100f; // Speed in units per second
	public float autoStopDistance = 0.05f; // Stop threshold

    void Awake()
    {
        cam = Camera.main;
        // Инициализируем zoomIndex по текущему size
        if (cam != null)
        {
            zoomIndex = ClosestZoomIndex(cam.orthographicSize);
            cam.orthographicSize = zoomSteps[zoomIndex];
        }
    }

    void Update()
    {
        // Блокируем перемещение камеры, если захвачен ввод миникартой
        if (MinimapController.InputCaptured)
        {
            inertia = Vector3.zero;
            return; // игнорируем остальной ввод до отпускания
        }
        bool dragging = false;
        Vector3 deltaPixels = Vector3.zero;

		// === Автоцентрирование камеры ===
		if (autoCenter)
		{
			// Проверяем, если пользователь начал взаимодействие - отменяем автоцентрирование
			if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
			{
				autoCenter = false;
				return;
			}

			// MoveTowards принимает maxDistanceDelta как расстояние ЗА КАДР
			// autoSpeed уже в единицах/секунду, поэтому умножаем на Time.deltaTime
			float step = autoSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, autoTarget, step);

			// Если камера почти дошла — выключаем режим автоприцепа
			if (Vector3.Distance(transform.position, autoTarget) < autoStopDistance)
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

        // === Mouse Wheel Zoom (дискретные шаги) ===
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f)
        {
            if (scroll > 0f)
                zoomIndex = Mathf.Max(0, zoomIndex - 1); // колесо вверх — приближение (меньше size)
            else
                zoomIndex = Mathf.Min(zoomSteps.Length - 1, zoomIndex + 1); // колесо вниз — отдаление (больше size)

            cam.orthographicSize = zoomSteps[zoomIndex];
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

        // === Mobile Pinch Zoom (двумя пальцами, дискретные шаги) ===
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                lastPinchDistance = Vector2.Distance(t0.position, t1.position);
            }
            else
            {
                float currentDistance = Vector2.Distance(t0.position, t1.position);
                float delta = currentDistance - lastPinchDistance;
                lastPinchDistance = currentDistance;

                if (Mathf.Abs(delta) > pinchThreshold)
                {
                    if (delta > 0f)
                        zoomIndex = Mathf.Max(0, zoomIndex - 1); // разводим пальцы — приближаем
                    else
                        zoomIndex = Mathf.Min(zoomSteps.Length - 1, zoomIndex + 1); // сводим — отдаляем

                    cam.orthographicSize = zoomSteps[zoomIndex];
                }
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

    private int ClosestZoomIndex(float size)
    {
        int idx = 0;
        float best = float.MaxValue;
        for (int i = 0; i < zoomSteps.Length; i++)
        {
            float d = Mathf.Abs(zoomSteps[i] - size);
            if (d < best)
            {
                best = d;
                idx = i;
            }
        }
        return idx;
    }

	public void CenterToPlayer(Vector3 playerPos)
	{
		autoTarget = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		autoCenter = true;
	}
}
