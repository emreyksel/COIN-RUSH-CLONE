using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;

    private Vector3 firstTouchPosition;
    private Vector3 curTouchPosition;
    [SerializeField] private float sensitivityMultiplier = 0.1f;
    private float finalTouchX;
    private float xBound = 5f;

    public List<GameObject> stackList = new List<GameObject>();
    public GameObject coin;

    public float speed;

    private MeshRenderer meshRend;

    private void Awake()
    {
        instance = this; 

        meshRend = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!GameManager.instance.isGameStart)
            return;

        if (GameManager.instance.isGameFinish && speed !=0)
        {
            StartCoroutine(FinishTower());

            for (int i = 1; i < stackList.Count; i++)
            {
                if (stackList[i].transform.localPosition != null)
                {
                    Vector3 pos = stackList[i].transform.position;
                    pos.z = stackList[0].transform.localPosition.z;
                    stackList[i].transform.position = pos;

                    Vector3 pos1 = stackList[i].transform.position;
                    pos1.x = stackList[0].transform.localPosition.x;
                    stackList[i].transform.position = pos1;
                }
            }
        }

        if (GameManager.instance.isGameOver || GameManager.instance.isGameFinish)
            return;

        Follow();
        Move();

        if (transform.position.x <= -xBound || transform.position.x >= xBound)
        {
            GameManager.instance.isGameOver = true;

            foreach (var item in stackList)
            {
                if (item.TryGetComponent(out Coin co))
                {
                    item.GetComponent<Coin>().forward = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isGameStart)
        {
            transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
        }          
    }

    public void Follow()
    {
        for (int i = 1; i < stackList.Count; i++)
        {
            if (stackList[i].transform.localPosition != null)
            {
                stackList[i].transform.localPosition = stackList[i - 1].transform.localPosition - stackList[i - 1].transform.right * 0.45f;

                Vector3 rot = stackList[i].transform.eulerAngles;
                rot.y = stackList[i - 1].transform.eulerAngles.y;
                stackList[i].transform.DOLocalRotate(rot, 0.5f);
            }
        }       
    }

    public IEnumerator CoinScale()
    {
        for (int i = 0; i < stackList.Count; i++)
        {
            int index = i;
            Vector3 scale = Vector3.one * 1.5f;
            stackList[index].transform.DOScale(scale, 0.1f).OnComplete(() => stackList[index].transform.DOScale(Vector3.one, 0.1f));
            yield return new WaitForSeconds(0.03f);
        }
    }

    public IEnumerator FinishTower()
    {
        for (int i = 1; i < stackList.Count; i++)
        {
            int index = i;
            float mainCoinY = stackList[0].transform.position.y;
            Vector3 poz = stackList[0].transform.position;
            poz.y = mainCoinY + (mainCoinY * 2) * index;
            stackList[i].transform.eulerAngles = new Vector3(0, -90, 0);
            stackList[index].transform.DOMoveY(poz.y, 0.1f);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            curTouchPosition = Input.mousePosition;

            Vector2 touchDelta = (curTouchPosition - firstTouchPosition);

            finalTouchX = (transform.eulerAngles.y + (touchDelta.x * sensitivityMultiplier));

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, finalTouchX, transform.eulerAngles.z);

            firstTouchPosition = Input.mousePosition;
        }
    }

    public void Stack(Collider col)
    {
        if (!stackList.Contains(col.gameObject))
        {
            Destroy(col.gameObject);
            GameObject poolCoin = ObjectPool.instance.GetPooledObject(0);
            stackList.Add(poolCoin);

            StartCoroutine(CoinScale());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Stack(other);
        }
        else if (other.CompareTag("Obstacle"))
        {
            if (stackList.Count ==1)
            {
                speed = 0;
                GameManager.instance.isGameOver = true;
                Camera.main.DOShakeRotation(1.5f, 4, fadeOut: true);
            }
            else
            {
                int count = stackList.Count;

                for (int i = 1; i < count; i++)
                {
                    GameObject clone = stackList[stackList.Count - 1];
                    stackList.RemoveAt(stackList.Count - 1);
                    clone.transform.DOJump(RandomPos(clone.transform), 1, 2, 1).OnComplete(() =>
                    ObjectPool.instance.SendPooledObject(0, clone));
                }

                Camera.main.DOShakeRotation(1.5f, 4, fadeOut: true);

                StartCoroutine(Crash());
            }
        }
        else if (other.CompareTag("Finish"))
        {
            transform.eulerAngles = new Vector3(0, -90, 0);
            GameManager.instance.isGameFinish = true;
            DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, 85, 1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            speed = 0;

            foreach (var item in stackList)
            {
                if (item.TryGetComponent(out Coin co))
                {
                    item.GetComponent<Coin>().forward = true;
                }
            }

            if (stackList.Count == 1)
            {
                GameManager.instance.isGameWin = true;
            }
        }
    }

    private IEnumerator Crash()
    {
        foreach (var item in GameManager.instance.obstacles)
        {
            item.tag = "Untagged";
        }

        for (int i = 0; i < 5; i++)
        {
            meshRend.enabled = false;
            yield return new WaitForSeconds(0.2f);
            meshRend.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }

        foreach (var item in GameManager.instance.obstacles)
        {
            item.tag = "Obstacle";
        }
    }

    private Vector3 RandomPos(Transform coin)
    {
        Vector3 randPos = coin.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        return randPos;
    }
}
