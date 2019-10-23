using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ListWrapper
{
    public List<GameObject> objectList;
}

public class PlayerInfo
{
    public int playerID = -1;
    public int skinID = -1;
    public int teamID = -1;

    public void SetID(int newPlayerID, int newSkinID) { playerID = newPlayerID; skinID = newSkinID; }
    public void SetTeamID(int newTeamID) { teamID = newTeamID; }
}

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private List<EventSystem> m_EventSystems;

    //Add players
    private bool[] m_PlayersJoined = new bool[4];
    [SerializeField] private List<ListWrapper> m_ToEnableOnJoin = new List<ListWrapper>();
    [SerializeField] private GameObject[] m_PlayerProfiles = new GameObject[4];

    //Select Character
    private PlayerInfo[] m_PlayerInfo = new PlayerInfo[4];
    [SerializeField] private List<Sprite> m_SkinSprites = new List<Sprite>();
    [SerializeField] private List<GameObject> m_ButtonsPerPlayer = new List<GameObject>();

    //Countdown
    private bool m_StartCountdown = false;
    private float m_CurrentCountdown = 0.0f;
    private float m_TimeTillGameStarts = 5.0f;
    [SerializeField] private Text m_NotificationText;

    //Start Game
    [SerializeField] private GameManager m_GameManager;
    [SerializeField] private TeamManager m_TeamManager;
    [SerializeField] private Slider m_GameTimeSlider;
    [SerializeField] private GameObject m_CharacterSelectionScreen;
    [SerializeField] private GameObject m_InGameUI;

    //Cancel
    [SerializeField] private GameObject m_MainMenu;
    [SerializeField] private Sprite m_BackgroundSprite; 
    //Sound
    SoundManager m_SoundManager;

    // Use this for initialization
    void Start()
    {
        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        for (int i = 0; i < m_PlayerInfo.Length; i++)
        {
            m_PlayerInfo[i] = new PlayerInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if a player want to return to the main menu
        ReturnToMainMenu();

        //If all players have selected a character
        if (m_StartCountdown)
        {
            if (CountDownCompleted())
            {
                StartGame();
            }
        }
        //If players are still selecting characters
        else
        {
            //Add players to the game
            AddPlayer();

            //Check how many players are in the game
            int playerCount = 0;
            foreach (bool isJoined in m_PlayersJoined)
            {
                if (isJoined) playerCount++;
            }

            //If the playercount is greater than 0 && even, check if they all have selected a character
            if (playerCount > 0 && playerCount % 2 == 0)
            {
                CheckIfAllSelected(playerCount);
            }

            //If a player wants to undo their selection
            //For each player that has an eventsytem
            for (int i = 0; i < m_EventSystems.Count; i++)
            {
                //If they press the cancel button
                if (Input.GetButton("P" + (i+1) + "_Menu_Cancel"))
                {
                    //Check if they have a skin selected
                    if (m_PlayerInfo[i].skinID != -1)
                    {
                        //If false ==> clear their selction
                        ClearSelectionOfPlayer(i);
                    }
                }
            }

            //If a player has a skin selected make them able to choose a team
            //For each player
            for (int i = 0; i < playerCount; i++)
            {
                //If the player has selected a skin
                if (m_PlayerInfo[i].skinID != -1)
                {
                    //Select a team
                    SelectTeam(i);
                }
            }
        }
    }

    public void DebugTest()
    {
        Debug.Log("CharacterSelection::DebugTest: I Feel Good... sometimes. Coming from: " + transform.parent.name);
    }

    //Determine how many players want to play
    public void AddPlayer()
    {
        //Check each event system
        foreach (EventSystem system in m_EventSystems)
        {
            //Identify the player
            string playerIdentification = "P" + (system.EventSystemID + 1);

            //Check if they press the submit button
            if (Input.GetButton(playerIdentification + "_Menu_Submit"))
            {
                //See if the current player isn't already in the noticed playerList
                if (m_PlayersJoined[system.EventSystemID] == false)
                {
                    //Add player
                    m_PlayersJoined[system.EventSystemID] = true;
                    m_ToEnableOnJoin[system.EventSystemID].objectList.ForEach(action => action.SetActive(true));
                    m_PlayerProfiles[system.EventSystemID].GetComponentInChildren<Text>().text = "Player " + (system.EventSystemID + 1);

                    Debug.Log("Added player" + (system.EventSystemID + 1));
                }
            }
        }
    }

    //Determine wich player has chosen wich character
    public void SelectCharacter(GameObject button)
    {
        string buttonName = button.name;
        int playerID = -1;
        int skinID = -1;
        bool succes = false;

        //Identify the player
        succes = int.TryParse(buttonName.Substring(1, 1), out playerID);
        playerID -= 1;
        if (!succes)
        {
            Debug.Log("CharacterSelection::SelectCharacter(GameObject button) ERROR reading playerID from string");
            return;
        }

        //Check if the player already selected a skin
        if (m_PlayerInfo[playerID].skinID != -1)
        {
            //If a skin is selected ==> return
            return;
        }

        //Identify the skin
        string skinName = buttonName.Substring(3);

        if (skinName == "Incredible")
        {
            skinID = 1;
        }
        else if (skinName == "Lucha")
        {
            skinID = 2;
        }
        else if (skinName == "Builder")
        {
            skinID = 0;
        }
        else if (skinName == "Viking")
        {
            skinID = 3;
        }
        else
        {
            Debug.Log("CharacterSelection::SelectCharacter(GameObject button) ERROR skin does not exist: " + skinName);
            return;
        }

        //Check if the skin is not yet chosen
        if (IsSkinAlreadySelected(skinID) == false)
        {
            //Set the variables
            m_PlayerInfo[playerID].SetID(playerID, skinID);

            //Set the profiles
            SetProfile(playerID);

            //Disable Other buttons with the same skin
            DisableSkinButtons(skinID, playerID);

            //Disable the cursor movement of the player
            DisableInputOfPlayer(playerID, true);
        }

        ////If there would be a selector on a button with the same skin, ==> Move it
        //MoveSelectors();
    }

    //Set the selected skins on the playerProfile
    private void SetProfile(int playerID)
    {
        m_PlayerProfiles[playerID].GetComponent<Image>().sprite = m_SkinSprites[m_PlayerInfo[playerID].skinID];
    }

    //Clear the selected skin on the playerprofile
    private void ClearProfile(int playerID)
    {
        m_PlayerProfiles[playerID].GetComponent<Image>().sprite = m_BackgroundSprite;
    }

    //Disable the Selected skin buttons from the other players
    private void DisableSkinButtons(int skinID, int playerID)
    {
        //For each player buttons
        for (int i = 0; i < m_ButtonsPerPlayer.Count; i++)
        {
            //Disable the selected skin button
            //m_ButtonsPerPlayer[i].transform.GetChild(skinID).gameObject.GetComponent<Button>().interactable = false;

            //Show that the skin can't be choses anymore
            ColorBlock newColorBlock = m_ButtonsPerPlayer[i].transform.GetChild(skinID).gameObject.GetComponent<Button>().colors;
            Color disabledColor = new Color(0, 0, 0, 255);
            PlayerColors playerColors = new PlayerColors();

            disabledColor = playerColors.playerColorList[playerID];

            newColorBlock.normalColor = disabledColor;
            newColorBlock.pressedColor = disabledColor;
            newColorBlock.highlightedColor = disabledColor;

            m_ButtonsPerPlayer[i].transform.GetChild(skinID).gameObject.GetComponent<Button>().colors = newColorBlock;
        }
    }

    //Enable the Selected skin buttons from the other players
    private void EnableSkinButtons(int skinID, int playerID)
    {
        //For each player buttons
        for (int i = 0; i < m_ButtonsPerPlayer.Count; i++)
        {
            //Show that the skin can be chosen
            ColorBlock newColorBlock = m_ButtonsPerPlayer[i].transform.GetChild(skinID).gameObject.GetComponent<Button>().colors;
            Color enabledColor = new Color(191, 191, 191, 255);

            newColorBlock.normalColor = enabledColor;
            newColorBlock.pressedColor = enabledColor;
            newColorBlock.highlightedColor = enabledColor;

            m_ButtonsPerPlayer[i].transform.GetChild(skinID).gameObject.GetComponent<Button>().colors = newColorBlock;
        }
    }
    //If a the selected button is uninteractable, Move the cursor to an interacteble button
    private void MoveSelectorOfPlayer(int playerID, int SkinID)
    {
        m_EventSystems[playerID].SetSelectedGameObject(GetInteractableButton(playerID, SkinID));
    }

    //Get an interactable button
    private GameObject GetInteractableButton(int playerID, int skinID = -1)
    {
        //If a skinID is given skip the rest
        if (skinID != -1)
        {
            return m_ButtonsPerPlayer[playerID].transform.GetChild(skinID).gameObject;
        }

        //Create a new button list
        List<GameObject> buttonList = new List<GameObject>();

        //Get the buttons in a list
        for (int i = 0; i < m_ButtonsPerPlayer[playerID].transform.childCount; i++)
        {
            buttonList.Add(m_ButtonsPerPlayer[playerID].transform.GetChild(i).gameObject);
        }

        //For each button of this player
        foreach (GameObject button in buttonList)
        {
            //Check if the button is interactable
            if (button.GetComponentInChildren<Button>().IsInteractable())
            {
                //if true ==> return this button
                return button;
            }
        }

        Debug.Log("No button found that is interactable");
        return null;
    }

    //Check if a skin is already chosen
    private bool IsSkinAlreadySelected(int skinID)
    {
        //For each player
        foreach (PlayerInfo currentPlayer in m_PlayerInfo)
        {
            //Check if the currentPlayer selected skin is == skinId
            if (currentPlayer.skinID == skinID)
            {
                //If true ==> return true
                return true;
            }
        }
        return false;
    }

    //Tell the system to start the countdown When everybody has selected a skin
    private void CheckIfAllSelected(int playerCount)
    {
        int playersHaveSelected = 0;
        //Check all joined players for if they have selected a character
        foreach (PlayerInfo info in m_PlayerInfo)
        {
            if (info.skinID != -1) playersHaveSelected++;
        }

        //If all have a skin && team selected, start the countdown
        if (playersHaveSelected == playerCount && CheckIfAllSelectedTeam(playerCount))
        {
            m_StartCountdown = true;
            m_SoundManager.PlaySound("CountDown");
            DisableInput(true);
        }
    }

    //disable further selection
    private void DisableInput(bool disable)
    {
        m_EventSystems.ForEach(action => action.transform.parent.gameObject.SetActive(!disable));
    }

    //Disable input of a specific player
    private void DisableInputOfPlayer(int playerID, bool disable)
    {
        m_EventSystems[playerID].transform.gameObject.SetActive(!disable);
    }

    //CountDownTillGameStart
    private bool CountDownCompleted()
    {

        int prevnumber = (int)m_CurrentCountdown;
        m_CurrentCountdown += Time.deltaTime;

        if ((int)m_CurrentCountdown != prevnumber && prevnumber!=4)
        {
            m_SoundManager.PlaySound("CountDown");

        }
        m_NotificationText.text = "Time till the game starts: " + string.Format("{0:00}", Mathf.Floor((m_TimeTillGameStarts - m_CurrentCountdown) % 60));

        if (m_CurrentCountdown >= m_TimeTillGameStarts) return true;

        return false;
    }

    //StartTheGame
    private void StartGame()
    {
        //Get all the registered players and their properties
        List<PlayerInfo> registeredPlayers = new List<PlayerInfo>();
        foreach (PlayerInfo playerInfo in m_PlayerInfo)
        {
            if (playerInfo.playerID != -1)
            {
                registeredPlayers.Add(playerInfo);
            }
        }

        //Sort the registered players per team
        registeredPlayers = SortRegisteredPlayerPerTeam(registeredPlayers);

        //Add the registerd players to the game
        m_TeamManager.AddPlayers(registeredPlayers);

        //Set the needed game variables
        m_GameManager.SetMaxGameTime((int)m_GameTimeSlider.value * 60);

        //Start the game
        m_GameManager.StartGame();

        //Reset character selection variables
        m_StartCountdown = false;
        m_CurrentCountdown = 0.0f;

        WipeSelection();

        //Show the needed UI
        m_InGameUI.SetActive(true);
        m_CharacterSelectionScreen.SetActive(false);

        //Disavle character selection inputs and so on
        gameObject.SetActive(false);
    }

    //Method to sort the player list according to their team
    private List<PlayerInfo> SortRegisteredPlayerPerTeam(List<PlayerInfo> playerInfoList)
    {
        List<PlayerInfo> sortedList = new List<PlayerInfo>();

        int ammountOfTeams = 2;
        for (int i = 0; i < ammountOfTeams; i++)
        {
            int currentTeamID = i;

            foreach (PlayerInfo playerInfo in playerInfoList)
            {
                if (playerInfo.teamID == currentTeamID)
                {
                    sortedList.Add(playerInfo);
                }
            }
        }

        return sortedList;
    }

    //Clear all variables
    public void WipeSelection()
    {
        //Reset player joined
        for (int i = 0; i < m_PlayersJoined.Length; i++)
        {
            m_PlayersJoined[i] = false;
        }

        //Disable all object that got enalbe on join
        foreach (ListWrapper wrappedList in m_ToEnableOnJoin)
        {
            foreach (GameObject obj in wrappedList.objectList)
            {
                obj.SetActive(false);
            }
        }

        //Reset each player Profile
        foreach (GameObject profile in m_PlayerProfiles)
        {
            profile.GetComponent<Image>().sprite = m_BackgroundSprite;
            profile.GetComponent<Image>().color = new Color(255, 255, 255, 70);
            profile.GetComponentInChildren<Text>().text = "Press A to join";
        }

        //Reset the player info
        foreach (PlayerInfo info in m_PlayerInfo)
        {
            info.SetID(-1, -1);
            info.SetTeamID(-1);
        }

        //Reset the buttons
        foreach (GameObject buttonHolder in m_ButtonsPerPlayer)
        {
            for (int i = 0; i < buttonHolder.transform.childCount; i++)
            {
                //Show that the skin can't be choses anymore
                ColorBlock newColorBlock = buttonHolder.gameObject.transform.GetChild(i).GetComponent<Button>().colors;
                Color enabledColor = new Color(191, 191, 191, 255);

                newColorBlock.normalColor = enabledColor;
                newColorBlock.pressedColor = enabledColor;
                newColorBlock.highlightedColor = enabledColor;

                buttonHolder.gameObject.transform.GetChild(i).GetComponent<Button>().colors = newColorBlock;
            }
        }

        //Reset the countdow, if there would have been a "Feature"
        m_StartCountdown = false;
        m_CurrentCountdown = 0.0f;

        //Reset the notification text
        m_NotificationText.text = "Choose your avatar";

        DisableInput(false);
        Debug.Log("Wiped the current character selction info");
    }

    //Unselect the current selected skin of a player
    private void ClearSelectionOfPlayer(int playerID)
    {
        //Get the skin ID
        int skinID = m_PlayerInfo[playerID].skinID;

        //Clear the variables
        m_PlayerInfo[playerID].SetID(playerID, -1);

        //Clear the profile
        ClearProfile(playerID);

        //Enable Other buttons with the same skin
        EnableSkinButtons(skinID, playerID);

        //Re-Enable the players movement
        DisableInputOfPlayer(playerID, false);

        //Make sure that the player has a skin button selected
        MoveSelectorOfPlayer(playerID, skinID);
    }

    //Make the players select a team
    private void SelectTeam(int playerID)
    {
        //If the player presses left put into red team
        if (Input.GetAxis("P"+(playerID + 1)+"_Menu_Horizontal") == -1)
        {
            m_PlayerInfo[playerID].SetTeamID(0);
            m_PlayerProfiles[playerID].GetComponent<Image>().color = new Color(255, 0, 0);
        }
        //If the player presses right put into blue team
        if (Input.GetAxis("P" + (playerID + 1) + "_Menu_Horizontal") == 1)
        {
            m_PlayerInfo[playerID].SetTeamID(1);
            m_PlayerProfiles[playerID].GetComponent<Image>().color = new Color(0, 0, 255);
        }
    }

    //Check if everyplayer has selected a team
    private bool CheckIfAllSelectedTeam(int playerCount)
    {
        int team0Ammount = 0;

        //For each player check their team && Check if there is no team in overcapacity
        foreach (PlayerInfo playerInfo in m_PlayerInfo)
        {
            //If they have no team selected return false and stop checking
            if (playerInfo.teamID == -1 && playerInfo.playerID != -1)
            {
                return false;
            }

            if (playerInfo.teamID == 0)
            {
                team0Ammount++;
            }
        }
        //If team 0 doesn't contain excactly half the players return false
        if (team0Ammount != (playerCount / 2)) return false;

        //If there was no player without a team && half of the players are in team 0 (other half is in team1) return true
        return true;
    }

    //Press B to go back to main menu
    private void ReturnToMainMenu()
    {
        //For each player check:
        for (int i = 0; i < m_PlayersJoined.Length; i++)
        {
            //Check if the player joined
            if (m_PlayersJoined[i])
            {
                //Check if a player presses the b button
                if(Input.GetButtonDown("P" + (i + 1) + "_Menu_Cancel"))
                {
                    //check if that player has nothing selected
                    if (m_PlayerInfo[i].skinID == -1)
                    {
                        //If there was nothing selected return to main menu
                        WipeSelection();
                        m_MainMenu.SetActive(true);
                        m_CharacterSelectionScreen.SetActive(false);
                    }
                }
            }
        }
    }

    //On awake
    public void Awake()
    {
        Debug.Log("Character selection woke up");
        m_EventSystems[0].SetSelectedGameObject(m_ButtonsPerPlayer[0].transform.GetChild(0).gameObject);

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            Debug.Log(Input.GetJoystickNames()[i]);
        }
    }
}
