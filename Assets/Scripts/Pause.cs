using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject MenuUI;
    private Controls _controls;
    private bool isPaused = false;

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    void Update()
    {
        if (_controls.Player.Pause.IsPressed())
        {
            if (isPaused)
            {
                UnpauseAction();
                isPaused = false;
            } 
            else
            {
                PauseAction();
                isPaused = true;
            }
        }
    }

    public void PauseAction()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        MenuUI.SetActive(true);
    }

    public void UnpauseAction()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        MenuUI.SetActive(false);
    }
}
