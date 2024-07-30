using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Options : MonoBehaviour
{

    [SerializeField] private MainMenu menu;
    [SerializeField] private PauseScreen pause;


    [Header("FPS")]
    [SerializeField] private TextMeshProUGUI _fpsText;

    [Header("Rhythm Difficulty")]
    [SerializeField] private TextMeshProUGUI _rhythmDifficultyText1;
    [SerializeField] private TextMeshProUGUI _rhythmDifficultyText2;

    private void Start()
    {

        _fpsText.text = MusicManager.Instance.maxFPS.ToString();


        RhythmDifficultyText(PlayerRhythm.Instance.rhythmDifficulty);

    }



    public void BackToMenu()
    {

        SaveSystem.Instance.Save();
        menu.BackFromOptions();
    }


    public void BackToPauseMenu()
    {
        SaveSystem.Instance.Save();
        pause.BackFromOptions();
    }


    public void ChangeFPS(int dir)
    {

        int fps = MusicManager.Instance.maxFPS;
        fps += dir * 10;
        fps = Mathf.Clamp(fps, 30, 300);
        MusicManager.Instance.maxFPS = fps;

        SaveSystem.Instance.GetData().maxFPS = fps;

        _fpsText.text = fps.ToString();

    }


    public void ChangeRhythmDifficulty(int dir)
    {

        RhythmDifficulty difficulty = PlayerRhythm.Instance.rhythmDifficulty;

        if ((int)difficulty + dir < 0 || (int)difficulty + dir > 2)
        {
            return;
        }

        difficulty += dir;
        PlayerRhythm.Instance.rhythmDifficulty = difficulty;

        SaveSystem.Instance.GetData().rhythmDifficulty = (int)difficulty;

        RhythmDifficultyText(difficulty);

        if (difficulty == RhythmDifficulty.normal)
        {
            PlayerRhythm.Instance.perfectHitTime = 0.06;
            PlayerRhythm.Instance.okayHitTime = 0.12;
        }
        else if (difficulty == RhythmDifficulty.easy)
        {
            PlayerRhythm.Instance.perfectHitTime = 0.1;
            PlayerRhythm.Instance.okayHitTime = 0.2;
        }
    }

    private void RhythmDifficultyText(RhythmDifficulty difficulty)
    {
        if (difficulty == RhythmDifficulty.normal)
        {
            _rhythmDifficultyText1.text = "Normal";
            _rhythmDifficultyText2.text = "The recommended way to play the game, you need to play by pressing to the rhythm";
        }
        else if (difficulty == RhythmDifficulty.easy)
        {
            _rhythmDifficultyText1.text = "Easy";
            _rhythmDifficultyText2.text = "For players that struggle with playing to the rhythm. Allows hits a bit further from the beat.";
        }
        else if (difficulty == RhythmDifficulty.removed)
        {
            _rhythmDifficultyText1.text = "No Rhythm";
            _rhythmDifficultyText2.text = "This is no longer a rhythm game. This is NOT the intended way to play this game. But hey, rhythm games aren't for everyone";
        }
    }

}
