using UnityEngine;
using System;

public class PlayerCollisionDetector : MonoBehaviour
{
    public event Action<bool> OnGroundedChanged;
    private bool isGrounded;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.1f;

    private void Update()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (grounded != isGrounded)
        {
            isGrounded = grounded;
            OnGroundedChanged.Invoke(isGrounded);
        }
    }
}
