using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed;

    private Animator animator;

    private Vector3 origin;
    private Vector3 lastFrameInput;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        lastFrameInput = Vector3.zero;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetInput = Vector3.zero;

        if (KeybindManager.HoldingUp())
        {
            targetInput += Vector3.up;
        }
        if (KeybindManager.HoldingDown())
        {
            targetInput += Vector3.down;
        }
        if (KeybindManager.HoldingLeft())
        {
            targetInput += Vector3.left;
        }
        if (KeybindManager.HoldingRight())
        {
            targetInput += Vector3.right;
        }

        // if two adjacent buttons are currently the only ones held down
        if (targetInput.x != 0 && targetInput.y != 0)
        {
            // if both buttons in question just entered their pressed state on this frame at the same time
            if (targetInput.x != lastFrameInput.x && targetInput.y != lastFrameInput.y)
            {
                targetInput = Vector3.zero;
            }
            else
            {
                if (KeybindManager.PressedLeft() || KeybindManager.PressedRight())
                {
                    targetInput.y = 0f;
                }
                else if (KeybindManager.PressedUp() || KeybindManager.PressedDown())
                {
                    targetInput.x = 0f;
                }
                else
                {
                    if (lastFrameInput.x != 0f)
                    {
                        targetInput.y = 0f;
                    }
                    else if (lastFrameInput.y != 0f)
                    {
                        targetInput.x = 0f;
                    }
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, origin + targetInput, moveSpeed * Time.deltaTime);

        lastFrameInput = targetInput;

        if (targetInput == Vector3.up)
        {
            animator.SetInteger("PlayerState", 1);
        }
        else if (targetInput == Vector3.down)
        {
            animator.SetInteger("PlayerState", 2);
        }
        else if (targetInput == Vector3.left)
        {
            animator.SetInteger("PlayerState", 3);
        }
        else if (targetInput == Vector3.right)
        {
            animator.SetInteger("PlayerState", 4);
        }
        else
        {
            animator.SetInteger("PlayerState", 0);
        }
    }
}
