using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float powerOfJump = 7f;
    [SerializeField] private float runBoost = 2f;
    [SerializeField] private Menu menu;

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Controls _controls;
    private Animator _animator;
    private bool isGrounded = true;
    private float temporarySpeed;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        _controls.Enable();
    }

    private void OnDisable()
    {
        //Cursor.lockState = CursorLockMode.None;
        _controls.Disable();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rb = gameObject.GetComponent<Rigidbody>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        temporarySpeed = speed;
    }

    private void Update()
    {   
        if (_controls.Player.Move.IsPressed())
        {
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            RotateCharacter(target);
        }
    }

    private void FixedUpdate()
    {
        if (_controls.Player.Jump.IsPressed() && isGrounded)
        {
            Jump();
        } 

        if (_controls.Player.Pause.IsPressed())
        {
            menu.Pause();
        }

        if (_controls.Player.Run.IsPressed())
        {
            Run();
        }

        if (_controls.Player.Dance.IsPressed())
        {
            Dance();
        }

        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            MovePhysics(target);
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }

        _animator.speed = speed / 10;
        speed = temporarySpeed;
    }

    private Vector3 HandleInput(Vector2 input)
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        
        return transform.position + direction * speed * Time.deltaTime;
    }

    private void RotateCharacter(Vector3 target)
    {
        transform.rotation = Quaternion.LookRotation(target - transform.position);
    }

    private void MovePhysics(Vector3 target)
    {
        _rb.MovePosition(target); 
    }

    public void Jump()
    {
        _rb.AddForce(Vector3.up * 10 * powerOfJump);
        _animator.SetTrigger("Jump");
    }

    public void Dance()
    {
        _animator.SetTrigger("Dance");
    }

    public void Run()
    {
        speed = temporarySpeed * runBoost;
        CameraShake.Instance.ShakeCam(2f, .1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
