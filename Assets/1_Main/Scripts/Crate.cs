using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private DetructCrate_Rules detructCrateScript;
    private Score scoreScript;
    private DestructSound destructSoundScript;

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
        detructCrateScript = GetComponentInParent<DetructCrate_Rules>();
        detructCrateScript.cratesNumber++;

        scoreScript = detructCrateScript.scoreScript;

        crateCollider = GetComponent<BoxCollider2D>();

        crateColliderBoundsX = crateCollider.bounds.size.x;
        crateColliderBoundsY = crateCollider.bounds.size.y;
        pos = transform.position;

        destructSoundScript = GetComponentInChildren<DestructSound>();
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
                    scoreScript.AddScore(1, 0, 0, 0);
                    break;
                case 1:
                    scoreScript.AddScore(0, 1, 0, 0);
                    break;
                case 2:
                    scoreScript.AddScore(0, 0, 1, 0);
                    break;
                case 3:
                    scoreScript.AddScore(0, 0, 0, 1);
                    break;
            }

            detructCrateScript.cratesNumber--;
            Destroy(gameObject);
            destructSoundScript.PlayerDestroy();
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
