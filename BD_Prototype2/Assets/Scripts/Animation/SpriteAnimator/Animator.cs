using UnityEngine;
using UnityEngine.U2D.Animation;

using BulletDance.Graphics;


namespace BulletDance.Animation
{

    //Namespace enums
    public enum Direction { 
        Front = 0, Back = 1, Left = 2, Right = 3 , 
        LeftDown = 4 , LeftUp = 5 , RightDown = 6 , RightUp = 7 };

    public enum AnimAction { 
        Idle = 0, Walk = 1, Dash = 2, Attack = 3, 
        Defeat = 50 };


//Any Sprite Animation WILL require these components
[ExecuteAlways]
[RequireComponent(typeof(SpriteLibrary))]
public class Animator : MonoBehaviour
{
    // -- Set up -- //

    //To change Sprite library
    [SerializeField]
    protected SpriteLibraryGroupAsset _spriteGroupAsset;
    [SerializeField]
    protected SpriteLibrary      _spriteLib;
    protected SpriteLibraryAsset _curLib  = null;

    protected void OnEnable()
    {
        GetReferences();

        //For some reason GetComponent doesn't work for SpriteLibrary???
        if(_spriteLib == null)
            _spriteLib = GetComponentInChildren<SpriteLibrary>();

        if(_spriteGroupAsset != null)
            Initialize();
    }

    protected virtual void GetReferences() {}


    //Set default Sprite Group and default Sprite Library
    public string currentLibrary { get; private set; }
    public void Initialize()
    {
        _spriteGroupAsset.CreateSpriteLibraryLookup();

        //Set first sprite library as default
        SpriteLibraryGroup defaultLibGroup = _spriteGroupAsset.libraries[0];
        _curLib  = defaultLibGroup.spriteLibraries[0].spriteLibraryAsset;
        _spriteLib.spriteLibraryAsset = _curLib;

        currentLibrary = defaultLibGroup.name;
    }


    // -- AnimState tracker -- //
    /* Keep track of which kind of animation is playing */

    public int animState { get { return GetState(); } set { SetState(value); } }
    public virtual int  GetState() { return 0; }
    public virtual void SetState(int state) { SetInt("State", state); }
    protected int _prevAnimState;

    /// <summary> Check if animation Action has changed </summary>
    public bool hasActionChanged() {  return animState != _prevAnimState;  }

    /// <summary> Check if animation Action has changed from a specified Action/State </summary>
    public bool hasActionChangedFrom(AnimAction animationAction)
    {
        int _animationState = (int)animationAction;
        return animState != _animationState && _prevAnimState == _animationState;
    }

    /// <summary> Check if animation Action has changed from a specified Action/State </summary>
    /// <param name="animationState">int value</param>
    public bool hasActionChangedFrom(int animationState)
    {
        return animState != animationState && _prevAnimState == animationState;
    }

    void Update()
    {
        if(hasActionChanged())
            AnimActionChanged();

        _prevAnimState = animState;
    }

    public event System.Action OnAnimActionChanged;
    protected void AnimActionChanged()
    {
        OnAnimActionChanged?.Invoke();
    }


    // -- Setting sprite library -- //
    /* Used to change phase & direction of sprite */

    protected Direction _direction = Direction.Front;

    /// <summary>
    /// Set Sprite Library to a specified Direction
    /// Does not change the current State/Phase
    /// </summary>
    /// <param name="newDirection">Type SpriteAnimator.Direction</param>
    public void SetDirection(Direction newDirection)
    {
        if(_spriteGroupAsset == null || newDirection == _direction) return;

        //Set new direction
        _direction = newDirection;

        //Set appropriate sprite library
        _curLib = _spriteGroupAsset.GetLibrary(currentLibrary, _direction.ToString());
        _spriteLib.spriteLibraryAsset = _curLib;
    }


    /// <summary>
    /// Set new Sprite Group by Phase number
    ///  Also sets Sprite Library by the current direction
    /// </summary>
    public void SetLibraryByPhase(int phaseNumber)
    {
        string libraryName = "Phase " + phaseNumber.ToString();
        SetLibraryByName(libraryName);
    }

    /// <summary>
    /// Set new Sprite Group by name/currentLibrary
    ///  Also sets Sprite Library by the current direction
    /// </summary>
    public void SetLibraryByName(string libraryName)
    {
        if(_spriteGroupAsset == null || libraryName == currentLibrary) return;

        currentLibrary = libraryName;
        _curLib = _spriteGroupAsset.GetLibrary(currentLibrary, _direction.ToString());
        _spriteLib.spriteLibraryAsset = _curLib;
    }


    // -- Animator wrapper -- //

    protected virtual int GetLayerIndex(string layerName)
    {
        return 0;
    }

    /// <summary> Sets a layer's weight (override/additive influence) </summary>
    public virtual void SetLayerWeight(string layerName, float weight) {}

    /// <summary> Sets speed for the animator </summary>
    public virtual void SetSpeed(float speed) {}

    /// <summary> Get playback point of the current AnimationState on layer [layerName] </summary>
    public virtual float GetStatePlaybackPoint(string layerName)
    {
        return 0;
    }


    /// <summary> Same as Animator.SetTrigger </summary>
    /// <param name="triggerName">Name of Trigger parameter in Animator Controller</param>
    public virtual void SetTrigger(string triggerName) {}

    /// <summary> Same as Animator.SetBool </summary>
    /// <param name="boolName">Name of Bool parameter in Animator Controller</param>
    /// <param name="value">true or false</param>
    public virtual void SetBool(string boolName, bool value) {} 

    /// <summary> Same as Animator.SetInteger </summary>
    /// <param name="intName">Name of Int parameter in Animator Controller</param>
    /// <param name="value">int value</param>
    public virtual void SetInt(string intName, int value) {}

    /// <summary> Same as Animator.SetFloat </summary>
    /// <param name="floatName">Name of Float parameter in Animator Controller</param>
    /// <param name="value">float value</param>
    public virtual void SetFloat(string floatName, float value) {}


    /// <summary> Start animation </summary>
    /// <param name="animationName">Animation Name</param>
    /// <param name="playAtPoint">The point where animation starts to play. Default 0 to play at the beginning</param>
    /// <param name="layerName">A specific layer where the animation is. Default "" to find first matching name</param>
    public virtual void Anim(string animationName, float playAtPoint = 0, string layerName = "") {}

    /// <summary> Start animation </summary>
    public virtual void Anim(AnimAction animationAction)
    {
        animState = (int)animationAction;
    }

    /// <summary> Start animation </summary>
    public virtual void Anim(int animationState)
    {
        animState = animationState;
    }

    public virtual void AnimIdle()
    {
        animState = (int)AnimAction.Idle;
    }

    public virtual void AnimWalk()
    {
        animState = (int)AnimAction.Walk;
    }

    public virtual void AnimDash()
    {
        animState = (int)AnimAction.Dash;
    }

    public virtual void AnimAttack()
    {
        animState = (int)AnimAction.Attack;
    }

    public virtual void AnimDefeat()
    {
        animState = (int)AnimAction.Defeat;
    }

    public virtual void AnimHurt()
    {
        SetTrigger("Hurt");        
    }


    public virtual void UpdateInEditor(float time){}
}


}