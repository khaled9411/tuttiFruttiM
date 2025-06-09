using UnityEngine;

public class StartMiniGame : MonoBehaviour
{

    [SerializeField] private bool makeItRandom;
    [SerializeField] private int miniGameNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            MiniGamesManager.instance.OpenGameBox(makeItRandom, miniGameNumber);
            Destroy(gameObject);
        }
    }
}
