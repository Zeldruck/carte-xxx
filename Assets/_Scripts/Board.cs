using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private float snapSize;
    public float SnapSize
    {
        get => snapSize;
        private set => snapSize = value;
    }

    [Header("Debug")]
    [SerializeField] private bool showGridDebug;
    [SerializeField] private Color gridColorDebug;
    [SerializeField] private float gridSizeDebug;

    private void Awake()
    {
        if (snapSize == 0f)
        {
            snapSize = 0.01f;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGridDebug || gridSizeDebug <= 0f)
            return;
        
        float y = transform.position.y + transform.localScale.y / 2f;
        
        for (float x = transform.position.x - gridSizeDebug; x <= transform.position.x + gridSizeDebug; x += snapSize)
        {
            Gizmos.color = gridColorDebug;
            Gizmos.DrawLine(new Vector3(x, y, transform.position.z - gridSizeDebug), new Vector3(x, y, transform.position.z + gridSizeDebug));
        }
        
        for (float z = transform.position.z - gridSizeDebug; z <= transform.position.z + gridSizeDebug; z += snapSize)
        {
            Gizmos.color = gridColorDebug;
            Gizmos.DrawLine(new Vector3(transform.position.x - gridSizeDebug, y, z), new Vector3(transform.position.x + gridSizeDebug, y, z));
        }
    }
}
