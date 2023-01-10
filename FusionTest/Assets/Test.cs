using NetStack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


// Create a one-time allocation buffer pool
static class BufferPool
{
    [ThreadStatic]
    private static BitBuffer bitBuffer;

    public static BitBuffer GetBitBuffer()
    {
        if (bitBuffer == null)
            bitBuffer = new BitBuffer(1024);

        return bitBuffer;
    }
}


// Define a networking message
struct MessageObject
{
    public const ushort id = 1; // Used to identify the message, can be packed or sent as packet header
    public uint peer;
    public byte race;
    public ushort skin;

    public void Serialize(ref Span<byte> packet)
    {
        BitBuffer data = BufferPool.GetBitBuffer();

        data.AddUInt(peer)
        .AddByte(race)
        .AddUShort(skin)
        .ToSpan(ref packet);

        data.Clear();
    }

    public void Deserialize(ref ReadOnlySpan<byte> packet, int length)
    {
        BitBuffer data = BufferPool.GetBitBuffer();
        data.FromSpan(ref packet, length);

        peer = data.ReadUInt();
        race = data.ReadByte();
        skin = data.ReadUShort();

        data.Clear();
    }
}