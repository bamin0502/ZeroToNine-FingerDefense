using UnityEngine;

public class GameTutorialManager : MonoBehaviour
{
    public TutorialController gameTutorial;
    public TutorialController game2Tutorial;
    public TutorialController game3Tutorial;

    public void Start()
    {
        if (DataManager.LoadFile().Game1TutorialCheck==false && Variables.LoadTable.StageId==13001)
        {
            gameTutorial.gameObject.SetActive(true);
        }
        else if (DataManager.LoadFile().Game2TutorialCheck==false && Variables.LoadTable.StageId==13002)
        {
            game2Tutorial.gameObject.SetActive(true);
        }
        else if (DataManager.LoadFile().Game3TutorialCheck == false && Variables.LoadTable.StageId == 13003)
        {
            game3Tutorial.gameObject.SetActive(true);
        }
        else
            return;
    }
}
