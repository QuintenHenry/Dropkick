using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndScreen : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    private Text m_WonTeamText = null;
    [SerializeField] private List<GameObject> m_TeamInfoHolders = new List<GameObject>();
    private int m_TeamThatLost = 0;

    [Header("Managers")]
    [SerializeField]
    private TeamManager m_TeamManager = null;

    [Header("Sprites")]
    [SerializeField]
    private List<Sprite> m_SkinSprites = new List<Sprite>();

    [Header("Navigation")]
    [SerializeField]
    EventSystem m_EventSystem; 

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInfo()
    {
        #region OldCode
        ////Check ammount of players
        //int ammountOfPlayers = m_TeamManager.GetAmmountOfPlayers();

        ////If ammountOfPlayers == 2 --> Disble second info of each infoholder
        //if (ammountOfPlayers == 2)
        //{
        //    foreach (GameObject teamInfo in m_TeamInfoHolders)
        //    {
        //        teamInfo.transform.GetChild(1).gameObject.SetActive(false);
        //    }
        //}

        ////For each team
        //for (int teamId = 0; teamId < m_TeamManager.GetAmmountOfTeams(); teamId++)
        //{
        //    Debug.Log("Looping team: " + teamId);
        //    //For each player of that team
        //    for (int i = 0; i < m_TeamManager.GetPlayersOfTeam(teamId).Count; i++)
        //    {
        //        Debug.Log("Looping player: " + i);
        //        //For each player get its info == a child of the teamInfoHolder
        //        for (int holderID = 0; holderID < m_TeamInfoHolders[teamId].transform.childCount; holderID++)
        //        {
        //            GameObject infoHolder = m_TeamInfoHolders[teamId].transform.GetChild(holderID).gameObject;
        //            PlayerManager playerManager = m_TeamManager.GetPlayersOfTeam(teamId)[i].GetComponentInChildren<PlayerManager>();

        //            //Set each info of the infoHolder
        //            infoHolder.transform.GetChild(0).GetComponent<Image>().sprite = m_SkinSprites[playerManager.GetSkinId()];
        //            infoHolder.transform.GetChild(1).GetComponent<Text>().text = "Goals: " + playerManager.GetTimesGoalScored().ToString();
        //            //infoHolder.transform.GetChild(1).GetComponent<Text>().text = "Scored: " + playerManager.GetTimesScored().ToString();
        //            infoHolder.transform.GetChild(2).GetComponent<Text>().text = "Knocked Down: " + playerManager.GetTimesKnockedDown().ToString();
        //            //infoHolder.transform.GetChild(3).GetComponent<Text>().text = "Revived: " + playerManager.GetTimesRevived().ToString();
        //            //infoHolder.transform.GetChild(4).GetComponent<Text>().text = "Power Ups: " + playerManager.GetTimesPickedPowerUp().ToString();
        //        }
        //    }
        //}
        #endregion 

        //Get the palyers of the team that won
        List<GameObject> winTeamPlayers = m_TeamManager.GetPlayersOfTeam(1 - m_TeamThatLost);

        //If the team contains lees than 2 players disble the other playerinfo
        if (winTeamPlayers.Count < 2)
        {
            m_TeamInfoHolders[0].transform.GetChild(1).gameObject.SetActive(false);
        }

        //For each player in the team set the infoholder information
        for (int i = 0; i < winTeamPlayers.Count; i++)
        {
            PlayerManager playerManager = winTeamPlayers[i].GetComponentInChildren<PlayerManager>();
            //Icon
            m_TeamInfoHolders[0].transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = m_SkinSprites[playerManager.GetSkinId()];
            //Goals
            m_TeamInfoHolders[0].transform.GetChild(i).transform.GetChild(1).GetComponent<Text>().text = "Goals: " + playerManager.GetTimesGoalScored().ToString();
            //Time KnockedDown
            m_TeamInfoHolders[0].transform.GetChild(i).transform.GetChild(2).GetComponent<Text>().text = "Knocked Down: " + playerManager.GetTimesKnockedDown().ToString();
        }

        //Get the players of the team that lost
        List<GameObject> lossTeamPlayers = m_TeamManager.GetPlayersOfTeam(m_TeamThatLost);

        //If the team contains lees than 2 players disble the other playerinfo
        if (lossTeamPlayers.Count < 2)
        {
            m_TeamInfoHolders[1].transform.GetChild(1).gameObject.SetActive(false);
        }

        //For each player in the team set the infoholder information
        for (int i = 0; i < lossTeamPlayers.Count; i++)
        {
            PlayerManager playerManager = lossTeamPlayers[i].GetComponentInChildren<PlayerManager>();
            //Icon
            m_TeamInfoHolders[1].transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = m_SkinSprites[playerManager.GetSkinId()];
            //Goals
            m_TeamInfoHolders[1].transform.GetChild(i).transform.GetChild(1).GetComponent<Text>().text = "Goals: " + playerManager.GetTimesGoalScored().ToString();
            //Time KnockedDown
            m_TeamInfoHolders[1].transform.GetChild(i).transform.GetChild(2).GetComponent<Text>().text = "Knocked Down: " + playerManager.GetTimesKnockedDown().ToString();
        }
    }

    public void SetWinTeamText(int losTeamId)
    {
        //if (losTeamId == 1)
        //{
        //    m_WonTeamText.text = "MAGENTA TEAM WON!";
        //    m_WonTeamText.color = new Color(255, 0, 255);
        //}
        //else if (losTeamId == 0)
        //{
        //    m_WonTeamText.GetComponent<Text>().text = "CYAN TEAM WON";
        //    m_WonTeamText.GetComponent<Text>().color = new Color(0, 255, 255);
        //}

        m_TeamThatLost = losTeamId;
    }

    void Awake()
    {
        m_EventSystem.gameObject.SetActive(true);
    }
}