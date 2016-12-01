using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AstarGrid))]
public class AStarPathfinder : MonoBehaviour {

    #region member variables

    AstarGrid m_grid;

    #endregion

    void Start()
    {
        m_grid = GetComponent<AstarGrid>();
    }

    public List<AstarNode> FindPath(Vector3 start, Vector3 end)
    {
        AstarNode startnode = m_grid.GetNodeFromPosition(start);
        AstarNode endNode = m_grid.GetNodeFromPosition(end);

        List<AstarNode> openSet = new List<AstarNode>();
        List<AstarNode> closedSet = new List<AstarNode>();

        openSet.Add(startnode);

        while (openSet.Count > 0)
        {
            AstarNode current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost || openSet[i].fCost == current.fCost && openSet[i].m_hCost < current.m_hCost)
                    current = openSet[i];
            }
            openSet.Remove(current);
            closedSet.Add(current);

            if (current == endNode)
            {
                //retrace steps to beginning
                List<AstarNode> finalPath = new List<AstarNode>();
                AstarNode currentNode = endNode;
                while (currentNode != startnode)
                {
                    finalPath.Add(currentNode);
                    currentNode = currentNode.m_parent;
                }
                finalPath.Reverse();
                m_grid.m_testPath = finalPath;
                print("found!");
                return finalPath;
            }

            foreach (AstarNode neighbour in m_grid.GetNeighbours(current))
            {
                if (!neighbour.m_walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int moveCostToNeighbour = current.m_gCost + GetDistance(current, neighbour);
                if (moveCostToNeighbour < neighbour.m_gCost || !openSet.Contains(neighbour))
                {
                    neighbour.m_gCost = moveCostToNeighbour;
                    neighbour.m_hCost = GetDistance(neighbour, endNode);
                    neighbour.m_parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        print("no findino pathino...");
        return null;
    }

    /// <summary>
    /// Returns the distance between two nodes, to be used in FindPath
    /// </summary>
    /// <param name="nodea">first node</param>
    /// <param name="nodeb">second node</param>
    /// <returns></returns>
    int GetDistance(AstarNode nodea, AstarNode nodeb)
    {
        int ax, ay, bx, by;
        ax = Mathf.RoundToInt(nodea.m_gridPos.x);
        ay = Mathf.RoundToInt(nodea.m_gridPos.y);
        bx = Mathf.RoundToInt(nodeb.m_gridPos.x);
        by = Mathf.RoundToInt(nodeb.m_gridPos.y);

        int distX = Mathf.Abs(ax - bx);
        int distY = Mathf.Abs(ay - by);
        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
