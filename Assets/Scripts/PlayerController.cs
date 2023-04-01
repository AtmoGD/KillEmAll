using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControllSheme
{
    Keyboard,
    Gamepad
}

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float startFreezeTime = 1f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator sniperAnimator;
    [SerializeField] private float shootTimeout = 1.5f;
    [SerializeField] private Transform panTransform;
    [SerializeField] private float panSpeedKeyboard = 1f;
    [SerializeField] private float panSpeedGamepad = 30f;
    [SerializeField] private bool invertPan = false;
    [SerializeField] private Transform tiltTransform;
    [SerializeField] private float tiltSpeedKeyboard = 1f;
    [SerializeField] private float tiltSpeedGamepad = 30f;
    [SerializeField] private bool invertTilt = false;
    private float currentPanSpeed = 0f;
    private float currentTiltSpeed = 0f;

    private float PanSpeed
    {
        get
        {
            if (controllSheme == ControllSheme.Keyboard)
                return panSpeedKeyboard;
            else
                return panSpeedGamepad;
        }
    }
    private float TiltSpeed
    {
        get
        {
            if (controllSheme == ControllSheme.Keyboard)
                return tiltSpeedKeyboard;
            else
                return tiltSpeedGamepad;
        }
    }
    private ControllSheme controllSheme = ControllSheme.Keyboard;

    private float shootTimer = 0f;

    private float timeInGame = 0f;

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (playerInput.currentControlScheme == "Gamepad")
            controllSheme = ControllSheme.Gamepad;
        else
            controllSheme = ControllSheme.Keyboard;

        if ((timeInGame += Time.deltaTime) < startFreezeTime) return;

        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        Rotate();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Rotate()
    {
        float panAngle = (invertPan ? -1f : 1f) * currentPanSpeed * PanSpeed * Time.deltaTime;
        panTransform.Rotate(Vector3.up, panAngle);

        float tiltAngle = (invertTilt ? -1f : 1f) * currentTiltSpeed * TiltSpeed * Time.deltaTime;
        tiltTransform.Rotate(Vector3.right, tiltAngle);
    }

    private void Shoot()
    {
        if (shootTimer > 0f) return;

        shootTimer = shootTimeout;
        sniperAnimator.SetTrigger("Shoot");

        RaycastHit hit;
        if (Physics.Raycast(tiltTransform.position, tiltTransform.forward, out hit))
        {
            if (hit.collider.CompareTag("People"))
            {
                PeopleController people = hit.collider.GetComponent<PeopleController>();
                if (people)
                    people.GetHit();
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        currentPanSpeed = input.x;
        currentTiltSpeed = input.y;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
            Shoot();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                UnlockCursor();
            else
                LockCursor();
        }
    }

}
