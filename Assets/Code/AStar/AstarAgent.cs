using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AstarAgent : MonoBehaviour {

    #region member variables

    public GameObject m_target;
    public float m_speed;
    public float velocity
    {
        get
        {
            return _vel * 10;
        }
    }

    private AStarPathfinder m_finder;
    private bool m_moving = true;
    private Queue<AstarNode> m_path;
    private AstarNode currentTarget;
    private float _vel;

    #endregion

    void Start ()
    {
        m_finder = FindObjectOfType<AStarPathfinder>();
        m_path = new Queue<AstarNode>();
	}
	
	void Update ()
    {
	    if (m_moving && currentTarget != null)
        {
            _vel = m_speed * Time.deltaTime;
            if (transform.position != currentTarget.m_position)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.m_position, _vel);
                transform.LookAt(currentTarget.m_position);
            }
            else
            {
                currentTarget = GetNextNode();
            }
        }
        else
        {
            _vel = 0;
        }
	}

    AstarNode GetNextNode()
    {
        if (m_path.Count > 0)
        {
            return m_path.Dequeue();
        }
        else
            return null;
    }

    public void SetPath(GameObject target)
    {
        if (target != m_target)
        {
            m_target = target;
            List<AstarNode> pathlist = m_finder.FindPath(transform.position, m_target.transform.position);
            foreach (AstarNode node in pathlist)
            {
                m_path.Enqueue(node);
            }
            currentTarget = GetNextNode();
        }
    }

    public void Stop()
    {
        m_moving = false;
    }

    public void Resume()
    {
        m_moving = true;
    }
}
