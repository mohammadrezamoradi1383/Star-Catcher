using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallPhysicsController : MonoBehaviour
{
    private Vector2 velocity;
    private Vector2 startPoint;
    private bool isDragging = false;
    private bool isThrown = false;
    private float throwStartTime;
    private float dragDuration;

    public float forceMultiplier = 5f;         // قدرت ضربه کلی
    public float maxPower = 3f;                // بیشترین قدرت ممکن برای کشیدن
    public float minDragTime = 0.2f;           // حداقل زمان توقف
    public float maxDragTime = 2f;             // حداکثر زمان توقف

    [SerializeField] LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        // فقط وقتی هنوز پرتاب نشده دنبال کلیک باش
        if (!isThrown && Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            // اگه روی توپ کلیک شده بود
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                startPoint = worldPoint;
                isDragging = true;
            }
        }

        // در حین کشیدن، خط نشون بده
        if (isDragging)
        {
            Vector2 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentPoint);
        }

        // وقتی کاربر ول می‌کنه
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (startPoint - endPoint).normalized;
            float power = Mathf.Clamp(Vector2.Distance(startPoint, endPoint), 0f, maxPower);

            velocity = dir * power * forceMultiplier;

            // زمان توقف بر اساس قدرت کشیدن
            dragDuration = Mathf.Lerp(minDragTime, maxDragTime, power / maxPower);
            throwStartTime = Time.time;

            isThrown = true;
            isDragging = false;
            lineRenderer.enabled = false;
        }

        // حرکت توپ بعد از پرتاب
        if (isThrown)
        {
            float t = (Time.time - throwStartTime) / dragDuration;
            velocity = Vector2.Lerp(velocity, Vector2.zero, t);  // کاهش سرعت
            transform.position += (Vector3)(velocity * Time.deltaTime);

            // وقتی کاملاً وایساد، وضعیت رو ریست کن
            if (velocity.magnitude < 0.01f)
            {
                velocity = Vector2.zero;
                isThrown = false;
            }
        }
    }
}
