using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AstarGrid : MonoBehaviour {

    #region member variables

    private AstarNode[,] m_grid;
    private int m_sizeX, m_sizeY;

    public Vector2 m_size;
    public float m_nodeRadius;
    public LayerMask m_unwalkableMask;
    public LayerMask m_walkableMask;
    public Transform m_player;
    public Transform endNode;
    public bool m_debugDraw;
    public List<AstarNode> m_testPath;

    #endregion

    void Start()
    {
        RecalculateGrid();
    }

    public void RecalculateGrid()
    {
        int diam = Mathf.RoundToInt(m_nodeRadius * 2);
        m_sizeX = Mathf.RoundToInt(m_size.x / diam);
        m_sizeY = Mathf.RoundToInt(m_size.y / diam);
        CreateGrid();
    }

    void CreateGrid()
    {
        m_grid = new AstarNode[m_sizeX, m_sizeY];

        Vector3 bottomLeft = transform.position - (Vector3.right * m_size.x / 2) - (Vector3.forward * m_size.y / 2);

        for (int x = 0; x < m_sizeX; x++)
        {
            for (int y = 0; y < m_sizeY; y++)
            {
                Vector3 cell = bottomLeft + Vector3.right * (x * m_nodeRadius * 2 + m_nodeRadius) + Vector3.forward * (y * m_nodeRadius * 2 + m_nodeRadius);
                //check if can walk
                bool walk = (Physics.CheckSphere(cell, m_nodeRadius, m_unwalkableMask)) ? false : true;
                walk = Physics.CheckSphere(cell, m_nodeRadius, m_walkableMask) ? walk : false;
                m_grid[x, y] = new AstarNode(cell, walk, new Vector2(x,y));
            }
        }
    }

    public AstarNode GetNodeFromPosition(Vector3 pos)
    {
        float percentX = (pos.x + m_size.x / 2) / m_size.x;
        float percentY = (pos.z + m_size.y / 2) / m_size.y; //keep in mind the 3D to 2D transition!!!
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((m_sizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_sizeY - 1) * percentY);
        return m_grid[x, y];
    }

    public List<AstarNode> GetNeighbours(AstarNode node)
    {
        List<AstarNode> neighbours = new List<AstarNode>();
        int nx = Mathf.RoundToInt(node.m_gridPos.x);
        int ny = Mathf.RoundToInt(node.m_gridPos.y);

        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                else
                {
                    int checkX = nx + x;
                    int checkY = ny + y;

                    if (checkX >= 0 && checkX < m_sizeX && checkY >=0 && checkY < m_sizeY)
                    {
                        neighbours.Add(m_grid[checkX, checkY]);
                    }
                }
            }

        return neighbours;
    }

    void Update()
    {
        //m_testPath = GetComponent<AStarPathfinder>().FindPath(m_player.transform.position, endNode.transform.position);
    }

    void OnDrawGizmos()
    {
        if (m_debugDraw)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(m_size.x, 1, m_size.y));
            if (m_grid != null)
            {
                AstarNode playerNode = GetNodeFromPosition(m_player.transform.position);
                foreach (AstarNode n in m_grid)
                {
                    Gizmos.color = n.m_walkable ? Color.white : Color.red;
                    if (m_testPath != null)
                        if (m_testPath.Contains(n))
                            Gizmos.color = Color.blue;
                    if (n == playerNode) Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(n.m_position, Vector3.one * (m_nodeRadius * 2 - .1f));
                }
            }
        }
    }
}
