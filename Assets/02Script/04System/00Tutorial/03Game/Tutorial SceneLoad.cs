using UnityEngine.SceneManagement;

public class TutorialSceneLoad : TutorialBase
{
    private UpgradeTable upgradeTable;
    
    private void Start()
    {
        upgradeTable ??= DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
    }
    
    public override void Enter()
    {
        if(Variables.LoadTable.StageId == 13001)
        {
            GameManager.instance.GameData.Game1TutorialCheck = true;
            GameManager.instance.GameData.StageChoiceTutorialCheck = true;
            GameManager.instance.GameData.ObtainedGachaIDs.Add(300470);
            GameManager.instance.GameData.characterIds.Add(300470);
            GameManager.instance.GameData.Diamond += 800;
            GameManager.instance.GameData.StageClear[Variables.LoadTable.StageId] = true;
            GameManager.instance.GameData.StageClearNum = Variables.LoadTable.StageId;
            
            DataManager.SaveFile(GameManager.instance.GameData);
        }

        if(Variables.LoadTable.StageId == 13002)
        {
            GameManager.instance.GameData.Game2TutorialCheck = true;
            GameManager.instance.GameData.DeckUITutorialCheck = true;
            GameManager.instance.GameData.ObtainedGachaIDs.Add(300596);
            GameManager.instance.GameData.characterIds.Add(300596);
            GameManager.instance.GameData.Diamond += 800;
            GameManager.instance.GameData.MonsterDragLevel[12031] = (int)GameData.MonsterDrag.UNLOCK;
            GameManager.instance.GameData.Gold += 100;
            GameManager.instance.GameData.StageClear[Variables.LoadTable.StageId] = true;
            GameManager.instance.GameData.StageClearNum = Variables.LoadTable.StageId;
            Variables.LoadTable.ItemId.Clear();
            DataManager.SaveFile(GameManager.instance.GameData);
        }
        
        if(Variables.LoadTable.StageId == 13003)
        {
            GameManager.instance.GameData.ShopDragTutorialCheck = true;
            GameManager.instance.GameData.Game3TutorialCheck = true; 
            GameManager.instance.GameData.ObtainedGachaIDs.Add(300530);
            GameManager.instance.GameData.characterIds.Add(300530);
            GameManager.instance.GameData.Diamond += 800;
            GameManager.instance.GameData.StageClear[Variables.LoadTable.StageId] = true;
            GameManager.instance.GameData.StageClearNum = Variables.LoadTable.StageId;
            
            DataManager.SaveFile(GameManager.instance.GameData);
        }        
    }

    public override void Execute(TutorialController controller)
    {
        DataManager.SaveFile(GameManager.instance.GameData);    
        controller.SetNextTutorial();
        SceneManager.LoadScene(1);
    }

    public override void Exit()
    {
    }
}


