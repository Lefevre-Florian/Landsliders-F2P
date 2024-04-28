using UnityEngine;

using Mono.Data.Sqlite;

using System.IO;
using System.Collections;

using TMPro;
using System.Text;

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
        
        /// Database related const
        private const string DATABASE_SOURCE = "URI=file:";
        private const string DATABASE_NAME = "/scopa.db";

        /// JSON Save file related const
        private const string FILE_NAME = "/Save.json";

        /// Save system
        public static PlayerSave playerSave = null;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _DatabaseLabelDebug = null;

        // Variables
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
        /// <summary>
        /// Return an open sql connection to the database
        /// </summary>
        /// <returns></returns>
        private SqliteConnection OpenDatabase()
        {
            string lPath;
            #if UNITY_EDITOR
            lPath = Application.dataPath + DATABASE_PATH + DATABASE_NAME;
            #elif UNITY_ANDROID
            lPath = Application.persistentDataPath + DATABASE_NAME;
            
            if (!File.Exists(lPath))
            {
                _Session = StartCoroutine(CopyDatabase());
            }
            #endif

            SqliteConnection lDB = new SqliteConnection(DATABASE_SOURCE + lPath);
            lDB.Open();

            return lDB;
        }

        private void CloseDatabase(SqliteConnection pDB)
        {
            if(pDB != null 
               && pDB.State != System.Data.ConnectionState.Closed)
                pDB.Close();
        }

        /// <summary>
        /// Copy the database from StreamingAssets to PersistentPath on Android in order to use the database 
        /// (Operation done only when the user start the application for the first time)
        /// </summary>
        /// <returns></returns>
        private IEnumerator CopyDatabase()
        {
            string lPath = Application.streamingAssetsPath + DATABASE_NAME;
            byte[] lContent;

            // Require reading in the .jar compressed file
            WWW lWWWByteReader = new WWW(lPath);
            while(!lWWWByteReader.isDone)
                yield return null;

            lContent = lWWWByteReader.bytes;

            // Copy to the persistent app storage location
            File.WriteAllBytes(Application.persistentDataPath + DATABASE_NAME, lContent);

            if(_Session != null)
            {
                StopCoroutine(_Session);
                _Session = null;
            }

            yield return null;
        }
        #endregion

        #region Save system
        public void ReadDataFromSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;
            print(lPath);
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
            File.WriteAllText(lPath, JsonUtility.ToJson(playerSave), Encoding.UTF8);
        }
        #endregion

        private void OnDestroy()
        {
            if(_Instance == this)
            {
                _Instance = null;

                StopAllCoroutines();
                _Session = null;
            }
        }
    }
}
