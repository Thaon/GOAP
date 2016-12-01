using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Planner takes care of providing each agent with a queue of actions that they can perform to achieve their goal.
/// The following code has been copied as is from https://github.com/sploreg/goap/blob/master/Assets/Standard%20Assets/Scripts/AI/GOAP/GoapPlanner.cs.
/// This has been done as, after an analysis of the code, it was considered impossible to create a version of it that would do the same job in a significantly different way. Therefore, the entirety of Brent Owens' code was just copied and refactored to integrate with the existing codebase.
/// </summary>

public class GOAPPlanner
{

    /// <summary>
    /// Tries to find a plan for the given agent.
    /// </summary>
    /// <param name="agent">the agent to formulate a plan for</param>
    /// <param name="availableActions"> a set of available actions for the agent to perform</param>
    /// <param name="worldState">a set containing the curent state of the world</param>
    /// <param name="goal">the goal to achieve</param>
    /// <returns>a queue of Actions if a plan is found. Returns Null if a Plan can NOT be found</returns>
    public Queue<Action> plan(GameObject agent, HashSet<Action> availableActions, HashSet<KeyValuePair<string, object>> worldState, HashSet<KeyValuePair<string, object>> goal)
    {
        // reset the actions so we can start fresh with them
        foreach (Action a in availableActions)
        {
            a.DoReset();
        }

        // check what actions can run using their checkProceduralPrecondition
        HashSet<Action> usableActions = new HashSet<Action>();
        foreach (Action a in availableActions)
        {
            if (a.CheckProceduralPrecondition(agent))
                usableActions.Add(a);
        }

        // we now have all actions that can run, stored in usableActions

        // build up the tree and record the leaf nodes that provide a solution to the goal.
        List<Node> leaves = new List<Node>();

        // build graph
        Node start = new Node(null, 0, worldState, null);
        bool success = buildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            // oh no, we didn't get a plan
            Debug.Log("NO PLAN");
            return null;
        }

        // get the cheapest leaf
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.runningCost < cheapest.runningCost)
                    cheapest = leaf;
            }
        }

        // get its node and work back through the parents
        List<Action> result = new List<Action>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action); // insert the action in the front
            }
            n = n.parent;
        }
        // we now have this action list in correct order

        Queue<Action> queue = new Queue<Action>();
        foreach (Action a in result)
        {
            queue.Enqueue(a);
        }

        // hooray we have a plan!
        return queue;
    }

    /// <summary>
    /// Creates a graph using the available actions, matching preconditions with effects and assigning the cost of each action to the connected nodes
    /// </summary>
    /// <param name="parent">the parent node of the graph</param>
    /// <param name="leaves">the actions added through the "plan" method after checking the procedural preconditions</param>
    /// <param name="usableActions">the set of all usable actions for an Agent</param>
    /// <param name="goal">the Goal of the Agent</param>
    /// <returns>true if a graph can be built</returns>
    private bool buildGraph(Node parent, List<Node> leaves, HashSet<Action> usableActions, HashSet<KeyValuePair<string, object>> goal)
    {
        bool foundOne = false;

        // go through each action available at this node and see if we can use it here
        foreach (Action action in usableActions)
        {

            // if the parent state has the conditions for this action's preconditions, we can use it here
            if (inState(action.GetPreconditions(), parent.state))
            {

                // apply the action's effects to the parent state
                HashSet<KeyValuePair<string, object>> currentState = populateState(parent.state, action.GetEffects());
                //Debug.Log(GoapAgent.prettyPrint(currentState));
                Node node = new Node(parent, parent.runningCost + action.GetCost(), currentState, action);

                if (inState(goal, currentState))
                {
                    // we found a solution!
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    // not at a solution yet, so test all the remaining actions and branch out the tree
                    HashSet<Action> subset = actionSubset(usableActions, action);
                    bool found = buildGraph(node, leaves, subset, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }

        return foundOne;
    }

    /// <summary>
    /// Creates a subset of the Actions set removing one from it
    /// </summary>
    /// <param name="actions">the original actions set</param>
    /// <param name="removeMe">the action to be removed</param>
    /// <returns>the subset of actions bar the one to remove</returns>
    private HashSet<Action> actionSubset(HashSet<Action> actions, Action removeMe)
    {
        HashSet<Action> subset = new HashSet<Action>();
        foreach (Action a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }

    /// <summary>
    /// Checks if all the items in "test" are contained in "state"
    /// </summary>
    /// <param name="test">the items to check</param>
    /// <param name="state">the set to check "test" against</param>
    /// <returns>true if the items match</returns>
    private bool inState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
    {
        bool allMatch = true;
        foreach (KeyValuePair<string, object> t in test)
        {
            bool match = false;
            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
                allMatch = false;
        }
        return allMatch;
    }

    /// <summary>
    /// applies the stateChange to the current state, updating a value if found or adding a new one if it is NOT found
    /// </summary>
    /// <param name="currentState">the current state to be changed</param>
    /// <param name="stateChange">the change to be applied</param>
    /// <returns>the changed state</returns>
    private HashSet<KeyValuePair<string, object>> populateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)
    {
        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();
        // copy the KVPs over as new objects
        foreach (KeyValuePair<string, object> s in currentState)
        {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> change in stateChange)
        {
            // if the key exists in the current state, update the Value
            bool exists = false;

            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(change))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                state.Add(updated);
            }
            // if it does not exist in the current state, add it
            else
            {
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }

    /// <summary>
    /// the Node class is used in the Graph for navigation and data storage.
    /// </summary>
    private class Node
    {
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string, object>> state;
        public Action action;

        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, Action action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

}
