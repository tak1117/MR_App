using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenmaScript : MonoBehaviour
{


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("天満だお");
        }
    }
}
