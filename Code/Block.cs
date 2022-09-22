using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "Block")]
public class Block : ScriptableObject
{
    public enum states { solid, liquid, gas }

    //public string name;
    public uint id;
    public Texture2D texture;
    public Vector2 uv;
    public uint blockType;
    public bool transparent;

    public states state;//: solid, liquid, gass
    public Vector3 velocity;
    public float friction;
    public float mass;
    //public conducivity?
    //public magnetism?
    //public thermal;
    public bool isMoving;

    public Block(string idk)
    {
        name = idk;
        texture = null;
        uv = new Vector2(0.1f, 0.1f);
    }
}