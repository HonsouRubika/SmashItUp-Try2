using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    private PlayerController playerControllerScript;

    private ParticleSystem jumpParticle;
    private ParticleSystem runParticle;

    public GameObject attackEmptyParticle;
    public GameObject attackTouchParticle;
    public GameObject blockParticle;

    private void Start()
    {
        playerControllerScript = GetComponent<PlayerController>();
    }

    public void FindParticlesSystems(GameObject skin, GameObject hammer)
    {
        jumpParticle = skin.transform.Find("FX_Jump").GetComponent<ParticleSystem>();
        //runParticle = skin.transform.Find("RunFX").GetComponent<ParticleSystem>();
    }

    public void JumpFX()
    {
        jumpParticle.Play();
    }

    public void RunFX()
    {
        //runParticle.Play();
    }

    public void AttackFXEmpty()
    {
        StartCoroutine(attackEmpty());
    }

    public void AttackFXTouch()
    {
        StartCoroutine(attackTouch());
    }

    public void BlockFX()
    {
        Instantiate(blockParticle, playerControllerScript.hammerFXSpawn.position, Quaternion.identity);
    }

    IEnumerator attackEmpty()
    {
        yield return new WaitForSeconds(0.05f);
        Instantiate(attackEmptyParticle, playerControllerScript.hammerFXSpawn.position, Quaternion.identity);
    }

    IEnumerator attackTouch()
    {
        yield return new WaitForSeconds(0.05f);
        Instantiate(attackTouchParticle, playerControllerScript.hammerFXSpawn.position, Quaternion.identity);
    }
}
