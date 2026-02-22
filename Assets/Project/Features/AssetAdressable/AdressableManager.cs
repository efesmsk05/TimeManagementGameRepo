using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    public static AddressableManager Instance { get; private set; }

    // --- DEPO: Yüklenen her şeyi burada kategorize edip tutuyoruz ---
    // Key: Label Adı (örn: "Customer"), Value: Yüklenen Objeler Listesi
    private Dictionary<string, IList<GameObject>> _loadedGameObjects = new Dictionary<string, IList<GameObject>>();
    private Dictionary<string, IList<AudioClip>> _loadedAudioClips = new Dictionary<string, IList<AudioClip>>();

    // Hafıza temizliği için Handle'ları tutuyoruz
    private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // --- GENEL YÜKLEME FONKSİYONU (GAMEOBJECT İÇİN) ---
    // Label: "Customer", "Food" vb.
    public void LoadAssetByLabel<T>(string label, Action<IList<T>> onComplete) where T : UnityEngine.Object
    {
        // Addressables sistemine "Bu etikete sahip her şeyi getir" diyoruz
        Addressables.LoadAssetsAsync<T>(label, null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"[{label}] etiketiyle {handle.Result.Count} adet obje yüklendi.");
                
                // Handle'ı kaydet (İleride silmek için)
                _handles.Add(handle);

                // Callback ile çağıran yere listeyi ver
                onComplete?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"[{label}] yüklenirken hata oluştu!");
            }
        };
    }
    
    // --- TEMİZLİK ---
    // Sahne değişirken veya oyun kapanırken hafızayı boşaltmak için
    public void ReleaseAllAssets()
    {
        foreach (var handle in _handles)
        {
            Addressables.Release(handle);
        }
        _handles.Clear();
        _loadedGameObjects.Clear();
        _loadedAudioClips.Clear();
        Debug.Log("Tüm Addressable varlıkları RAM'den temizlendi.");
    }

    // public bool IsPoolEmpty()
    // {
        
     
    // }
}