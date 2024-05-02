using UnityEngine;

using Mono.Data.Sqlite;

using System;
using System.IO;
using System.Text;
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
        private const string SELECT_ALL_BIOME_IDS_PATH = "SELECT id, prefabPath FROM BIOME WHERE level = 1";
        private const string SELECT_ALL_BIOMES_RESOURCES_WHERE = "SELECT prefabPath FROM BIOME WHERE id IN ";

        /// JSON Save file related const
        private const string FILE_NAME = "/Save.json";

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

        public List<List<object>> GetRowsWhereIN<T>(string pCmdText, T[] pParams)
        {
            int lLength = pParams.Length;
            string lCmdParam = "(";
            for (int i = 0; i < lLength; i++)
            {
                lCmdParam += pParams[i].ToString();

                if (i == lLength - 1)
                    lCmdParam += ')';
                else
                    lCmdParam += ',';
            }

            return GetRows(pCmdText + lCmdParam);
        }

        #endregion

        #region Save system
        public void ReadDataFromSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (File.Exists(lPath))
            {
                Save.data = JsonUtility.FromJson<PlayerSave>(File.ReadAllText(lPath));

                List<List<object>> lResult = GetRowsWhereIN(SELECT_ALL_BIOMES_RESOURCES_WHERE, Save.data.cards);
                Save.data.cardPrefabs = new GameObject[lResult.Count];

                int lLength = lResult.Count;
                for (int i = 0; i < lResult.Count; i++)
                    Save.data.cardPrefabs[i] = Resources.Load<GameObject>(lResult[i][0].ToString());
            }
            else
            {
                // New save
                Save.data = new PlayerSave();
                
                List<GameObject> lCardsPrefabs = new List<GameObject>();

                List<List<object>> lRawDatas = GetRows(SELECT_ALL_BIOME_IDS_PATH);

                int lLength = lRawDatas.Count;
                Save.data.cards = new int[lLength];
                Save.data.fragments = new Fragment[lLength];

                for (int i = 0; i < lLength; i++)
                {
                    Save.data.cards[i] = Convert.ToInt32(lRawDatas[i][0]);
                    Save.data.fragments[i] = new Fragment(Save.data.cards[i], 0);

                    lCardsPrefabs.Add(Resources.Load<GameObject>(lRawDatas[i][1].ToString()));
                }

                lRawDatas.Clear();
                WriteDataToSaveFile();
            }   
        }

        public void WriteDataToSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;
            File.WriteAllText(lPath, JsonUtility.ToJson(Save.data), Encoding.UTF8);
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
