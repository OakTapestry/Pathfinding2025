using UnityEngine;
using UnityEngine.Rendering;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] public GameObject startNode, endNode, currentNode, targetNode, prevNode;
    [SerializeField] public GameObject[] escapeNodes;
    [SerializeField] public GameObject button1, button2, button3, document;
    [SerializeField] public GameObject jailDoor;
    [SerializeField] public GameObject[] guards;

    float movementSpeed = 6.0f;
    public bool jailed = false;
    public bool isBlueSpy = false;
    public float jailTime = 0;
    bool setRunning = false;
    float sightDistance = 10.0f;
    bool canSwap = true;

    float disguiseTimer = 0;
    float disguiseCooldown = 0;
    public bool disguised = false;
    bool disguiseReady = false;


    float speedTimer = 0;
    float speedCooldown = 0;
    bool speedBoost = false;
    bool speedReady = false;

    bool setEscape = false;

    Color basicColor = Color.red;
    Color disguisedColor = Color.green;

    GameObject escapeNode;

    public goalState currentGoal = goalState.BUTTON1;
    goalState lastGoal = goalState.BUTTON1;

    public enum goalState
    {
        RUNNING,
        BUTTON1,
        BUTTON2,
        BUTTON3,
        DOCUMENT,
        ESCAPE
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentNode = startNode;
        targetNode = currentNode;

        transform.position = currentNode.transform.position;

        if (isBlueSpy)
        {
            speedReady = true;
        }
        else
        {
            disguiseReady = true;
        }
        foreach (GameObject node in escapeNodes)
        {
            node.GetComponent<Pathnode>().nodeActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentGoal)
        {
            case goalState.RUNNING:
                // endNode remains unchanged
                break;
            case goalState.BUTTON1:
                endNode = button1;
                if (currentNode == button1)
                {
                    currentGoal = goalState.BUTTON2;
                    lastGoal = goalState.BUTTON2;
                }
                break;
            case goalState.BUTTON2:
                endNode = button2;
                if (currentNode == button2)
                {
                    currentGoal = goalState.BUTTON3;
                    lastGoal = goalState.BUTTON3;
                }
                break;
            case goalState.BUTTON3:
                endNode = button3;
                if (currentNode == button3)
                {
                    currentGoal = goalState.DOCUMENT;
                    lastGoal = goalState.DOCUMENT;
                }
                break;
            case goalState.DOCUMENT:
                endNode = document;
                if (currentNode == document)
                {
                    currentGoal = goalState.ESCAPE;
                    lastGoal = goalState.ESCAPE;
                }
                break;
            case goalState.ESCAPE:
                endNode = escapeNode;
                if(!setEscape)
                {
                    setEscape = true;
                    escapeNode = escapeNodes[Random.Range(0, escapeNodes.Length)];
                    escapeNode.GetComponent<Pathnode>().nodeActive = true;
                }
                break;
        }

        if (Vector3.Distance(transform.position, currentNode.transform.position) > 1f)
        {
            canSwap = false;
        }

        if (isBlueSpy)
        {
            if (speedBoost)
            {
                movementSpeed = 12.0f;
                speedTimer += Time.deltaTime;
                if (speedTimer > 5f)
                {
                    speedBoost = false;
                    speedReady = false;
                    movementSpeed = 6;
                    speedTimer = 0;
                    speedCooldown = 0;
                }
            }
            if (!speedReady)
            {
                speedCooldown += Time.deltaTime;
                if (speedCooldown > 10f)
                {
                    speedReady = true;
                    speedCooldown = 0;
                }
            }
        }
        else
        {
            if (disguised)
            {
                GetComponentInChildren<Renderer>().material.color = disguisedColor;
                disguiseTimer += Time.deltaTime;
                if (disguiseTimer > 5f)
                {
                    disguised = false;
                    disguiseReady = false;
                    disguiseTimer = 0;
                    disguiseCooldown = 0;
                }
            }
            if (!disguiseReady)
            {
                GetComponentInChildren<Renderer>().material.color = basicColor;
                disguiseCooldown += Time.deltaTime;
                if (disguiseCooldown > 10f)
                {
                    disguiseReady = true;
                    disguiseCooldown = 0;
                }
            }
        }


        if (canSwap)
        {
            RayDetection();
        }


        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.2f)
        {
            prevNode = currentNode;
            currentNode = targetNode;

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
                        canSwap = true;
                    }
                }
            }
        }

        if (!jailed)
        {
            transform.Translate((targetNode.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime);
        }

        if (jailed)
        {
            jailTime += Time.deltaTime;
            if (jailTime > 10f)
            {
                jailed = false;
                jailTime = 0;
                jailDoor.GetComponent<Door>().open = true;
            }
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
            Debug.DrawLine(from, to, Color.green);

            // consider a guard "seen" when the linecast hit that guard (or one of its child colliders)
            if (hitSomething && hit.collider != null)
            {
                var hitRoot = hit.collider.transform.root.gameObject;
                var guardRoot = guard.transform.root.gameObject;
                if (hitRoot == guardRoot)
                {
                    foundGuard[i] = true;
                    if (disguiseReady)
                    {
                        disguised = true;
                    }
                    else
                    {
                        if (!setRunning && Vector3.Distance(transform.position, guards[i].transform.position) < sightDistance)
                        {
                            lastGoal = currentGoal;
                            currentGoal = goalState.RUNNING;
                            setRunning = true;
                        }
                    }
                }
            }
        }

        // if none seen, stop running
        if (!disguised)
        {
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
                currentGoal = lastGoal;
                setRunning = false;
            }
        }
        else
        {
            currentGoal = lastGoal;
            setRunning = false;
        }


        if (currentGoal == goalState.RUNNING)
        {
            if (speedReady)
            {
                speedBoost = true;
            }
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
                canSwap = false;
            }
        }
    }
}
