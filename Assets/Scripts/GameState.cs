using System.Collections;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    void Start () {
        Instance = this;
        StartCoroutine("StartGame");
    }

    public void OnDie () {
        StartCoroutine("DieActions");
    }

    public void Restart () {

    }

    private IEnumerator DieActions () {
        yield break;
    }

    public IEnumerator StartGame () {
        yield break;
    }
}
