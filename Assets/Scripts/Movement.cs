using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private int speed = 10;
    [SerializeField] private bool usePhysics = true;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int runSpeed = 2; // multiplied from speed
    private bool isGrounded = true;
    private int tempSpeed;

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Controls _controls;
    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        _controls.Disable();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rb = gameObject.GetComponent<Rigidbody>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        tempSpeed = speed;
    }

    private void Update()
    {
        if (usePhysics)
        {
            return;
        }
        
        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            Move(target);

            Run();
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }

        Jump();
    }

    private void FixedUpdate()
    {
        if (!usePhysics)
        {
            return;
        }

        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            MovePhysics(target);

            Run();
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }

        Jump();
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

    private void Move(Vector3 target)
    {
        transform.position = target;
    }

    private void MovePhysics(Vector3 target)
    {
        _rb.MovePosition(target); 
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    public void Run()
    {
        if (_controls.Player.Run.IsPressed())
        {
            speed = tempSpeed * runSpeed;
            CameraShake.Instance.ShakeCam(10f, .1f);
        }
        else
        {
            speed = tempSpeed;
        }

        _animator.speed = speed / 10;
    }

    public void Jump()
    {
        if (_controls.Player.Jump.IsPressed())
        {
            if (isGrounded)
            {
                _rb.AddForce(Vector3.up * jumpForce * 10);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Stair")
        {
            _rb.AddForce(Vector3.up * 7f, ForceMode.VelocityChange);
        }
    }
}
