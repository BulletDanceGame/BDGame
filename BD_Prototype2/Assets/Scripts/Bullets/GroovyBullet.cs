using UnityEngine;

public class GroovyBullet : MonoBehaviour
{
    //https://i1.sndcdn.com/artworks-000137022626-cqw4pk-t500x500.jpg <--- this

    public float Amplitude = 1f; // amplitude of the sine wave
    public float Frequency = 1f; // frequency of the sine wave

    float _time = 0f; // current time for the sine wave


    void Update()
    {
        // update the time for the sine wave
        _time += Time.deltaTime * Frequency;

        // calculate the new position for the bullet
        Vector3 position = transform.position;
        //position += transform.up * Time.deltaTime * _speed;
        position += transform.right * Mathf.Sin(_time) * Amplitude * Time.deltaTime;

        // set the new position for the bullet
        transform.position = position;

        
    }
}
