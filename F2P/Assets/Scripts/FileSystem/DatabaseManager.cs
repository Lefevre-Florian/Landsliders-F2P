using UnityEngine;

using Mono.Data.Sqlite;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


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

        // Events
        public event Action OnResourcesLoaded;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;

            if(Save.data == null)
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
                StartCoroutine(CopyDatabase());
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

        public List<object> GetRow(string pCmdText)
        {
            List<object> lResult = new List<object>();

            SqliteConnection lDB = OpenDatabase();

            SqliteCommand lCmd = lDB.CreateCommand();
            lCmd.CommandText = pCmdText;

            SqliteDataReader lReader = lCmd.ExecuteReader();

            int lLength = lReader.FieldCount;
            for (int i = 0; i < lLength; i++)
                lResult.Add(lReader.GetValue(i));

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

        public List<object> GetRowWhere<T>(string pCmdText, T pParam)
        {
            return GetRow(pCmdText + pParam.ToString());
        }
        #endregion

        #region Save system
        public void ReadDataFromSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (File.Exists(lPath))
            {
                Save.data = JsonUtility.FromJson<PlayerSave>(File.ReadAllText(lPath));
                Save.data.startTime = DateTime.UtcNow;

                List<List<object>> lResult = GetRowsWhereIN(SELECT_ALL_BIOMES_RESOURCES_WHERE, Save.data.cards);

                int lLength = lResult.Count;
                string[] lPaths = new string[lLength];

                for (int i = 0; i < lLength; i++)
                    lPaths[i] = lResult[i][0].ToString();

                StartCoroutine(RetrievePrefabs(lPaths));
            }
            else
            {
                // New save
                Save.data = new PlayerSave();
                Save.data.startTime = DateTime.UtcNow;
                
                List<List<object>> lRawDatas = GetRows(SELECT_ALL_BIOME_IDS_PATH);

                int lLength = lRawDatas.Count;
                Save.data.cards = new int[lLength];
                Save.data.fragments = new Fragment[lLength];

                string[] lPaths = new string[lLength];

                for (int i = 0; i < lLength; i++)
                {
                    Save.data.cards[i] = Convert.ToInt32(lRawDatas[i][0]);
                    Save.data.fragments[i] = new Fragment(Save.data.cards[i], 0);

                    lPaths[i] = lRawDatas[i][1].ToString();
                }

                StartCoroutine(RetrievePrefabs(lPaths));

                lRawDatas.Clear();
                WriteDataToSaveFile();
            }
        }

        public void WriteDataToSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;
            File.WriteAllText(lPath, JsonUtility.ToJson(Save.data), Encoding.UTF8);
        }

        private IEnumerator RetrievePrefabs(string[] pPaths)
        {
            AsyncOperationHandle<GameObject>[] lHandles = new AsyncOperationHandle<GameObject>[pPaths.Length];

            bool lIsDone = false;

            int lLength = lHandles.Length;
            int lTotal = 0;

            for (int i = 0; i < lLength; i++)
                lHandles[i] = Addressables.LoadAssetAsync<GameObject>(pPaths[i]);

            while (!lIsDone)
            {
                for (int i = 0; i < lLength; i++)
                {
                    if (lHandles[i].IsDone)
                        lTotal += 1;
                }

                if (lTotal == lLength)
                    lIsDone = true;
                else
                    lTotal = 0;

                yield return new WaitForEndOfFrame();
            }

            Save.data.cardPrefabs = new GameObject[lLength];

            for (int i = 0; i < lLength; i++)
                Save.data.cardPrefabs[i] = lHandles[i].Result;

            StopCoroutine(RetrievePrefabs(pPaths));

            OnResourcesLoaded?.Invoke();
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
