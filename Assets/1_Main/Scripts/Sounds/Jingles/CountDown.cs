using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    [Space]
    [Header("CountDown Sound")]
    public AudioClip countDown;
    [Range(0f, 1f)] public float countDownVolume = 0.5f;

    public Timer timerScript;
    private bool playOneTime = false;

    private void Update()
    {
        if (timerScript.miniGameTimer <= 5 && !playOneTime)
        {
            PlayerInLastSeconds();
            playOneTime = true;
        }
    }

    public void PlayerInLastSeconds()
    {
        SoundManager.Instance.PlaySfx(countDown, countDownVolume);
    }
}