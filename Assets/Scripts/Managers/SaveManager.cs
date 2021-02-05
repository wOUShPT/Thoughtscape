using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public ScoreScriptableObject scoreScriptableObject;
    private AudioManager _audioManager;
    private InputManager _inputManager;
    private int _score;
    private bool _audioOnOff;
    private bool _vibrationOnOff;
    void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _inputManager = FindObjectOfType<InputManager>();
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/0.sav"))
        {
                FileStream fs = File.OpenRead(Application.persistentDataPath + "/0.sav");
                BinaryFormatter formatter = new BinaryFormatter();
                Data data = (Data)formatter.Deserialize(fs);
                scoreScriptableObject.bestScore = data.score;
                _audioManager.isMuted = data.audioMute;
                _inputManager.canVibrate = data.vibrationOnOff;
                fs.Close();
        }
    }
    
    public void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fS = File.OpenWrite(Application.persistentDataPath + "/0.sav");
        Data data = new Data();
        data.score = scoreScriptableObject.bestScore;
        data.audioMute = _audioManager.isMuted;
        data.vibrationOnOff = _inputManager.canVibrate;
        formatter.Serialize(fS, data);
        fS.Close();
    }


    [Serializable]
    private struct Data
    {
        public int score;
        public bool audioMute;
        public bool vibrationOnOff;
    }
}

