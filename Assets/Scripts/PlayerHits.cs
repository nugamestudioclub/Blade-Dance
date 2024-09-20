using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHits : MonoBehaviour
{
    private CircleCollider2D playerCollider;

    private int bulletMask;
    private ContactFilter2D bulletFilter;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<CircleCollider2D>();

        bulletMask = LayerMask.GetMask("Bullet");

        bulletFilter = new ContactFilter2D().NoFilter();
        bulletFilter.SetLayerMask(bulletMask);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCollider.IsTouchingLayers(bulletMask))
        {
            Collider2D[] overlaps = new Collider2D[8];
            int overlapNumber = playerCollider.OverlapCollider(bulletFilter, overlaps);

            for (int i = 0; i < overlapNumber; i++)
            {
                overlaps[i].GetComponent<BulletMover>().HitPlayer();
            }
        }
    }
}
