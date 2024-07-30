

[System.Serializable]
public class SaveData
{
    public int maxFPS = 120;
    public int volume = 100;

    public int rhythmDifficulty = 0;

    public bool haveCalibrated = false;
    public double visualOffset = 0, swingOffset = 0, dashOffset = 0;



    public bool haveStartedPlaying = false;

    //Level 0 = Menu
    public int currentLevel = 0;

    public float currentCheckpointX;
    public float currentCheckpointY;
    public float currentCheckpointZ;

    public bool hasBat;

    public bool hasplayed1stcutscene;
    public bool hasplayed2stcutscene;

    public bool bossdeath;
    public bool misscutsceneplayed=false;
}
