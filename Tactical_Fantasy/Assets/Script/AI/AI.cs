using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI
{
    public const float DisavoidAngle45 = 30;

    public struct LastObtainedPath
    {
        public AstarMapBaking astarMap;
        public int startIndex;
        public int destiantionIndex;
        public Vector2 NextPathPos;


        public LastObtainedPath(int i)
        {
            astarMap = null;
            startIndex = -1;
            destiantionIndex = -1;
            NextPathPos = new Vector2(0, 0);
        }
        public void SetPropertys(AstarMapBaking setAstarMap, int setStartIndex, int setDestinationIndex , Vector2 setNextPathPos)
        {
            astarMap = setAstarMap;
            startIndex = setStartIndex;
            destiantionIndex = setDestinationIndex;
            NextPathPos = setNextPathPos;
        }
        public bool isSame(AstarMapBaking checkAstarMap,int checkStartIndex,int checkDestinationIndex)
        {
            return astarMap == checkAstarMap && startIndex == checkStartIndex && destiantionIndex == checkDestinationIndex;
        }
    }

    public struct HearInformation
    {
        public GameObject obj;
        public int targetID;
        public float loudness;
    }

    public static void MoveToTargetWithFrontDetect(Transform thisTransform,Transform target,ref float nowSpeed, float acceleration,float minSpeed,float maxSpeed,float turnSpeed,float detectWidth,float detectDis)
    {
        Vector2 t2t = target.position - thisTransform.position;

        if (t2t.magnitude < detectDis)
            detectDis = t2t.magnitude;

        //RaycastHit2D[] hits = Physics2D.BoxCastAll(thisTransform.position, new Vector2(detectWidth, 1), VectorToDegree(t2t), t2t, detectDis).OrderBy(h => h.distance).ToArray();
        RaycastHit2D[] hits = Physics2D.RaycastAll(thisTransform.position, t2t, detectDis).OrderBy(h => h.distance).ToArray();

        bool isObstacle = false;
        bool right = true;
        float avoidObstacleAngle = 0;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.gameObject == target.gameObject)
                break;

            if (hit.transform.GetComponent<Obstacle>() != null && hit.transform.gameObject != thisTransform.gameObject)
            {
                isObstacle = true;
                avoidObstacleAngle = Mathf.Atan(DisavoidAngle45 / Vector2.Distance(thisTransform.position, hit.point)) * Mathf.Rad2Deg;
                Vector2 t2o = hit.transform.position - thisTransform.position;
                if (Vector2.SignedAngle(thisTransform.up, t2o) < 0)
                    right = false;
                break;
            }
        }

        if (isObstacle)
        {
            if (right)
                t2t = TurnVector(t2t, -avoidObstacleAngle);
            else
                t2t = TurnVector(t2t, avoidObstacleAngle);
        }

        thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation,Quaternion.Euler(0,0, VectorToDegree(t2t)),Time.deltaTime * turnSpeed);
        nowSpeed += acceleration * Time.deltaTime;
        if (nowSpeed < minSpeed)
            nowSpeed = minSpeed;
        else if (nowSpeed > maxSpeed)
            nowSpeed = maxSpeed;

        thisTransform.Translate(0, nowSpeed * Time.deltaTime, 0);
    }

    public static void MoveToTargetWithFrontDetect(Transform thisTransform, Vector2 target, ref float nowSpeed, float acceleration, float minSpeed, float maxSpeed, float turnSpeed, float detectWidth, float detectDis)
    {
        Vector2 t2t = target - (Vector2)thisTransform.position;

        if (t2t.magnitude < detectDis)
            detectDis = t2t.magnitude;

        //RaycastHit2D[] hits = Physics2D.BoxCastAll(thisTransform.position, new Vector2(detectWidth, 1), VectorToDegree(t2t), t2t, detectDis).OrderBy(h => h.distance).ToArray();
        RaycastHit2D[] hits = Physics2D.RaycastAll(thisTransform.position, t2t, detectDis).OrderBy(h => h.distance).ToArray();


        bool isObstacle = false;
        bool right = true;
        float avoidObstacleAngle = 0;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Obstacle>() != null && hit.transform.gameObject != thisTransform.gameObject)
            {
                isObstacle = true;
                avoidObstacleAngle = Mathf.Atan(DisavoidAngle45 / Vector2.Distance(thisTransform.position, hit.point)) * Mathf.Rad2Deg;
                Vector2 t2o = hit.transform.position - thisTransform.position;
                if (Vector2.SignedAngle(thisTransform.up, t2o) < 0)
                    right = false;
                break;
            }
        }

        if (isObstacle)
        {
            if (right)
                t2t = TurnVector(t2t, -avoidObstacleAngle);
            else
                t2t = TurnVector(t2t, avoidObstacleAngle);
        }

        thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, Quaternion.Euler(0, 0, VectorToDegree(t2t)), Time.deltaTime * turnSpeed);
        nowSpeed += acceleration * Time.deltaTime;
        if (nowSpeed < minSpeed)
            nowSpeed = minSpeed;
        else if (nowSpeed > maxSpeed)
            nowSpeed = maxSpeed;

        thisTransform.Translate(0, nowSpeed * Time.deltaTime, 0);
    }

    public static void MoveToTargetWithAstar(Transform thisTransform, Vector2 target, ref float nowSpeed, float acceleration, float minSpeed, float maxSpeed, float turnSpeed, float detectWidth, float detectDis,ref LastObtainedPath lastObtainedPath)
    {
        Vector2 t2t = target - (Vector2)thisTransform.position;

        //RaycastHit2D[] firstHits = Physics2D.BoxCastAll(thisTransform.position, new Vector2(detectWidth, 1), VectorToDegree(t2t), t2t, t2t.magnitude).OrderBy(h => h.distance).ToArray();
        RaycastHit2D[] firstHits = Physics2D.RaycastAll(thisTransform.position, t2t, t2t.magnitude);


        bool isObstacle = false;

        foreach (RaycastHit2D hit in firstHits)
        {
            if (hit.transform.GetComponent<Obstacle>() != null && hit.transform.gameObject != thisTransform.gameObject)
            {
                isObstacle = true;
            }
        }

        if (!isObstacle)
        {
            MoveToTargetWithFrontDetect(thisTransform, target, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
            return;
        }



        PathNode thisPathNode = null;
        PathNode targetPathNode = null;

        RaycastHit2D[] hits = Physics2D.RaycastAll(thisTransform.position, Vector2.zero);

        foreach(RaycastHit2D hit in hits)
        {
            PathNode nowPathNode = hit.collider.GetComponent<PathNode>();
            if(nowPathNode != null)
            {
                thisPathNode = nowPathNode;
                break;
            }
        }

        hits = Physics2D.RaycastAll(target, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            PathNode nowPathNode = hit.collider.GetComponent<PathNode>();
            if (nowPathNode != null)
            {
                targetPathNode = nowPathNode;
                break;
            }
        }

        if(thisPathNode == null && targetPathNode == null)
        {
            MoveToTargetWithFrontDetect(thisTransform, target, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
        }
        else if(thisPathNode == null && targetPathNode != null)
        {
            int[] entrancesIndexs = targetPathNode.astarMap.entrances;

            Vector2[] entrancesPos = new Vector2[entrancesIndexs.Length];

            for(int i = 0; i < entrancesPos.Length; i++)
            {
                entrancesPos[i] = targetPathNode.astarMap.NodesPos[entrancesIndexs[i]];
            }

            Vector2 destination = GetClosestVector(thisTransform.position, entrancesPos);
            MoveToTargetWithFrontDetect(thisTransform, destination,ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
        }
        else if(thisPathNode != null && targetPathNode == null)
        {
            int[] entrancesIndexs = thisPathNode.astarMap.entrances;

            foreach(int nowEntrance in entrancesIndexs)
            {
                if(thisPathNode.index == nowEntrance)
                {
                    MoveToTargetWithFrontDetect(thisTransform, target, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
                    return;
                }
            }


            Vector2[] entrancesPos = new Vector2[entrancesIndexs.Length];

            for (int i = 0; i < entrancesPos.Length; i++)
            {
                entrancesPos[i] = thisPathNode.astarMap.NodesPos[entrancesIndexs[i]];
            }

            int destinationIndex = GetClosestVectorIndex(target, entrancesPos);

            int destinationNodeIndex = entrancesIndexs[destinationIndex];

            if(lastObtainedPath.isSame(thisPathNode.astarMap, thisPathNode.index, destinationNodeIndex))
            {
                MoveToTargetWithFrontDetect(thisTransform, lastObtainedPath.NextPathPos, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
            }
            else
            {
                Vector2[] paths = thisPathNode.astarMap.FindPath(thisPathNode.index, destinationNodeIndex);
                if(paths.Length > 1)
                {
                    Vector2 resultDestination = paths[1];
                    lastObtainedPath.SetPropertys(thisPathNode.astarMap, thisPathNode.index, destinationNodeIndex, resultDestination);
                    MoveToTargetWithFrontDetect(thisTransform, lastObtainedPath.NextPathPos, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
                }

            }


        }
        else if(thisPathNode != null && targetPathNode != null)
        {
            if(thisPathNode.astarMap == targetPathNode.astarMap)
            {
                if(thisPathNode.index == targetPathNode.index)
                {
                    MoveToTargetWithFrontDetect(thisTransform, target, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
                }
                else
                {
                    if (lastObtainedPath.isSame(thisPathNode.astarMap, thisPathNode.index, targetPathNode.index))
                    {
                        MoveToTargetWithFrontDetect(thisTransform, lastObtainedPath.NextPathPos, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
                    }
                    else
                    {
                        Vector2[] paths = thisPathNode.astarMap.FindPath(thisPathNode.index, targetPathNode.index);
                        if(paths.Length > 1)
                        {
                            Vector2 resultDestination = paths[1];
                            lastObtainedPath.SetPropertys(thisPathNode.astarMap, thisPathNode.index, targetPathNode.index, resultDestination);
                            MoveToTargetWithFrontDetect(thisTransform, lastObtainedPath.NextPathPos, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
                        }

                    }

                }

            }
            else
            {

                int[] entrancesIndexs = thisPathNode.astarMap.entrances;

                foreach (int nowEntrance in entrancesIndexs)
                {
                    if (thisPathNode.index == nowEntrance)
                    {
                        int[] targetEntrancesIndexs = targetPathNode.astarMap.entrances;

                        Vector2[] targetEntrancesPos = new Vector2[targetEntrancesIndexs.Length];

                        for (int i = 0; i < targetEntrancesPos.Length; i++)
                        {
                            targetEntrancesPos[i] = targetPathNode.astarMap.NodesPos[targetEntrancesIndexs[i]];
                        }

                        Vector2 destination = GetClosestVector(thisTransform.position, targetEntrancesPos);
                        MoveToTargetWithFrontDetect(thisTransform, destination, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);

                        return;
                    }
                }

                Vector2[] entrancesPos = new Vector2[entrancesIndexs.Length];

                for (int i = 0; i < entrancesPos.Length; i++)
                {
                    entrancesPos[i] = thisPathNode.astarMap.NodesPos[entrancesIndexs[i]];
                }

                int destinationIndex = GetClosestVectorIndex(target, entrancesPos);

                int destinationNodeIndex = entrancesIndexs[destinationIndex];

                if (lastObtainedPath.isSame(thisPathNode.astarMap, thisPathNode.index, destinationNodeIndex))
                {
                    MoveToTargetWithFrontDetect(thisTransform, lastObtainedPath.NextPathPos, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);
                }
                else
                {
                    Vector2[] paths = thisPathNode.astarMap.FindPath(thisPathNode.index, destinationNodeIndex);
                    if (paths.Length > 1)
                    {
                        Vector2 resultDestination = paths[1];
                        lastObtainedPath.SetPropertys(thisPathNode.astarMap, thisPathNode.index, destinationNodeIndex, resultDestination);
                        MoveToTargetWithFrontDetect(thisTransform, lastObtainedPath.NextPathPos, ref nowSpeed, acceleration, minSpeed, maxSpeed, turnSpeed, detectWidth, detectDis);                        
                    }

                }

            }



        }



    }


    public static float CheckSightSensor(Transform thisTransform,Transform target,float sightDis,float sightAngle) // 감지된 거리 리턴, -1은 미감지
    {
        float t2tDis = Vector2.Distance(thisTransform.position, target.position);

        if (t2tDis > sightDis)
            return -1;

        Vector2 t2t = target.position - thisTransform.position;

        if (Vector2.Angle(thisTransform.up, t2t) > sightAngle / 2)
            return -1;

        RaycastHit2D[] hits = Physics2D.RaycastAll(thisTransform.position, t2t, t2tDis + 1).OrderBy(h => h.distance).ToArray();       

        foreach(RaycastHit2D hit in hits)
        {
            if(hit.transform.gameObject == target.gameObject)
                return t2tDis;

            Obstacle obs = hit.transform.GetComponent<Obstacle>();

            if (hit.transform.gameObject != thisTransform.gameObject && obs != null && obs.isBlind)
                return -1;

        }

        return -1;


    }

    public static void MakeSound(GameObject obj,int targetID,Vector2 pos,float loudness)
    {
        HearInformation hearInformation = new HearInformation();
        hearInformation.obj = obj;
        hearInformation.targetID = targetID;
        hearInformation.loudness = loudness;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(pos, loudness * 10, Vector2.zero, 0);

        foreach(RaycastHit2D hit in hits)
        {
            SoundSensor soundSensor = hit.transform.GetComponent<SoundSensor>();

            if(soundSensor != null)
            {
                hearInformation.loudness = (1 - (Vector2.Distance(pos, hit.transform.position) / (loudness * 10))) * loudness;
                if (hearInformation.loudness < 0)
                    hearInformation.loudness = 0;
                soundSensor.Hear(hearInformation);
            }

        }

    }

    public static GameObject CreateDead(GameObject deadobj,Vector3 pos,float angle,Vector2 velocity,float movetime)
    {
        GameObject dead = GameObject.Instantiate(deadobj);
        dead.transform.position = pos;
        dead.transform.rotation = Quaternion.Euler(0, 0, angle);
        dead.GetComponent<Rigidbody2D>().velocity = velocity;
        dead.GetComponent<Dead>().moveTime = movetime;

        return dead;
    }

    public static GameObject CreateDead(GameObject deadobj, Vector3 pos, Vector2 direct, Vector2 velocity, float movetime)
    {
        GameObject dead = GameObject.Instantiate(deadobj);
        dead.transform.position = pos;
        dead.transform.rotation = Quaternion.Euler(0, 0, AI.VectorToDegree(direct));
        dead.GetComponent<Rigidbody2D>().velocity = velocity;
        dead.GetComponent<Dead>().moveTime = movetime;

        return dead;
    }

    public static GameObject CreateDead(GameObject deadobj, Vector3 pos, Vector2 direct, Vector2 velocity, float movetime,bool isHitWeaknees,AudioClip hitSound,AudioClip lickHitSound,AudioClip DeadSound)
    {
        GameObject result = CreateDead(deadobj, pos, direct, velocity, movetime);
        if (isHitWeaknees)
            result.transform.Find("Hit").GetComponent<AudioSource>().clip = hitSound;
        else
            result.transform.Find("Hit").GetComponent<AudioSource>().clip = lickHitSound;

        result.GetComponent<AudioSource>().clip = DeadSound;

        result.GetComponent<AudioSource>().Play();
        result.transform.Find("Hit").GetComponent<AudioSource>().Play();

        return result;
    }

    public static void IncreaseDetectWithSight(ref float detect,float plus,float detectDis,float sightDis)
    {
        detect += plus * (1 - (detectDis / sightDis)) * Time.deltaTime;
        if (detect > 1)
            detect = 1;
    }

    public static void DecreaseDetect(ref float detect,float SubPerSecond,float coolTime,float suspicion,float battle)
    {
        float beforeDetect = detect;
        detect -= SubPerSecond * Time.deltaTime;
        if (detect < 0)
            detect = 0;

        if (beforeDetect >= battle && detect < battle && coolTime > 0)
            detect = battle;
        else if (beforeDetect >= suspicion && detect < suspicion && coolTime > 0)
            detect = suspicion;
    }


    public static bool DecreaseCool(ref float cool)
    {
        if (cool > 0)
        {
            cool -= Time.deltaTime;
            return false;
        }
        else
            return true;
    }

    public static bool CheckStop(ref float cool,float maxCool,Transform transform,ref Vector2 beforePos,float checkDis)
    {
        if (AI.DecreaseCool(ref cool))
        {
            bool result = ((Vector2)transform.position - beforePos).magnitude < checkDis;

            beforePos = transform.position;
            cool = maxCool;

            return result;
        }
        else
            return false;
    }

    public static bool SetRandomDestination(ref Vector2 OriginPos,float xRange,float yRange)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 newPos = OriginPos + new Vector2(Random.Range(-xRange/2, xRange/2), Random.Range(-yRange / 2, yRange / 2));
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
                OriginPos = newPos;
                return true;
            }
        }
        return false;
    }

    public static Vector2 TurnVector(Vector2 vector,float angle)
    {
        Vector2 result = new Vector2();

        result.x = Mathf.Cos(angle * Mathf.Deg2Rad) * vector.x - Mathf.Sin(angle * Mathf.Deg2Rad) * vector.y;
        result.y = Mathf.Sin(angle * Mathf.Deg2Rad) * vector.x + Mathf.Cos(angle * Mathf.Deg2Rad) * vector.y;

        return result;
    }

    public static float VectorToDegree(Vector2 vector)
    {
       return Vector2.SignedAngle(new Vector2(0, 1), vector);
    }

    public static Vector2 GetClosestVector(Vector2 start, Vector2[] targets)
    {
        float closestDis = Vector2.Distance(start ,targets[0]);
        Vector2 result = targets[0];

        foreach(Vector2 target in targets)
        {
            float nowDis = Vector2.Distance(start, target);
            if(nowDis < closestDis)
            {
                closestDis = nowDis;
                result = target;
            }

        }

        return result;

    }

    public static int GetClosestVectorIndex(Vector2 start, Vector2[] targets)
    {
        float closestDis = Vector2.Distance(start, targets[0]);
        int result = 0;


        for (int i = 1; i < targets.Length; i++)
        {
            float nowDis = Vector2.Distance(start, targets[i]);
            if (nowDis < closestDis)
            {
                closestDis = nowDis;
                result = i;
            }
        }
        

        return result;

    }

    public static Vector2 GetVectorRangeLimit(Vector2 setVector,Vector2 xRange,Vector2 yRange)
    {
        if(xRange != new Vector2(0, 0))
        {
            setVector.x = Mathf.Clamp(setVector.x, xRange.x, xRange.y);
        }

        if(yRange != new Vector2(0, 0))
        {
            setVector.y = Mathf.Clamp(setVector.y, yRange.x, yRange.y);
        }

        return setVector;
    }

}
