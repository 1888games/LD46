using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundController : MonoBehaviourSingleton<SoundController>
{

	public List<InAudioNode> Nodes;

	public Dictionary<string, InAudioNode> Sounds;
	
    // Start is called before the first frame update
    void Start()
    {

		Sounds = new Dictionary<string, InAudioNode> ();

		foreach (InAudioNode node in Nodes) {

			Sounds.Add (node.Name, node);
			
		}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	public async void PlaySound (string name, int delay = 0, float volume = 1f) {

		if (Sounds.ContainsKey (name)) {

	
			InAudio.SetVolumeForAudioFolder (Sounds [name], volume);
		
		

			await Task.Delay (delay);

			if (volume < 1f) {
				InAudio.Play (Camera.main.gameObject, Sounds [name], new AudioParameters (volume, Random.Range (0.8f, 1.1f), 0, 1));

			} else {
				InAudio.Play (Camera.main.gameObject, Sounds [name]);
			}
		
		}


	}
}
