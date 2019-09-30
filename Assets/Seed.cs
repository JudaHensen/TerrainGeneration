using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed {

    private Random.State state;
    private uint seed;

    public Seed(uint _seed)
    {
        seed = _seed;
    }
	
    public Random.State GetState() { return state; }

    public void SetState(Random.State _state) { state = _state; }

    public uint GetSeed() { return seed; }

    // Return next value in sequence
    public float NextValue()
    {
        /// Save value before return or else the number will change,
        /// and the state wouldn't be correct.
        float value = Random.value;
        state = Random.state;

        return value;
    }
}
