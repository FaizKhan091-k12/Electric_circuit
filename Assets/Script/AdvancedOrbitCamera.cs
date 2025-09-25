using UnityEngine;

public class AdvancedOrbitCamera : MonoBehaviour
{
    public static AdvancedOrbitCamera instance;

    [Header("Initial View")]
    [SerializeField] private float defaultZoom = 10f;
    [SerializeField] private float defaultHorizontalRotation = 0f;
    [SerializeField] private float defaultVerticalRotation = 20f;

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.15f;

    public bool canOrbit = true;

    private float targetX, targetY, currentX, currentY;
    private float rotationVelocityX, rotationVelocityY;
    private float targetDistance, currentDistance, zoomVelocity;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!target)
        {
           // Debug.LogWarning("OrbitCamera: No target assigned. Using world origin.");
            target = new GameObject("CameraTarget").transform;
        }

        ApplyInitialView();
    }

    private void ApplyInitialView()
    {
        defaultZoom = Mathf.Clamp(defaultZoom, minZoom, maxZoom);
        defaultVerticalRotation = Mathf.Clamp(defaultVerticalRotation, minVerticalAngle, maxVerticalAngle);

        targetDistance = currentDistance = defaultZoom;
        targetX = currentX = defaultHorizontalRotation;
        targetY = currentY = defaultVerticalRotation;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        ApplyInitialView();
    }

    public void OrbitControlsWorkingState(bool CanOrbit)
    {
        this.canOrbit = CanOrbit;
    }

    private void LateUpdate()
    {
        if (!canOrbit) return;

        HandleInput();
        ApplySmoothCamera();
    }

    private void HandleInput()
    {
        bool isPointerOverUI = false;
        bool clickedOnCollider = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        isPointerOverUI = UnityEngine.EventSystems.EventSystem.current != null &&
                          UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            clickedOnCollider = Physics.Raycast(ray, out _);
        }

        if (!isPointerOverUI && !clickedOnCollider && Input.GetMouseButton(0))
        {
            targetX += Input.GetAxis("Mouse X") * rotationSpeed;
            targetY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS || UNITY_SIMULATOR
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 delta = Input.GetTouch(0).deltaPosition;
            targetX += delta.x * rotationSpeed * 0.02f;
            targetY -= delta.y * rotationSpeed * 0.02f;
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevPos0 = t0.position - t0.deltaPosition;
            Vector2 prevPos1 = t1.position - t1.deltaPosition;
            float prevMag = Vector2.Distance(prevPos0, prevPos1);
            float currMag = Vector2.Distance(t0.position, t1.position);
            float deltaMag = prevMag - currMag;

            targetDistance += deltaMag * zoomSpeed * 0.005f;
        }
#endif

        targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);
        targetY = Mathf.Clamp(targetY, minVerticalAngle, maxVerticalAngle);
    }

    private void ApplySmoothCamera()
    {
        currentX = Mathf.SmoothDamp(currentX, targetX, ref rotationVelocityX, smoothTime);
        currentY = Mathf.SmoothDamp(currentY, targetY, ref rotationVelocityY, smoothTime);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref zoomVelocity, smoothTime);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.forward;

        transform.position = target.position - direction * currentDistance;
        transform.LookAt(target.position);
    }
}
