using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    /// <summary>
    /// Any agent that wants to use GOAP must implement
    /// this interface. It provides information to the GOAP
    /// planner so it can plan what actions to use.
    /// It also provides an interface for the planner to give
    /// feedback to the Agent and report success/failure.
    /// </summary>
public interface IGoap
{
    /// <summary>
    /// gets a reference to the state of the World
    /// </summary>
    /// <returns>a Hash Set containing the state of the World</returns>
    HashSet<KeyValuePair<string, object>> getWorldState();

    /// <summary>
    /// Provides a new Goal to the Planner
    /// </summary>
    /// <returns>the new Goal created</returns>
    HashSet<KeyValuePair<string, object>> createGoalState();

    /// <summary>
    /// If a plan failed to be created, a new one will be tried excluding the failed goal from the creation process
    /// </summary>
    /// <param name="failedGoal">the goal we failed to achieve</param>
    void planFailed(HashSet<KeyValuePair<string, object>> failedGoal);

    /// <summary>
    /// If a plan was found, a queue of Actions is provided for the requested Goal
    /// </summary>
    /// <param name="goal">the requested Goal</param>
    /// <param name="actions">the queue of Actions to be executed to achieve the Goal</param>
    void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<Action> actions);

    /// <summary>
    /// decides what happens once all the actions are completed
    /// </summary>
    void actionsFinished();

    /// <summary>
    /// If one of the actions made the plan be aborted, this function gets called
    /// </summary>
    /// <param name="aborter">the Action that caused the plan to get aborted</param>
    void planAborted(Action aborter);

    /// <summary>
    /// this method gets called during update and takes care of moving the Agent to his next destination
    /// </summary>
    /// <param name="nextAction">the Action to perform when the destination is reached</param>
    /// <returns>true if destination has been reached</returns>
    bool moveAgent(Action nextAction);
}
