using UnityEngine;

public class VendingMachine : MonoBehaviour{

    #region member variables

    [SerializeField]
    private int m_food = 1;
    private Vector3 m_initialPos, m_finalPos;

    public Vector3 m_targetPos;

    #endregion

    void Start()
    {
        m_initialPos = transform.position;
        m_finalPos = transform.position + new Vector3(0, 2, 0);

        if (m_food > 0)
            m_targetPos = m_finalPos;
        else
            m_targetPos = m_initialPos;
    }

    void Update()
    {
        if (transform.position != m_targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, m_targetPos, 0.1f);
        }
    }

    public int GetFood()
    {
        return m_food;
    }

    public void SetFood(int food)
   {
        m_food = food;
        if (m_food > 0)
            m_targetPos = m_finalPos;
        else
            m_targetPos = m_initialPos;
    }
	
}
