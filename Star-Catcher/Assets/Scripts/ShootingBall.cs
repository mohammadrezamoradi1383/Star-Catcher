using System;
using UnityEngine;

public class ShootingBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float minForceMultiplier = 3f;
    [SerializeField] private float maxForceMultiplier = 5f;
    [SerializeField] private float maxDragDistance = 2f;

    private Vector3 _initialPosition;
    private Vector2 _dragStartPos;
    private Vector2 _dragEndPos;
    private Camera _camera;
    private bool _isDragging = false;
    private bool _isShooting = false;

    
    void Start()
    {
        _camera = Camera.main;
        rb.isKinematic = true;
        _initialPosition = transform.position;
        SetupLineRendererGradient();
    }
    
    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif

        if (_isDragging && _isShooting==false)
        {
#if UNITY_EDITOR
            Vector2 currentDragPos = _camera.ScreenToWorldPoint(Input.mousePosition);
#else
            Vector2 currentDragPos = Input.touchCount > 0 ? cam.ScreenToWorldPoint(Input.GetTouch(0).position) : dragStartPos;
#endif
            UpdateLinePreview(_dragStartPos, currentDragPos);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            _isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _dragEndPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Shoot();
            _isDragging = false;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = _camera.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _dragStartPos = touchPos;
                _isDragging = true;
            }
            else if (touch.phase == TouchPhase.Ended && _isDragging)
            {
                _dragEndPos = touchPos;
                Shoot();
                _isDragging = false;
            }
        }
    }

    private void Shoot()
    {
        if (_isShooting) return;

        _isShooting = true;

        Vector2 dragVector = _dragStartPos - _dragEndPos;
        float dragDistance = dragVector.magnitude;
        float normalizedPower = Mathf.Clamp01(dragDistance / maxDragDistance);
        float power = Mathf.Lerp(minForceMultiplier, maxForceMultiplier, normalizedPower);

        rb.isKinematic = false;
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.AddForce(dragVector.normalized * power, ForceMode2D.Impulse);
    }

    private void UpdateLinePreview(Vector2 dragStart, Vector2 dragCurrent)
    {
        Vector2 dir = dragStart - dragCurrent;
        dir = Vector2.ClampMagnitude(dir, maxDragDistance);

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position - (Vector3)dir); // جهت درست به سمت شوت
    }

    private void SetupLineRendererGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.5f),    
                new GradientAlphaKey(0f, 1f)     
            }
        );
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(1, 0.06f);  
        widthCurve.AddKey(0, 0.7f);    

        lineRenderer.widthCurve = widthCurve;
        
        lineRenderer.colorGradient = gradient;
    }
    public void ResetBall()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        rb.gravityScale = 0;

        transform.position = _initialPosition;

        _isShooting = false;
        _isDragging = false;

        lineRenderer.enabled = false;
    }
}
