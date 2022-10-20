using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float speed = 3f;
    public bool forward = false;
    private float xBound = 5f;

    void Update()
    {
        if (transform.position.x <= -xBound || transform.position.x >= xBound)
        {
            if (!gameObject.TryGetComponent(out Rigidbody rb))
            {
                gameObject.AddComponent<Rigidbody>();
            }
        }
    }

    private void FixedUpdate()
    {
        if (forward)
        {
            transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            speed = 0;

            if (gameObject == CoinController.instance.stackList[CoinController.instance.stackList.Count-1])
            {
                GameManager.instance.isGameWin = true;
            }
        }
    }
}
