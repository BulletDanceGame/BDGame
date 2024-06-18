using UnityEngine;
using TMPro;

/* Stolen from here: https://github.com/Madalaski/TextTutorial/blob/master/Assets/CharacterWobble.cs */

public class VFX_TextCharWobble : MonoBehaviour
{
    private TMP_Text _textMesh;
    private Mesh _mesh;
    private Vector3[] _vertices;

    [SerializeField]
    private Vector2 _wobbleStrength, _wobbleSpeed;


    void Start()
    {
        _textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        _textMesh.ForceMeshUpdate();
        _mesh     = _textMesh.mesh;
        _vertices = _mesh.vertices;

        for (int i = 0; i < _textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo _c = _textMesh.textInfo.characterInfo[i];

            Vector3 _offset = Wobble(Time.time + i);

            int _index = _c.vertexIndex;
            for(int v = 0; v < 4; v++)
            {
                _vertices[_index + v] += _offset;
            }
        }

        _mesh.vertices = _vertices;
        _textMesh.canvasRenderer.SetMesh(_mesh);
    }

    Vector2 Wobble(float time) {
        return new Vector2(Mathf.Sin(time*_wobbleSpeed.x) * _wobbleStrength.x, Mathf.Cos(time*_wobbleSpeed.y) * _wobbleStrength.y);
    }
}
