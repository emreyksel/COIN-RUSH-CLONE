using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartText : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.instance.isGameStart = true;
            gameObject.SetActive(false);
        }
    }
}
