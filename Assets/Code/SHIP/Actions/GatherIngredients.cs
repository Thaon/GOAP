using UnityEngine;
using System.Collections;

public class GatherIngredients : Action
{

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;

    public float m_timetoComplete;

    #endregion

    GatherIngredients()
    {
        AddPrecondition("hasIngredients", false);
        AddEffect("hasIngredients", true);
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
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        // find the nearest farm
        GameObject[] rb = GameObject.FindGameObjectsWithTag("Farm");
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
        GetComponent<Animator>().SetBool("interacting", true);
        if (m_startingTime == 0)
            m_startingTime = Time.time;

        if (Time.time - m_startingTime > m_timetoComplete)
        {
            GetComponent<Animator>().SetBool("interacting", false);
            GetComponent<CookAI>().AddToInventory("ingredients");
            m_completed = true;
        }
        return true;
    }
}
