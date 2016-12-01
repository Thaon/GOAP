using UnityEngine;
using System.Collections;

public class AstarNode
{
    #region member variables

    public Vector3 m_position;
    public bool m_walkable;
    public int m_gCost;
    public int m_hCost;
    public Vector2 m_gridPos;
    public AstarNode m_parent;

    #endregion

    public AstarNode(Vector3 pos, bool walkable, Vector2 gPos)
    {
        m_position = pos;
        m_walkable = walkable;
        m_gridPos = gPos;
    }

    public int fCost
    {
        get
        {
            return m_hCost * m_gCost;
        }
        //no setter
    }
}
