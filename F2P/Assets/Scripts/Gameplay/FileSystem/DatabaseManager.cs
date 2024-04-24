using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

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
        private const string DATABASE_PATH = "URI:GameDatabase.db";
        private const string FILE_NAME = "Save.json";

        /// SQL Command Line
        private const string SELECT_PLAYER = "SELECT (id, musicVolume, soundVolume, softcurrency, hardcurrency) FROM PLAYER";

        public PlayerSave _PlayerSave = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;
        }

        private void Start()
        {
            
        }

        private void GetValues()
        {
            SqliteConnection lDb = new SqliteConnection(DATABASE_PATH);
            lDb.Open();

            if (lDb.State == ConnectionState.Open)
            {
                SqliteCommand lCommandLine = lDb.CreateCommand();
                lCommandLine.CommandText = SELECT_PLAYER;

                IDataReader lReader = lCommandLine.ExecuteReader();
                while (lReader.Read())
                {
                    Debug.Log(lReader["softcurrency"]);
                }
            }

            lDb.Close();
        }

        private void SetValues()
        {

        }

        public void ReadDataFromSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (!File.Exists(lPath))
                return;

            _PlayerSave = JsonUtility.FromJson<PlayerSave>(File.ReadAllText(lPath));
        }

        public void WriteDataToSaveFile()
        {
            string lPath = Application.persistentDataPath + FILE_NAME;

            if (!File.Exists(lPath))
                File.Create(lPath);

            File.WriteAllText(lPath, JsonUtility.ToJson(_PlayerSave));
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
