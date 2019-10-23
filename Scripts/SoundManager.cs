using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour {

    [SerializeField] AudioSource m_SourcePlayer1;
    [SerializeField] AudioSource m_SourcePlayer2;
    [SerializeField] AudioSource m_SourcePlayer3;
    [SerializeField] AudioSource m_SourcePlayer4;
    [SerializeField] AudioSource m_SourceBackGround;
    [SerializeField] AudioSource m_SourceEffects;
    // Use this for initialization
    void Start () {
        AudioClip clip = (AudioClip)Resources.Load("Sounds/" + "BackgroundSong");
        m_SourceBackGround.PlayOneShot(clip);
	}

    public void PlaySound(string sound,int player=-1)
    {

        AudioClip clip = (AudioClip)Resources.Load("Sounds/" + sound);

        AudioSource source=m_SourcePlayer1;
        switch (player){
            case 0:
                source = m_SourcePlayer1;
                break;
            case 1:
                source = m_SourcePlayer2;
                break;
            case 2:
                source = m_SourcePlayer3;
                break;
            case 3:
                source = m_SourcePlayer4;
                break;
            default:
                source = m_SourceEffects;
                break;
        }

        if (sound == "Whoosh")
        {
            //Debug.Log(sound);
            if (source.pitch == 1.0f)
                source.pitch = 3f;
            else
                source.pitch = 1.0f;
        }
        else
            source.pitch = 1.0f;
        source.PlayOneShot(clip);
    }
}
