using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A GOAP action can have a set of requirements and a cost, it then has an effect when completed so that the planner can use it to satisfy a Goal
/// </summary>

public abstract class Action : MonoBehaviour {

    #region member variables

    private HashSet<KeyValuePair<string, object>> m_preconditions;
    private HashSet<KeyValuePair<string, object>> m_effects;

    private byte m_cost;
    private bool m_inRange;

    public GameObject m_target;

    #endregion

    /// <summary>
    /// default constructor, initialises preconditions and effects
    /// </summary>
    public Action()
    {
        m_preconditions = new HashSet<KeyValuePair<string, object>>();
        m_effects = new HashSet<KeyValuePair<string, object>>();
    }

    /// <summary>
    /// resets the action to its default state
    /// </summary>
    public void DoReset()
    {
        m_inRange = false;
        m_target = null;
        Reset();
    }

    //abstract methods to be implemented in concrete classes
    public abstract void Reset();
    public abstract bool IsDone();
    public abstract bool CheckProceduralPrecondition(GameObject agent);
    public abstract bool Perform(GameObject agent);
    public abstract bool RequiresInRange();

    //getters and setters
    //cost
    /// <summary>
    /// gets the cost of the action
    /// </summary>
    /// <returns></returns>
    public byte GetCost() { return m_cost; }
    /// <summary>
    /// sets the cost for the action
    /// </summary>
    /// <param name="cost"></param>
    public void SetCost(byte cost) { m_cost = cost; }

    //range
    /// <summary>
    /// sets the action to be in range
    /// </summary>
    public void SetInRange() { m_inRange = true; }
    /// <summary>
    /// gets wether the action is in range or not
    /// </summary>
    /// <returns></returns>
    public bool GetInRange() { return m_inRange; }

    //target
    /// <summary>
    /// sets a target for the action
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(GameObject target) { m_target = target; }
    /// <summary>
    /// gets the action's target
    /// </summary>
    /// <returns></returns>
    public GameObject GetTarget() { return m_target; }

    //preconditions
    /// <summary>
    /// adds a new precondition to the HashSet
    /// </summary>
    /// <param name="str">the name of the precondition</param>
    /// <param name="obj">the value of the precondition, usually a bool</param>
    public void AddPrecondition(string str, object obj) { m_preconditions.Add(new KeyValuePair<string, object>(str, obj)); }
    /// <summary>
    /// removes a precondition from the HashSet
    /// </summary>
    /// <param name="key">the name of the precondition to be removed</param>
    public void RemovePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in m_preconditions)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            m_preconditions.Remove(remove);
    }
    public HashSet<KeyValuePair<string, object>> GetPreconditions() { return m_preconditions; }

    //effects
    /// <summary>
    /// similarly to preconditions, it adds a new effect in the HashSet
    /// </summary>
    /// <param name="str">name of the effect</param>
    /// <param name="obj">value of the effect, usually a bool</param>
    public void AddEffect(string str, object obj) { m_effects.Add(new KeyValuePair<string, object>(str, obj)); }
    /// <summary>
    /// removes an effect from the HashSet
    /// </summary>
    /// <param name="key">name of the effect to be removed</param>
    public void RemoveEffect(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in m_effects)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            m_effects.Remove(remove);
    }
    public HashSet<KeyValuePair<string, object>> GetEffects() { return m_effects; }


}
