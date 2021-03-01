using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Generator : MonoBehaviour {
    public bool endRoom = false;

    public int minRooms = 25;
    public int maxRooms = 100;


    public GameObject[] startRooms;
    public GameObject[] capRooms;

    [Header("Debug")]

    public List<Bounds> roomBounds = new List<Bounds>();
    public int currentRoomCount = 1;

    void Start() {
        Init();
    }

    void ResetWorld() {
        StopCoroutine(QA());
        Destroy(transform.GetChild(0).gameObject);
        currentRoomCount = 1;
        roomBounds.Clear();
        Init();
    }

    void Init() {
        GameObject startRoom = Instantiate(startRooms[Random.Range(0, startRooms.Length)], Vector3.zero, Quaternion.identity);
        startRoom.transform.parent = this.transform;
        roomBounds.Add(startRoom.GetComponent<MeshRenderer>().bounds);
        startRoom.GetComponent<Room>().Init(this);

        //  StartCoroutine(QA());

    }

    public void ResetQA() {
        StopCoroutine(QA());
        StartCoroutine(QA());
    }

    IEnumerator QA() {
        yield return new WaitForSeconds(0.2f);

        if (currentRoomCount < minRooms) {
            ResetWorld();
            yield break;
        }

        GameObject[] endRooms = GameObject.FindGameObjectsWithTag("GoalRoom");
        if (endRooms.Length == 0) {
            ResetWorld();
            yield break;
        }


        endRooms[0].GetComponent<NavMeshSurface>().BuildNavMesh();

        GameObject furthestRoom = endRooms[0];
        float goldenPathDistance = 0;
        for (int i = 0; i < endRooms.Length; i++) {


            NavMeshPath currentPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, furthestRoom.transform.position, NavMesh.AllAreas, currentPath);

            NavMeshPath testPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, endRooms[i].transform.position, NavMesh.AllAreas, testPath);

            if (PathDistance(testPath) > PathDistance(currentPath)) {
                furthestRoom = endRooms[i];
                goldenPathDistance = PathDistance(testPath);
            }
        }

        DrawGoldenPath(furthestRoom);
        print(goldenPathDistance);
    }

    // Helpers


    void DrawGoldenPath(GameObject furthestRoom) {
        NavMeshPath goldenPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, furthestRoom.transform.position, NavMesh.AllAreas, goldenPath);

        for (int i = 0; i < goldenPath.corners.Length - 1; i++) {
            Debug.DrawLine(goldenPath.corners[i], goldenPath.corners[i + 1], Color.yellow, 100f);
        }

    }

    float PathDistance(NavMeshPath path) {
        float distance = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++) {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return distance;
    }
}