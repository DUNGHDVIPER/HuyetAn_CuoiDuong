using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Base")]
    public Vector3 offset = new Vector3(0f, 1.0f, -10f);
    public float smoothTime = 0.15f;

    [Header("Dead Zone (focus vùng to hơn)")]
    public bool useDeadZone = true;
    public Vector2 deadZoneSize = new Vector2(6f, 3f); // vùng cho phép player di chuyển mà camera không đổi
    public float deadZoneFollowSpeed = 6f;             // tốc độ kéo khi ra khỏi zone

    [Header("Look Ahead (nhìn về hướng chạy)")]
    public bool useLookAhead = true;
    public float lookAheadDistance = 2.0f;
    public float lookAheadSmooth = 6f;

    [Header("Clamp (optional)")]
    public bool useClamp = false;
    public Vector2 minXY = new Vector2(-50, -10);
    public Vector2 maxXY = new Vector2(50, 10);

    private Vector3 _vel;
    private Vector3 _lookAhead;
    private Vector3 _lookAheadVel;
    private Vector3 _lastTargetPos;

    void Start()
    {
        if (target != null) _lastTargetPos = target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1) look-ahead theo hướng di chuyển
        if (useLookAhead)
        {
            float dx = target.position.x - _lastTargetPos.x;
            float desiredAheadX = Mathf.Sign(dx) * lookAheadDistance;
            // nếu đứng yên thì giảm lookahead về 0
            if (Mathf.Abs(dx) < 0.001f) desiredAheadX = 0f;

            Vector3 desiredAhead = new Vector3(desiredAheadX, 0f, 0f);
            _lookAhead = Vector3.SmoothDamp(_lookAhead, desiredAhead, ref _lookAheadVel, 1f / Mathf.Max(1f, lookAheadSmooth));
            _lastTargetPos = target.position;
        }
        else _lookAhead = Vector3.zero;

        // 2) desired camera pos
        Vector3 desired = target.position + offset + _lookAhead;

        // 3) Dead zone: camera chỉ di chuyển khi desired vượt khỏi vùng
        Vector3 camPos = transform.position;
        if (useDeadZone)
        {
            Vector2 half = deadZoneSize * 0.5f;

            float dx = desired.x - camPos.x;
            float dy = desired.y - camPos.y;

            if (Mathf.Abs(dx) > half.x) camPos.x = Mathf.Lerp(camPos.x, desired.x, Time.deltaTime * deadZoneFollowSpeed);
            if (Mathf.Abs(dy) > half.y) camPos.y = Mathf.Lerp(camPos.y, desired.y, Time.deltaTime * deadZoneFollowSpeed);

            // z luôn theo offset
            camPos.z = desired.z;
        }
        else
        {
            camPos = Vector3.SmoothDamp(transform.position, desired, ref _vel, smoothTime);
        }

        // 4) Clamp map nếu có
        if (useClamp)
        {
            camPos.x = Mathf.Clamp(camPos.x, minXY.x, maxXY.x);
            camPos.y = Mathf.Clamp(camPos.y, minXY.y, maxXY.y);
        }

        transform.position = camPos;
    }

#if UNITY_EDITOR
    // Vẽ dead-zone trong Scene view (để chỉnh dễ)
    void OnDrawGizmosSelected()
    {
        if (!useDeadZone) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Vector3 c = transform.position;
        Gizmos.DrawWireCube(new Vector3(c.x, c.y, 0f), new Vector3(deadZoneSize.x, deadZoneSize.y, 0f));
    }
#endif
}