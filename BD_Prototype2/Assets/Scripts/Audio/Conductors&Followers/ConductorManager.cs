using System.Collections.Generic;
using UnityEngine;

public class ConductorManager : MonoBehaviour
{
    public static ConductorManager Instance { get; private set; }

    [SerializeField] private List<MusicConductor> _controllersInScene = new List<MusicConductor>();
    private List<MusicConductor> _activeControllers = new List<MusicConductor> ();
    private MusicConductor _currentController;


    private void Awake()
    {
        //as its on AudioManager which is DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Instance._controllersInScene = this._controllersInScene;
            return;
        }


    }

    public MusicConductor GetCurrentController()
    {
        return _currentController;
    }

    public void AddController(MusicConductor controller, MusicManager.TransitionType startTranstition)
    {
        if (_activeControllers.Contains(controller))
        {
            return;
        }

        _activeControllers.Add(controller);

        //go through all controllers currently active
        for (int i = 0; i < _controllersInScene.Count; i++)
        {
            //controllersInScene are prioritized, so find the highest prio of the currently active controllers
            if (_activeControllers.Contains(_controllersInScene[i]))
            {
                //if highest prio is this newly added one, let it take control
                if (_controllersInScene[i] == controller)
                {
                    _currentController = controller;
                    _currentController.WhenTakingControl();
                    MusicManager.Instance.SwitchMusic(startTranstition);
                }
                //otherwise leave the currentcontroller to stay the same as before
                return;
            }
        }
    }


    public void RemoveController(MusicConductor controller, MusicManager.TransitionType leaveTranstition)
    {
        _activeControllers.Remove(controller);

        //if this was the one in control
        if (controller == _currentController)
        {
            //if there are other Controllers that can take over
            if (_activeControllers.Count > 0)
            {
                //find the new highest prio controller
                for (int i = 0; i < _controllersInScene.Count; i++)
                {
                    if (_activeControllers.Contains(_controllersInScene[i]))
                    {
                        _currentController = _controllersInScene[i];
                        MusicManager.Instance.SwitchMusic(leaveTranstition);
                        return;
                    }
                }
            }
            //if no other controllers, just silence
            else
            {
                MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.QUEUE_STOP);
                print("There's no controller! UwU /Definitely not Mohammed");
                _currentController = null;
            }
            
        }


    }
}
