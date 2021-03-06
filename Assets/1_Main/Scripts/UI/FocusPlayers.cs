using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPlayers : MonoBehaviour
{
    public GameObject[] players;

    [Header("Focus Settings")]
    public float timeShowingPlayers;

    public GameObject cerclePrefab;
    private GameObject[] cercles;

    public Vector2 cercleSize;

    [Header("Hider Settings")]
    public GameObject hideScreen;
    public Color colorHideScreen;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        cercles = new GameObject[players.Length];
        hideScreen.GetComponent<SpriteRenderer>().color = colorHideScreen;

        StartCoroutine(IncrementTimer());
    }

    private void SpawnCercle()
    {
        hideScreen.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            cercles[i] = Instantiate(cerclePrefab, players[i].transform.position, Quaternion.identity);
        }
    }

    private void DestroyCercle()
    {
        hideScreen.SetActive(false);
        Time.timeScale = 1f;

        for (int i = 0; i < cercles.Length; i++)
        {
            Destroy(cercles[i]);
        }
    }

    private IEnumerator IncrementTimer()
    {
        SpawnCercle();
        GameManager.Instance.isShowingPlayers = true;

        yield return new WaitForSecondsRealtime(timeShowingPlayers);

        DestroyCercle();
        GameManager.Instance.isShowingPlayers = false;
    }
}
