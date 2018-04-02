using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class PowerMeter : MonoBehaviour
{
    private Image _image;
    private RectTransform _rectTransform;

    private Hero _hero;

    void Start()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // TODO: Probably solve this in a different way.
        // Maybe listen to a certain event, or only create this object at game start or something...
        if (_hero == null)
        {
            _hero = FindObjectsOfType<Hero>().Where(h => h.hasAuthority == true).FirstOrDefault();
            if (_hero == null)
            {
                // The game has not started yet.
                return;
            }
        }

        if (_hero.CurrentShootPower == 0)
        {
            _rectTransform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
			// TODO: Change the color of the powerbar according to the power amount?
			float amountFilled = _hero.CurrentShootPower/_hero.MaxShootPower;
			_rectTransform.localScale = new Vector3(amountFilled, 1, 1);
        }
    }
}
