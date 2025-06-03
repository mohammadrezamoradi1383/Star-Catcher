using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    private Vector2 startTouchPos;
    private Vector2 currentTouchPos;
    private bool isDragging = false;

    public float forceMultiplier = 10f;

    // رنگ سفید با گرادیانت از پررنگ به کم‌رنگ
    public Color lineStartColor = new Color(1f, 1f, 1f, 0.8f);  // سفید با الفا 0.8
    public Color lineEndColor = new Color(1f, 1f, 1f, 0.1f);    // سفید با الفا 0.1

    public float lineWidth = 0.05f;
    public float lineWidth2 = 0.05f;

    public LineRenderer lineRenderer;
    // محدودیت طول کشیدن (مثلا حداکثر 3 واحد در دنیای بازی)
    public float maxDragDistance = 3f;
    

    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.colorGradient = CreateColorGradient();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 rawDragVector = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - startTouchPos;

            // محدود کردن طول کشیدن به maxDragDistance
            Vector2 limitedDragVector = Vector2.ClampMagnitude(rawDragVector, maxDragDistance);

            currentTouchPos = startTouchPos + limitedDragVector;

            DrawLine();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            lineRenderer.enabled = false;
            Shoot();
        }
        #endif

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                startTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
                lineRenderer.enabled = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 rawDragVector = (Vector2)Camera.main.ScreenToWorldPoint(touch.position) - startTouchPos;
                Vector2 limitedDragVector = Vector2.ClampMagnitude(rawDragVector, maxDragDistance);
                currentTouchPos = startTouchPos + limitedDragVector;

                DrawLine();
            }
            else if (touch.phase == TouchPhase.Ended && isDragging)
            {
                isDragging = false;
                lineRenderer.enabled = false;
                Shoot();
            }
        }
    }

    void DrawLine()
    {
        Vector3 playerPos = transform.position;
        Vector3 dragVector = currentTouchPos - startTouchPos;

        lineRenderer.SetPosition(0, playerPos);
        lineRenderer.SetPosition(1, playerPos - dragVector);
    }

    void Shoot()
    {
        Vector2 force = startTouchPos - currentTouchPos;
        rb.AddForce(force * forceMultiplier, ForceMode2D.Impulse);
    }

    Gradient CreateColorGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.white, 0f);
        colorKeys[1] = new GradientColorKey(Color.white, 1f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(lineStartColor.a, 0f);
        alphaKeys[1] = new GradientAlphaKey(lineEndColor.a, 1f);

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}
