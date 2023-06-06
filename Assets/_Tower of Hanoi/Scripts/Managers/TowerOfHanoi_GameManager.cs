using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TowerOfHanoi_GameManager : MonoBehaviour
{
    public enum GameMode { NORMAL_MODE = 0, ENDLESS_MODE = 1 };

    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;
    public UnityEvent OnEndlessGameStart;
    public UnityEvent OnEndlessGameSolvedSet;
    public UnityEvent OnEndlessGameNewSet;
    public UnityEvent OnEndlessGameEnd;

    public GameMode currentGameMode = GameMode.NORMAL_MODE;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private AudioClip winAudio;
    [SerializeField] private float elapsedTime = 0f;

    [Header("Pillars")]
    [SerializeField] private Pillar initialPillar;
    [SerializeField] private Pillar targetPillar;
    [SerializeField] private Pillar nextInitialPillar;
    [SerializeField] private List<Pillar> pillars;

    [Header("Rings")]
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private int numOfRings = 3;
    [SerializeField] private int maxRings;
    [SerializeField] private int minRings;
    [SerializeField] private float maxRingHeight;
    [SerializeField] private float minRingSize;
    [SerializeField] private float maxRingSize;

    [Header("Endless Mode Settings")]
    [SerializeField] private float timer = 30f;
    [SerializeField] private float additionalTime;
    [SerializeField] private AnimationCurve scalingValue;
    [SerializeField] private float currDifficulty, maxDifficulty;
    [SerializeField] private int endlessModeNumOfRings = 3;
    [SerializeField] private AudioClip solvedAudio;
    [SerializeField] private AudioClip endlessModeLoseAudio;
    [SerializeField] private int score = 0;

    private bool start;
    private bool settingUp;
    private float maxTime;

    public float GetElapsedTime() => elapsedTime;
    public float GetTimer() => timer;
    public int GetScore() => score;

    private void Awake()
    {
        maxTime = timer;
        numOfRings = minRings;
    }

    private void Update()
    {
        if (start == false) return;

        switch (currentGameMode)
        {
            case GameMode.NORMAL_MODE: NormalMode(); break;
            case GameMode.ENDLESS_MODE: EndlessMode(); break;
        }
    }

    private void NormalMode()
    {
        if (CheckWinCondition(numOfRings))
        {
            elapsedTime += Time.deltaTime;
            StartCoroutine(GameEnd());
            start = false;
        }
    }

    private void EndlessMode()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            StartCoroutine(EndlessGameEnd());
            start = false;
            return;
        }

        if (CheckWinCondition(endlessModeNumOfRings))
        {
            if (settingUp) return;

            OnEndlessGameSolvedSet?.Invoke();
            EndlessGameNewSet();
        }
    }

    private bool CheckWinCondition(int ringCount)
    {
        if (targetPillar.stack.Count > 0 && targetPillar.stack.Count == ringCount)
        {
            Debug.Log("WIN");
            return true;
        }

        return false;
    }

    private void Setup(int ringCount)
    {
        elapsedTime = 0;

        foreach (Pillar pillar in pillars)
        {
            pillar.IsTarget = false;
            pillar.ResetMaterial();

            if (pillar.stack.Count == 0) continue;

            for (int i = pillar.stack.Count - 1; i >= 0; i--)
            {
                Ring ring = pillar.stack[i];

                pillar.stack.Remove(ring);
                Destroy(ring.gameObject);
            }
        }

        List<Pillar> pillarHolder = new List<Pillar>(pillars);

        if (initialPillar == null)
            initialPillar = pillarHolder[Random.Range(0, pillarHolder.Count)];
        else
        {
            foreach (Pillar pillar in pillarHolder)
            {
                if (pillar == nextInitialPillar && pillar == targetPillar) continue;

                initialPillar = nextInitialPillar;
            }
        }

        StartCoroutine(CreateRings(initialPillar, ringCount));
        pillarHolder.Remove(initialPillar);

        targetPillar = pillarHolder[Random.Range(0, pillarHolder.Count)];
        targetPillar.IsTarget = true;
        targetPillar.ResetMaterial();
        pillarHolder.Remove(targetPillar);

        nextInitialPillar = pillarHolder[0];
        start = true;
        settingUp = false;
    }

    private IEnumerator CreateRings(Pillar pillar, int ringCount)
    {
        ringCount = Mathf.Clamp(ringCount, minRings, maxRings);

        float maxPillarHeight = initialPillar.transform.lossyScale.y;
        float height = (maxPillarHeight / maxRings * maxRings) > maxRingHeight ?
            maxRingHeight : (maxPillarHeight / maxRings * maxRings);
        float radiusInterval = (maxRingSize - minRingSize) / maxRings;
        float prevRingSize = maxRingSize;

        for (int i = 0; i < ringCount; i++)
        {
            if (prevRingSize <= minRingSize)
                prevRingSize = minRingSize;

            GameObject obj = Instantiate(ringPrefab, initialPillar.topPos.position, Quaternion.identity);
            obj.transform.localScale = new Vector3(prevRingSize, height, prevRingSize);

            Ring ring = obj.GetComponent<Ring>();
            ring.RingOrder = i;
            ring.AssignRandomColor();
            pillar.PlaceRing(ring);
            prevRingSize -= radiusInterval;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator GameEnd()
    {
        AudioManager.Instance.PlayAudio(winAudio);
        yield return new WaitForSeconds(1.5f);

        OnGameEnd?.Invoke();

        initialPillar = null;
        targetPillar = null;
    }

    private IEnumerator CreateNewSet()
    {
        settingUp = true;
        score++;

        float normalizedDifficulty = endlessModeNumOfRings / maxRings;
        timer += (additionalTime * scalingValue.Evaluate(normalizedDifficulty));

        endlessModeNumOfRings += 1;
        AudioManager.Instance.PlayAudio(solvedAudio);
        yield return new WaitForSeconds(1.5f);

        initialPillar = null;
        targetPillar = null;
        Setup(endlessModeNumOfRings);
    }

    private IEnumerator EndlessGameEnd()
    {
        AudioManager.Instance.PlayAudio(endlessModeLoseAudio);
        yield return new WaitForSeconds(1.5f);

        OnEndlessGameEnd?.Invoke();
        initialPillar = null;
        targetPillar = null;
    }

    public void EndlessGameNewSet()
    {
        OnEndlessGameNewSet?.Invoke();
        StartCoroutine(CreateNewSet());
    }

    public void EndlessGameStart()
    {
        score = 0;
        timer = maxTime;
        Setup(endlessModeNumOfRings);
        OnGameStart?.Invoke();
    }

    public void GameStart()
    {
        Setup(numOfRings);
        OnGameStart?.Invoke();
    }

    public void SetNumberOfRings(string ringCount)
    {
        int.TryParse(ringCount, out int rings);
        numOfRings = rings;
        numOfRings = Mathf.Clamp(numOfRings, minRings, maxRings);
        inputField.text = numOfRings.ToString();
    }

    public void AddNumberOfRings(int additionalRing)
    {
        numOfRings += additionalRing;
        numOfRings = Mathf.Clamp(numOfRings, minRings, maxRings);
        inputField.text = numOfRings.ToString();
    }


    public void SelectGameMode(int mode)
    {
        currentGameMode = (GameMode)mode;
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
