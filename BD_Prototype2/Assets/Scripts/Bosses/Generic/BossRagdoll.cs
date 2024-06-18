using UnityEngine;

public class BossRagdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbd;
    [SerializeField] private PhysicsMaterial2D bouncyMat;

    void Start()
    {
        EventManager.Instance.OnBossDeath += Ragdoll;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnBossDeath -= Ragdoll;
    }

    void Ragdoll()
    {
        rbd.sharedMaterial = bouncyMat;
        rbd.drag = 1.25f;
    }
}