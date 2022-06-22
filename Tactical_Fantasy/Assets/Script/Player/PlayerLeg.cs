using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerLeg : MonoBehaviour
{
    Transform leftFoot;
    Transform rightFoot;

    [System.Serializable]
    public struct footStepClips
    {
        public int loudness;
        public AudioClip[] clips;
    }

    public footStepClips[] footStepSounds;
    AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        leftFoot = transform.Find("LeftFoot");
        rightFoot = transform.Find("RightFoot");
    }

    public void PlayFootStepSound(int isLeft)
    {
        Vector2 footPos = new Vector2(0, 0);
        if (isLeft == 1)
            footPos = leftFoot.position;
        else
            footPos = rightFoot.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(footPos, Vector2.zero).OrderBy(h => h.transform.position.z).ToArray();

        for(int i = 0; i < hits.Length;i++)
        {
            FootFloor footfloor = hits[i].transform.GetComponent<FootFloor>();

            if(footfloor != null)
            {
                audioSource.clip = footStepSounds[footfloor.floorSoundCode].clips[footfloor.nowPlayIndex];
                audioSource.Play();
                if (footfloor.nowPlayIndex >= footStepSounds[footfloor.floorSoundCode].clips.Length - 1)
                    footfloor.nowPlayIndex = 0;
                else
                    footfloor.nowPlayIndex++;
                GameManager.player.MakeSound(GameManager.player.nowSpeed / 10 * footStepSounds[footfloor.floorSoundCode].loudness);
                break;
            }
        }

    }


}
