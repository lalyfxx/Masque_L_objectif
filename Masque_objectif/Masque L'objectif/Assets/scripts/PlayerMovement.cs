using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    private Rigidbody rb;
    private Camera playerCam;
    private float rotX = 0f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        playerCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -maxLookAngle, maxLookAngle);
        playerCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate() {
        rb.AddForce(transform.forward * moveSpeed, ForceMode.Force);
    }
}