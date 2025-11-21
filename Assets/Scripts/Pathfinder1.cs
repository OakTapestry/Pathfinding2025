using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Pathfinder1 : MonoBehaviour
{
    [SerializeField] GameObject startNode, endNode, currentNode, targetNode, prevNode, jailNode;
    [SerializeField] GameObject jailDoor;
    [SerializeField] GameObject[] waypoints;
    [SerializeField] GameObject BlueSpy;
    [SerializeField] GameObject RedSpy;
    [SerializeField] float forwardDist;

    float movementSpeed = 2.6f;
    float sightDistance = 10.0f;
    int waypointIndex = 0;
    [SerializeField] bool searchingForSpy = false;

    Vector3 SpyJail;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startNode.transform.position;
        currentNode = startNode;
        targetNode = currentNode;
        endNode = waypoints[waypointIndex];
        SpyJail = jailNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        RayDetection();


        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.2f && searchingForSpy)
        {

            prevNode = currentNode;
            currentNode = targetNode;

            float closestDist = 10000;

            Pathnode currentScript = currentNode.GetComponent<Pathnode>();

            for (int i = 0; i < currentScript.connections.Count; i++)
            {
                if (Vector3.Distance(currentScript.connections[i].transform.position, endNode.transform.position) < closestDist)
                {
                    if (currentScript.connections[i].GetComponent<Pathnode>().nodeActive)
                    {
                        closestDist = Vector3.Distance(currentScript.connections[i].transform.position, endNode.transform.position);
                        targetNode = currentScript.connections[i];
                    }
                }
            }

            if (currentNode == endNode)
            {
                searchingForSpy = false;
            }
        }

        if (searchingForSpy)
        {
            transform.Translate((targetNode.transform.position - transform.position).normalized * movementSpeed * 2 * Time.deltaTime);
        }


        // If the AI is at the targetNode then find a new target to move to.
        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.1f && !searchingForSpy)
        {

            prevNode = currentNode;
            currentNode = targetNode;

            if (currentNode == endNode)
            {
                waypointIndex++;

                if (waypointIndex >= waypoints.Length)
                {
                    waypointIndex = 0;
                }

                endNode = waypoints[waypointIndex];
            }

            float closestDistance = 10000;

            Pathnode pathscript = currentNode.GetComponent<Pathnode>();

            if (pathscript != null)
            {
                for (int i = 0; i < pathscript.connections.Count; i++)
                {
                    if (pathscript.connections[i] != prevNode && pathscript.connections[i].GetComponent<Pathnode>().nodeActive)
                    {
                        if (Vector3.Distance(pathscript.connections[i].transform.position, endNode.transform.position) < closestDistance)
                        {
                            targetNode = pathscript.connections[i];
                            closestDistance = Vector3.Distance(pathscript.connections[i].transform.position, endNode.transform.position);
                        }
                    }
                }
            }
        }

        if (!searchingForSpy)
        {
            transform.Translate((targetNode.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, BlueSpy.transform.position) < 0.5f)
        {
            jailDoor.GetComponent<Door>().open = false;
            //move the spy to jail
            BlueSpy.transform.position = new Vector3(SpyJail.x, BlueSpy.transform.position.y, SpyJail.z);
            //set their current node to jail
            BlueSpy.GetComponent<Pathfinder>().targetNode = jailNode;
            searchingForSpy = false;
            BlueSpy.GetComponent<Pathfinder>().jailed = true;
        }
        if (Vector3.Distance(transform.position, RedSpy.transform.position) < 0.5f && !RedSpy.GetComponent<Pathfinder>().disguised)
        {
            jailDoor.GetComponent<Door>().open = false;
            //move the spy to jail
            RedSpy.transform.position = new Vector3(SpyJail.x, RedSpy.transform.position.y, SpyJail.z);
            //set their current node to jail
            RedSpy.GetComponent<Pathfinder>().targetNode = jailNode;
            searchingForSpy = false;
            RedSpy.GetComponent<Pathfinder>().jailed = true;
        }
    }

    private void RayDetection()
    {
        float detectionRangeBlue, detectionRangeRed;
        bool foundBlue = false, foundRed = false;
        GameObject bestConnectionBlue = null;
        GameObject bestConnectionRed = null;


        //look for blue spy
        RaycastHit hitBlue;
        Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), BlueSpy.transform.position + new Vector3(0, 0.5f, 0), out hitBlue);
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), hitBlue.point - transform.position + new Vector3(0, 0.5f, 0), Color.green);

        detectionRangeBlue = Vector3.Distance(transform.position, BlueSpy.transform.position);

        if (hitBlue.collider != null && hitBlue.collider == BlueSpy.GetComponentInChildren<Collider>() && Vector3.Distance(transform.position, BlueSpy.transform.position) < sightDistance && !BlueSpy.GetComponent<Pathfinder>().jailed)
        {
            foundBlue = true;
            searchingForSpy = true;
            endNode = BlueSpy.GetComponent<Pathfinder>().currentNode;
            Pathnode currScript = currentNode.GetComponent<Pathnode>();

            float bestDist = float.MaxValue;


            for (int i = 0; i < currScript.connections.Count; i++)
            {
                var conn = currScript.connections[i];
                float dist = Vector3.Distance(conn.transform.position, endNode.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestConnectionBlue = conn;
                }
            }

            if (Vector3.Distance(transform.position, currentNode.transform.position) < 1f)
            {
                targetNode = bestConnectionBlue;
            }

        }


        //look for red spy
        RaycastHit hitRed;
        Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), RedSpy.transform.position + new Vector3(0, 0.5f, 0), out hitRed);
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), hitRed.point - transform.position + new Vector3(0, 0.5f, 0), Color.green);

        detectionRangeRed = Vector3.Distance(transform.position, RedSpy.transform.position);

        if (hitRed.collider != null && hitRed.collider == RedSpy.GetComponentInChildren<Collider>() && Vector3.Distance(transform.position, RedSpy.transform.position) < sightDistance && !RedSpy.GetComponent<Pathfinder>().jailed && !RedSpy.GetComponent<Pathfinder>().disguised)
        {
            foundRed = true;
            searchingForSpy = true;
            endNode = RedSpy.GetComponent<Pathfinder>().currentNode;
            Pathnode currScript = currentNode.GetComponent<Pathnode>();

            float bestDist = float.MaxValue;


            for (int i = 0; i < currScript.connections.Count; i++)
            {
                var conn = currScript.connections[i];
                float dist = Vector3.Distance(conn.transform.position, endNode.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestConnectionRed = conn;
                }
            }

            if (Vector3.Distance(transform.position, currentNode.transform.position) < 1f)
            {
                targetNode = bestConnectionRed;
            }

        }

        //go after the closest spy if both are found
        if (foundBlue && foundRed)
        {
            if (detectionRangeBlue <= detectionRangeRed && Vector3.Distance(transform.position, currentNode.transform.position) < 1f)
            {
                targetNode = bestConnectionBlue;
                endNode = BlueSpy.GetComponent<Pathfinder>().currentNode;
            }
        }

    }
}
