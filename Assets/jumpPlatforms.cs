using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpPlatforms : MonoBehaviour
{

    public PlatformEffector2D effector;

    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EffectorRotation(int offset) {
        effector.rotationalOffset = offset;
    }
}
