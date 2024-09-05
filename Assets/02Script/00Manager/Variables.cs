using System.Collections.Generic;

namespace Variables
{
    public static class LoadTable
    {
        //테스트 (스테이지 UI 버튼을 누르면 그 스테이지 id를 넘겨줄 예정)
        public static int StageId;
        
        //아이템 ID, 현재 가지고 있는 개수를 저장할 리스트
        public static List<(int itemId,int itemCount)> ItemId = new List<(int, int)>(); 
       
        
        //여기다가 슬롯 UI에서 선택한 캐릭터 ID를 넘겨줄 예정
        //To-Do 다시 8개로 변경 예정
        public static int[] characterIds = new int[8];

        public static int chapterId;

        public static bool isNextStage = false;
    }

}