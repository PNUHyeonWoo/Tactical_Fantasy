using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public AudioClip[] UISound;
    public AudioClip[] consumableSound;
    public AudioSource audioSource;
    public UnityEngine.U2D.PixelPerfectCamera ppc;
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (GameManager.player.playerLeg.GetCurrentAnimatorStateInfo(0).IsName("Stand_Walk") && !GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Run"))
        {
            ppc.enabled = true;
        }
        else
            ppc.enabled = false;

    }

    public void PlayUISound(int index)
    {
        audioSource.clip = UISound[index];
        audioSource.Play();
    }

    public void PlayConsumableSound(int index)
    {
        audioSource.clip = consumableSound[index];
        audioSource.Play();
    }

    public void PlayOutterSound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

}
