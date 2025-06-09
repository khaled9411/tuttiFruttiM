using UnityEngine;
using DG.Tweening;


public class GateObstacle : MonoBehaviour
{
    [SerializeField] private float moveHeight = 2f;    
    [SerializeField] private float moveDuration = 3f;  
    [SerializeField] private float delayBetweenMoves = 1f;

    private void Start()
    {
        Sequence gateSequence = DOTween.Sequence();

        gateSequence.Append(transform.DOLocalMoveY(transform.localPosition.y + moveHeight, moveDuration)
            .SetEase(Ease.InOutQuad));
        gateSequence.AppendInterval(delayBetweenMoves);
        gateSequence.Append(transform.DOLocalMoveY(transform.localPosition.y, moveDuration)
            .SetEase(Ease.InOutQuad));
        gateSequence.AppendInterval(delayBetweenMoves);

        gateSequence.SetLoops(-1, LoopType.Restart);
    }
}
