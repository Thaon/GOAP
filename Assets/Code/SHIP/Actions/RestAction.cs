using UnityEngine;
using System.Collections;

public class RestAction : Action {

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;
    [SerializeField]
    private float m_sleepiness = 0;

    public float m_timeToRest;
    public float m_sleepIncrease;
    public float m_sleepThreshold;

    #endregion

    RestAction()
    {
        AddPrecondition("tired", true);
        AddEffect("tired", false);
        SetCost(3);
    }

    public override void Reset()
    {
        SetTarget(null);
        m_completed = false;
        m_startingTime = 0;
    }

    public override bool IsDone()
    {
        return m_completed;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        // find the nearest resting booth
        GameObject[] rb = GameObject.FindGameObjectsWithTag("RestingBooth");
        GameObject nearest = null;
        float max = 0;

        foreach (GameObject booth in rb)
        {
            if (nearest == null)
            {
                nearest = booth;
                max = Vector3.Distance(booth.transform.position, transform.position);
            }
            else
            {
                float dist = Vector3.Distance(booth.transform.position, transform.position);
                if (dist < max)
                {
                    nearest = booth;
                    max = dist;
                }
            }
        }
        if (nearest == null)
            return false;

        SetTarget(nearest);

        return nearest != null;
    }

    public override bool Perform(GameObject agent)
    {
        if (m_startingTime == 0)
            m_startingTime = Time.time;

        if (Time.time - m_startingTime > m_timeToRest)
        {
            m_sleepiness = 0;
            m_completed = true;
        }
        return true;
    }

    public void IncreaseSleepiness() { m_sleepiness += m_sleepIncrease; }
    public bool IsSleepy() { return m_sleepiness > m_sleepThreshold; }
}
