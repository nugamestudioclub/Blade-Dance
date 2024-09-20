using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed;

    private Vector3 origin;
    private Vector3 lastFrameInput;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        lastFrameInput = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            targetInput += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetInput += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            targetInput += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
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
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    targetInput.y = 0f;
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
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
    }
}
