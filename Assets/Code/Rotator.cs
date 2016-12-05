using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    void Update ()
    {
        transform.LookAt(Camera.main.transform.position);
        print("yay");
	}
}
