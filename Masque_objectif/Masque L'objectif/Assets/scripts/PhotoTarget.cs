using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    public int targetID = 0;

    void OnDrawGizmos()
    {
        if (PhotoManager.Instance == null) return;

        bool isCurrent = (targetID == PhotoManager.Instance.currentTarget);

        Gizmos.color = isCurrent ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.6f);

        // Optionnel : petite flèche vers le haut pour mieux voir
        Gizmos.color = isCurrent ? new Color(0,1,0,0.6f) : new Color(1,1,0,0.4f);
        Gizmos.DrawRay(transform.position, Vector3.up * 1.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.8f);
    }
}