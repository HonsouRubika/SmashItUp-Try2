using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    private PlayerController playerControllerScript;

    private ParticleSystem jumpParticle;
    private ParticleSystem runParticle;
    private ParticleSystem ejectionParticle;

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
        runParticle = skin.transform.Find("FX_Run").GetComponent<ParticleSystem>();
        ejectionParticle = skin.transform.Find("FX_Ejection").GetComponent<ParticleSystem>();
    }

    public void JumpFX()
    {
        jumpParticle.Play();
    }

    public void RunFX()
    {
        runParticle.Play();
    }

    public void EjectionFX()
    {
        ejectionParticle.Play();
    }

    public void AttackFXEmpty()
    {
        Debug.Log("empty called");
        StartCoroutine(attackEmpty());
    }

    public void AttackFXTouch()
    {
        Debug.Log("touch called");
        StartCoroutine(attackTouch());
    }

    public void BlockFX()
    {
        Instantiate(blockParticle, playerControllerScript.hammerFXSpawn.position, Quaternion.identity);
    }

    IEnumerator attackEmpty()
    {
        yield return new WaitForSeconds(0f);
        Instantiate(attackEmptyParticle, playerControllerScript.hammerFXSpawnEmpty.position, Quaternion.identity);
    }

    IEnumerator attackTouch()
    {
        yield return new WaitForSeconds(0f);
        Instantiate(attackTouchParticle, playerControllerScript.hammerFXSpawn.position, Quaternion.identity);
    }
}
