using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {


    public ulong clientId;
    public int colorId;
    public int cloakColorId;
    public int bodyColorId;
    public int hairColorId;

    public int cloakOptionId;
    public int bodyOptionId;
    public int hairOptionId;
    public int headOptionId;
    public int eyesOptionId;
    public int mouthOptionId;

    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;


    public bool Equals(PlayerData other) {
        return 
            clientId == other.clientId && 
            colorId == other.colorId &&
            cloakColorId == other.cloakColorId &&
            bodyColorId == other.bodyColorId &&
            hairColorId == other.hairColorId &&
            cloakOptionId == other.cloakOptionId &&
            bodyOptionId == other.bodyOptionId &&
            hairOptionId == other.hairOptionId &&
            headOptionId == other.headOptionId &&
            eyesOptionId == other.eyesOptionId &&
            mouthOptionId == other.mouthOptionId &&
            playerName == other.playerName &&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref cloakColorId);
        serializer.SerializeValue(ref bodyColorId);
        serializer.SerializeValue(ref hairColorId);
        serializer.SerializeValue(ref cloakOptionId);
        serializer.SerializeValue(ref bodyOptionId);
        serializer.SerializeValue(ref hairOptionId);
        serializer.SerializeValue(ref headOptionId);
        serializer.SerializeValue(ref eyesOptionId);
        serializer.SerializeValue(ref mouthOptionId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }

}