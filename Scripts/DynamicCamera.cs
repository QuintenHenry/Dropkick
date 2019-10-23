using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour {

    [SerializeField] float m_MinDistance;
    [SerializeField] float m_MaxDistance;
    [SerializeField] float m_Speed;
    [SerializeField] float m_ZoomSpeed;
    [SerializeField] float m_IgnorePlayeronY = -1.0f;
    [SerializeField] Vector3 m_MiddleofField;
    private bool m_WithMiddle = true;
    private List<Transform> m_PlayerList = new List<Transform>();
    private Vector3 m_StartPosition;

    Vector3 m_Middlepos = Vector3.zero;

    Vector3 m_Min, m_Max;

    //camerashake
    float m_ShakeIntensity=0.5f;
    float m_CurrentShake = 0.0f;
    float m_DecreaseFactor = 1.0f;


    private void Start()
    {
        m_StartPosition = transform.position;
    }
    public void AddPlayer(Transform player) {
        m_PlayerList.Add(player);
    }
    // Update is called once per frame
    void Update() {
        if (m_PlayerList.Count == 0)
            return;

        Transform minX = null;
        Transform maxX = null;

        Vector3 middle = Vector3.zero;

        Transform Furthest = null;
        int counter=0;
        while ((maxX == null || minX == null) && counter<2)
        {
            foreach (Transform player in m_PlayerList)
            {
                if (maxX == null)
                {
                    if (player.transform.position.y > m_IgnorePlayeronY)
                    {
                        if (Furthest == null)
                            Furthest = player;

                        if ((player.transform.position - m_MiddleofField).magnitude > (Furthest.transform.position - m_MiddleofField).magnitude)
                            Furthest = player;
                    }
                    else
                        counter++;
                }
                else
                {
                    if (player.transform.position.y > m_IgnorePlayeronY)
                    {

                        if (Furthest == null)
                            Furthest = player;

                        if ((player.transform.position - maxX.transform.position).magnitude > (Furthest.transform.position - maxX.transform.position).magnitude)
                            Furthest = player;
                    }
                }
            }
            if (maxX == null)
                maxX = Furthest;
            else
                minX = Furthest;
        }
        if (minX == null && maxX == null)
            return;
        m_Min = minX.position;
        m_Max = maxX.position;

        foreach (Transform player in m_PlayerList)
        {
            middle += player.position;
        }

        m_Middlepos = middle / m_PlayerList.Count;
        m_Middlepos = ((m_Middlepos*1.5f+ ((maxX.position - minX.position) / 2 + minX.position))/2);
        m_Middlepos = Camera.main.WorldToScreenPoint(m_Middlepos);

        if (m_WithMiddle)
        m_Middlepos = Camera.main.WorldToScreenPoint((maxX.position-minX.position)/2 + minX.position);

        m_Middlepos.z = 0.0f;

        Vector3 middleofscreen = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight/2);

        //Debug.Log((middleofscreen - middlepos).magnitude);


       transform.Translate((m_Middlepos - middleofscreen)/100 * Time.deltaTime * m_Speed);


        m_CurrentShake -= Time.deltaTime;

        if (m_CurrentShake > 0.0f)
            transform.localPosition = transform.localPosition+Random.insideUnitSphere * m_ShakeIntensity;
            
            //float ZoomValue = 0.0f;
            //if ((maxX.position - minX.position).magnitude < m_DistanceToZoomIn && (m_StartPosition - transform.position).magnitude<m_MinDistance)
            //    ZoomValue = 1.0f;
            //if ((maxX.position - minX.position).magnitude > m_DistanceToZoomOut && (m_StartPosition - transform.position).magnitude>m_MaxDistance)
            //    ZoomValue = -1.0f;

        Vector2 minScreen = Camera.main.WorldToScreenPoint(minX.position);
        Vector2 maxScreen = Camera.main.WorldToScreenPoint(maxX.position);
        float distance = (maxScreen - minScreen).magnitude;
        float ZoomValue = 20 + (Mathf.Clamp(distance, m_MinDistance, m_MaxDistance) - m_MinDistance)*0.1f;

        if (maxX != minX)
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,ZoomValue,Time.deltaTime*m_ZoomSpeed); 


        //transform.Translate(transform.forward * ZoomValue  * m_ZoomSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(m_MiddleofField, 1.0F);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(min, 0.5f);
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(max, 0.5f);
    }
    public void Shake(float intensity,float time) {
        m_CurrentShake = time;
        m_ShakeIntensity = intensity;
    }
}
