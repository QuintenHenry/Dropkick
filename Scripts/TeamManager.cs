using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Team
{
    [SerializeField] public List<GameObject> spawnpoints;
    [HideInInspector] public List<GameObject> players;
    [SerializeField] public Color teamColor;

    public void SetColor(Color color) { teamColor = color; }
}

public class TeamManager : MonoBehaviour {

    [SerializeField] private List<Team> m_Teams = new List<Team>();
    [SerializeField] private GameObject m_PlayerPrefab = null;
    [SerializeField] private GameObject m_PlayerSkinPrefab = null;

    [Header("DEBUG")]
    [SerializeField] private bool m_UseSkins = false;

    // Use this for initialization
    void Start () {
        for(int i = 0; i < m_Teams.Count; i++)
        {
            SetSpawnPointColor(i);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Setters/Getter
    public List<GameObject> GetSpawnPointsOfTeam(int teamId)
    {
        return m_Teams[teamId].spawnpoints;
    }

    public void AddSpawnPoint(int teamId, GameObject spawnPoint)
    {
        for (int i = 0; i < m_Teams.Count; i++)
        {
            if (i == teamId)
            {
                m_Teams[i].spawnpoints.Add(spawnPoint);
            }
        }
    }

    public int GetAmmountOfTeams()
    {
        return m_Teams.Count;
    }

    public Color GetTeamColor(int teamId)
    {
        return m_Teams[teamId].teamColor;
    }
    public void SetTeamColor(int teamId, Color teamColor)
    {
        m_Teams[teamId].SetColor(teamColor);
        SetSpawnPointColor(teamId);
    }

    public int GetAmmountOfPlayers()
    {
        int playerCount = 0;

        foreach (Team team in m_Teams)
        {
            playerCount += team.players.Count;
        }
        return playerCount;
    }

    public int GetScoreOfTeam(int teamId)
    {
        int score = 0;

        for (int i = 0; i < m_Teams.Count; i++)
        {
            if (i != teamId)
            {
                score += GetTimesScored(i);
            }
        }

        return score;
    }
    #endregion

    private void SetPlayerIdAndTeam(List<PlayerInfo> registeredPlayers)
    {
        //int offsetId = 0;
        //for (int i = 0; i < m_Teams.Count; i++)
        //{
        //    for(int t = 0; t < m_Teams[i].players.Count; t++)
        //    {
        //        if (m_UseSkins)
        //        {
        //            m_Teams[i].players[t] = (Instantiate<GameObject>(m_PlayerSkinPrefab));
        //            m_Teams[i].players[t].GetComponent<CreatePlayer>().CreatePlayerGameObject();
        //            m_Teams[i].players[t].transform.GetChild(3 + characterIds[offsetId + t]).gameObject.SetActive(true);
        //            SkinnedMeshRenderer mesh = m_Teams[i].players[t].transform.GetChild(3 + characterIds[offsetId + t]).gameObject.GetComponent<SkinnedMeshRenderer>();
        //            m_Teams[i].players[t].GetComponentInChildren<PlayerManager>().SetMesh(mesh);
        //            m_Teams[i].players[t].GetComponentInChildren<PlayerManager>().SetSkinId(characterIds[offsetId + t]);
        //        }
        //        else
        //        {
        //            m_Teams[i].players[t] = (Instantiate<GameObject>(m_PlayerPrefab));
        //        }

        //        m_Teams[i].players[t].GetComponentInChildren<PlayerManager>().SetTeamManager(this);
        //        m_Teams[i].players[t].GetComponentInChildren<PlayerManager>().SetId(offsetId + t);
        //        m_Teams[i].players[t].GetComponentInChildren<PlayerManager>().SetTeam(i);
        //        Camera.main.GetComponent<DynamicCamera>().AddPlayer(m_Teams[i].players[t].transform.Find("mixamorig:Hips"));

        //    }
        //    offsetId += m_Teams[i].players.Count;
        //}

        //For each player
        foreach (PlayerInfo playerInfo in registeredPlayers)
        {
            GameObject currentPlayer = null;

            //If we are using the skins
            if (m_UseSkins)
            {
                //Add a prefab to the team of the player
                m_Teams[playerInfo.teamID].players.Add((Instantiate(m_PlayerSkinPrefab)));

                //Get the currentPlayer
                currentPlayer = m_Teams[playerInfo.teamID].players[m_Teams[playerInfo.teamID].players.Count - 1];

                //Set-up the player
                currentPlayer.GetComponent<CreatePlayer>().CreatePlayerGameObject();

                //Set the choses skin visible
                currentPlayer.transform.GetChild(3 + playerInfo.skinID).gameObject.SetActive(true);

                //Get the meshrender of the current player
                SkinnedMeshRenderer mesh = currentPlayer.transform.GetChild(3 + playerInfo.skinID).gameObject.GetComponent<SkinnedMeshRenderer>();

                //Set the mesh in the player manager ==> to enables recollering
                currentPlayer.GetComponentInChildren<PlayerManager>().SetMesh(mesh);

                //Set the players skin Id
                currentPlayer.GetComponentInChildren<PlayerManager>().SetSkinId(playerInfo.skinID);
            }
            else
            {
                currentPlayer = (Instantiate(m_PlayerPrefab));
            }

            currentPlayer.GetComponentInChildren<PlayerManager>().SetTeamManager(this);
            currentPlayer.GetComponentInChildren<PlayerManager>().SetId(playerInfo.playerID);
            currentPlayer.GetComponentInChildren<PlayerManager>().SetTeam(playerInfo.teamID);
            Camera.main.GetComponent<DynamicCamera>().AddPlayer(currentPlayer.transform.Find("mixamorig:Hips"));
        }
    }

    //Reserve players prefabs to m_Teams to be used in the game
    public void AddPlayers(List<PlayerInfo> registeredPlayers)
    {
        //Get how many players there are per team
        int playersPerTeam = registeredPlayers.Count / m_Teams.Count;

        ////For each team the game handles
        //for (int i = 0; i < m_Teams.Count; i++)
        //{
        //    //For each player that will be on the team
        //    for (int t = 0; t < playersPerTeam; t++)
        //    {
        //        //Reserve a prefab of a player
        //        m_Teams[i].players.Add(m_PlayerPrefab);
        //    }
        //}

        //Set the playervariables on the prefab
        SetPlayerIdAndTeam(registeredPlayers);
    }

    public List<GameObject> GetPlayersOfTeam(int teamId)
    {
        return m_Teams[teamId].players;
    }

    public int GetTimesScored(int teamId)
    {
        int totalTimesScored = 0;

        foreach (GameObject member in GetPlayersOfTeam(teamId))
        {
            totalTimesScored += member.transform.root.GetComponentInChildren<PlayerManager>().GetTimesScored();
        }

        return totalTimesScored;
    }

    private void SetSpawnPointColor(int teamId)
    {
        foreach (GameObject spawnPoint in m_Teams[teamId].spawnpoints)
        {
            spawnPoint.GetComponentInChildren<MeshRenderer>().material.color = m_Teams[teamId].teamColor;
        }
    }
}
