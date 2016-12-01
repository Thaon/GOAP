using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AstarAgent))]
[RequireComponent(typeof(Animator))]
public class CrewAI : MonoBehaviour, IGoap {

    #region member variables

    private List<string> m_inventory;
    private AstarAgent m_nav;
    private Animator m_anim;

    #endregion

    void Start ()
    {
        m_nav = GetComponent<AstarAgent>();
        m_anim = GetComponent<Animator>();
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

            return worldState;
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("hungry", false));
        goal.Add(new KeyValuePair<string, object>("tired", false));

        return goal;
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<Action> actions)
    {
        // write to the log the queue of actions found
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
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
        // move towards the NextAction's target
        if (Vector3.Distance(transform.position, nextAction.GetTarget().transform.position) <= 1)
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
}
