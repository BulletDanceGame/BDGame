using System.Collections.Generic;
using UnityEngine;

public class LesserYokaiMovelist : SmallYokaiMovelist
{

    void Update()
    {
        if (_isActivate)
        {
            RandomWalk();
            Dash();

        }
    }

    private void DashTowardThePlayer()
    {
        _direction = 5;
        _dashTime = startDashTime;
        print(_direction);
        PlayerBasedRaycast();
    }

    private void DashAwayFromThePlayer()
    {
        _direction = 6;
        _dashTime = startDashTime;
        print(_direction);
        PlayerBasedRaycast();
    }

    private void DashToTheLeftOfThePlayer()
    {
        _direction = 7;
        _dashTime = startDashTime;
        print(_direction);
        PlayerBasedRaycast();
    }

    private void DashToTheRightOfThePlayer()
    {
        _direction = 8;
        _dashTime = startDashTime;
        print(_direction);
        PlayerBasedRaycast();
    }
}
