using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EatAction : Action {

    #region member variables

    [SerializeField]
    private float m_hunger = 0;
    private bool m_isEating = false;
    private float m_startingTime = 0;

    public float m_timeToEat;
    public float m_maxHunger = 50;
    public float m_hungerIncrease;

    #endregion

    EatAction ()
    {
        AddPrecondition("hungry", true);
        AddEffect("hungry", false);
        SetCost(2);
    }

    public override void Reset()
    {
        //reset member variables, apart from hunger, that will need to go up
        m_isEating = false;
        SetTarget(null);
        m_startingTime = 0;
    }

    public override bool IsDone()
    {
        return m_hunger < m_maxHunger;
    }

    public override bool RequiresInRange()
    {
        return true; // we need to be in the refectory to eat
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        // find the nearest machine pile that has spare ores
        VendingMachine[] vendingMachines = FindObjectsOfType<VendingMachine>();
        GameObject nearest = null;
        float max = 0;

        List<GameObject> usefulMachines = new List<GameObject>();

        foreach (VendingMachine machine in vendingMachines)
        {
            //we need to get only machines that have food, we are here assuming a knowledge from the Agent that is not realistic, still, let's do it!
            if (machine.GetFood() >= 1)
            {
                usefulMachines.Add(machine.gameObject);
            }
        }

                foreach (GameObject machine in usefulMachines)
        {
            //we need to get only machines that have food, we are here assuming a knowledge from the Agent that is not realistic, still, let's do it!
            if (machine.GetComponent<VendingMachine>().GetFood() >= 1)
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

        if (Time.time - m_startingTime > m_timeToEat)
        {
            int food = GetTarget().GetComponent<VendingMachine>().GetFood();
            if (m_hunger > m_maxHunger || food >= 1)
            {
                food = GetTarget().GetComponent<VendingMachine>().GetFood();

                GetTarget().GetComponent<VendingMachine>().SetFood(food - 1);
                m_hunger -= 20;
            }
            m_isEating = true;
        }
        else
        {
            m_isEating = false;
        }
        return true;
    }

    public void IncreaseHunger()
    {
        if (!m_isEating)
        {
            m_hunger += m_hungerIncrease;
        }
    }

    public bool IsHungry() { return m_hunger > m_maxHunger; }
}
