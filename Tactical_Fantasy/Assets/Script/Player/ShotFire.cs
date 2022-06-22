using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotFire : MonoBehaviour
{
    public float life = 0.2f;
    public int direct;

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);

        if(direct == -1 && !GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Stand_Aim") && !GameManager.player.isAniStandFire())
            Destroy(gameObject);
        else if(direct == 0 && !(GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim")&& GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0) && !GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_PFire"))
            Destroy(gameObject);
        else if (direct == 1 && !(GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.25f) && !GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_LFire"))
            Destroy(gameObject);
        else if (direct == 2 && !(GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.5f) && !GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_BFire"))
            Destroy(gameObject);
        else if (direct == 3 && !(GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_Aim") && GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).normalizedTime == 0.75f) && !GameManager.player.playerBody.GetCurrentAnimatorStateInfo(0).IsTag("Prone_RFire"))
            Destroy(gameObject);
    }
}
