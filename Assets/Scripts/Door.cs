using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject[] nodes;

    [SerializeField] public bool open;
    [SerializeField] GameObject doorModel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            doorModel.SetActive(false);
            foreach (GameObject node in nodes)
            {
                node.GetComponent<Pathnode>().nodeActive = true;
            }

        }
        else
        {
            doorModel.SetActive(true);
            foreach (GameObject node in nodes)
            {
                node.GetComponent<Pathnode>().nodeActive = false;
            }
        }
    }
}
