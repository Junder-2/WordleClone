using UnityEngine;

public class ButtonTween : MonoBehaviour
{
    enum HoldState{Default, Hover}

    private HoldState _currentHoldState;
    
    private Vector3 _originalScale = Vector3.one;
    private Vector3 _baseScale = Vector3.one;
    private float _transitionTime = .2f;
    
    bool _pressAnim = false;

    private void OnEnable()
    {
        _baseScale = _originalScale = transform.localScale;
    }

    public void Hover()
    {
        _currentHoldState = HoldState.Hover;
        if(_pressAnim) return;
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _baseScale * 1.15f, _transitionTime).setEaseOutBack();
        
    }

    public void Default()
    {
        _currentHoldState = HoldState.Default;
        if(_pressAnim) return;
        
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _baseScale, _transitionTime).setEaseOutBack();
    }

    public void Press()
    {
        LeanTween.cancel(gameObject);
        _pressAnim = true;
        LeanTween.scale(gameObject, _baseScale * .9f, _transitionTime * .5f).setEaseOutQuad().setOnComplete(SetHoldState);
    }

    void SetHoldState()
    {
        _pressAnim = false;
        switch (_currentHoldState)
        {
            case HoldState.Default:
                Default();
                break;
            case HoldState.Hover:
                Hover();
                break;
        }
    }

    private bool _deactivated = false;
    public void Deactivate()
    {
        if(_deactivated) return;
        _deactivated = true;
        _baseScale *= .9f;
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, _baseScale, _transitionTime);
    }

    public void ResetState()
    {
        _deactivated = false;
        _currentHoldState = HoldState.Default;
        _baseScale = _originalScale;
        LeanTween.scale(gameObject, _baseScale, 0.1f);
    }
}
