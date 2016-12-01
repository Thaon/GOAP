using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// the FSM class covers the bare minimum to provide GOAP Agents with a way to transition between the 3 main states:
/// Idle
/// Move
/// Perform
/// </summary>
public class FSM
{
    #region member variables

    private Stack<FSMState> m_states = new Stack<FSMState>();

    //delegate needed to pass a reference to a FSM into the stack
    public delegate void FSMState(FSM fsm, GameObject gameObject);

    #endregion

    /// <summary>
    /// Updates the FSM for the given GameObject
    /// </summary>
    /// <param name="gameObject">the GameObject to update the FSM for</param>
    public void Update(GameObject gameObject)
    {
        if (m_states.Peek() != null)
            m_states.Peek().Invoke(this, gameObject);
    }

    /// <summary>
    /// Pushes a new state in the FSM
    /// </summary>
    /// <param name="state">the state to be pushed</param>
    public void PushState(FSMState state)
    {
        m_states.Push(state);
    }

    /// <summary>
    /// Pops the first state available from the top of the FSM stack.
    /// Note that this is the last state that was put in the stack.
    /// </summary>
    public void PopState()
    {
        m_states.Pop();
    }
}