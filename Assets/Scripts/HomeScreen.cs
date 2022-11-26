using System.Collections;
using System.Collections.Generic;
using AssetBundleHub;
using UnityEngine;

namespace AssetBundleHubSample
{
    public class HomeScreen : MonoBehaviour
    {
        string attackPrefabName = "Prefabs/001/BaseAttackPrefab";
        string hpPrefabName = "Prefabs/002/BaseHPPrefab";

        [SerializeField] Transform attackPrefabParent;
        [SerializeField] Transform hpPrefabPrent;

        GameObject attackObject;
        GameObject hpObject;

        public void OnEnter()
        {
            var attackPrefab = ABHub.GetAsset<GameObject>(attackPrefabName);
            attackObject = Instantiate(attackPrefab, attackPrefabParent);
            var hpPrefab = ABHub.GetAsset<GameObject>(hpPrefabName);
            hpObject = Instantiate(hpPrefab, hpPrefabPrent);
        }

        public void OnExit()
        {
            GameObject.Destroy(attackObject);
            GameObject.Destroy(hpObject);
        }
    }
}
