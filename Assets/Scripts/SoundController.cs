using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour {   //Será uma classe Singleton
    private static SoundController instance;

    public Sound[] sounds;
    public List<GameObject> objectsSounds;

    private Dictionary<string, bool> isPlayingOST = new Dictionary<string, bool> { { "OST_menu", false }, { "OST_level", false } };
    private Dictionary<string, float> originalVolumesOSTs = new Dictionary<string, float> { { "OST_menu", 0.6f }, { "OST_level", 0.25f } };
    private Dictionary<string, float> currentVolumesOSTs = new Dictionary<string, float> { { "OST_menu", 0.6f }, { "OST_level", 0.25f } };
    private Dictionary<string, float> originalVolumesSFXs = new Dictionary<string, float>();
    private Dictionary<string, float> currentVolumesSFXs = new Dictionary<string, float>();

    public static SoundController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void LoadSounds() {
        foreach (Sound s in sounds) {
            if (s.origins.Count() == 0) {   //Se o som não tiver origem especificada
                s.audioSource = gameObject.AddComponent<AudioSource>();
                s.audioSource.clip = s.clip;
                s.audioSource.clip.name = s.name;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.loop;
                s.audioSource.playOnAwake = false;
                if (s.is3D) {
                    s.audioSource.spatialBlend = 1f;
                    s.audioSource.maxDistance = 100f;
                    s.audioSource.minDistance = 10f;
                    s.audioSource.rolloffMode = AudioRolloffMode.Linear;
                }
            }
            else {
                foreach (GameObject go in s.origins) {
                    GameObject obj = objectsSounds.Find(a => a.name == go.name);
                    if(obj == null) 
                        objectsSounds.Add(go);
                    s.audioSource = go.AddComponent<AudioSource>();
                    s.audioSource.clip = s.clip;
                    s.audioSource.clip.name = s.name;
                    s.audioSource.volume = s.volume;
                    s.audioSource.pitch = s.pitch;
                    s.audioSource.loop = s.loop;
                    s.audioSource.playOnAwake = false;
                    if (s.is3D) {
                        s.audioSource.spatialBlend = 1f;
                        s.audioSource.maxDistance = 100f;
                        s.audioSource.minDistance = 10f;
                        s.audioSource.rolloffMode = AudioRolloffMode.Linear;
                    }
                }
            }
            if (!s.isOST) {
                currentVolumesSFXs[s.name] = s.volume;
                originalVolumesSFXs[s.name] = s.volume;
            }
        }
    }

    public void PlaySound(string soundName, GameObject go=null) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(soundName));   //Procurando o som informado pelo seu nome
        if (s != null) {
            if (go != null) {
                AudioSource[] audios = go.GetComponents<AudioSource>();
                audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Play();
            }
            else {
                if (soundName.Contains("OST"))   //Se eu estiver tentando mudar a música de fundo
                    SwapTrack(soundName);
                else {
                    AudioSource[] audios = gameObject.GetComponents<AudioSource>();
                    audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Play();
                }
            }
        }
    }

    public void StopSound(string soundName, GameObject go) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(soundName));   //Procurando o som informado pelo seu nome
        if (s != null) {
            if (go != null) {
                AudioSource[] audios = go.GetComponents<AudioSource>();
                audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Stop();
            }
            else {
                AudioSource[] audios = gameObject.GetComponents<AudioSource>();
                audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Stop();
            }
        }
    }


    private void SwapTrack(string nameNewOST) {
        string nameOSTPlaying = "";
        foreach (KeyValuePair<string, bool> valuePair in isPlayingOST) {
            if (valuePair.Value == true) {
                nameOSTPlaying = valuePair.Key;   //Pegando o nome da trilha que está tocando
                break;
            }
        }
        if (nameOSTPlaying != nameNewOST) {
            AudioSource[] audios = gameObject.GetComponents<AudioSource>();
            AudioSource newOST = audios.FirstOrDefault(a => a.clip.name.Equals(nameNewOST));
            AudioSource oldOST = null;
            if (nameOSTPlaying != "")
                oldOST = audios.FirstOrDefault(a => a.clip.name.Equals(nameOSTPlaying));

            StopAllCoroutines();
            StartCoroutine(FadeTrack(oldOST, newOST));
            isPlayingOST[nameOSTPlaying] = false;
            isPlayingOST[nameNewOST] = true;
        }
    }


    private IEnumerator FadeTrack(AudioSource oldOST, AudioSource newOST) {    //Esta co-rotina será usada para transiocionar entre uma música e outra
        float timeToFade = 1.25f, timeElapsed = 0;
        float volumeNewOst = currentVolumesOSTs[newOST.clip.name], volumeOldOST = 1;
        if (oldOST != null)
            volumeOldOST = currentVolumesOSTs[oldOST.clip.name];

        newOST.volume = 0;
        newOST.Play();
        while (timeElapsed < timeToFade) {
            if (oldOST != null)
                oldOST.volume = Mathf.Lerp(volumeOldOST, 0, timeElapsed / timeToFade);
            newOST.volume = Mathf.Lerp(0, volumeNewOst, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (oldOST != null)
            oldOST.Stop();
    }


    public void PauseCurrentTrack() {
        string nameOSTPlaying = "";
        foreach (KeyValuePair<string, bool> valuePair in isPlayingOST) {
            if (valuePair.Value == true) {
                nameOSTPlaying = valuePair.Key;   //Pegando o nome da trilha que está tocando
                break;
            }
        }
        if (nameOSTPlaying != "") {
            AudioSource[] audios = gameObject.GetComponents<AudioSource>();
            AudioSource currentTrack = audios.FirstOrDefault(a => a.clip.name.Equals(nameOSTPlaying));
            currentTrack.Pause();
        }
    }

    public void ResumeCurrentTrack() {
        string nameOSTPlaying = "";
        foreach (KeyValuePair<string, bool> valuePair in isPlayingOST) {
            if (valuePair.Value == true) {
                nameOSTPlaying = valuePair.Key;   //Pegando o nome da trilha que está tocando
                break;
            }
        }
        if (nameOSTPlaying != "") {
            AudioSource[] audios = gameObject.GetComponents<AudioSource>();
            AudioSource currentTrack = audios.FirstOrDefault(a => a.clip.name.Equals(nameOSTPlaying));
            currentTrack.Play();
        }
    }
    public void ChangeVolumes(bool sceneStart) {    //o parâmetro sceneStart é necessário para evitar que uma música comece com volume errado quando uma cena é carregada
        List<AudioSource> allAudios = GetComponents<AudioSource>().ToList<AudioSource>();
        foreach(GameObject obj in objectsSounds) {
            AudioSource[] objAudios = obj.GetComponents<AudioSource>();
            foreach(AudioSource a in objAudios) {
                allAudios.Add(a);
            }
        }

        foreach (AudioSource audio in allAudios) {
            if (audio.clip.name.Contains("OST")) {
                currentVolumesOSTs[audio.clip.name] = originalVolumesOSTs[audio.clip.name] * Globals.volumeOST;
                if (!sceneStart)
                    audio.volume = originalVolumesOSTs[audio.clip.name] * Globals.volumeOST;
            }
            else {
                currentVolumesSFXs[audio.clip.name] = originalVolumesSFXs[audio.clip.name] * Globals.volumeSFX;
                audio.volume = originalVolumesSFXs[audio.clip.name] * Globals.volumeSFX;
            }
        }
    }
}
