using UnityEngine;

public class VendingMachine : MonoBehaviour{

    #region member variables

    [SerializeField]
    private int m_food = 5;

    #endregion

    public int GetFood() { return m_food; }
    public void SetFood(int food) { m_food = food; }
	
}
