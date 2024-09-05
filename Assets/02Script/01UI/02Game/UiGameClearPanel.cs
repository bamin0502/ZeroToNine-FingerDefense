using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiGameClearPanel : MonoBehaviour
{
    public List<TextMeshProUGUI> textIsAlreadyGet;

    public void ActiveRewardGetText(bool isAlreadyGet)
    {
        foreach (var text in textIsAlreadyGet)
        {
            text.gameObject.SetActive(isAlreadyGet);
        }
    }
}
