using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownTimerUI : MonoBehaviour
{

    private Text _countdownText;
    private GameManager _gameManager;

    void Start()
    {
        _countdownText = GetComponent<Text>();
        if (_countdownText == null)
        {
            Debug.LogError("CountdownTimerUI: No Text component could be found.");
        }

    }

    void Update()
    {
        // TODO: Probably solve this in a different way.
        // Maybe listen to a certain event, or only create this object at game start or something...
        if (_gameManager == null)
        {
            _gameManager = FindObjectOfType<GameManager>();
            if (_gameManager == null)
            {
                // The game has not started yet.
                return;
            }
        }

        _countdownText.text = _gameManager.TimeLeft.ToString("0.0");
    }
}
