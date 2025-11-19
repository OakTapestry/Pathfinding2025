using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject linkedDoor;
    [SerializeField] GameObject spy;
    [SerializeField] GameObject button;
    bool pressed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(spy.tag == "Blue")
        {
            button.GetComponent<Renderer>().material.color = Color.blue;
        }
        else if(spy.tag == "Red")
        {
            button.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(spy.transform.position, transform.position) < 1.5f && !pressed)
        {
            pressed = true;
            linkedDoor.GetComponent<Door>().open = true;
            button.transform.position = new Vector3(button.transform.position.x, button.transform.position.y - 0.1f, button.transform.position.z);
        }
    }
}
