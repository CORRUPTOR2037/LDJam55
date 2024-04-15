using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f; // Speed of camera movement
    [SerializeField] private float zoomSpeed = 10f; // Speed of camera zoom
    [SerializeField] private Vector2 zoomLimits = new Vector2(5f, 50f); // Limits for camera zoom
    [SerializeField] private Vector2 moveLimitsX, moveLimitsZ;    // Limits for camera movement
    [SerializeField] private BoxCollider mouseBlocker;

    private Camera mainCamera;

    private void Awake()
    {
        GameInstancesHolder.Register(this);
    }

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        SetBlockerActive(false);
    }

    void Update()
    {
        ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));
    }

    public void MoveCamera(float x, float y)
    {
        // Add delta movement to the camera position
        Vector3 newPosition = transform.localPosition;
        newPosition.x = Mathf.Clamp(newPosition.x - x * moveSpeed, moveLimitsX.x + mainCamera.orthographicSize, moveLimitsX.y - mainCamera.orthographicSize);
        newPosition.z = Mathf.Clamp(newPosition.z - y * moveSpeed, moveLimitsZ.x + mainCamera.orthographicSize, moveLimitsZ.y - mainCamera.orthographicSize);
        transform.localPosition = newPosition;
    }

    public void ZoomCamera(float deltaZoom)
    {
        // Add delta zoom to the camera's orthographic size
        float newZoom = Mathf.Clamp(mainCamera.orthographicSize - deltaZoom * zoomSpeed, zoomLimits.x, zoomLimits.y);
        mainCamera.orthographicSize = newZoom;
    }

    public void SetBlockerActive(bool value) => mouseBlocker.enabled = value;
}
