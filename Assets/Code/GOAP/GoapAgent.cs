using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Each GameObject that wants to use GOAP will have to have a GoapAgent component.
/// To be noted here is that the class is sealed, meaning it canNOT be inherited from.
/// </summary>
public sealed class GoapAgent : MonoBehaviour
{

    private FSM stateMachine;

    private FSM.FSMState idleState; // finds something to do
    private FSM.FSMState moveToState; // moves to a target
    private FSM.FSMState performActionState; // performs an action

    private HashSet<Action> availableActions;
    private Queue<Action> currentActions;

    private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

    private GOAPPlanner planner;

    /// <summary>
    /// Initialises the default values for the class.
    /// It also creates a new FSM and a GOAPPlanner.
    /// Finds a data provider, that is a class implementing IGoap which will provide World data and will communicate with the GOAPPlanner
    /// I then feeds the FSM with an Idle and a MoveTo state, pushing the Idle state first.
    /// Finally it procedes to load all the attached Actions into the availableActions HashSet
    /// </summary>
    void Start()
    {
        stateMachine = new FSM();
        availableActions = new HashSet<Action>();
        currentActions = new Queue<Action>();
        planner = new GOAPPlanner();
        findDataProvider();
        createIdleState();
        createMoveToState();
        createPerformActionState();
        stateMachine.PushState(idleState);
        loadActions();
    }

    /// <summary>
    /// Updates the FSM
    /// </summary>
    void Update()
    {
        stateMachine.Update(this.gameObject);
    }

    /// <summary>
    /// Adds an Action to the availableActions HashSet
    /// </summary>
    /// <param name="a">the Action to add</param>
    public void addAction(Action a)
    {
        availableActions.Add(a);
    }

    /// <summary>
    /// Fetches an Action from the availableActions set
    /// </summary>
    /// <param name="action">the Action to fetch</param>
    /// <returns></returns>
    public Action getAction(Action action)
    {
        foreach (Action g in availableActions)
        {
            if (g.GetType().Equals(action.GetType()))
                return g;
        }
        return null;
    }

    /// <summary>
    /// Removes and Action from the availableActions set
    /// </summary>
    /// <param name="action">the Action to remove</param>
    public void removeAction(Action action)
    {
        availableActions.Remove(action);
    }

    /// <summary>
    /// Checks whether or not the Agent has a plan to execute
    /// </summary>
    /// <returns>true if the Agent has a plan to execute, false if it does NOT</returns>
    private bool hasActionPlan()
    {
        return currentActions.Count > 0;
    }

    /// <summary>
    /// Creates the Idle state to be fed into the FSM.
    /// The IdleState will communicate with the concrete implementation of the IGoap interface and get from it the World state.
    /// It will then proceed to create a Goal and formulate a plan from those variables.
    /// If a plan is found, the event is communicated to the data provider through the planFound(goal, plan) method, then the PerformActionState is pushed on top of the FSM.
    /// If a plan is NOT found, the IdleState gets pushed again and a planFailed(goal) message is sent to the data provider.
    /// </summary>
    private void createIdleState()
    {
        idleState = (fsm, gameObj) => {
            // GOAP planning

            // get the world state and the goal we want to plan for
            HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            HashSet<KeyValuePair<string, object>> goal = dataProvider.createGoalState();

            // Plan
            Queue<Action> plan = planner.plan(gameObject, availableActions, worldState, goal);
            if (plan != null)
            {
                // we have a plan, hooray!
                currentActions = plan;
                dataProvider.planFound(goal, plan);

                fsm.PopState(); // move to PerformAction state
                fsm.PushState(performActionState);

            }
            else
            {
                // ugh, we couldn't get a plan
                Debug.Log("<color=orange>Failed Plan:</color>" + prettyPrint(goal));
                dataProvider.planFailed(goal);
                fsm.PopState(); // move back to IdleAction state
                fsm.PushState(idleState);
            }

        };
    }

    /// <summary>
    /// This state takes care of checking if the action has a target and if it requires the target to be in range.
    /// If this is NOT true, it proceeds to perform the action and goes back to the IdleState.
    /// If on the other side, it is true, it tells the data provider to move the Agent.
    /// </summary>
    private void createMoveToState()
    {
        moveToState = (fsm, gameObj) => {
            // move the game object

            Action action = currentActions.Peek();
            if (action.RequiresInRange() && action.GetTarget() == null)
            {
                Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                fsm.PopState(); // move
                fsm.PopState(); // perform
                fsm.PushState(idleState);
                return;
            }

            // get the agent to move itself
            if (dataProvider.moveAgent(action))
            {
                fsm.PopState();
            }
        };
    }

    /// <summary>
    /// In here a Perform Action state is created.
    /// If the Agent does NOT have an Action plan, the FSM gets pushed back to the IdleState.
    /// If a plan is available, the first Action in the queue is peeked, and, if completed, it is removed.
    /// If the Action has NOT been completed, on the other hand, it will be taken into consideration for completion.
    /// A distance check will be made if the Action requires to be in range. At this point if still it is not, the MoveTo state will be pushed.
    /// If NONE of the above is true, the action will be performed and marked as completed.
    /// </summary>
    private void createPerformActionState()
    {

        performActionState = (fsm, gameObj) => {
            // perform the action

            if (!hasActionPlan())
            {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            Action action = currentActions.Peek();
            if (action.IsDone())
            {
                // the action is done. Remove it so we can perform the next one
                currentActions.Dequeue();
            }

            if (hasActionPlan())
            {
                // perform the next action
                action = currentActions.Peek();
                bool inRange = action.RequiresInRange() ? action.GetInRange() : true;

                if (inRange)
                {
                    // we are in range, so perform the action
                    bool success = action.Perform(gameObj);

                    if (!success)
                    {
                        // action failed, we need to plan again
                        fsm.PopState();
                        fsm.PushState(idleState);
                        dataProvider.planAborted(action);
                    }
                }
                else
                {
                    // we need to move there first
                    // push moveTo state
                    fsm.PushState(moveToState);
                }

            }
            else
            {
                // no actions left, move to Plan state
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.actionsFinished();
            }

        };
    }

    /// <summary>
    /// Finds a component that inherits from IGoap and binds it as the data provider
    /// </summary>
    private void findDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    /// <summary>
    /// Loads all the Action components and fills in the availableActions HashSet
    /// </summary>
    private void loadActions()
    {
        Action[] actions = gameObject.GetComponents<Action>();
        foreach (Action a in actions)
        {
            availableActions.Add(a);
        }
        Debug.Log("Found actions: " + prettyPrint(actions));
    }

    /// <summary>
    /// Debug print method to format a state into a string
    /// </summary>
    /// <param name="state"> the state to format</param>
    /// <returns>the formatted string</returns>
    public static string prettyPrint(HashSet<KeyValuePair<string, object>> state)
    {
        string s = "";
        foreach (KeyValuePair<string, object> kvp in state)
        {
            s += kvp.Key + ":" + kvp.Value.ToString();
            s += ", ";
        }
        return s;
    }

    /// <summary>
    /// Debug print method to format a queue of actions into a string
    /// </summary>
    /// <param name="actions"> the actions to format</param>
    /// <returns>the formatted string</returns>
    public static string prettyPrint(Queue<Action> actions)
    {
        string s = "";
        foreach (Action a in actions)
        {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    /// <summary>
    /// Debug print method to format a queue of actions into a string
    /// </summary>
    /// <param name="actions"> the actions to format</param>
    /// <returns>the formatted string</returns>
    public static string prettyPrint(Action[] actions)
    {
        string s = "";
        foreach (Action a in actions)
        {
            s += a.GetType().Name;
            s += ", ";
        }
        return s;
    }

    /// <summary>
    /// Debug print method to format an action into a string
    /// </summary>
    /// <param name="action"> the action to format</param>
    /// <returns>the formatted string</returns>
    public static string prettyPrint(Action action)
    {
        string s = "" + action.GetType().Name;
        return s;
    }
}