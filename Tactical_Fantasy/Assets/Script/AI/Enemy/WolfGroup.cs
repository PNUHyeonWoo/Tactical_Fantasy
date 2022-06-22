using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfGroup : MonoBehaviour
{
    public List<Wolf> wolfs = new List<Wolf>();
    public Vector2 lastPlayerPosition = new Vector2();

    public Vector2 xRange = new Vector2(0,0);
    public Vector2 yRange = new Vector2(0,0);
    void Start()
    {
        lastPlayerPosition = transform.position;
        foreach (Wolf wolf in wolfs)
            wolf.group = this;
    }

    // Update is called once per frame
    void Update()
    {
        List<Wolf> removeWolfs = new List<Wolf>();
        foreach(Wolf wolf in wolfs)
        {
            if (wolf == null || !wolf.gameObject)
                removeWolfs.Add(wolf);
        }
        foreach(Wolf wolf in removeWolfs)
            wolfs.Remove(wolf);

        if (wolfs.Count <= 0)
        {
            Destroy(gameObject);
            return;
        }

        float maxDetect = 0;
        bool maxCool = false;

        foreach(Wolf wolf in wolfs)
        {
            if (wolf.detectPlayer > maxDetect)
                maxDetect = wolf.detectPlayer;
            if (wolf.detectCool > 9.5f)
                maxCool = true;
        }
        if(maxDetect > 0)
        {
            foreach (Wolf wolf in wolfs)
            {
                wolf.detectPlayer = maxDetect;
                if (maxCool)
                    wolf.detectCool = 9.5f;
            }
        }

        if (maxDetect < Wolf.suspicion)
        {
            lastPlayerPosition = AI.GetVectorRangeLimit(lastPlayerPosition, xRange, yRange);

            foreach (Wolf wolf in wolfs)
            {
                if (wolf.lastPlayerPosition != lastPlayerPosition)
                {
                    AI.SetRandomDestination(ref lastPlayerPosition, 200, 200);
                    lastPlayerPosition = AI.GetVectorRangeLimit(lastPlayerPosition, xRange, yRange);

                    foreach (Wolf wolf2 in wolfs)
                        wolf2.lastPlayerPosition = lastPlayerPosition;

                    break;
                }

            }

        }
        else
        {
            foreach (Wolf wolf in wolfs)
            {
               if(Vector2.Distance(wolf.lastPlayerPosition,GameManager.player.transform.position) < 200 * Time.deltaTime)
               {
                    lastPlayerPosition = wolf.lastPlayerPosition;
                    foreach (Wolf wolf2 in wolfs)
                        wolf2.lastPlayerPosition = lastPlayerPosition;
                    break;
               }

            }

        }
            
    }

    public void setDetect()
    {
        foreach (Wolf wolf in wolfs)
        {
            wolf.detectPlayer = 1;
            wolf.detectCool = 10;
            wolf.lastPlayerPosition = GameManager.player.transform.position;
        }
        lastPlayerPosition = GameManager.player.transform.position;
    }
}
