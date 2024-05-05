using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNote : MonoBehaviour
{
    
    public float noteBeat;
    public Vector2 startPos;
    Vector2 removeposition;
    ConductorClass cc;
    // Start is called before the first frame update
    void Start()
    {       

    }


    public void initialise(ConductorClass conductor, float beatOfThisNote, Vector2 _startPos)
    {
        cc = conductor;

        //setting start and end pos
        transform.position = _startPos;
        startPos = transform.position;
        removeposition = cc.transform.position - Vector3.right * 13f;

        Debug.Log(transform.position);

        //Note's beat index
        noteBeat = beatOfThisNote;

        //StartCoroutine(MoveAlongBarCo());

    }

    IEnumerator MoveAlongBarCo()
    {
        float timeElapsed = 0f;
        float time = cc.beatsShownInAdvance * cc.secPerBeat;


        while( timeElapsed < time)
        {
            transform.position = Vector2.Lerp(startPos, removeposition, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            Debug.Log(timeElapsed);
            yield return null;
        }
        transform.position = removeposition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(startPos.x + (removeposition.x - startPos.x) * (1f - (noteBeat - cc.songPosition / cc.secPerBeat) / cc.beatsShownInAdvance), transform.position.y);


        if (transform.position.x < removeposition.x-3)
        {
            Destroy(gameObject);
        }

    }
}
