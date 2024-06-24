using System.Collections.Generic;
using UnityEngine;

public class BeatMapReader : MonoBehaviour
{

    //Player

    public static void ReadBeatMapPlayer(Row[] sheet, List<int> beatList, int lastBeatOfSequence)
    {
        for (int y = 0; y < sheet.Length; y++)
        {
            for (int x = 1; x < 9; x++)
            {
                //Color b = sheet[y].notes[x - 1].color;

                //if (b != Color.white)
                //{
                //    beatList.Add((x) + (y * 8) + lastBeatOfSequence);
                //}

                if (sheet[y].notes[x - 1].forPlayer)
                {
                    beatList.Add((x) + (y * 8) + lastBeatOfSequence);
                }
            }
        }

    }




    public static void ReadBeatMapsFollower(FollowSequence[] sequences)
    {
        //For each sequence
        for (int seq = 0; seq < sequences.Length; seq++)
        {
            List<(int, Note)> beats = new List<(int, Note)>();

            //Finding right beat
            for (int y = 0; y < sequences[seq].sheet.Length; y++)
            {
                for (int x = 1; x < 9; x++)
                {
                    if (sequences[seq].sheet[y].notes.Length == 0)
                        break;
                    Note note = sequences[seq].sheet[y].notes[x - 1];
                    if (note.functionName == " - " || note.functionName == "" || note.functionName == " ")
                    {
                        continue;
                    }

                    int beat = (x) + (y * 8);
                    beats.Add((beat, note));

                }
            }
            sequences[seq].noteList = beats;
        }
    }

    public static void ReadBeatMapsBoss(BossConductor.Phase[] phases)
    {
        //For each phase
        for (int p = 0; p < phases.Length; p++)
        {
            BossConductor.Phase phase = phases[p];

            //for transitions
            if (phase.transition == true)
            {
                for (int seq = 0; seq < phase.sequences.Length; seq++)
                {
                    phase.sequences[seq].forCutscene = true;
                    phase.sequences[seq].lastCutscene = phase.lastTransition;
                    phase.sequences[seq].cutsceneName = phase.cutsceneName;
                }
                continue;
            }

            //For each sequence (and beatmap)
            for (int seq = 0; seq < phase.sequences.Length; seq++)
            {
                List<(int, Note)> beats = new List<(int, Note)>();

                //Finding right beat
                for (int y = 0; y < phase.sequences[seq].sheet.Length; y++)
                {
                    for (int x = 1; x < 9; x++)
                    {
                        Note note = phase.sequences[seq].sheet[y].notes[x - 1];
                        if (note.functionName == " - " || note.functionName == "" || note.functionName == " ")
                        {
                            continue;
                        }

                        int beat = (x) + (y * 8);
                        beats.Add((beat, note));

                    }
                }
                phase.sequences[seq].noteList = beats;

                //weights
                if (phase.sequencePlayedFirst == "")
                {
                    phase.sequences[seq].weight = 100.0f / phase.sequences.Length;
                }
                else
                {
                    //if theres one that should be played first
                    if (phase.sequencePlayedFirst == phase.sequences[seq].name)
                    {
                        phase.sequences[seq].weight = 100.0f;
                    }
                    else
                    {
                        phase.sequences[seq].weight = 0f;
                    }
                }
            }
        }

    }



    //Old pixel based



    /// <summary> 
    /// Finds the pixels on the beat map that corresponds with a specific action, then add said action to the beats-list which is
    /// then referenced in CheckBeats() which invokes the action on the correct beats 
    /// </summary>
    public static void ReadBeatMapsFollower(FollowSequence[] sequences, BeatAction[] beatActions)
    {
        //For each phase
        for (int seq = 0; seq < sequences.Length; seq++)
        {
            List<(int, BeatAction)> beats = new List<(int, BeatAction)>();

            //Finding right pixel
            for (int y = 0; y < sequences[seq].beatMap.height; y++)
            {
                for (int x = 0; x < sequences[seq].beatMap.width; x++)
                {
                    Color color = sequences[seq].beatMap.GetPixel(x, y);

                    if (color.a == 0)
                    {
                        continue;
                    }

                    //Finding right beatAction
                    for (int i = 0; i < beatActions.Length; i++)
                    {
                        BeatAction action = beatActions[i];

                        if (color.r == action.color.r &&
                            color.g == action.color.g &&
                            color.b == action.color.b)
                        {
                            //get number of beat
                            int beat = (x + 1) + (y * sequences[seq].beatMap.width);

                            //get variation
                            for (int v = 0; v < action.allVariations.Length; v++)
                            {
                                //check if pixels alpha is close variant alpha value
                                if (Mathf.Abs(color.a - action.allVariations[v].alpha) < 0.05f)
                                {
                                    action.variation = action.allVariations[v];
                                    break;
                                }

                                //last one and still not correct, just take the first one
                                if (v == action.allVariations.Length - 1)
                                {
                                    action.variation = action.allVariations[0];
                                }
                            }

                            //add it
                            beats.Add((beat, action));

                            break;
                        }
                    }
                }
            }
            sequences[seq].beatList = beats;
        }
    }

    public static void ReadBeatMapsBoss(BossConductor.Phase[] phases, BeatAction[] beatActions)
    {
        //For each phase
        for (int p = 0; p < phases.Length; p++)
        {
            BossConductor.Phase phase = phases[p];

            //for transitions
            if (phase.transition == true)
            {
                for (int seq = 0; seq < phase.sequences.Length; seq++)
                {
                    phase.sequences[seq].forCutscene = true;
                    phase.sequences[seq].lastCutscene = phase.lastTransition;
                    phase.sequences[seq].cutsceneName = phase.cutsceneName;
                }
                continue;
            }

            //For each sequence (and beatmap)
            for (int seq = 0; seq < phase.sequences.Length; seq++)
            {
                List<(int, BeatAction)> beats = new List<(int, BeatAction)>();

                //Finding right pixel
                for (int y = 0; y < phase.sequences[seq].beatMapActions.height; y++)
                {
                    for (int x = 0; x < phase.sequences[seq].beatMapActions.width; x++)
                    {
                        Color color = phase.sequences[seq].beatMapActions.GetPixel(x, y);

                        if (color.a == 0)
                        {
                            continue;
                        }

                        //Finding right beatAction
                        for (int i = 0; i < beatActions.Length; i++)
                        {
                            BeatAction action = beatActions[i];

                            if (color.r == action.color.r &&
                                color.g == action.color.g &&
                                color.b == action.color.b)
                            {
                                //get number of beat
                                int beat = (x + 1) + (y * phase.sequences[seq].beatMapActions.width);

                                //get variation
                                for (int v = 0; v < action.allVariations.Length; v++)
                                {
                                    //check if pixels alpha is close variant alpha value
                                    if (Mathf.Abs(color.a - action.allVariations[v].alpha) < 0.05f)
                                    {
                                        action.variation = action.allVariations[v];
                                        break;
                                    }

                                    //last one and still not correct, just take the first one
                                    if (v == action.allVariations.Length - 1)
                                    {
                                        action.variation = action.allVariations[0];
                                    }
                                }

                                //add it
                                beats.Add((beat, action));

                                break;
                            }
                        }
                    }
                }

                phase.sequences[seq].beatList = beats;

                //weights
                if (phase.sequencePlayedFirst == "")
                {
                    phase.sequences[seq].weight = 100.0f / phase.sequences.Length;
                }
                else
                {
                    //if theres one that should be played first
                    if (phase.sequencePlayedFirst == phase.sequences[seq].name)
                    {
                        phase.sequences[seq].weight = 100.0f;
                    }
                    else
                    {
                        phase.sequences[seq].weight = 0f;
                    }
                }
            }
        }

    }
}
