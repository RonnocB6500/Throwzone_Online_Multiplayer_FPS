using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowGameMultiplayer : NetworkBehaviour
{


    public const int MAX_PLAYER_AMOUNT = 8;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";


    public static ThrowGameMultiplayer Instance { get; private set; }


    public static bool playMultiplayer = true;


    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;


    [SerializeField] private List<Color> playerColorList;
    [SerializeField] private List<Color> playerCloakColorList;
    [SerializeField] private List<Color> playerBodyColorList;
    [SerializeField] private List<Color> playerHairColorList;


    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;
    public NetworkList<int> playerKills;



    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

        playerKills = new NetworkList<int>();
    }

    private void Start()
    {
        if (!playMultiplayer)
        {
            // Singleplayer
            StartHost();
            Loader.Load(Loader.Scene.MapSelectScene);
            //Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
        playerKills.Add(0);
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }
    public Color GetPlayerCloakColor(int colorId)
    {
        return playerCloakColorList[colorId];
    }
    public Color GetPlayerBodyColor(int colorId)
    {
        return playerBodyColorList[colorId];
    }
    public Color GetPlayerHairColor(int colorId)
    {
        return playerHairColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // Color not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public void ChangePlayerCloakColor(int cloakColorId)
    {
        ChangePlayerCloakColorServerRpc(cloakColorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerCloakColorServerRpc(int cloakColorId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.cloakColorId = cloakColorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerBodyColor(int bodyColorId)
    {
        ChangePlayerBodyColorServerRpc(bodyColorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerBodyColorServerRpc(int bodyColorId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.bodyColorId = bodyColorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerHairColor(int hairColorId)
    {
        ChangePlayerHairColorServerRpc(hairColorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerHairColorServerRpc(int hairColorId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.hairColorId = hairColorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }



    public void ChangePlayerCloak(int cloakOptionId)
    {
        ChangePlayerCloakServerRpc(cloakOptionId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerCloakServerRpc(int cloakOptionId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.cloakOptionId = cloakOptionId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerBody(int bodyOptionId)
    {
        ChangePlayerBodyServerRpc(bodyOptionId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerBodyServerRpc(int bodyOptionId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.bodyOptionId = bodyOptionId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerHair(int hairOptionId)
    {
        ChangePlayerHairServerRpc(hairOptionId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerHairServerRpc(int hairOptionId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.hairOptionId = hairOptionId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerHead(int headOptionId)
    {
        ChangePlayerHeadServerRpc(headOptionId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerHeadServerRpc(int headOptionId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.headOptionId = headOptionId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerEyes(int eyesOptionId)
    {
        ChangePlayerEyesServerRpc(eyesOptionId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerEyesServerRpc(int eyesOptionId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.eyesOptionId = eyesOptionId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    public void ChangePlayerMouth(int mouthOptionId)
    {
        ChangePlayerMouthServerRpc(mouthOptionId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerMouthServerRpc(int mouthOptionId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.mouthOptionId = mouthOptionId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }




    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                // Already in use
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

    public void AddKillForClientId(int clientId)
    {
        AddKillForClientIdServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddKillForClientIdServerRpc(int clientId)
    {
        playerKills[clientId] += 1;
    }
}
