using UnityEngine;

[System.Serializable]
public class Sound {   //Essa classe representa cada som adicionado ao jogo
    public string name;
    public bool loop, is3D, isOST;
    public AudioClip clip;
    public GameObject[] origins;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource audioSource;
}
