using UnityEngine;
using System.Collections;

public class FaffAboutAction : Action {

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;

    public float m_timeToFaff;

    #endregion

    FaffAboutAction()
    {
        AddPrecondition("hungry", false);
        SetCost(5);
    }

    public override void Reset()
    {
        m_completed = false;
        m_startingTime = 0;
    }

    public override bool IsDone()
    {
        return m_completed;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        // we can always faff about
        return true;
    }

    public override bool Perform(GameObject agent)
    {
        if (m_startingTime == 0)
            m_startingTime = Time.time;

        if (Time.time - m_startingTime > m_timeToFaff)
        {
            m_completed = true;
        }
        return true;
    }
}
