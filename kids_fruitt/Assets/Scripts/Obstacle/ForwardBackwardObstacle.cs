using UnityEngine;
using DG.Tweening;

public class ForwardBackwardObstacle : MonoBehaviour
{

    [SerializeField] private float moveDistance = 3f;   
    [SerializeField] private float moveDuration = 2f;   

    private void Start()
    {

        transform.DOLocalMoveX(transform.localPosition.x + moveDistance, moveDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
