//    ExampleScript - LowHealth


using UnityEngine;
using UnityEngine.AI;
using Leguar.LowHealth;

namespace LowHP {
	public class LowHealthEffect : MonoBehaviour {
		public LowHealthController shaderControllerScript;	//정신력 별 시각효과
		public LowHealthDirectAccess shaderAccessScript;	//피격 시각효과

		private StatusController player;
		public NavmeshMove_rayT2 nav;

		//정신력 별 체력효과 필드
		private int currentPlayerHealth;
		
		//피격효과 필드
		private float takingDamage;
		private float cameraDizzy;

		void Start() {
			currentPlayerHealth = 100;
			shaderControllerScript.SetPlayerHealthInstantly(currentPlayerHealth);

			takingDamage = 0f;
		}
		void Update(){
			if(takingDamage > 0f){
				takingDamage -= Time.deltaTime * 0.5f;
				shaderAccessScript.SetColorLossEffect(takingDamage, 1f);
				shaderAccessScript.SetDetailLossEffect(takingDamage * 1f);
			}
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

		public void takeDamageByNPC(){
			takingDamage = 1f;
			cameraDizzy = 1f;
		}
		private float smoothCurve(float time){
			if(time >= 1f){
				return 0f;
			}
			float t;
			if(time < 0.1f){
				t = time * 5f;
			}
			else{
				t = 0.5f + (time - 0.1f) / 0.9f * 0.5f;
			}
			float sin = Mathf.Sin(Mathf.PI * t);
			return sin;
		}


	}

}
