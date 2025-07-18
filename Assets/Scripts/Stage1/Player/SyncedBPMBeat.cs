using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SyncedBPMBeat : MonoBehaviour
{
    public AudioSource audioSource;
    public float bpm = 145f;
    public BeatIndicatorUI beatUI;
    public TimingGrade lastTimingGrade;
    private HashSet<int> inputBeats = new HashSet<int>();
    private bool lastBeatSkipped = false;
    private bool wasBeatTriggered = false;
    private bool wasBeatHit = false;
    [SerializeField] private BackgroundAudioSelector backgroundAudioSelector;

    private float beatInterval;
    private int currentBeatWindow = 0;
    private Coroutine beatCoroutine;

    [SerializeField] private float perfectWindow = 0.05f;
    [SerializeField] private float goodWindow = 0.07f;
    [SerializeField] private float badWindow = 0.08f;

    void Start()
    {
        if (backgroundAudioSelector != null)
        {
            backgroundAudioSelector.onSongChanged += OnSongChanged;
        }
        backgroundAudioSelector.PlaySong(0);
    }

    public enum TimingGrade
    {
        Perfect,
        Good,
        Bad,
        Miss,
        None
    }

    public float BeatInterval => beatInterval;

    void Update()
    {
        if (!audioSource.isPlaying) return;

        // Calculate how many beats have occurred since the audio started
        int currentWholeBeat = Mathf.FloorToInt(audioSource.time / beatInterval);
        int calculatedBeatWindow = Mathf.RoundToInt(audioSource.time / beatInterval);
        // Check for exact beat moment
        if (!wasBeatTriggered && currentWholeBeat == currentBeatWindow)
        {
            TriggerBeatUI();
            wasBeatTriggered = true;
        }
        // Check for new beat window
        else if (calculatedBeatWindow != currentBeatWindow)
        {
            //Debug.Log("NEW BEAT WINDOW: " + calculatedBeatWindow);
            CheckIfBeatWindowSkipped();
            currentBeatWindow = calculatedBeatWindow;
            wasBeatHit = false;
            wasBeatTriggered = false;
        }

    }

    public TimingGrade GetTimingRating()
    {
        float inputTime = audioSource.time;
        int inputBeatWindow = currentBeatWindow;
        inputBeats.Add(inputBeatWindow);
        //Debug.Log("BEAT INPUT AT : " + inputBeatWindow);
        float beatTime = inputBeatWindow * beatInterval;
        float offset = Mathf.Abs(inputTime - beatTime);

        if (!wasBeatHit) {
            if (offset <= perfectWindow)
            {
                lastTimingGrade = TimingGrade.Perfect;
                beatUI.ShowPerfectRating();
                wasBeatHit = true;
                return TimingGrade.Perfect;
            }
            else if (offset <= goodWindow)
            {
                lastTimingGrade = TimingGrade.Good;
                beatUI.ShowGoodRating();
                wasBeatHit = true;
                return TimingGrade.Good;
            }
            else if (offset <= badWindow)
            {
                lastTimingGrade = TimingGrade.Bad;
                beatUI.ShowOKRating();
                wasBeatHit = true;
                return TimingGrade.Bad;
            }
            else
            {
                //Debug.Log(offset);
                lastTimingGrade = TimingGrade.Miss;
                beatUI.ShowMissRating();
                return TimingGrade.Miss;
            }
        }
        else
        {
            //Debug.Log("last beat was attempted??");
            lastTimingGrade = TimingGrade.Miss;
            beatUI.ShowMissRating();
            return TimingGrade.Miss;
        }
    }

    void CheckIfBeatWindowSkipped()
    {
        if (!inputBeats.Contains(currentBeatWindow))
        {
            lastTimingGrade = TimingGrade.None;
            lastBeatSkipped = true;
            //Debug.Log("SKIPPED BEAT WINDOW: " + currentBeatWindow);
        }
        else
        {
            lastBeatSkipped = false;
        }
        inputBeats.Remove(currentBeatWindow);
    }

    void TriggerBeatUI()
    {
        beatUI.TurnGreen();
        if (beatCoroutine != null)
        {
            StopCoroutine(beatCoroutine);
        }
        beatCoroutine = StartCoroutine(TurnRedAfterDelay(badWindow));
    }

    System.Collections.IEnumerator TurnRedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        beatUI.TurnBlack();
    }

    public bool WasLastBeatSkipped()
    {
        return lastBeatSkipped;
    }

    public void ClearBeatIndicatorText()
    {
        beatUI.ShowNoRating();
    }

    private void OnSongChanged(float newBPM)
    {
        beatUI.TurnBlack();
        lastBeatSkipped = false;
        wasBeatTriggered = false;
        wasBeatHit = false;
        currentBeatWindow = 0;
        bpm = newBPM;
        beatInterval = 60f / bpm;
        lastTimingGrade = TimingGrade.None;
        ClearBeatIndicatorText();
        inputBeats.Clear();
    }

    void OnDestroy()
    {
        if (backgroundAudioSelector != null)
        {
            backgroundAudioSelector.onSongChanged -= OnSongChanged;
        }

    }
}