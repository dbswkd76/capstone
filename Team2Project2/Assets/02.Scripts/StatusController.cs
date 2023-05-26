using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//플레이어 데미지컨트롤
public struct DamageMsg{
  public GameObject damager;
  public int amount;
  public Vector3 hitPoint;
  public Vector3 hitNormal;
}
public class StatusController : MonoBehaviour{
  private GameObject player;

  private int startingHealth = 100;  //초기 정신력
  public int health{get; protected set;} //현재 정신력
  public bool dead{get; protected set;}  //사망 상태

  private const float minDelayDamaged = 0.1f;
  private float lastDamagedTime;
  protected bool CanDamagable{
    get{
      if(Time.time >= lastDamagedTime + minDelayDamaged) return false;
      return true;
    }
  }

  [SerializeField]
  private Image hpGauge;

  protected virtual void OnEnable(){
    dead = false;
    health = startingHealth;
  }
  public virtual bool OnDamage(DamageMsg damageMsg){
    if(CanDamagable ||damageMsg.damager == gameObject || dead){
      return false;
    }
    lastDamagedTime = Time.time;
    health -= damageMsg.amount;
    
    if(health <= 0) Die();
    return true;
  }

  public virtual void healingHealth(int newHealth){ //정신력 회복
    if(dead){
      return;
    }
    health += newHealth;
  }
  public virtual void Die(){  //사망
    dead = true;

    SceneManager.LoadScene("Fail");
    Debug.Log("사망하였습니다!");
  }
  //정신력게이지 UI 업데이트
  private void GaugeUpdate(){
    hpGauge.fillAmount = (float)health / startingHealth;
  }

  void Start(){
    player = GameObject.FindWithTag("Player");
  }
  void Update(){
    GaugeUpdate();
    //Debug.Log("player health: " + health);
  }
}

