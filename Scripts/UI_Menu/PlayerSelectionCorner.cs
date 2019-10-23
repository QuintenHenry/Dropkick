using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSelectionCorner : MonoBehaviour {
    [SerializeField] private EventSystem m_EventSystem = null;
    [SerializeField] private GameObject m_FirstSelected;
    [SerializeField] private CanvasScaler m_CanvasScaler;

	// Use this for initialization
	void Start ()
    {
        //m_EventSystem.SetSelectedGameObject(m_FirstSelected);
    }
	
	// Update is called once per frame
	void Update () {
        if (!m_EventSystem.IsActive())
        {
            //gameObject.SetActive(false);
            return;
        }

        if (m_EventSystem.currentSelectedGameObject != null)
        {
            if (m_EventSystem.currentSelectedGameObject.GetComponent<Button>() != null)
            {
                Vector3 pos = m_EventSystem.currentSelectedGameObject.transform.position;
                int offset = -20;
                switch (m_EventSystem.EventSystemID)
                {
                    case 0:
                        pos.x += (GetComponent<RectTransform>().rect.x - offset) * m_CanvasScaler.scaleFactor;
                        pos.y -= (GetComponent<RectTransform>().rect.y - offset) * m_CanvasScaler.scaleFactor;
                        break;
                    case 1:
                        pos.x += (GetComponent<RectTransform>().rect.x - offset) * m_CanvasScaler.scaleFactor;
                        pos.y += (GetComponent<RectTransform>().rect.y - offset) * m_CanvasScaler.scaleFactor;
                        break;
                    case 2:
                        pos.x -= (GetComponent<RectTransform>().rect.x - offset) * m_CanvasScaler.scaleFactor;
                        pos.y += (GetComponent<RectTransform>().rect.y - offset) * m_CanvasScaler.scaleFactor;
                        break;
                    case 3:
                        pos.x -= (GetComponent<RectTransform>().rect.x - offset) * m_CanvasScaler.scaleFactor;
                        pos.y -= (GetComponent<RectTransform>().rect.y - offset) * m_CanvasScaler.scaleFactor;
                        break;
                    default:
                        break;
                }
                transform.position = pos;
            }
        }
	}

    private void Awake()
    {
        Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
        m_EventSystem.SetSelectedGameObject(m_FirstSelected);

        PlayerColors playerColors = new PlayerColors();

        switch (m_EventSystem.EventSystemID)
        {
            case 0:
                //GetComponent<Image>().color = Color.green;
                GetComponent<Image>().color = playerColors.playerColorList[0];
                Debug.Log(playerColors.playerColorList[0]);
                GetComponent<RectTransform>().localScale = scale * m_CanvasScaler.scaleFactor;
                break;
            case 1:
                //GetComponent<Image>().color = Color.red;
                GetComponent<Image>().color = playerColors.playerColorList[1];
                GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0.707f, 0.707f);
                GetComponent<RectTransform>().localScale = scale * m_CanvasScaler.scaleFactor;
                break;
            case 2:
                //GetComponent<Image>().color = Color.yellow;
                GetComponent<Image>().color = playerColors.playerColorList[2];
                GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 1.0f, 0);
                GetComponent<RectTransform>().localScale = scale * m_CanvasScaler.scaleFactor;
                break;
            case 3:
                //GetComponent<Image>().color = Color.magenta;
                GetComponent<Image>().color = playerColors.playerColorList[3];
                GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, -0.707f, 0.707f);
                GetComponent<RectTransform>().localScale = scale * m_CanvasScaler.scaleFactor;
                break;
            default:
                GetComponent<Image>().color = Color.cyan;
                break;
        }
    }
}
