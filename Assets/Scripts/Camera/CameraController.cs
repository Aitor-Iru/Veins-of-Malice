using UnityEngine;

/// <summary>
/// Controlador de cámara 2.5D para el prototipo Greybox.
/// Sigue al jugador con suavizado en X e Y, bloqueando el eje Z.
/// Funciona sin Cinemachine (fallback manual) para el prototipo.
/// Si tienes Cinemachine instalado, usa directamente una CinemachineVirtualCamera.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Dead Zone")]
    [SerializeField] private float deadZoneX = 0.5f;
    [SerializeField] private float deadZoneY = 0.3f;

    [Header("Bounds (optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private float currentShakeTimer = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // Handle Shake
        if (currentShakeTimer > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeIntensity;
            currentShakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        Vector3 desiredPosition = target.position + offset + shakeOffset;

        // Apply dead zone — only move camera if player exits dead zone
        float deltaX = desiredPosition.x - transform.position.x;
        float deltaY = desiredPosition.y - transform.position.y;

        Vector3 targetPos = transform.position;

        if (Mathf.Abs(deltaX) > deadZoneX)
            targetPos.x = desiredPosition.x;

        if (Mathf.Abs(deltaY) > deadZoneY)
            targetPos.y = desiredPosition.y;

        // Keep Z fixed for 2.5D
        targetPos.z = offset.z;

        // Smooth follow
        Vector3 smoothed = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Apply bounds clamping
        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minBounds.x, maxBounds.x);
            smoothed.y = Mathf.Clamp(smoothed.y, minBounds.y, maxBounds.y);
        }

        transform.position = smoothed;
    }

    /// <summary>
    /// Activa una sacudida de cámara.
    /// </summary>
    public void Shake()
    {
        currentShakeTimer = shakeDuration;
    }

    /// <summary>
    /// Asigna el target desde código (útil si el jugador se instancia en runtime).
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, 0f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
