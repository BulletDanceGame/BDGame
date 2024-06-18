using UnityEngine;

public class MusicConductor : MonoBehaviour
{
    protected MusicSequence _nextSequence;


    //NEEDs to be expanded in child classes, to allow Music Manager to get the sequences
    public virtual MusicSequence GetNextSequence()
    {
        return _nextSequence;
    }


    // If anything should happen when the conductor takes control
    public virtual void WhenTakingControl()
    {

    }

    //If anything should happen when the sequence starts, if it starts playing later
    public virtual void SequenceHasStarted()
    {

    }

}
