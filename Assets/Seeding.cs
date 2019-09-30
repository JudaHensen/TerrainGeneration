using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeding : MonoBehaviour {

    [SerializeField]
    // use creatorSeed for seed generation
    private uint creatorSeed = 1;

    [SerializeField]
    private bool randomCreator = false;

    private string currentSeed = "creator";
    private readonly uint maximumLength = 99999999;

    // Store all seeds & states
    private Dictionary<string, Seed> seeds = new Dictionary<string, Seed>();


	void Awake () {
        if(randomCreator)
        {
            creatorSeed = (uint) Random.Range(0, maximumLength);
        }

        Random.InitState( (int) creatorSeed);
        seeds.Add("creator", new Seed(creatorSeed));
        seeds["creator"].SetState(Random.state);

        CreateSeed("main");
        ChangeSeed("main"); 
    }

    // Retrieves next seed value and updates that seeds state.
    public float NextValue()
    {
        return seeds[currentSeed].NextValue();
    }

    // Changes seed
    public void ChangeSeed(string seedName)
    {
        Random.state = seeds[seedName].GetState();
        currentSeed = seedName;
    }

    // Create seed based on "creator" seed
    public void CreateSeed(string seedName)
    {
        string previousSeed = currentSeed;
        ChangeSeed("creator");
        seeds.Add(seedName, new Seed( (uint) (seeds[currentSeed].NextValue() * maximumLength) ));
        Random.InitState( (int) seeds[seedName].GetSeed() );
        seeds[seedName].SetState(Random.state);
        ChangeSeed(previousSeed);
    }

    // Removes seed, cannot remove main & creator seed.
    public void RemoveSeed(string seedName)
    {
        if(seedName != "main" && seedName != "creator") seeds.Remove(seedName);
    }
}