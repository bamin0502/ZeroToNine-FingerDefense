using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour, IResourceObserver
{
    public TextMeshProUGUI playerNameText;
    private GameManager gameManager;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI ticketText;

    private void Start()
    {
        gameManager = GameManager.instance;
        
        gameManager.GameData.RegisterObserver(this);
        UpdatePlayerInfo();
    }

    private void OnDestroy()
    {
        if (gameManager != null && gameManager.GameData != null)
        {
            gameManager.GameData.RemoveObserver(this);
        }
    }

    public void OnResourceUpdate(ResourceType resourceType, int newValue)
    {
        switch (resourceType)
        {
            case ResourceType.Gold:
                goldText.text = newValue.ToString();
                break;
            case ResourceType.Diamond:
                diamondText.text = newValue.ToString();
                break;
            case ResourceType.Ticket:
                ticketText.text = newValue.ToString();
                break;
        }
    }

    private void UpdatePlayerInfo()
    {
        if (playerNameText)
        {
            playerNameText.text = gameManager.GameData.PlayerName;
        }
        
        if (goldText)
        {
            goldText.text = gameManager.GameData!.Gold.ToString();
        }

        if (diamondText)
        {
            diamondText.text = gameManager.GameData!.Diamond.ToString();
        }

        if (ticketText)
        {
            ticketText.text = gameManager.GameData!.Ticket.ToString();
        }
    }
}
