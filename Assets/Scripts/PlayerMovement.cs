using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Animator animator;
    [HideInInspector]
    public Vector2 moveDir;
    private Vector2 lastMoveDir = new Vector2(0f, -1f);

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError(
                "PlayerMovement: No Animator found on " + gameObject.name,
                this
            );
        }
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        }

        moveDir = input.normalized;
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);

        bool isWalking = input != Vector2.zero;

        if (animator == null)
            return;

        animator.SetBool("isWalking", isWalking);

        if (isWalking)
            lastMoveDir = moveDir;

        animator.SetFloat("InputX", moveDir.x);
        animator.SetFloat("InputY", moveDir.y);
        animator.SetFloat("LastInputX", lastMoveDir.x);
        animator.SetFloat("LastInputY", lastMoveDir.y);
    }
}