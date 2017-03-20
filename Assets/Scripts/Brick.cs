using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    public GameObject coinPrefab;
    public bool hasCoin;

    void OnKill(){
       
        if (hasCoin) {
           GameObject coinObject = GameObject.Instantiate(coinPrefab);
            coinObject.transform.position = transform.position + new Vector3();

            Coin coin = coinObject.GetComponent<Coin>();
            coin.Vanish();

            //GameObject.Find("Player").GetComponent<Player>().onCollectCoin();
        }
    }
}
