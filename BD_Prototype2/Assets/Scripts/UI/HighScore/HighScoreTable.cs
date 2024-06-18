using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreTable : MonoBehaviour
{

    Transform entryContainer;
    Transform entryTemplate;

    List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainer.Find("HighScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey("HighscoreTable"))
        {
            string json = JsonUtility.ToJson(new Highscores());
            PlayerPrefs.SetString("HighscoreTable", json);
            PlayerPrefs.Save();
        }

        string jsonString = PlayerPrefs.GetString("HighscoreTable");
        Highscores highscore = JsonUtility.FromJson<Highscores>(jsonString);

        highscore.highScoreEntryList.Sort((x, y) => y.score.CompareTo(x.score));

        highscoreEntryTransformList = new List<Transform>();

        foreach(HighScoreEntry highscoreEntry in highscore.highScoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    RefreshHighScore();
        //}
    }

    void RefreshHighScore()
    {
        highscoreEntryTransformList.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("HighScoreEntry"))
        {
            Destroy(obj);
        }

        string json = JsonUtility.ToJson(new Highscores());
        PlayerPrefs.SetString("HighscoreTable", json);
        PlayerPrefs.Save();
    }

    void CreateHighscoreEntryTransform(HighScoreEntry highscoreEntry, Transform container, List<Transform> transformList)
        {
            float templateHeight = 70f;

            Transform entryTransform = Instantiate(entryTemplate, container);
            RectTransform entryrectTransform = entryTransform.GetComponent<RectTransform>();
            entryrectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            string rankstring;

            switch (rank)
            {
                default:
                    rankstring = rank + "TH";
                    break;
                case 1:
                    rankstring = "1ST";
                    break;
                case 2:
                    rankstring = "2ND";
                    break;
                case 3:
                    rankstring = "3RD";
                    break;
            }

            entryTransform.Find("PosText").GetComponent<TextMeshProUGUI>().text = rankstring;


            int score = highscoreEntry.score;
            entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

            string name = highscoreEntry.name;
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = name;

            transformList.Add(entryTransform);
        }

    void AddHighscoreEntry(int score, string name)
    {
        //Create highscoreEntry
        HighScoreEntry highscoreEntry = new HighScoreEntry {score = score, name = name };

        //Load saved HighScores
        string jsonString = PlayerPrefs.GetString("HighscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        //Add new entry
        highscores.highScoreEntryList.Add(highscoreEntry);

        //save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("HighscoreTable", json);
        PlayerPrefs.Save();

        CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);

        
    }

    private class Highscores
    {
        public List<HighScoreEntry> highScoreEntryList;
    }

    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }

}


