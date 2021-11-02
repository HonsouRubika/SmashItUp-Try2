using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private DetructCrate_Rules detructCrateRulesScript;

    [Header("Hammer")]
    public LayerMask hammerLayer;

    [Header("Score")]
    public int pointsToAdd = 1;

    private Collider2D hammerCollider = null;
    private BoxCollider2D crateCollider;

    private float crateColliderBoundsX;
    private float crateColliderBoundsY;

    private Vector2 pos;

    private void Start()
    {
        detructCrateRulesScript = GetComponentInParent<DetructCrate_Rules>();

        crateCollider = GetComponent<BoxCollider2D>();

        crateColliderBoundsX = crateCollider.bounds.size.x;
        crateColliderBoundsY = crateCollider.bounds.size.y;
        pos = transform.position;
    }

    private void FixedUpdate()
    {
        hammerCollider = Physics2D.OverlapBox(pos, new Vector2(crateColliderBoundsX, crateColliderBoundsY), 0f, hammerLayer);
    }

    private void Update()
    {
        if (hammerCollider != null)
        {
            switch (hammerCollider.GetComponentInParent<PlayerController>().playerID)
            {
                case 0:
                    detructCrateRulesScript.scorePlayer0 += pointsToAdd;
                    break;
                case 1:
                    detructCrateRulesScript.scorePlayer1 += pointsToAdd;
                    break;
                case 2:
                    detructCrateRulesScript.scorePlayer2 += pointsToAdd;
                    break;
                case 3:
                    detructCrateRulesScript.scorePlayer3 += pointsToAdd;
                    break;
            }

            Destroy(gameObject);
        }
    }

    /* Renderer the overlappBox 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(pos, new Vector2(crateColliderBoundsX, crateColliderBoundsY));
    }
    */
}
