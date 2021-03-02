using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Generator : MonoBehaviour
{
    [Header("Generation Settings")]

    public AnimationCurve tensionMap;

    public int minRooms = 25;
    public int maxRooms = 100;

    [Space(10)]

    public float minPathDistance = 50f;
    public float maxPathDistance = 75f;


    [Space(10)]

    public GameObject[] startRooms;
    public GameObject[] capRooms;

    [Space(10)]

    [SerializeField]
    int bifurcationDepth = 3;

    [Header("Debug")]

    public List<Bounds> roomBounds = new List<Bounds>();
    public int currentRoomCount = 1;
    public List<Room> rooms = new List<Room>();

    void Start()
    {
        Init();
    }

    void Init()
    {
        GameObject startRoom = Instantiate(startRooms[Random.Range(0, startRooms.Length)], Vector3.zero, Quaternion.identity);
        startRoom.transform.parent = this.transform;
        roomBounds.Add(startRoom.GetComponent<MeshRenderer>().bounds);
        rooms.Add(startRoom.GetComponent<Room>());
        startRoom.GetComponent<Room>().Init(this);



    }

    public void ResetQA()
    {
        CancelInvoke("StartQA");
        Invoke("StartQA", 0.2f);
    }

    void StartQA()
    {
        StartCoroutine(QA());
    }

    IEnumerator QA()
    {
        //  yield return new WaitForSeconds(0.5f);
        yield return null;

        if (currentRoomCount < minRooms)
        {

            ResetWorld();
            yield break;
        }

        GameObject[] endRooms = GameObject.FindGameObjectsWithTag("GoalRoom");
        if (endRooms.Length == 0)
        {

            ResetWorld();
            yield break;
        }

        endRooms[0].GetComponent<NavMeshSurface>().BuildNavMesh();

        GameObject furthestRoom = endRooms[0];
        float goldenPathDistance = 0;
        for (int i = 0; i < endRooms.Length; i++)
        {
            NavMeshPath currentPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, furthestRoom.transform.position, NavMesh.AllAreas, currentPath);

            NavMeshPath testPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, endRooms[i].transform.position, NavMesh.AllAreas, testPath);

            if (PathDistance(testPath) > PathDistance(currentPath))
            {
                furthestRoom = endRooms[i];
                goldenPathDistance = PathDistance(testPath);
            }
        }


        // We check if the path is proper length
        if (goldenPathDistance < minPathDistance || goldenPathDistance > maxPathDistance)
        {

            ResetWorld();
            yield break;
        }

        DrawGoldenPath(furthestRoom);
        print(goldenPathDistance);
        yield return null;


        // This level template is a go, time to process it.
        RemoveMissingRooms();
        Process(furthestRoom);
    }

    void Process(GameObject furthestRoom)
    {
        Transform currentSelection = furthestRoom.transform;
        while (currentSelection.transform.parent != null)
        {
            Room room;
            if (currentSelection.TryGetComponent<Room>(out room))
            {
                room.isCritical = true;
            }
            currentSelection = currentSelection.transform.parent;
        }


        // we go along each room. if it's critical, we wind down it's outputs, trimming bifurcations when they become too long.
        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            if (room.isCritical && !room.isBifurcation)
            {
                foreach (Transform child in room.transform)
                {
                    // look for outputs in the room
                    if (child.name == "DoorFrame")
                    {
                        // we found a bifurcation. loop down and trim as needed!
                        if (child.GetChild(0).GetComponent<Room>().isCritical == false)
                        {
                            int bifurcation = 0;
                            GameObject bifurcatedRoom = child.GetChild(0).gameObject;

                            while (bifurcation < bifurcationDepth)
                            {
                                bifurcatedRoom.GetComponent<Room>().isCritical = true;
                                bifurcatedRoom.GetComponent<Room>().isBifurcation = true;
                                foreach (Transform bifurcatedChild in bifurcatedRoom.transform)
                                {
                                    if (bifurcatedChild.name == "DoorFrame")
                                    {
                                        bifurcatedRoom = bifurcatedChild.GetChild(0).gameObject;
                                    }
                                }
                                bifurcation++;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            if (room.isCritical == false)
            {
                if (room.transform.parent.parent.GetComponent<Room>().isCritical)
                {
                    Destroy(room.gameObject);
                }
            }
        }

        Invoke("Furnish", 0.1f);
    }

    void Furnish()
    {
        RemoveMissingRooms();
    }



    // Helpers

    void ResetWorld()
    {
        StopCoroutine(QA());
        Destroy(transform.GetChild(0).gameObject);
        currentRoomCount = 1;
        roomBounds.Clear();
        rooms.Clear();
        Init();
    }

    void DrawGoldenPath(GameObject furthestRoom)
    {
        NavMeshPath goldenPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, furthestRoom.transform.position, NavMesh.AllAreas, goldenPath);

        for (int i = 0; i < goldenPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(goldenPath.corners[i], goldenPath.corners[i + 1], Color.yellow, 100f);
        }

    }

    float PathDistance(NavMeshPath path)
    {
        float distance = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return distance;
    }

    public void RemoveMissingRooms()
    {
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if (rooms[i] == null)
            {
                rooms.RemoveAt(i);
                roomBounds.RemoveAt(i);
            }
        }
    }
}