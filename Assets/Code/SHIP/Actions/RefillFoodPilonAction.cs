using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefillFoodPilonAction : Action {

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;

    public float m_timeToComplete;

    #endregion

    RefillFoodPilonAction()
    {
        AddPrecondition("hasIngredients", true);
        AddEffect("makeFood", true);
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
        // find the nearest pilon
        VendingMachine[] vendingMachines = FindObjectsOfType<VendingMachine>();
        GameObject nearest = null;
        float max = 0;

        List<GameObject> usefulMachines = new List<GameObject>();

        foreach (VendingMachine machine in vendingMachines)
        {
            //we need to get only machines that DON'T have food
            if (machine.GetFood() == 0)
            {
                usefulMachines.Add(machine.gameObject);
            }
        }

        foreach (GameObject machine in usefulMachines)
        {
            if (nearest == null)
            {
                nearest = machine;
                max = Vector3.Distance(machine.transform.position, transform.position);
            }
            else
            {
                float dist = Vector3.Distance(machine.transform.position, transform.position);
                if (dist < max)
                {
                    nearest = machine;
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
        print("performing!");
        GetComponent<Animator>().SetBool("interacting", true);
        if (m_startingTime == 0)
            m_startingTime = Time.time;

        if (Time.time - m_startingTime > m_timeToComplete)
        {
            GetComponent<Animator>().SetBool("interacting", false);
            GetComponent<CookAI>().RemoveFromInventory("ingredients");
            m_target.GetComponent<VendingMachine>().SetFood(1);
            m_completed = true;
        }
        return true;
    }
}
