using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("GamePlay")]
    [SerializeField] float m_MaxGameTimer = 0.0f;
    [SerializeField] int m_MaxScore = 0;
    [SerializeField] int m_PostMaxHealth = 0;

    [Header("UI")]
    [SerializeField] GameObject m_InGameUI = null;
    [SerializeField] GameObject m_EndScreen = null;
    [SerializeField] List<Sprite> m_EndScreenSprites = new List<Sprite>();

    [Header("Managers")]
    [SerializeField] TeamManager m_TeamManager;
    [SerializeField] GameObject m_ScoreBoard = null;

    private float m_GameTimer = 0.0f;
    private bool m_IsGameRunning = false;
    private bool m_IsPostGameModeActive = false;
    private int m_TeamThatLost = 0;

    #region getter/setters
    public void SetMaxGameTime(int maxTime)
    {
        m_MaxGameTimer = maxTime;
    }

    public float GetMaxGameTime() { return m_MaxGameTimer; }

    public float GetElapsedGameTime() { return m_GameTimer; }
    public bool IsGameRunning() { return m_IsGameRunning; }
    #endregion  

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (m_IsGameRunning)
        {
            if (!m_IsPostGameModeActive)
            {
                m_GameTimer += Time.deltaTime;
            }
            
            if (m_GameTimer >= m_MaxGameTimer || IsMaxScoreReached())
            {
                if (!IsMaxScoreReached()  && IsDraw() && !m_IsPostGameModeActive)
                {
                    EnterPostGameMode();
                }
                else if(IsMaxScoreReached()||!IsDraw() && !m_IsPostGameModeActive)
                {
                    //Determine wich team won!
                    SetWonTeamIdByScoredTimes();

                    //End the game
                    EndGame();
                }
                else if (m_IsPostGameModeActive)
                {
                    if (PostEndReached())
                    {
                        EndGame();
                    }
                }
            }
        }
    }

    private void SetWonTeamIdByScoredTimes()
    {
        int[] scoredTimesOfTeams = { 0, 0};

        //For each team
        for (int i = 0; i < m_TeamManager.GetAmmountOfTeams(); i++)
        {
            //Check knockdowns of ezch player of the team
            foreach (GameObject playerObj in m_TeamManager.GetPlayersOfTeam(i))
            {
                //Get the player MAnanger
                PlayerManager playerManager = playerObj.GetComponentInChildren<PlayerManager>();
                scoredTimesOfTeams[i] += playerManager.GetTimesScored();
            }
        }

        if (scoredTimesOfTeams[0] > scoredTimesOfTeams[1])
        {
            m_TeamThatLost = 0;
        }
        else
        {
            m_TeamThatLost = 1;
        }
    }
    private bool IsMaxScoreReached()
    {
        for (int i = 0; i < m_TeamManager.GetAmmountOfTeams(); i++)
        {
            if (m_TeamManager.GetTimesScored(i) >= m_MaxScore)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsDraw()
    {
        if (m_TeamManager.GetTimesScored(0)==m_TeamManager.GetTimesScored(1))
                return true;
        return false;
    }
    private void SetScriptInList(List<GameObject> objects, List<MonoBehaviour> scriptList)
    {
        foreach (GameObject obj in objects)
        {
            foreach (MonoBehaviour script in obj.GetComponentsInChildren<MonoBehaviour>())
            {
                scriptList.Add(script);
            }
        }
    }

    private void DissableScripts(List<MonoBehaviour> scriptList)
    {
        foreach (MonoBehaviour script in scriptList)
        {
            script.enabled = false;
        }
    }

    public void StartGame()
    {
        m_IsGameRunning = true;
    }

    private void EnterPostGameMode()
    {
        m_IsPostGameModeActive = true;

        for (int i = 0; i < m_TeamManager.GetAmmountOfTeams(); i++)
        {
            foreach (GameObject player in m_TeamManager.GetPlayersOfTeam(i))
            {
                PlayerManager playerManager = player.GetComponentInChildren<PlayerManager>();

                playerManager.SetMaxHealth(m_PostMaxHealth);
                playerManager.Respawn(true, true, false);
                playerManager.SetCanRespawn(false);
            }
        }

        m_ScoreBoard.GetComponentInChildren<ScoreBoard>().SetShouldUpdate(false);
        m_ScoreBoard.GetComponentInChildren<TextMesh>().text = "D-Brawl";
    }

    private bool PostEndReached()
    {
        for (int i = 0; i < m_TeamManager.GetAmmountOfTeams(); i++)
        {
            int playersPermaDeath = 0;
            foreach (GameObject player in m_TeamManager.GetPlayersOfTeam(i))
            {
                if (player.GetComponentInChildren<PlayerManager>().GetPermaDeathState())
                {
                    playersPermaDeath += 1;

                    if (playersPermaDeath >= m_TeamManager.GetPlayersOfTeam(i).Count)
                    {
                        m_TeamThatLost = i;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void EndGame()
    {
        m_IsGameRunning = false;
        ManageScripts();

        m_InGameUI.SetActive(false);

        m_EndScreen.SetActive(true);
        m_EndScreen.GetComponent<EndScreen>().SetWinTeamText(m_TeamThatLost);
        m_EndScreen.GetComponent<EndScreen>().SetInfo();

        if (m_TeamThatLost == 0)
        {
            m_EndScreen.GetComponent<Image>().sprite = m_EndScreenSprites[0];
        }
        else
        {
            m_EndScreen.GetComponent<Image>().sprite = m_EndScreenSprites[1];
        }
    }
}
