using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private List<GameObject> backgrounds;
    private GameObject currentBackground;

    private void Awake()
    {
        currentBackground = Instantiate(backgrounds[Variables.LoadTable.chapterId], transform);
    }
}
