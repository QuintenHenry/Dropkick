using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    [Header("Timing")]
    [SerializeField] private float m_TimeBetweenSwitches = 1.0f;

    [Header("Managers")]
    [SerializeField] private TeamManager m_TeamManager = null;
    [SerializeField] private GameManager m_GameManager = null;

    [Header("TextMeshes")]
    [SerializeField] private TextMesh m_MainText = null;
    [SerializeField] private TextMesh m_TimeText = null;
    [SerializeField] private List<TextMesh> m_TeamScoreText = null;

    private bool m_ShouldUpdate = true;
    private bool m_IsShowingTime = true;
    private string m_TimeString = "";

    private int m_PrevTimer;

    SoundManager m_SoundManager;
	// Use this for initialization
	void Start () {
        StartCoroutine("ChangeMainText");
        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_ShouldUpdate)
        {
            //Set scores o fteams
            SetNewScore();

            //Get time variables
            float elapsedGameTime = m_GameManager.GetElapsedGameTime();
            float maxGameTime = m_GameManager.GetMaxGameTime();

            //Calculate game time
            CalculateGameTime(elapsedGameTime, maxGameTime);

            //Set Main Time
            //If showing time --> show score
            if (m_IsShowingTime)
            {
                m_MainText.text = m_TeamManager.GetTimesScored(1).ToString() + " - " + m_TeamManager.GetTimesScored(0).ToString();
                //m_IsShowingTime = false;
            }
            //if showing score --> show time
            else
            {
                m_MainText.text = m_TimeString;
                //m_IsShowingTime = true;
            }
        }
        else
        {
            StopCoroutine(ChangeMainText());
        }
    }

    #region  Getters/Setters
    public bool GetShouldUpdate() { return m_ShouldUpdate; }
    public void SetShouldUpdate(bool shouldUpdateScore) { m_ShouldUpdate = shouldUpdateScore; }
    #endregion

    public void SetNewScore()
    {
        //For each team on bilboard --> include ID
        for (int i = 0; i < m_TeamScoreText.Count; i++)
        {
            m_TeamScoreText[i].text = m_TeamManager.GetScoreOfTeam(i).ToString();
        }
    }

    IEnumerator ChangeMainText()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_TimeBetweenSwitches);
            m_IsShowingTime = !m_IsShowingTime;
        }
    }

    public void CalculateGameTime(float elapsedTime, float maximumTime)
    {

        float timeLeft = maximumTime - elapsedTime;
        float seconds = Mathf.Floor(timeLeft % 60);
        float minutes = Mathf.Floor(timeLeft / 60);

        if (m_PrevTimer!=(int)seconds && (int)seconds<10 && (int)minutes <= 0)
            m_SoundManager.PlaySound("CountDown");

        m_PrevTimer = (int)seconds;

        m_TimeString = string.Format("{0:00} : {1:00}", minutes, seconds);
        m_TimeText.text = m_TimeString;
    }
}
