using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using TMPro;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Database
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
        private const string SELECT_PACK = "SELECT price,name FROM PACK ORDER BY price DESC";
        private const string SELECT_FRAGMENT = "SELECT biome.name, fragment.rarity FROM fragment LEFT JOIN biome ON biome.id = fragment.fk_biome ORDER BY fragment.rarity ASC";

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

            //TEMP
            GetValues();
        }

        private void GetValues()
        {
            string lPath = "";
            
            #if UNITY_EDITOR
            lPath = DATABASE_SOURCE + Application.dataPath + "/StreamingAssets/Database" + DATABASE_NAME;
            #elif UNITY_ANDROID || UNITY_IOS
            lPath = DATABASE_SOURCE + Application.streamingAssetsPath + "/StreamingAssets/Database" + DATABASE_NAME;
            #endif

            SqliteConnection lDb = new SqliteConnection(lPath);
            lDb.Open();

            if (lDb.State == ConnectionState.Open)
            {
                SqliteCommand lQuery = lDb.CreateCommand();
                lQuery.CommandText = "SELECT name FROM biome";
                object lResult = lQuery.ExecuteScalar();
                _DatabaseLabelDebug.text = lResult.ToString();
                Debug.Log("Connection established !");
            }
            else
            {
                _DatabaseLabelDebug.text = "No database found! ";
            }

            lDb.Close();
        }

        public void ReadDataFromSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (File.Exists(lPath))
                playerSave = JsonUtility.FromJson<PlayerSave>(File.ReadAllText(lPath));
            else
                playerSave = new PlayerSave();
        }

        public void WriteDataToSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (!File.Exists(lPath))
                File.Create(lPath);

            File.WriteAllText(lPath, JsonUtility.ToJson(playerSave));
        }

        private void OnDestroy()
        {
            if(_Instance == this)
            {
                _Instance = null;
            }
        }
    }
}
