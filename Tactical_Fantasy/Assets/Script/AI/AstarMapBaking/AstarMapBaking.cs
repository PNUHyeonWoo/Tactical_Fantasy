using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarMapBaking : MonoBehaviour
{
    public int[] entrances;
    [Header("Don't Touch Bottom")]
    public Vector2[] NodesPos;
    public NodeConnect[] NodeConnects;

    [System.Serializable]
    public struct NodeConnect
    {
        public int[] connect;
    }

    [System.Serializable]
    public struct AstarNode
    {
        public int index;
        public bool isClose;
        public float startToThis;
        public float totalDis;
        public int parentIndex;
    }

    public AstarNode[] astarNodes;

    const float tolerance = 0.001f;

    public void Start()
    {
        Collider2D[] collider2Ds = GetComponents<Collider2D>();
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();

        if(collider2Ds.Length > 0)
        {
            foreach (Collider2D nowCollider in collider2Ds)
            {
                Destroy(nowCollider);
            }

        }

        if (rigid != null)
            Destroy(rigid);

    }


    public void Baking()
    {
        Mesh com = GetComponent<CompositeCollider2D>().CreateMesh(false,false);
        Vector3[] vertices = com.vertices;
        int[] indexList = com.triangles;

        GameObject parentOBJ = new GameObject("PathMesh");
        parentOBJ.transform.SetParent(transform);
        parentOBJ.transform.localPosition = Vector3.zero;

        NodesPos = new Vector2[indexList.Length / 3];
        NodeConnects = new NodeConnect[indexList.Length / 3];
        astarNodes = new AstarNode[indexList.Length / 3];

        for(int i = 0; i < indexList.Length;i += 3)
        {
            GameObject pathNodeObj = new GameObject("PathNode_" + (i/3).ToString());
            pathNodeObj.transform.SetParent(parentOBJ.transform);
            pathNodeObj.transform.localPosition = Vector3.zero;

            PolygonCollider2D pC2D = pathNodeObj.AddComponent<PolygonCollider2D>();
            pC2D.isTrigger = true;
            pC2D.SetPath(0, new Vector2[3] { vertices[indexList[i]], vertices[indexList[i + 1]], vertices[indexList[i + 2]] });

            PathNode pathNodeScr = pathNodeObj.AddComponent<PathNode>();
            pathNodeScr.astarMap = this;
            pathNodeScr.index = i / 3;

            NodesPos[pathNodeScr.index] = new Vector2((vertices[indexList[i]].x + vertices[indexList[i + 1]].x + vertices[indexList[i + 2]].x) / 3, (vertices[indexList[i]].y + vertices[indexList[i + 1]].y + vertices[indexList[i + 2]].y) / 3) + new Vector2(transform.position.x,transform.position.y);
            List<int> neighbors = new List<int>();

            for (int j = 0; j < indexList.Length; j += 3)
            {
                if (j == i)
                    continue;

                if(isOverlapLine(vertices[indexList[i]] , vertices[indexList[i+1]], vertices[indexList[j]], vertices[indexList[j+1]]))                
                    neighbors.Add(j/3);
                else if (isOverlapLine(vertices[indexList[i]], vertices[indexList[i + 1]], vertices[indexList[j+1]], vertices[indexList[j + 2]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i]], vertices[indexList[i + 1]], vertices[indexList[j + 2]], vertices[indexList[j]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i+1]], vertices[indexList[i + 2]], vertices[indexList[j]], vertices[indexList[j + 1]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i+1]], vertices[indexList[i + 2]], vertices[indexList[j + 1]], vertices[indexList[j + 2]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i+1]], vertices[indexList[i + 2]], vertices[indexList[j + 2]], vertices[indexList[j]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i+2]], vertices[indexList[i]], vertices[indexList[j]], vertices[indexList[j + 1]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i+2]], vertices[indexList[i]], vertices[indexList[j + 1]], vertices[indexList[j + 2]]))
                    neighbors.Add(j / 3);
                else if (isOverlapLine(vertices[indexList[i+2]], vertices[indexList[i]], vertices[indexList[j + 2]], vertices[indexList[j]]))
                    neighbors.Add(j / 3);


            }

            NodeConnects[pathNodeScr.index].connect = neighbors.ToArray();
        }
    }

    public bool isOverlapLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        float angle = Vector2.Angle((a1 - a2), (b1 - b2));

        if (angle == 0 || angle == 180)
        {

            float dis = Mathf.Abs((a2.x - a1.x) * (a1.y - b1.y) - (a1.x - b1.x) * (a2.y - a1.y)) / Mathf.Sqrt((a2.x - a1.x) * (a2.x - a1.x) + (a2.y - a1.y) * (a2.y - a1.y));


            if (dis < tolerance)
            {
                if ((Vector2.Distance(a1, b1) < tolerance && Vector2.Distance(a2, b2) < tolerance) || (Vector2.Distance(a1, b2) < tolerance && Vector2.Distance(a2, b1) < tolerance))
                    return true;


                float minX;
                float maxX;
                if(a1.x < a2.x)
                {
                    minX = a1.x;
                    maxX = a2.x;
                }
                else
                {
                    minX = a2.x;
                    maxX = a1.x;
                }

                if (Mathf.Abs(a1.x - a2.x) < tolerance)
                {

                    if (Vector2.Distance(a1, b1) < tolerance)
                    {
                        if (b1.y < b2.y && a2.y < a1.y)
                            return false;

                        if (b1.y > b2.y && a1.y < a2.y)
                            return false;
                    }
                    else if (Vector2.Distance(a1, b2) < tolerance)
                    {
                        if (b1.y > b2.y && a2.y < a1.y)
                            return false;

                        if (b1.y < b2.y && a1.y < a2.y)
                            return false;
                    }
                    else if (Vector2.Distance(a2, b1) < tolerance)
                    {
                        if (b1.y < b2.y && a2.y > a1.y)
                            return false;

                        if (b1.y > b2.y && a1.y > a2.y)
                            return false;
                    }
                    else if (Vector2.Distance(a2, b2) < tolerance)
                    {
                        if (b1.y > b2.y && a2.y > a1.y)
                            return false;

                        if (b1.y < b2.y && a1.y > a2.y)
                            return false;
                    }


                    if (a1.y < a2.y)
                    {
                        minX = a1.y;
                        maxX = a2.y;
                    }
                    else
                    {
                        minX = a2.y;
                        maxX = a1.y;
                    }

                    if (minX - tolerance <= b1.y && b1.y <= maxX + tolerance)
                        return true;

                    if (minX - tolerance <= b2.y && b2.y <= maxX + tolerance)
                        return true;

                    if (b1.y <= minX + tolerance && maxX <= b2.y - tolerance)
                        return true;

                    if (b2.y <= minX + tolerance && maxX <= b1.y - tolerance)
                        return true;
                }
                else
                {
                    if(Vector2.Distance(a1, b1) < tolerance)
                    {
                        if (b1.x < b2.x && a2.x < a1.x)
                            return false;

                        if (b1.x > b2.x && a1.x < a2.x)
                            return false;
                    }
                    else if(Vector2.Distance(a1, b2) < tolerance)
                    {
                        if (b1.x > b2.x && a2.x < a1.x)
                            return false;

                        if (b1.x < b2.x && a1.x < a2.x)
                            return false;
                    }
                    else if (Vector2.Distance(a2, b1) < tolerance)
                    {
                        if (b1.x < b2.x && a2.x > a1.x)
                            return false;

                        if (b1.x > b2.x && a1.x > a2.x)
                            return false;
                    }
                    else if (Vector2.Distance(a2, b2) < tolerance)
                    {
                        if (b1.x > b2.x && a2.x > a1.x)
                            return false;

                        if (b1.x < b2.x && a1.x > a2.x)
                            return false;
                    }


                    if (minX - tolerance <= b1.x && b1.x <= maxX + tolerance)
                        return true;

                    if (minX - tolerance <= b2.x && b2.x <= maxX + tolerance)
                        return true;

                    if (b1.x <= minX + tolerance && maxX <= b2.x - tolerance)
                        return true;

                    if (b2.x <= minX + tolerance && maxX <= b1.x - tolerance)
                        return true;
                }

                return false;

            }
            else
                return false;

        }
        else
            return false;
    }


    public Vector2[] FindPath(int startIndex,int destinateIndex)
    {
        for (int i = 0; i < astarNodes.Length; i++)
        {
            astarNodes[i] = new AstarNode();
            astarNodes[i].index = i;
        }

        astarNodes[startIndex].startToThis = 0;
        astarNodes[startIndex].parentIndex = -1;

        AstarNodePriorityQueue openList = new AstarNodePriorityQueue(astarNodes[startIndex]);

        while(openList.nodes.Count > 0)
        {
            AstarNode astarNode = openList.GetNode();
            astarNodes[astarNode.index].parentIndex = astarNode.parentIndex;

            if(astarNode.index == destinateIndex)
            {
                List<int> result = new List<int>();
                int rootIndex = astarNode.index;
                while (rootIndex != -1)
                {
                    result.Add(rootIndex);
                    rootIndex = astarNodes[rootIndex].parentIndex;
                }

                result.Reverse();

                Vector2[] resultVector = new Vector2[result.Count];

                for(int j = 0; j < result.Count; j++)
                {
                    resultVector[j] = NodesPos[result[j]];
                }

                return resultVector;

            }

            int[] neighbors = NodeConnects[astarNode.index].connect;

            for(int i = 0; i < neighbors.Length; i++)
            {
                if(!astarNodes[neighbors[i]].isClose)
                {
                    astarNodes[neighbors[i]].startToThis = astarNode.startToThis + Vector2.Distance(NodesPos[astarNode.index], NodesPos[neighbors[i]]);
                    astarNodes[neighbors[i]].totalDis = astarNodes[neighbors[i]].startToThis + Vector2.Distance(NodesPos[neighbors[i]], NodesPos[destinateIndex]);
                    astarNodes[neighbors[i]].parentIndex = astarNode.index;
                    openList.Add(astarNodes[neighbors[i]]);
                }
            }

            astarNodes[astarNode.index].isClose = true;
        }
        return new Vector2[0];

    }


    public struct AstarNodePriorityQueue
    {
        public LinkedList<AstarNode> nodes;

        public AstarNodePriorityQueue(AstarNode node)
        {
            nodes = new LinkedList<AstarNode>();
            nodes.AddFirst(node);
        }

        public void Add(AstarNode node)
        {

            LinkedListNode<AstarNode> cNode = nodes.First;

            bool isAdd = false;

            while (cNode != null)
            {
                if (node.totalDis < cNode.Value.totalDis)
                {
                    nodes.AddBefore(cNode, node);
                    isAdd = true;
                    break;
                }

                cNode = cNode.Next;
            }

            if (!isAdd)
                nodes.AddLast(node);

        }

        public AstarNode GetNode()
        {
            AstarNode result = nodes.First.Value;
            nodes.RemoveFirst();
            return result;
        }


    }

}
