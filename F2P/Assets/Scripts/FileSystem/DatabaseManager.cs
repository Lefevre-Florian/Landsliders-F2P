using UnityEngine;
using UnityEngine.Networking;

using Mono.Data.Sqlite;

using System.Data;
using System;
using System.IO;
using System.Collections;

using TMPro;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FileSystem
{
    public class DatabaseManager : MonoBehaviour
    {
        #region Singleton
        private static DatabaseManager _Instance = null;

        public static DatabaseManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new DatabaseManager();
            return _Instance;
        }

        private DatabaseManager() : base() { }
        #endregion

        /// Constant path
        private const string DATABASE_SOURCE = "URI=file:";
        private const string DATABASE_NAME = "/scopa.db";
        private const string FILE_NAME = "Save.json";

        /// SQL Command lines
        private const string SELECT_BIOMES = "SELECT name, description, expUnlocked FROM BIOME WHERE fk_upgrade IS NOT NULL";
        private const string SELECT_FRAGMENT = "SELECT biome.name, fragment.rarity FROM fragment LEFT JOIN biome ON biome.id = fragment.fk_biome ORDER BY fragment.rarity ASC";

        /// Save system
        public static PlayerSave playerSave = null;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _DatabaseLabelDebug = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;

            ReadDataFromSaveFile();
        }

        #region Database & Cache
        private void GetValues()
        {
            string lPath = "";
            
            #if UNITY_EDITOR
            lPath = Application.dataPath + "/StreamingAssets/Database" + DATABASE_NAME;
            #elif UNITY_ANDROID
            lPath = Application.streamingAssetsPath + DATABASE_NAME;
            #endif

            /*SqliteConnection lDb = new SqliteConnection(lPath);

            lDb.Open();

            if (lDb.State == ConnectionState.Open)
            {
                SqliteCommand lQuery = lDb.CreateCommand();
                lQuery.CommandText = SELECT_BIOMES;
                SqliteDataReader lReader = lQuery.ExecuteReader();
                int lLength = lReader.FieldCount;
            }
            else
            {
                _DatabaseLabelDebug.text = "No database found! ";
            }

            lDb.Close();*/
        }

        private IEnumerator GetDatabase()
        {
            string lPath = Application.streamingAssetsPath + "/scopa.db";
            UnityWebRequest www = UnityWebRequest.Get(lPath);
            string lResult = www.downloadHandler.text;
            print(lResult);

            yield return null;
        }
        #endregion

        #region Save system
        public void ReadDataFromSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (File.Exists(lPath))
                playerSave = JsonUtility.FromJson<PlayerSave>(File.ReadAllText(lPath));
            else
            {
                playerSave = new PlayerSave();
                WriteDataToSaveFile();
            }
                
        }

        public void WriteDataToSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (!File.Exists(lPath))
                File.Create(lPath);

            File.WriteAllText(lPath, JsonUtility.ToJson(playerSave));
        }
        #endregion

        private void OnDestroy()
        {
            if(_Instance == this)
            {
                _Instance = null;
            }
        }
    }
}
