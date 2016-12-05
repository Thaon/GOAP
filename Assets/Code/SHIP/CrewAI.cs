using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(AstarAgent))]
[RequireComponent(typeof(Animator))]
public abstract class CrewAI : MonoBehaviour, IGoap {

    #region member variables

    private List<string> m_inventory;
    private AstarAgent m_nav;
    private Animator m_anim;
    private Text m_indicator;

    #endregion

    void Start ()
    {
        m_nav = GetComponent<AstarAgent>();
        m_anim = GetComponent<Animator>();
        m_inventory = new List<string>();
        m_indicator = GetComponentInChildren<Text>();
    }
	
	void Update ()
    {
        m_anim.SetFloat("speed", m_nav.velocity);

        //increase hunger over time
        if (GetComponent<EatAction>() != null)
        {
            GetComponent<EatAction>().IncreaseHunger();
        }
        //increase sleepyness
        if (GetComponent<RestAction>() != null)
        {
            GetComponent<RestAction>().IncreaseSleepiness();
        }
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldState = new HashSet<KeyValuePair<string, object>>();

        if (GetComponent<EatAction>() != null)
        {
            worldState.Add(new KeyValuePair<string, object>("hungry", GetComponent<EatAction>().IsHungry()));
        }
        if (GetComponent<RestAction>() != null)
        {
            worldState.Add(new KeyValuePair<string, object>("tired", GetComponent<RestAction>().IsSleepy()));
        }

        //food
        worldState.Add(new KeyValuePair<string, object>("hasIngredients", m_inventory.Contains("ingredients")));

        //check if we need to refill any pilons
        bool needToRefill = true;
        VendingMachine[] vendingMachines = FindObjectsOfType<VendingMachine>();

        foreach (VendingMachine machine in vendingMachines)
        {
            //we need to get only machines that DON'T have food
            if (machine.GetFood() == 0)
            {
                needToRefill = false;
                break;
            }
        }
        worldState.Add(new KeyValuePair<string, object>("makeFood", needToRefill)); //to be changed
        //worldState.Add(new KeyValuePair<string, object>("hasMadeFood", ));


        //materials
        //worldState.Add(new KeyValuePair<string, object>("hasMaterials", m_inventory.Contains("materials")));


        return worldState;
    }

    public abstract HashSet<KeyValuePair<string, object>> createGoalState();

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<Action> actions)
    {
        // write to the log the queue of actions found
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));

        //set the text on the action display done here for static actions purposes
        m_indicator.text = actions.Peek().GetType().ToString();
    }

    public void actionsFinished()
    {
        Debug.Log("<color=blue>Actions queue empty</color>");
    }

    public void planAborted(Action aborter)
    {
        // log the action that made us fail
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(Action nextAction)
    {
        //set the text on the action display done here for demonstration purposes
        m_indicator.text = nextAction.GetType().ToString();

        // move towards the NextAction's target
        Vector3 fullDis = transform.position - nextAction.GetTarget().transform.position;
        fullDis.y = 0;
        if (fullDis.magnitude <= 1)
        {
            // we are at the target location, we are done
            nextAction.SetInRange();
            m_nav.Stop();
            return true;
        }
        else
        {
            m_nav.SetPath(nextAction.GetTarget());
            m_nav.Resume();
            return false;
        }
    }

    public void AddToInventory(string item)
    {
        m_inventory.Add(item);
    }

    public void RemoveFromInventory(string item)
    {
        m_inventory.Remove(item);
    }
}
