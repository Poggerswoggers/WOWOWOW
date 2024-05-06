using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ConductorClass : MonoBehaviour
{
    /*The Conductor class is the main song managing class that the rest of our rhythm game will be built on. 
        With it, we'll track the song position, and control any other synced actions.
        To track the song, we'll need a few variables:*/

    // Used to display "HIT" or "MISS".


    //Song beats per minute
    //This is determined by the song you're trying to sync up to
    public float songBpm;

    //The number of seconds for each song beat //Crochet
    public float secPerBeat;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public float songPositionInBeats;

    //How many seconds have passed since the song started
    public float dspSongTime;

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;

    //First beatOffset
    public float firstBeatOffset;


    //beats per minute of a song
    float bpm;

    //keep all the position-in-beats of notes in the track
    public float[] track;


    //the index of the next note to be spawned
    int nextIndex = 0;

    //Delay in beats 
    public float beatsShownInAdvance;

    //Note Prefab
    public GameObject notePrefab;

    //Has song started
    [SerializeField] private bool songStarted = false;


    // Used to display "HIT" or "MISS".
    public GameObject scoreText;
    Vector3 scoreStartPos;

    public Queue<MusicNote> notesOnscreen;

    //EndPoint target
    [SerializeField] Transform Target;
    TargetController tc;


    //DistanceAndBeatOffset
    public float offsetToTarget;
    public float beatOffset;

    void StartGame()
    {
        if(!songStarted)
        {
            songStarted = true;

            tc = Target.GetComponent<TargetController>();
            scoreStartPos = scoreText.transform.position;

            StartSong();
        }
    }

    // Start is called before the first frame update
    void StartSong()
    {
        //Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        //Play music
        musicSource.Play();

        notesOnscreen = new Queue<MusicNote>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && songStarted)
        {
            HitNote();
        }
        if(Input.GetKeyDown(KeyCode.Space) && !songStarted)
        {
            StartGame();
        }

        if (!songStarted) return;

        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;

        if (nextIndex < track.Length && track[nextIndex] < songPositionInBeats + beatsShownInAdvance)
        {
            //initialize the fields of the music note
            MusicNote musicNote = ((GameObject)Instantiate(notePrefab, Vector2.zero, Quaternion.identity)).GetComponent<MusicNote>();
            //Debug.Log(songPositionInBeats);

            musicNote.initialise(this, track[nextIndex], transform.position);

            notesOnscreen.Enqueue(musicNote);

            nextIndex++;
        }
    }


    public void HitNote()
    {
        tc.Pusle();

        if(notesOnscreen.Count>0)
        {
            MusicNote frontNote = notesOnscreen.Peek();

            float offset = Mathf.Abs(frontNote.gameObject.transform.position.x - Target.position.x);
            Debug.Log(offset);

            if(offset <=offsetToTarget)
            {
                BeatMatch(frontNote);
            }
            
        }
    }

    void BeatMatch(MusicNote frontNote)
    {
        if (Mathf.Abs(frontNote.noteBeat - songPositionInBeats) < beatOffset)
        {
            Debug.Log("HitTarget");
            Debug.Log(frontNote.noteBeat + "Current Beat is " + songPositionInBeats);

            //Destroy and remove note from list
            notesOnscreen.Dequeue();

            Destroy(frontNote.gameObject);
            
            //pass
            StartCoroutine(scoreCo(true));
        }
    }

    IEnumerator scoreCo(bool hit)
    {

            scoreText.GetComponent<TextMeshProUGUI>().text = "HIT";

        scoreText.transform.position = scoreStartPos;

        scoreText.SetActive(true);
        LeanTween.cancel(scoreText);

        scoreText.LeanMoveY(scoreText.transform.position.y + 25, 0.5f).setEase(LeanTweenType.easeInQuad);
        yield return new WaitForSeconds(0.6f);
        scoreText.SetActive(false);

        
    }
}
