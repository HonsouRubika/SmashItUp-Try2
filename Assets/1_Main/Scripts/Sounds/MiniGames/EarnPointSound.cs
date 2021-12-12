using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ce son peut s'appliquer à chaque fois que le joueur gagne des points, quand il a un +1 au dessus de son perso par exemple, quelque soit le mini jeu
public class EarnPointSound : MonoBehaviour
{
    [Space]
    [Header("EarnPoint Sound")]
    public AudioClip earnPoint;
    [Range(0f, 1f)] public float earnPointVolume = 0.5f;

    public void PlayerEarnPoint()
    {
        SoundManager.Instance.PlaySfx(earnPoint, earnPointVolume);
    }
}
