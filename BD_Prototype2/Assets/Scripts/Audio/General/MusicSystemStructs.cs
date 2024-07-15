using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;


//BEAT ACTION
//The Action connected to each Beat
//Contains a color that through the beat maps allows a function of the functionName to be called on the correct beats

[System.Serializable]
public struct BeatAction
{
    public string name; //functionName
    public Color color;

    [System.Serializable]
    public struct Variation
    {
        public string name;
        [Range(0.0f, 1.0f)]
        public float alpha;
        public BulletBag.BulletTypes[] bullets;
    }
    public Variation[] allVariations;
    [HideInInspector] public Variation variation;
}


public struct Sequence
{

}


//SEQUENCE
//A Music Sequence is a part of a song with information about what happens during that song
//Its BossMusicControllers and EnvironmentMusicControllers MusicSequences that tell the MusicManager what song should be played
//Contains a song, beatmaps describing what the boss and player does on each beat, and other information about how the sequence. 
//Contains a song name and a beat map, which allows it to know which BeatActions should happen on which beats
[System.Serializable]
public struct MusicSequence
{
    public string name;
    public AK.Wwise.Event song;
    public Texture2D beatMapActions;
    public Texture2D beatMapPlayer;
    public int duration;
    public int bpm;
    public bool stoppedEarly;
    public bool keepPlayingOnSwitch;
    public bool replayInSameSequence;

    public Row[] sheet;
    [HideInInspector] public List<(int, Note)> noteList;
    public Note infoNote;

    //what beat action happens on each beat
    [HideInInspector] public List<(int, BeatAction)> beatList;

    [HideInInspector] public float weight;
    [HideInInspector] public bool forCutscene;
    [HideInInspector] public bool lastCutscene;
    [HideInInspector] public string cutsceneName;
}

//Follow SEQUENCE
//Is not used to start sequences and songs, but used to follow sequences that are played from the main controllers
[System.Serializable]
public struct FollowSequence
{
    public string name;
    public Texture2D beatMap;
    public bool followSpecificSong;
    public AK.Wwise.Event songToFollow;
    public int duration;


    [HideInInspector] public List<(int, BeatAction)> beatList;


    public Row[] sheet;
    [HideInInspector] public List<(int, Note)> noteList;
    public Note infoNote;
};



[System.Serializable]
public class Note
{
    public string functionName;
    public Color color;

    public bool forPlayer;

    public BulletBag.BulletTypes[] bullets;

    public enum MovelistsToTrigger {  RandomizeBetweenAll, RandomizeBetweenSpecified, AllSpecified, All }
    public MovelistsToTrigger movelistsToTrigger;
    public int amountToRandomizeBetween;
    public Movelist[] specifiedMovelists;

    //for inspector
    public VisualElement rowNote;
}



[System.Serializable]
public class Row
{
    public string barNr;

    public Note[] notes;

    public VisualElement seq;
}


