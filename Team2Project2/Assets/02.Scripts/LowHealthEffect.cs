//    ExampleScript - LowHealth


using UnityEngine;
using UnityEngine.AI;
using Leguar.LowHealth;

namespace LowHP {

	public class LowHealthEffect : MonoBehaviour {

		// This script is attached to camera in the scene
		public LowHealthController shaderControllerScript;

		// Keep actual value of player health in own classes
		// In this example player health is integer between 0 and 100
		private StatusController player;
		public NavmeshMove_rayT2 nav;
		private int currentPlayerHealth;

		void Start() {
			// Set full health to player
			//player = GameObject.FindWithTag("Player").GetComponent<StatusController>();
			//currentPlayerHealth = player.health;

			// By default 'LowHealthController' starts from full health, but no harm to call this is start
			currentPlayerHealth = 100;
			shaderControllerScript.SetPlayerHealthInstantly(currentPlayerHealth);

		}

		void Update(){
			//Debug.Log("npc attack?: " + nav.isAttack + ", hp: " + player.health);
			//if(nav.isAttack){	//nav에서 Attack 메소드 호출 시에만
				//helathChangeByNPC(player.health);
			//}
		}
		public void helathChangeByNPC(int newPlayerHealth) {
			Debug.Log("lhe run : " + newPlayerHealth);
			// Set effects
			bool healthGoingDown = (newPlayerHealth<currentPlayerHealth);
			setNewPlayerHealth(newPlayerHealth/100f, healthGoingDown);

			// Remember the new health
			currentPlayerHealth = newPlayerHealth;

		}

		private void setNewPlayerHealth(float newPlayerHealthPercent, bool healthGoingDown) {
			Debug.Log("set: " + newPlayerHealthPercent);
			shaderControllerScript.SetPlayerHealthSmoothly(newPlayerHealthPercent, 1f);
			if (healthGoingDown) {
				Debug.Log("run!");
				// Player took damage, make transition faster (1 second)
				shaderControllerScript.SetPlayerHealthSmoothly(newPlayerHealthPercent, 1f);

			} else {
				Debug.Log("no run!");
				// Player gained health (medikit?), make transition slightly slower (2 seconds)
				shaderControllerScript.SetPlayerHealthSmoothly(newPlayerHealthPercent, 2f);

			}

		}

	}

}
