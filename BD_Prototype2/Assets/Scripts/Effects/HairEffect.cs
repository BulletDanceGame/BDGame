using UnityEngine;

public class HairEffect : MonoBehaviour
{

    [SerializeField] private ParticleSystem _fireHair;
    public float offsetX;
    public float offsetY;
    private void Start()
    {
        var VoL = _fireHair.velocityOverLifetime;
        VoL.space = ParticleSystemSimulationSpace.Local;

        
    }
    public void Update()
    {
        //fuck me for this
        AnimationCurve curve = new AnimationCurve();
        if (Input.GetKeyDown("a"))
        {
            transform.position+=new Vector3(-offsetX, -offsetY, 0);
        }
        if(Input.GetKeyUp("a"))
        {
            transform.position += new Vector3(offsetX, offsetY, 0);
        }

        if (Input.GetKeyDown("d"))
        {
            transform.position += new Vector3(offsetX, -offsetY, 0);
        }
        if (Input.GetKeyUp("d"))
        {
            transform.position += new Vector3(-offsetX, offsetY, 0);
        }
        if (Input.GetKeyDown("s"))
        {
            transform.position += new Vector3(0, -offsetY, 0);
        }
        if (Input.GetKeyUp("s"))
        {
            transform.position += new Vector3(0, offsetY, 0);
        }

    }
}
