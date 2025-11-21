using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Camera : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI[] doorData;
    [SerializeField] public GameObject[] doors;
    // Update is called once per frame
    void Update()
    {
        foreach (var door in doors)
        {
            if (!door.GetComponent<Door>().open)
            {
                doorData[System.Array.IndexOf(doors, door)].text = (door.name) + ": Closed";
            }
            else
            {
                doorData[System.Array.IndexOf(doors, door)].text = (door.name) + ": Open";
            }



            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * 0.1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * 0.1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * 0.1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * 0.1f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= transform.up * 0.1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.position += transform.up * 0.1f;
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
