using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRectArrangement : MonoBehaviour
{
    public Vector2 topLeft;
    public Vector2 bottomRight;

    public Transform[] singleObjs;    
    [System.Serializable]
    public struct GroupObj
    {
        public Transform[] group;
    }
    public GroupObj[] groupObjs;

    public void Awake()
    {
        foreach(Transform nowTransform in singleObjs)
        {
            Vector2 nowPos = GetRandomPos();
            nowTransform.position = new Vector3(nowPos.x, nowPos.y, nowTransform.position.z);
        }

        foreach(GroupObj nowGroup in groupObjs)
        {
            Vector2 nowPos = GetRandomPos();
            foreach(Transform nowTransform in nowGroup.group)
            {
                nowTransform.position = new Vector3(nowPos.x, nowPos.y, nowTransform.position.z);
            }
        }

        Destroy(gameObject);
    }

    public Vector2 GetRandomPos()
    {
        for (int i = 0; i < 1000; i++)
        {
            Vector2 newPos = new Vector2(Random.Range(topLeft.x, bottomRight.x), Random.Range(bottomRight.y, topLeft.y));
            RaycastHit2D[] rayhit = Physics2D.RaycastAll(newPos, Vector2.zero);

            bool isObstacle = false;

            foreach (RaycastHit2D hit in rayhit)
            {
                if (hit.transform.GetComponent<Obstacle>() != null)
                {
                    isObstacle = true;
                    break;
                }
            }

            if (!isObstacle)
            {
                return newPos;
            }
        }
        return (topLeft + bottomRight)/2;

    }

}
