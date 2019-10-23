

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColors
{
    private static Color colorPlayer0 = new Color(0.39f, 0.59f, 0.24f);
    private static Color colorPlayer1 = new Color(0.78f, 0.59f, 0.16f);
    private static Color colorPlayer2 = new Color(0.75f, 0.22f, 0.92f);
    private static Color colorPlayer3 = new Color(0.95f, 0.6f, 0.46f);

    public Color[] playerColorList = { colorPlayer0 , colorPlayer1 , colorPlayer2, colorPlayer3 };
}

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int m_MaxHealth = 0;
    [SerializeField] private float m_Speed = 0;
    [SerializeField] private float m_PushBackForce = 0;
    [SerializeField] private float m_JumpForce = 0;
    [SerializeField] private SkinnedMeshRenderer m_Mesh = null;
    [SerializeField] private HealthBar m_Healthbar;
    [SerializeField] private float m_DeathzoneY = -2.0f;

    private TeamManager m_TeamManager = null;
    private int m_Team = 0;
    private int m_Id = 0;
    private float m_CurrentHealth = 0;
    private bool m_IsKnockedDown = false;
    private int m_TotalKnockdowns = 0;
    private bool m_IsOnGround = false;
    private bool m_IsInvulnerable = false;
    private int m_TimesScored = 0;
    private bool m_CanBeScored = true;
    private float m_CannotBeScoredDuration = 0.0f;
    private float m_CannotBeScoredDurationMax = 0.5f;
    private bool m_CanRespawn = true;
    private bool m_IsPermaDeath = false;
    private int m_CurrentStamina = 100;
    [SerializeField] private int m_MaxStamina = 100;
    private bool m_ShouldStaminaRegen = false;
    private float m_StaminaTimer = 0.0f;
    [SerializeField] private float m_TimeTillStaminaRegenStart = 2.0f;
    [SerializeField] private int m_StaminaRegenAmmount = 5;
    private bool m_IsStaminaRegening = false;
    private StaminaBar m_StaminaBar = null;
    private float m_StaminaInterval = 0.0f;
    private float m_StaminaInterValMax = 0.2f;

    private int m_SkinId = 0;
    private Color m_SkinColor = new Color();
    private int m_Defence = 0;

    private PlayerManager m_PlayerThatLastTouchedMe = null;

    #region EndStats
    private int m_TimesGoalScored = 0;
    private int m_TimesKnockdown = 0;
    private int m_TimesRevived = 0;
    private int m_TimesPickUpPowerUp = 0;

    public void IncrementTimesGoalScored()
    {
        m_TimesGoalScored++;
    }
    public int GetTimesGoalScored()
    {
        return m_TimesGoalScored;
    }
    #endregion

    #region Skeleton
    private GameObject[] m_Skeleton = new GameObject[13];

    public void SetSkeleton(GameObject[] skeleton) { m_Skeleton = skeleton; }
    public GameObject GetSkeleton_Part(int index) { return m_Skeleton[index]; }
    public GameObject[] GetSkeleton() { return m_Skeleton; }
    #endregion

    // Use this for initialization
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_CurrentStamina = m_MaxStamina;
        InvokeRepeating("CheckKillZ", 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //ChangeColor();

        ManageCanBeScored();
        RegenStamina();
    }

    #region Getters/Setters
    public PlayerManager GetPlayerThatLastTouchedMe() { return m_PlayerThatLastTouchedMe; }
    public void SetPlayerThatLastTouchedMe(PlayerManager playerThatTouchesMe) { m_PlayerThatLastTouchedMe = playerThatTouchesMe; }

    public int GetDefence() { return m_Defence; }
    public void SetDefence(int defence) { m_Defence = defence; }

    public int GetMaxHealth() { return m_MaxHealth; }
    public void SetMaxHealth(int maxHealth) { m_MaxHealth = maxHealth; }

    public float GetCurrentHealth() { return m_CurrentHealth; }
    public void SetCurrentHealth(float currentHealth) {

        if (!m_IsInvulnerable)
        {
            m_CurrentHealth = currentHealth;
            m_Healthbar.SetHealthPercentage(m_CurrentHealth / m_MaxHealth, m_IsKnockedDown);
        }
    }

    public float GetSpeed() { return m_Speed; }
    public void SetSpeed(float speed) { m_Speed = speed; }

    public int GetTeam() { return m_Team; }
    public void SetTeam(int team)
    {
        m_Team = team;

        //change outline
        m_Mesh.materials[2].SetColor("_OutlineColor", m_TeamManager.GetTeamColor(m_Team));

        //Set character color
        PlayerColors playerColors = new PlayerColors();
        m_SkinColor = playerColors.playerColorList[m_Id];
        m_Mesh.material.color = m_SkinColor;

        //Set character color skin
        if (m_Team == 1)
        {
            //Get the Correct texture for team 1
            var mat = m_Mesh.materials[3].GetTexture("_MainTex");

            //Set the correct texture to the one that will show in-game
            m_Mesh.materials[1].SetTexture("_MainTex", mat);
        }

        //Set the texture of the correct team also to the skin 1== builder, 2 == lucha
        if (m_SkinId != 0) //3
        {
            m_Mesh.materials[0].SetTexture("_MainTex", m_Mesh.materials[1].GetTexture("_MainTex"));
        }
        //else
        //{
        //    m_Mesh.materials[0].SetTexture("_MainTex", m_Mesh.materials[1].GetTexture("_MainTex"));
        //}

        Respawn(false, true, false);
    }

    public void SetMesh(SkinnedMeshRenderer mesh)
    {
        m_Mesh = mesh;
    }

    public Color GetPlayerColor()
    {
        return m_SkinColor;
    }

    public void SetHealthBar(HealthBar script)
    {
        m_Healthbar = script;
    }

    public void SetDeathHeight(float deadheight)
    {
        m_DeathzoneY = deadheight;
    }

    public int GetId() { return m_Id; }
    public void SetId(int id)
    {
        m_Id = id;
        transform.root.name = "Player" + m_Id;
    }

    public bool GetKnockDownState() { return m_IsKnockedDown; }
    public void SetKnockDownState(bool isKnockedDown)
    {
        m_IsKnockedDown = isKnockedDown;

        if (m_IsKnockedDown)
        {
            m_TotalKnockdowns = +1;

            List<GameObject> limbs = new List<GameObject> { m_Skeleton[(int)PlayerSkeleton.rightArm], m_Skeleton[(int)PlayerSkeleton.rightElbow], m_Skeleton[(int)PlayerSkeleton.rightKnee], m_Skeleton[(int)PlayerSkeleton.leftKnee], m_Skeleton[(int)PlayerSkeleton.leftElbow] };

            foreach (GameObject limb in limbs)
            {
                limb.GetComponent<ConstantForce>().force = Vector3.zero;
                limb.GetComponent<ConstantForce>().relativeForce = Vector3.zero;
                limb.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    public int GetTotalKnockDowns() { return m_TotalKnockdowns; }
    public void SetTotalKnockDowns(int totalKnockdowns) { m_TotalKnockdowns = totalKnockdowns; }

    public float GetPushBackForce() { return m_PushBackForce; }
    public void SetPushBackForce(float pushBackForce) { m_PushBackForce = pushBackForce; }

    public float GetJumpForce() { return m_JumpForce; }
    public void SetJumpForce(float jumpForce) { m_JumpForce = jumpForce; }

    public bool GetIsOnGround() { return m_IsOnGround; }
    public void SetIsOnGround(bool isOnGround) { m_IsOnGround = isOnGround; }

    public bool GetInvulnerableState() { return m_IsInvulnerable; }
    public void SetInvulnerableState(bool isInvulnerable) { m_IsInvulnerable = isInvulnerable;}

    public int GetTimesScored() { return m_TimesScored; }
    public void SetTimesScored(int newTimesScored) { m_TimesScored = newTimesScored; }

    public TeamManager GetTeamManager() { return m_TeamManager; }
    public void SetTeamManager(TeamManager teamManager) { m_TeamManager = teamManager; }

    public bool GetCanBeScored() { return m_CanBeScored; }
    public void SetCanBeScored(bool canBeScored) { m_CanBeScored = canBeScored; }

    public bool GetCanRespawn() { return m_CanRespawn; }
    public void SetCanRespawn(bool canRespawn) { m_CanRespawn = canRespawn; }

    public bool GetPermaDeathState() { return m_IsPermaDeath; }
    public void SetPermaDeathState(bool isPermaDeath) { m_IsPermaDeath = isPermaDeath; }

    public int GetCurrentStamina() { return m_CurrentStamina; }
    public void SetCurrentStamina(int ammount) {
        m_CurrentStamina = ammount;
        m_StaminaBar.SetStaminaPercentage((float)m_CurrentStamina / m_MaxStamina);
    }

    public void SetStaminaBar(StaminaBar bar)
    {
        m_StaminaBar = bar;
    }

    public int GetMaxStamina() { return m_MaxStamina; }
    public void SetMaxStamina(int ammount) { m_MaxStamina = ammount; }

    public void SetTimeTillStaminaRegen(float time) { m_TimeTillStaminaRegenStart = time; }

    public void SetStaminaTimer(float time) { m_StaminaTimer = time; }

    public void ShouldStaminaRegen(bool shouldRegen) { m_ShouldStaminaRegen = shouldRegen; m_StaminaTimer = 0.0f; }
    
    public void  SetStaminaRegenAmmount(int ammount) { m_StaminaRegenAmmount = ammount; }

    public void SetSkinId(int id) { m_SkinId = id; }
    public int GetSkinId() { return m_SkinId; }

    #region EndScreen
    public void SetTimesKnockedDown(int totalTimesKnockedDown) { m_TimesKnockdown = totalTimesKnockedDown; }
    public int GetTimesKnockedDown() { return m_TimesKnockdown; }

    public void SetTimesRevived(int timesRevived) { m_TimesRevived = timesRevived; }
    public int GetTimesRevived() { return m_TimesRevived; }

    public void SetTimesPickedPowerUp(int timespickedUp) { m_TimesPickUpPowerUp = timespickedUp; }
    public int GetTimesPickedPowerUp() { return m_TimesPickUpPowerUp; }
    #endregion EndScreen

    #endregion

    private void ManageCanBeScored()
    {
        //Check if the player currently can be scored, if not start the counter
        if (!m_CanBeScored)
        {
            m_CannotBeScoredDuration += Time.deltaTime;

            //When the duration goes over the max, Set the player scoreAble
            if (m_CannotBeScoredDuration > m_CannotBeScoredDurationMax)
            {
                m_CannotBeScoredDuration = 0.0f;
                m_CanBeScored = true;
            }
        }
    }

    public void Respawn(bool resetKnockdowns, bool resetHealth, bool KnockdownState)
    {
        //Check if a player can respawn
        if (m_CanRespawn)
        {
            //Make sure the player is not knockeddown anymore
            SetKnockDownState(KnockdownState);

            //Make sure the player can again be scored
            m_CanBeScored = false;

            //Remove the reference to the last hit
            m_PlayerThatLastTouchedMe = null;

            //Check if the health needs to be reset
            if (resetHealth)
            {
                //Set the health to its maximum value
                SetCurrentHealth(GetMaxHealth());
            }

            //Check if the currentKnockdowns need to be reset 
            if (resetKnockdowns)
            {
                //Set the current knockdowns to 0
                m_TotalKnockdowns = 0;
            }

            //Make sure the player has collision
            SetLayerRecursively(transform.gameObject, LayerMask.NameToLayer("Player"));

            //Search for a spawnpoints, and spawn the player on it
            List<GameObject> spawnPoints = m_TeamManager.GetSpawnPointsOfTeam(GetTeam());
            int randomIndex = Random.Range(0, spawnPoints.Count);
            transform.position = spawnPoints[randomIndex].transform.position + new Vector3(0, 1, 0);

            //Constraint the player movement for a few seconds (loses all current apllying forces)
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            StartCoroutine(ResetConstraints());
        }
        //If the player cannot respawn
        else if (!m_CanRespawn)
        {
            //Make sure the player can't play anymore
            m_IsPermaDeath = true;

            //Disable the actor since the player can't play anymore
            transform.parent.root.gameObject.SetActive(false);
        }
    }

    void SetLayerRecursively(GameObject obj, int newlayer)
    {
        obj.layer = newlayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newlayer);
        }
    }

    IEnumerator ResetConstraints()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
    }

    private void CheckKillZ()
    {
        //Check if the player position is below the killz
        if (transform.position.y <= m_DeathzoneY)
        {
            //If the actor is below the killz, respawn it without reseting the health and the current knockdowns
            Respawn(false, false, m_IsKnockedDown);
        }
    }

    private void RegenStamina()
    {
        //Check if the curent stamina is less then the max stamina
        if (m_CurrentStamina < m_MaxStamina)
        {
            //Enable the regeneration code
            m_ShouldStaminaRegen = true;
        }

        //Check if the stamina should regen, but the player is not yet regening it --> creates a delay
        if (m_ShouldStaminaRegen && !m_IsStaminaRegening)
        {
            //Start the delay timer
            m_StaminaTimer += Time.deltaTime;

            //When the delay has passed enable the regeneration process
            if (m_StaminaTimer > m_TimeTillStaminaRegenStart)
            {
                //Enable regeneretion process
                m_IsStaminaRegening = true;
            }
        }

        //Check if the stamina should not be regened
        if (!m_ShouldStaminaRegen)
        {
            //Reset the stamina variables
            m_IsStaminaRegening = false;
            m_StaminaTimer = 0.0f;
            m_StaminaInterval = 0.0f;
        }

        //If the stamina is regening
        if (m_IsStaminaRegening)
        {
            //Timer for delay of regen intervals
            m_StaminaInterval += Time.deltaTime;

            //When enough time has passed
            if (m_StaminaInterval > m_StaminaInterValMax)
            {
                //Reset the interval counter
                m_StaminaInterval = 0.0f;

                //Add stamina
                SetCurrentStamina(m_CurrentStamina + m_StaminaRegenAmmount);

                //Make sure the curent stamine doesn't go over the maximum stamina
                if (m_CurrentStamina >= m_MaxStamina)
                {
                    m_CurrentStamina = m_MaxStamina;
                }

                //Stop regening stamina
                m_IsStaminaRegening = false;
                m_ShouldStaminaRegen = false;
            }
        }
    }
}
