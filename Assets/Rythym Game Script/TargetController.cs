using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    Vector3 _startSize;
    [SerializeField] float pulseSize;
    [SerializeField] float returnTime;

    // Start is called before the first frame update
    void Start()
    {
        _startSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * returnTime);
    }

    public void Pusle()
    {
        transform.localScale = _startSize * pulseSize;
    }
}
