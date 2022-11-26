using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace AssetBundleHubSample
{
    public class DialogYN : MonoBehaviour
    {
        [SerializeField] Text label;
        [SerializeField] Button yesButton;
        [SerializeField] Button noButton;

        public async UniTask<bool> Show(string text)
        {
            this.gameObject.SetActive(true);
            label.text = text;

            var result = await UniTask.WhenAny(
                noButton.OnClickAsync(),
                yesButton.OnClickAsync()
            );

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            this.gameObject.SetActive(false);
            return result == 1;
        }
    }
}
