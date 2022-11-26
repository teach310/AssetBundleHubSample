using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetBundleHub;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AssetBundleHubSample
{
    public class SampleScene : MonoBehaviour
    {
        // [SerializeField] ConsolePanel consolePanel = null;
        enum SceneState
        {
            Title,
            Starting,
            Downloading,
            Home
        }

        SceneState sceneState = SceneState.Title;
        [SerializeField] DialogYN dialogYN;
        [SerializeField] ProgressDialog progressDialog;
        AssetBundleDownloader downloader = null;
        bool displayProgress = false;

        string attackPrefab = "Prefabs/001/BaseAttackPrefab";
        string hpPrefab = "Prefabs/002/BaseHPPrefab";

        [SerializeField] GameObject titleScreen;
        [SerializeField] HomeScreen homeScreen;
        AssetContainer assetContainer = null;

        void Start()
        {
            ABHub.Initialize();
            var settings = AssetBundleHubSettings.Instance;
            // 以下の値はサーバーからもらう方がより安全
            settings.baseUrl = "https://teach310.github.io/AssetBundleHubSample/AssetBundles/StandaloneOSX/";
            settings.assetBundleListName = "AssetBundleList.json";
            settings.assetBundleListUrl = settings.baseUrl + settings.assetBundleListName;
        }

        void SetSceneState(SceneState state)
        {
            sceneState = state;
        }

        public void OnClickTitleStart()
        {
            if (sceneState != SceneState.Title)
            {
                return;
            }
            StartAsync().Forget();
        }

        async UniTaskVoid StartAsync()
        {
            SetSceneState(SceneState.Starting);

            if (!ABHub.ExistsAssetBundleList())
            {
                await ABHub.DownloadAssetBundleList();
            }
            ABHub.LoadAndCacheAssetBundleList();

            downloader = ABHub.CreateDownloader();
            downloader.SetDownloadTarget(new List<string>() { attackPrefab, hpPrefab });
            if (downloader.DownloadSize > 0L)
            {
                string downloadSize = DownloadSizeText(downloader.DownloadSize);
                var isYes = await dialogYN.Show($"{downloadSize}のダウンロードを実行しますか？");
                if (!isYes)
                {
                    SetSceneState(SceneState.Title);
                    return;
                }
                var result = await DownloadAsync();
                if (!result.IsSuccess)
                {
                    Debug.Log("Download Failed " + result.Error.Message);
                    SetSceneState(SceneState.Title);
                    return;
                }
            }

            await LoadHomeAssetAsync();
            homeScreen.gameObject.SetActive(true);
            titleScreen.SetActive(false);
            SetSceneState(SceneState.Home);
            homeScreen.OnEnter();
        }

        async UniTask<AssetBundleDownloadResult> DownloadAsync()
        {
            progressDialog.DisplayProgress(0f);
            displayProgress = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f)); // 見栄えの問題でちょっと待つ。本来不要。
            AssetBundleDownloadResult result = null;

            try
            {
                result = await downloader.DownloadAsync();
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f)); // 見栄えの問題でちょっと待つ。本来不要。
            }
            finally
            {
                progressDialog.Hide();
                displayProgress = false;
            }

            return result;
        }

        async UniTask LoadHomeAssetAsync()
        {
            var homeAssets = new List<string>() { attackPrefab, hpPrefab };
            assetContainer = ABHub.CreateLoadContainer();
            await assetContainer.LoadAllAsync(homeAssets);
        }

        public void BackToTitle()
        {
            homeScreen.OnExit();
            assetContainer?.Dispose();
            assetContainer = null;
            homeScreen.gameObject.SetActive(false);
            titleScreen.SetActive(true);
            Resources.UnloadUnusedAssets();
            GC.Collect();
            SetSceneState(SceneState.Title);
        }

        void Update()
        {
            if (displayProgress)
            {
                var progress = downloader.CalcProgress();
                progressDialog.DisplayProgress(progress);
            }
        }

        public void OnClickDeleteSaveData()
        {
            var localAssetBundleTable = ServiceLocator.Instance.Resolve<ILocalAssetBundleTable>() as LocalAssetBundleTable;
            localAssetBundleTable.Clear();

            var settings = AssetBundleHubSettings.Instance;
            if (Directory.Exists(settings.SaveDataPath))
            {
                Directory.Delete(settings.SaveDataPath, true);
            }

            Debug.Log("SaveData deleted");
        }

        string DownloadSizeText(ulong downloadBytes)
        {
            var downloadKB = downloadBytes / 1024;
            if (downloadKB < 1024L)
            {
                return $"{downloadKB}KB";
            }

            var downloadMB = downloadKB / 1024;
            return $"{downloadMB}MB";
        }
    }
}
