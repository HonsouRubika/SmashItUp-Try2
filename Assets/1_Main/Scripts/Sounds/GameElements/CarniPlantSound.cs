using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarniPlantSound : MonoBehaviour
{
    [Space]
    [Header("CarniPlant Sound")]
    public AudioClip carniPlant;
    [Range(0f, 1f)] public float carniPlantVolume = 0.5f;

    public void CarniPlantCross()
    {
        SoundManager.Instance.PlaySfx(carniPlant, carniPlantVolume);
    }
}

