using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(LineRenderer))]
public class BallPhysicsController : MonoBehaviour
{
    private Vector2 velocity;
    private Vector2 startPoint;
    private bool isDragging = false;
    private bool isThrown = false;

    [Header("Physics Settings")]
    public float forceMultiplier = 5f;
    public float powerBoost = 1.2f;
    public float maxPower = 3f;

    [Header("Friction Settings")]
    [Range(0.9f, 0.999f)]
    public float frictionFactor = 0.98f;

    [Header("Drag Limits")]
    public float maxDragDistance = 3f;

    [Header("Line Renderer Settings")]
    public float lineStartWidth = 0.1f;
    public float lineMiddleWidth = 0.15f;
    public float lineEndWidth = 0.1f;
    public Color lineColor = Color.white;
    [Range(0f, 1f)] public float lineAlphaStart = 0.1f;
    [Range(0f, 1f)] public float lineAlphaEnd = 0.4f;

    [SerializeField] LineRenderer lineRenderer;
    
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        UpdateLineRendererStyle();
    }

    void Update()
    {
        // شروع درگ (فقط روی توپ)
        if (!isThrown && Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                startPoint = worldPoint;
                isDragging = true;
            }
        }

        // کشیدن و رسم خط
        if (isDragging)
        {
            Vector2 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = currentPoint - startPoint;

            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
                currentPoint = startPoint + dragVector;
            }

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentPoint);
        }

        // ول کردن
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = endPoint - startPoint;

            if (dragVector.magnitude > maxDragDistance)
                dragVector = dragVector.normalized * maxDragDistance;

            Vector2 dir = (-dragVector).normalized;
            float power = Mathf.Clamp(dragVector.magnitude, 0f, maxPower);
            float boostedPower = Mathf.Pow(power, powerBoost);
            velocity = dir * boostedPower * forceMultiplier;

            isThrown = true;
            isDragging = false;
            lineRenderer.enabled = false;
        }

        // حرکت توپ بعد از پرتاب با اصطکاک
        if (isThrown)
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            velocity *= frictionFactor;

            if (velocity.magnitude < 0.01f)
            {
                velocity = Vector2.zero;
                isThrown = false;
            }
        }
    }

    void UpdateLineRendererStyle()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(lineColor, 0f),
                new GradientColorKey(lineColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(lineAlphaStart, 0f),
                new GradientAlphaKey(lineAlphaEnd, 1f)
            }
        );
        lineRenderer.colorGradient = gradient;

        lineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, lineStartWidth),
            new Keyframe(0.5f, lineMiddleWidth),
            new Keyframe(1, lineEndWidth)
        );

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.numCapVertices = 4;
    }
}
