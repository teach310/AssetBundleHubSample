using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBundleHubSample
{
    public class ProgressDialog : MonoBehaviour
    {
        [SerializeField] Slider progressBar = null;

        public void DisplayProgress(float value)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
            if(progressBar.value != value)
            {
                progressBar.value = value;
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
