using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Player player;
    public float sensitivity = 100f;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
        // make sure this audiolistener is the only one
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>() as AudioListener[];
        int noActiveListeners = 0;
        foreach (AudioListener listener in audioListeners)
        {
            if (listener.enabled) {
                noActiveListeners++;
            }
            listener.GetComponent<AudioListener>().enabled = false;
        }
        this.GetComponent<AudioListener>().enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorMode();
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Look();
        }
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
