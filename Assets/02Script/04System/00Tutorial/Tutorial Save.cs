using UnityEngine.SceneManagement;

public class TutorialSave : TutorialBase
{
    public TutorialController saveObject;
    
    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (saveObject.gameObject.CompareTag("ShopDrag"))
        {
            GameManager.instance.GameData.ShopDragTutorialCheck = true;
            DataManager.SaveFile(GameManager.instance.GameData);
            controller.SetNextTutorial();
            SceneManager.LoadScene(1);
            Logger.Log("저장 완료");
        }
        if (saveObject.gameObject.CompareTag("ShopGimmick"))
        {
            GameManager.instance.GameData.ShopGimmickTutorialCheck = true;
            DataManager.SaveFile(GameManager.instance.GameData);
            controller.SetNextTutorial();
            Logger.Log("저장 완료");
        }
        if (saveObject.gameObject.CompareTag("ShopCharacter"))
        {
            GameManager.instance.GameData.ShopCharacterTutorialCheck = true;
            DataManager.SaveFile(GameManager.instance.GameData);
            controller.SetNextTutorial();
            Logger.Log("저장 완료");
        }
        if (saveObject.gameObject.CompareTag("ShopStage"))
        {
            GameManager.instance.GameData.ShopFeatureTutorialCheck = true;
            DataManager.SaveFile(GameManager.instance.GameData);
            controller.SetNextTutorial();
            Logger.Log("저장 완료");
        }
        else
        {   
        }
    }

    public override void Exit()
    {
    }
}

