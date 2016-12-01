using UnityEngine;

/// <summary>
/// An interface for concrete implemetations of FSM states
/// </summary>
public interface FSMState
{
    /// <summary>
    /// Updates the State
    /// </summary>
    /// <param name="fsm">the state machine that owns the state</param>
    /// <param name="gameObject">the GameObject to operate upon</param>
    void Update(FSM fsm, GameObject gameObject);
}
