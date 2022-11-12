using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;

    FPSControllerPlayer m_Player;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static GameController GetGameController()
    {
        return m_GameController;
    }

    public FPSControllerPlayer GetPlayer()
    {
        return m_Player;
    }
}
