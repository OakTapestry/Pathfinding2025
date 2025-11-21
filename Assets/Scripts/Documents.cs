using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.LookDev;

public class Documents : MonoBehaviour
{
    [SerializeField] GameObject Document;
    [SerializeField] GameObject[] spies;
    bool grabbed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(spies[0].transform.position, transform.position) < 2f && !grabbed)
        {
            grabbed = true;
            Destroy(this);

        }
        else if (Vector3.Distance(spies[0].transform.position, transform.position) < 2f && !grabbed)
        {
            grabbed = true;
            Destroy(this);
        }
    }
}
