using UnityEngine;

using Mono.Data.Sqlite;

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;


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

        /// Database cmd
        private const string SELECT_ALL_BIOME_IDS = "SELECT id FROM BIOME WHERE level = 1";

        /// JSON Save file related const
        private const string FILE_NAME = "/Save.json";

        /// Save system
        public static PlayerSave playerSave = null;

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
        
        public List<List<object>> GetRows(string pCmdText)
        {
            List<List<object>> lResult = new List<List<object>>();

            SqliteConnection lDB = OpenDatabase();

            SqliteCommand lCmd = lDB.CreateCommand();
            lCmd.CommandText = pCmdText;

            SqliteDataReader lReader = lCmd.ExecuteReader();

            // Get the data set
            int lFieldCount = 0;
            List<object> lRow = null;

            while (lReader.Read())
            {
                lFieldCount = lReader.FieldCount;
                lRow = new List<object>();

                for (int i = 0; i < lFieldCount; i++)
                    lRow.Add(lReader.GetValue(i));

                lResult.Add(lRow);
            }

            CloseDatabase(lDB);
            return lResult;
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
                // New save
                playerSave = new PlayerSave();
                
                List<int> lCardIDs = new List<int>();
                List<List<object>> lRawDatas = GetRows(SELECT_ALL_BIOME_IDS);

                int lLength = lRawDatas.Count;
                for (int i = 0; i < lLength; i++)
                    lCardIDs.Add(Convert.ToInt32(lRawDatas[i][0])); 

                lRawDatas.Clear();
                lRawDatas = null;

                // Reprensting every IDs of unlocked cards
                playerSave.cards = lCardIDs.ToArray(); 

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
