
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

        if (Input.GetKey(KeyCode.W))
        {
            targetInput += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetInput -= Vector3.forward;
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

        if (targetInput == Vector3.forward)
        {
            animator.SetInteger("PlayerState", 1);
        }
        else if (targetInput == -Vector3.forward)
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

        ClampRotations();
    }

    private void ClampRotations()
    {
        Vector3 camDif = Camera.main.transform.position - transform.position;
        camDif = new Vector3(camDif.x, 0, camDif.z);
        camDif.Normalize();
        Vector3 target = GetClampedTarget(camDif);

        transform.forward = Vector3.Lerp(transform.forward, target, Time.deltaTime * 5);

    }
    private Vector3 GetClampedTarget(Vector3 vector)
    {
        Vector3 vectorNormalized = vector.normalized;
        Vector3 absoluteVector = new Vector3(Mathf.Abs(vectorNormalized.x), Mathf.Abs(vectorNormalized.y), Mathf.Abs(vectorNormalized.z));
        if (absoluteVector.x < absoluteVector.z)
        {
            Vector3 ret = new Vector3(0, 0, vectorNormalized.z);
            return ret;
        }
        else
        {
            Vector3 ret = new Vector3(vectorNormalized.x, 0, 0);
            return ret;
        }
    }
}
