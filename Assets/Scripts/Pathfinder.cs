using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] public GameObject startNode, endNode, currentNode, targetNode, prevNode;
    [SerializeField] public GameObject button1, button2, button3, document;
    [SerializeField] public GameObject jailDoor, button1Door, button2Door, documentDoor;
    [SerializeField] public GameObject[] guards;

    float movementSpeed = 6.0f;
    public bool jailed = false;
    public float jailTime = 0;

    float closeDoorTime = 0;
    bool runningAway = false;

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
        RayDetection();

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

        if (jailed)
        {
            jailTime += Time.deltaTime;
            if (jailTime > 5.0f)
            {
                jailed = false;
                jailTime = 0;
                jailDoor.GetComponent<Door>().open = true;
                closeDoorTime = 0;
            }
        }

        closeDoorTime += Time.deltaTime;

        if (closeDoorTime > 2)
        {
            jailDoor.GetComponent<Door>().open = false;
        }
    }

    private void RayDetection()
    {

        bool[] foundGuard = new bool[guards.Length];
        GameObject bestConnection = null;

        // detect visible guards
        for (int i = 0; i < guards.Length; i++)
        {
            var guard = guards[i];

            RaycastHit hit;
            Vector3 from = transform.position + new Vector3(0, 0.5f, 0);
            Vector3 to = guard.transform.position + new Vector3(0, 0.5f, 0);

            bool hitSomething = Physics.Linecast(from, to, out hit);
            Debug.DrawLine(from, to, hitSomething ? Color.green : Color.red);

            // consider a guard "seen" when the linecast hit that guard (or one of its child colliders)
            if (hitSomething && hit.collider != null)
            {
                var hitRoot = hit.collider.transform.root.gameObject;
                var guardRoot = guard.transform.root.gameObject;
                if (hitRoot == guardRoot)
                {
                    foundGuard[i] = true;
                    runningAway = true;
                }
            }
        }

        // if none seen, stop running
        bool anyGuardFound = false;
        for (int i = 0; i < foundGuard.Length; i++)
        {
            if (foundGuard[i]) 
            { 
                anyGuardFound = true;
            }
        }
            
        if (!anyGuardFound)
        {
            runningAway = false;
        }

        if (runningAway)
        {
            Pathnode currScript = currentNode.GetComponent<Pathnode>();

            float bestOverallScore = float.MinValue;

            // choose the connection that maximizes distance from detected guards
            for (int i = 0; i < guards.Length; i++)
            {
                if (foundGuard[i])
                {
                    for (int j = 0; j < currScript.connections.Count; j++)
                    {
                        var conn = currScript.connections[j];
                        if (conn == null) continue;

                        // you can optionally skip prevNode: if you don't want backtracking uncomment next line
                        // if (conn == prevNode) continue;

                        float distToGuard = Vector3.Distance(conn.transform.position, guards[i].transform.position);

                        // We want the largest distance (farthest from guard)
                        if (distToGuard > bestOverallScore)
                        {
                            bestOverallScore = distToGuard;
                            bestConnection = conn;
                        }
                    }
                }


                
            }
            if (bestConnection != null)
            {
                targetNode = bestConnection;
            }
        }
    }
}
