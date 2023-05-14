using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float rotX;
    public float sens;
    public Transform player;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sens;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sens;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        transform.localEulerAngles = new Vector3(rotX, 0f, 0f);

        player.Rotate(Vector3.up * mouseX);
    }
}
