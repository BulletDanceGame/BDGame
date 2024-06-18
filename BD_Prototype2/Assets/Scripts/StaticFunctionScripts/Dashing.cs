using UnityEngine;

public class Dashing : MonoBehaviour
{
    // Will try and move all of the dash in here
    //Same as Shooting but for dashing for the enemies 
    //Dash to the player
    public static void DashTowardThePlayer(Transform dasher,float Speed, float Timer)
    {
        if(Timer > 0)
        {
            dasher.transform.position = Vector2.MoveTowards(dasher.transform.position, UnitManager.Instance.GetPlayer().transform.position, Speed * Time.deltaTime);
            Timer -= Time.deltaTime;
            print("To");

        }
    }

    public static void DashAwayFromThePlayer(Transform dasher, float Speed, float Timer)
    {
        if (Timer > 0)
        {
            dasher.transform.position = Vector2.MoveTowards(dasher.transform.position, UnitManager.Instance.GetPlayer().transform.position, -1 * Speed * Time.deltaTime);
            Timer -= Time.deltaTime;
            print("Away");
        }

    }

    public static void DashToTheRightOfThePlayer(Transform dasher, float Distance, float Speed, float Timer)
    {
        Vector2 DasherDes=new Vector2(UnitManager.Instance.GetPlayer().transform.position.x - Distance,
                UnitManager.Instance.GetPlayer().transform.position.y);
        if (Timer > 0)
        {
            dasher.transform.position = Vector2.MoveTowards(dasher.transform.position,DasherDes,Speed * Time.deltaTime);
            Timer -= Time.deltaTime;
        }

    }

    public static void DashToTheLeftOfThePlayer(Transform dasher, float Distance, float Speed, float Timer)
    {
        Vector2 DasherDes = new Vector2(UnitManager.Instance.GetPlayer().transform.position.x + Distance,
                UnitManager.Instance.GetPlayer().transform.position.y);
        if (Timer > 0)
        {
            dasher.transform.position = Vector2.MoveTowards(dasher.transform.position, DasherDes, Speed * Time.deltaTime);
            Timer -= Time.deltaTime;
        }

    }


}
