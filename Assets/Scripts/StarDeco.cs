using UnityEngine;
using Random = UnityEngine.Random;

public class StarDeco : MonoBehaviour
{
    Vector3 _baseScale = Vector3.zero;
    private void OnEnable()
    {
        if (_baseScale == Vector3.zero)
            _baseScale = transform.localScale;
        LeanTween.scale(gameObject, _baseScale * 1.5f, 5f * Random.Range(.8f, 1.2f)).setEaseInOutQuad()
            .setLoopPingPong();
    }
}
