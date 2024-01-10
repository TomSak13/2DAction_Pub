using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommonData", menuName = "ScriptableObjects/CommonParam", order = 1)]
public class CommonParam : ScriptableObject
{
    [SerializeField] private TitleData.Difficulty _gameDifficulty;

    public TitleData.Difficulty GameDifficulty { get => _gameDifficulty; set => _gameDifficulty = value; }

    
}
