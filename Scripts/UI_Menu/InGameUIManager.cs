using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InGameUIManager : MonoBehaviour {

    //Awake
    private bool m_ShouldP2UseP3Profile = false;
    private bool m_DoFirstTimeSetUp = true;

    //Profiles
    [SerializeField] private List<GameObject> m_PlayerProfiles = new List<GameObject>();
    [SerializeField] private TeamManager m_TeamManager;

    //Skins
    [SerializeField] private List<Sprite> m_SkinSprites = new List<Sprite>();

    //Ability
    [SerializeField] private List<Sprite> m_AbilitySprites = new List<Sprite>();

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (m_DoFirstTimeSetUp)
        {
            SetOnAwake();
        }

        SetPlayerProfiles();	
	}

    //Update the playerprofile
    private void SetPlayerProfiles()
    {
        //For each team
        for (int i = 0; i < m_TeamManager.GetAmmountOfTeams(); i++)
        {
            //for each player of team set their profile
            foreach (GameObject player in m_TeamManager.GetPlayersOfTeam(i))
            {
                //Get their playerManager
                PlayerManager playerManager = player.GetComponentInChildren<PlayerManager>();

                //Set the profile of the player with id
                //Set the skinSprite
                SetSkinSprite(ref playerManager, m_TeamManager.GetTeamColor(playerManager.GetTeam()));

                //Set the overlay sprite
                SetOverlaySprite(ref playerManager, !playerManager.GetCanRespawn());

                //Set the ability sprite
                PowerUpManager powerUpManager = player.GetComponentInChildren<PowerUpManager>();
                SetAbilitySprite(ref playerManager, ref powerUpManager);

                //Set the team/player Colors
                SetTeamAndPlayerIndicators(ref playerManager);
            }
        }
    }

    void SetSkinSprite(ref PlayerManager playerManager, Color teamColor)
    {
        int toUseID = playerManager.GetId();

        if (m_ShouldP2UseP3Profile && toUseID == 1)
        {
            toUseID += 1;
        }

        Image image = m_PlayerProfiles[toUseID].GetComponent<Image>();
        image.sprite = m_SkinSprites[playerManager.GetSkinId()];
       // image.color = new Color(teamColor.r, teamColor.g, teamColor.b, 255);
    }

    private void SetOverlaySprite(ref PlayerManager playerManager, bool isvisible)
    {
        int toUseID = playerManager.GetId();

        if (m_ShouldP2UseP3Profile && toUseID == 1)
        {
            toUseID += 1;
        }

        Image overlay = m_PlayerProfiles[toUseID].transform.GetChild(0).GetComponent<Image>();
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.g, 255 * Convert.ToInt16(isvisible));
    }

    private void SetAbilitySprite(ref PlayerManager playerManager, ref PowerUpManager powerUpManager)
    {
        int toUseID = playerManager.GetId();

        if (m_ShouldP2UseP3Profile && toUseID == 1)
        {
            toUseID += 1;
        }

        Image image = m_PlayerProfiles[toUseID].transform.GetChild(1).GetComponent<Image>();

        if (powerUpManager.GetCurrentPowerUp() == PowerUpManager.PowerUp.None)
        {
            image.sprite = null;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        else
        {
            image.sprite = m_AbilitySprites[(int)powerUpManager.GetCurrentPowerUp() - 1];
            image.color = new Color(image.color.r, image.color.g, image.color.b, 255);
        }
    }

    void SetTeamAndPlayerIndicators(ref PlayerManager playerManager)
    {
        int toUseID = playerManager.GetId();

        if (m_ShouldP2UseP3Profile && toUseID == 1)
        {
            toUseID += 1;
        }

        Image imageTeam = m_PlayerProfiles[toUseID].transform.GetChild(2).GetComponent<Image>(); ;
        Image imagePlayer = m_PlayerProfiles[toUseID].transform.GetChild(3).GetComponent<Image>(); ;

        Color teamColor = m_TeamManager.GetTeamColor(playerManager.GetTeam());
        teamColor.a = 255;

        imageTeam.color = teamColor;

        Color playerColor = playerManager.GetPlayerColor();
        playerColor.a = 255;

        imagePlayer.color = playerColor;
    }

    public void SetOnAwake()
    {
        if (m_TeamManager.GetAmmountOfPlayers() >0)
        {
            m_DoFirstTimeSetUp = false;
        }

        if ( m_TeamManager.GetAmmountOfPlayers() == 2)
        {
            m_PlayerProfiles[1].gameObject.SetActive(false);
            m_PlayerProfiles[3].gameObject.SetActive(false);
            m_ShouldP2UseP3Profile = true;
        }
    }
}
