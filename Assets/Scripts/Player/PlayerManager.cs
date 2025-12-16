using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform graphicsChild;

    [Header("Modules")]
    public PlayerMovement movement;
    public PlayerJump jump;
    public PlayerCollision collision;
    public PlayerGravity gravity;
    public PlayerHealth health;
    public PlayerInput input;
    public PlayerAnimation animationController;

    private void Awake()
    {
        // Obtener componentes principales
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        // Obtener todos los módulos
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        collision = GetComponent<PlayerCollision>();
        gravity = GetComponent<PlayerGravity>();
        health = GetComponent<PlayerHealth>();
        input = GetComponent<PlayerInput>();
        animationController = GetComponent<PlayerAnimation>();

        // Inicializar módulos con referencias necesarias
        InitializeModules();
    }

    private void InitializeModules()
    {
        // Cada módulo recibe la referencia al manager
        if (movement != null) movement.Initialize(this);
        if (jump != null) jump.Initialize(this);
        if (collision != null) collision.Initialize(this);
        if (gravity != null) gravity.Initialize(this);
        if (health != null) health.Initialize(this);
        if (input != null) input.Initialize(this);
        if (animationController != null) animationController.Initialize(this);
    }
}