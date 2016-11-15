using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A GOAP action can have a set of requirements and a cost, it then has an effect when completed so that the planner can use it to satisfy a Goal
/// </summary>

public class Action {

    #region member variables

    public List<Precondition> m_requirements;
    public Effect m_effect;
    public byte m_cost;

    #endregion

}
