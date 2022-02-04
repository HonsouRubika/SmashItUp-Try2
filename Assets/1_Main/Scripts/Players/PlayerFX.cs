using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    private ParticleSystem jumpParticle;
    private ParticleSystem runParticle;
    private ParticleSystem attackParticle;

    public void FindParticlesSystems(GameObject skin, GameObject hammer)
    {
        jumpParticle = skin.transform.Find("JumpFX").GetComponent<ParticleSystem>();
        runParticle = skin.transform.Find("RunFX").GetComponent<ParticleSystem>();
        attackParticle = hammer.transform.Find("AttackFX").GetComponent<ParticleSystem>();
    }

    public void JumpFX()
    {
        jumpParticle.Play();
    }

    public void RunFX(bool enable)
    {
        switch (enable)
        {
            case true:
                runParticle.Play();
                break;
            case false:
                runParticle.Stop();
                break;
        }
    }

    public void AttackFX()
    {
        attackParticle.Play();
    }
}
