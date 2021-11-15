using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerIsClose : MonoBehaviour
{
    public bool playerIsClose = false;

    [Header("Hammer")]
    public LayerMask playerLayer;

    private Collider2D playerCollider = null;

    private BoxCollider2D moleCollider;

    private float moleColliderBoundsX;
    private float moleColliderBoundsY;

    private Vector2 pos;

    private void Start()
    {
        moleCollider = GetComponent<BoxCollider2D>();

        moleColliderBoundsX = moleCollider.bounds.size.x;
        moleColliderBoundsY = moleCollider.bounds.size.y;
        pos = transform.position;
    }

    private void FixedUpdate()
    {
        playerCollider = Physics2D.OverlapBox(pos, new Vector2(moleColliderBoundsX, moleColliderBoundsY), 0f, playerLayer);
    }

    private void Update()
    {
        if (playerCollider != null)
        {
            playerIsClose = true;
        }
        else
        {
            playerIsClose = false;
        }
    }
}
