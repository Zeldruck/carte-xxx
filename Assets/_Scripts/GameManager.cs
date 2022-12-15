using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Board board { get; private set; }

    [SerializeField] private PopupCard popupCard;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        board = FindObjectOfType<Board>();

        R_Card[] rCards = FindObjectsOfType<R_Card>();

        foreach (R_Card rCard in rCards)
        {
            rCard.onClikcCard += popupCard.OpenPopUp;
        }
    }
}
