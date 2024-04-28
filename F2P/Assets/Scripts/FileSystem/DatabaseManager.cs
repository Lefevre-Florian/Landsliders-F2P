using UnityEngine;

using Mono.Data.Sqlite;

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

        #if UNITY_EDITOR
        private const string DATABASE_PATH = "/StreamingAssets";
        #endif

        private const string DATABASE_SOURCE = "URI=file:";
        private const string DATABASE_NAME = "/scopa.db";
        
        private const string FILE_NAME = "Save.json";

        /// Save system
        public static PlayerSave playerSave = null;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _DatabaseLabelDebug = null;

        // Variables
        private SqliteConnection _Database = null;

        private Coroutine _Session = null;

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
        private IEnumerator OpenDatabase()
        {
            string lPath;
            #if UNITY_EDITOR
            lPath = Application.dataPath + DATABASE_PATH + DATABASE_NAME;
            #endif

            _Database = new SqliteConnection(DATABASE_SOURCE + lPath);
            _Database.Open();

            if(_Session != null)
            {
                StopCoroutine(_Session);
                _Session = null;
            }

            yield return null;
        }

        private void CloseDatabase()
        {
            if(_Database != null)
            {
                _Database.Close();
                _Database = null;
            }
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

                CloseDatabase();
                StopAllCoroutines();

                _Session = null;
            }
        }
    }
}
