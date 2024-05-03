using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapCamera : MonoBehaviour
{
    [SerializeField] GameObject Panel;
    public GameObject blackout;

    [SerializeField] Vector3 lastMousePosition;

    [SerializeField] Transform CameraReticle;

    RaycastHit2D[] contact;
    [SerializeField] GameObject[] taggedGameObject;
    [SerializeField] Transform closestGameObject;

    //Bound for changes
    [SerializeField] Transform CameraHolder;

    //Camera origin
    Vector3 cameraOrigin;
    bool switchMode;


    public float LerpTime = 1f;

    private void Start()
    {
        cameraOrigin = CameraHolder.position;
    }

    private void Update()
    {

        contact = Physics2D.BoxCastAll(CameraReticle.position, new Vector2(2,2), 0, Vector2.zero);

        
        if (contact != null && contact.Length > 0)
        {
            RayToCollider(contact);
        }
        if(contact.Length ==0)
        {
            taggedGameObject = new GameObject[0];
        }

        

        WhenMouseIsMoving();
        SnapSystem();

        /*if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            WhenMouseIsMoving();

            timeElapsed = 0f;
        }
        else
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed>= lockDuration)
            {
                WhenMouseIsntMoving();
            }
        }*/



    }
    GameObject[] RayToCollider(RaycastHit2D[] contact)
    {
        taggedGameObject = new GameObject[contact.Length];

        for(int i=0; i<contact.Length; i++)
        {
            //Debug.Log(contact[i].collider);
            taggedGameObject[i] = contact[i].collider.gameObject;
        }
        return taggedGameObject;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(CameraReticle.position, new Vector3(2, 2, 0));
    }
    void WhenMouseIsMoving()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CameraReticle.position = mousePosition;
    }

    void WhenMouseIsntMoving()
    {
        Debug.Log("StoppedMoving");
    }

    void SnapSystem()
    {
        if(Input.GetMouseButtonDown(0) && taggedGameObject.Length >0 && switchMode == false)
        {
            switchMode = true;
            closestGameObject = GetClosestEnemy(taggedGameObject);
            StartCoroutine(LerpFunction(2.5f, LerpTime, closestGameObject));
            CameraReticle.gameObject.SetActive(false);
            blackout.SetActive(true);
            closestGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    IEnumerator LerpFunction(float endValue, float duration, Transform target)
    {
        float time = 0;
        float startValue = Camera.main.orthographicSize;
        Vector2 cameraHold = CameraHolder.position;


        while (time < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startValue, endValue, time / duration);
            CameraHolder.position = Vector2.Lerp(cameraHold, target.position, time / duration);

            time += Time.deltaTime;
            yield return null;
        }
        Camera.main.orthographicSize = endValue;
        Panel.SetActive(true);

    }
    Transform GetClosestEnemy(GameObject[] enemies)
    {
        Transform bestTarget = null;
        float mindist = Mathf.Infinity;
        foreach(GameObject col in enemies)
        {
            Vector2 dirToTarget = col.transform.position - CameraReticle.position;
            float dSqrTarget = dirToTarget.sqrMagnitude;
            if(dSqrTarget < mindist)
            {
                mindist = dSqrTarget;
                bestTarget = col.transform;
            }
        }
        Debug.Log(bestTarget);
        return bestTarget;
    }

    public void backButton()
    {
        Panel.SetActive(false);
        Camera.main.orthographicSize = 5f;
        switchMode = false;
        CameraHolder.position = cameraOrigin;
        closestGameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        blackout.SetActive(false);
        CameraReticle.gameObject.SetActive(true);
    }

}
