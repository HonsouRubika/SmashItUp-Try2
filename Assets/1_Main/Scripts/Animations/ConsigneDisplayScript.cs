using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsigneDisplayScript : MonoBehaviour
{
    [HideInInspector] public Animator consigneAnimator;

    public GameObject Team_1v3;
    public GameObject Team_2v2;
    public GameObject Team_FFA;

    public GameObject Rules_CatchTheCrown;
    public GameObject Rules_Coins;
    public GameObject Rules_Contamination;
    public GameObject Rules_DontTouchTheWolf;
    public GameObject Rules_KeepTheCrown;
    public GameObject Rules_MineTheCristal;
    public GameObject Rules_StayInTheLight;
    public GameObject Rules_WhackAMole;

    public GameObject Illustration_CatchTheCrown;
    public GameObject Illustration_Coins;
    public GameObject Illustration_Contamination;
    public GameObject Illustration_DontTouchTheWolf;
    public GameObject Illustration_KeepTheCrown;
    public GameObject Illustration_MineTheCristal;
    public GameObject Illustration_StayInTheLight;
    public GameObject Illustration_WhackAMole;


    private void Awake()
    {
        consigneAnimator = GetComponent<Animator>();
    }

    public void SetActiveGoodConsigne()
    {
        if (GameManager.Instance.teamCompoToTest.Length > 0)
        {
            //Debug.Log("affichage team compo debug");
            Debug.Log(GameManager.Instance._nbMancheActu);
            switch (GameManager.Instance.teamCompoToTest[GameManager.Instance._nbMancheActu - 1])
            {
                case GameManager.TeamCompo.OneVSThree:
                    //Debug.Log("Team compo : 1v3");
                    Team_1v3.SetActive(true);
                    break;
                case GameManager.TeamCompo.TwoVSTwo:
                    //Debug.Log("Team compo : 2v2");
                    Team_2v2.SetActive(true);
                    break;
                case GameManager.TeamCompo.FFA:
                    //Debug.Log("Team compo : ffa");
                    Team_FFA.SetActive(true);
                    break;
            }
        }
        else
        {
            switch (GameManager.Instance.getTeamCompo())
            {
                case (int)GameManager.TeamCompo.OneVSThree:
                    Team_1v3.SetActive(true);
                    break;
                case (int)GameManager.TeamCompo.TwoVSTwo:
                    Team_2v2.SetActive(true);
                    break;
                case (int)GameManager.TeamCompo.FFA:
                    Team_FFA.SetActive(true);
                    break;
            }
        }

        //Debug.Log(GameManager.Instance._nbMancheActu - 1);
        //Debug.Log(GameManager.Instance.GetGameModeActu());

        if (GameManager.Instance.gameModeToTest.Length > 0)
        {
            //Debug.Log("affichage game mode debug");
            switch (GameManager.Instance.gameModeToTest[GameManager.Instance._nbMancheActu - 1])
            {
                case GameManager.GameMode.CaptureTheFlag:
                    //Debug.Log("CaptureTheFlag");
                    Rules_CatchTheCrown.SetActive(true);
                    Illustration_CatchTheCrown.SetActive(true);
                    break;
                case GameManager.GameMode.CaptureDeZone:
                    //Debug.Log("CaptureDeZone");
                    Rules_StayInTheLight.SetActive(true);
                    Illustration_StayInTheLight.SetActive(true);
                    break;
                case GameManager.GameMode.CaptureDeZoneMouvante:
                    //Debug.Log("CaptureDeZoneMouvante");
                    Rules_StayInTheLight.SetActive(true);
                    Illustration_StayInTheLight.SetActive(true);
                    break;
                case GameManager.GameMode.Contamination:
                    //Debug.Log("Contamination");
                    Rules_Contamination.SetActive(true);
                    Illustration_Contamination.SetActive(true);
                    break;
                case GameManager.GameMode.KeepTheFlag:
                    //Debug.Log("KeepTheFlag");
                    Rules_KeepTheCrown.SetActive(true);
                    Illustration_KeepTheCrown.SetActive(true);
                    break;
                case GameManager.GameMode.Loup:
                    //Debug.Log("Loup");
                    Rules_DontTouchTheWolf.SetActive(true);
                    Illustration_DontTouchTheWolf.SetActive(true);
                    break;
                case GameManager.GameMode.CollectTheCoin:
                    //Debug.Log("CollectTheCoin");
                    Rules_Coins.SetActive(true);
                    Illustration_Coins.SetActive(true);
                    break;
                case GameManager.GameMode.WhackAMole:
                    //Debug.Log("WhackAMole");
                    Rules_WhackAMole.SetActive(true);
                    Illustration_WhackAMole.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("GameMode inconnu : Ajouter un case; GameMode : " + GameManager.Instance.GetGameModeActu());
                    break;
            }
        }
        else
        {
            switch (GameManager.Instance.GetGameModeActu())
            {
                case (int)GameManager.GameMode.CaptureTheFlag:
                    Rules_CatchTheCrown.SetActive(true);
                    Illustration_CatchTheCrown.SetActive(true);
                    break;
                case (int)GameManager.GameMode.CaptureDeZone:
                    Rules_StayInTheLight.SetActive(true);
                    Illustration_StayInTheLight.SetActive(true);
                    break;
                case (int)GameManager.GameMode.CaptureDeZoneMouvante:
                    Rules_StayInTheLight.SetActive(true);
                    Illustration_StayInTheLight.SetActive(true);
                    break;
                case (int)GameManager.GameMode.Contamination:
                    Rules_Contamination.SetActive(true);
                    Illustration_Contamination.SetActive(true);
                    break;
                case (int)GameManager.GameMode.KeepTheFlag:
                    Rules_KeepTheCrown.SetActive(true);
                    Illustration_KeepTheCrown.SetActive(true);
                    break;
                case (int)GameManager.GameMode.Loup:
                    Rules_DontTouchTheWolf.SetActive(true);
                    Illustration_DontTouchTheWolf.SetActive(true);
                    break;
                case (int)GameManager.GameMode.CollectTheCoin:
                    Rules_Coins.SetActive(true);
                    Illustration_Coins.SetActive(true);
                    break;
                case (int)GameManager.GameMode.WhackAMole:
                    Rules_WhackAMole.SetActive(true);
                    Illustration_WhackAMole.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("GameMode inconnu : Ajouter un case; GameMode : " + GameManager.Instance.GetGameModeActu());
                    break;

            }
        }
    }


}
