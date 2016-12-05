using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CookAI : CrewAI {

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("hungry", false));
        goal.Add(new KeyValuePair<string, object>("tired", false));
        goal.Add(new KeyValuePair<string, object>("makeFood", true));

        return goal;
    }
}
