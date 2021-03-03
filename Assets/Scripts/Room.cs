using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField]
    float boundShrinkSize = 0.1f;

    [SerializeField]
    GameObject[] allowedOutputs;

    [Header("Game Data")]
    public bool isCritical;
    public bool isBifurcation;
    public float tension = 0;

    public void Init(Generator generator)
    {
        StartCoroutine("Generate", generator);
    }

    public IEnumerator Generate(Generator generator)
    {

        foreach (Transform child in transform)
        {

            // look for outputs in the room
            if (child.name == "DoorFrame")
            {

                // create rooms if we can
                if (generator.currentRoomCount < generator.maxRooms)
                {

                    bool success = false;

                    // copy room order index 
                    int[] shuffledRooms = new int[allowedOutputs.Length];
                    for (int x = 0; x < shuffledRooms.Length; x++)
                    {
                        shuffledRooms[x] = x;

                    }
                    // and shuffle
                    for (int t = 0; t < shuffledRooms.Length; t++)
                    {
                        int tmp = shuffledRooms[t];
                        int r = Random.Range(t, shuffledRooms.Length);
                        shuffledRooms[t] = shuffledRooms[r];
                        shuffledRooms[r] = tmp;
                    }

                    // we go through the rooms, trying them all to see if it would work
                    for (int i = 0; i < allowedOutputs.Length; i++)
                    {

                        if (!success)
                        {
                            GameObject instance = Instantiate(allowedOutputs[shuffledRooms[i]], child.position, child.rotation);
                            instance.name = allowedOutputs[shuffledRooms[i]].name;

                            bool doesIntersect = false;
                            Bounds testInstanceBounds = instance.GetComponent<MeshRenderer>().bounds;
                            testInstanceBounds.Expand(-boundShrinkSize);

                            for (int j = 0; j < generator.roomBounds.Count; j++)
                            {
                                Bounds testRoomBounds = generator.roomBounds[j];
                                testRoomBounds.Expand(-boundShrinkSize);

                                if (testInstanceBounds.Intersects(testRoomBounds))
                                {
                                    doesIntersect = true;
                                }
                            }

                            if (!doesIntersect)
                            {
                                generator.roomBounds.Add(instance.GetComponent<MeshRenderer>().bounds);
                                generator.rooms.Add(instance.GetComponent<Room>());
                                generator.currentRoomCount++;
                                instance.transform.parent = child.transform;
                                instance.GetComponent<Room>().Init(generator);
                                success = true;
                            }

                            if (doesIntersect)
                            {
                                Destroy(instance);
                            }

                        }
                    }

                    yield return null;

                    if (!success)
                    {
                        // no rooms fit in here, we need to cap the output
                        GameObject cap = Instantiate(generator.capRooms[Random.Range(0, generator.capRooms.Length)], child.position, child.rotation);
                        cap.transform.parent = this.transform;
                        Destroy(child.gameObject);
                    }
                }
                else
                {
                    // no more rooms allowe, cap them off with closets or the like
                    GameObject cap = Instantiate(generator.capRooms[Random.Range(0, generator.capRooms.Length)], child.position, child.rotation);
                    cap.transform.parent = this.transform;
                    Destroy(child.gameObject);

                }
            }
        }


        // Here we fix errors, yuck
        foreach (Transform child in transform)
        {

            // look for outputs in the room
            if (child.name == "DoorFrame")
            {
                if (child.childCount > 2)
                {
                    while (child.childCount > 2)
                    {
                        Destroy(child.GetChild(1).gameObject);
                        yield return null;
                    }

                }
            }
        }

        generator.ResetQA();

    }

    //  void OnDrawGizmosSelected()
    //   {
    //    Gizmos.color = Color.yellow;
    //   Gizmos.DrawWireCube(GetComponent<MeshRenderer>().bounds.center, GetComponent<MeshRenderer>().bounds.size - Vector3.one * boundShrinkSize);
    //  }

}