using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFilling : MonoBehaviour
{
    private Sprite platformSprite;
    public Sprite platformEmptySprite;

    private BoxCollider2D platformCollider;

    [Header("Settings")]
    public float timePlatformEmpty;
    public float timePlatformFilled;
    private float timer = 0;

    public bool startEmpty = false;

    private void Start()
    {
        platformSprite = GetComponent<SpriteRenderer>().sprite;
        platformCollider = GetComponent<BoxCollider2D>();

        if (startEmpty)
        {
            timer = timePlatformFilled;
        }
        else
        {
            timer = 0;
        }
    }

    private void Update()
    {
        if (timer <= timePlatformFilled)
        {
            timer += Time.deltaTime;
            GetComponent<SpriteRenderer>().sprite = platformSprite;
            platformCollider.enabled = true;
        }
        else if (timer <= timePlatformFilled + timePlatformEmpty)
        {
            timer += Time.deltaTime;
            GetComponent<SpriteRenderer>().sprite = platformEmptySprite;
            platformCollider.enabled = false;
        }
        else
        {
            timer = 0;
        }
    }
}
