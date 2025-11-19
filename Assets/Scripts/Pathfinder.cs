using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] public GameObject startNode, endNode, currentNode, targetNode, prevNode;

    float movementSpeed = 6.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentNode = startNode;
        targetNode = currentNode;

        transform.position = currentNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.2f)
        {
            prevNode = currentNode;
            currentNode = targetNode;

            //bool found = false;
            //int breakOut = 0;

            //while (!found)
            //{
            //    targetNode = currentNode.GetComponent<Pathnode>().connections[Random.Range(0, currentNode.GetComponent<Pathnode>().connections.Count)];

            //    if (targetNode != prevNode || breakOut > 10)
            //    {
            //        found = true;
            //    }
            //    breakOut++;
            //}

            float closestDist = 10000;

            Pathnode currentScript = currentNode.GetComponent<Pathnode>();

            for (int i = 0; i < currentScript.connections.Count; i++)
            {
                if (Vector3.Distance(currentScript.connections[i].transform.position, endNode.transform.position) < closestDist)
                {
                    if (currentScript.connections[i] != prevNode && currentScript.connections[i].GetComponent<Pathnode>().nodeActive)
                    {
                        closestDist = Vector3.Distance(currentScript.connections[i].transform.position, endNode.transform.position);
                        targetNode = currentScript.connections[i];
                    }                    
                }
            }
        }

        transform.Translate((targetNode.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime);
    }
}
