using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoxelBuffer
{
    public ComputeBuffer noiseBuffer;
    public ComputeBuffer countBuffer;
    public bool Initialized;
    public bool Cleared;
    public IndexedArray<Voxel> voxelArray;

    public void InitializeBuffer()
    {
        countBuffer = new ComputeBuffer(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        voxelArray = new IndexedArray<Voxel>();
        noiseBuffer = new ComputeBuffer(voxelArray.Count, 4);
        noiseBuffer.SetData(voxelArray.GetData);
        Initialized = true;
    }

    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        Initialized = false;
    }

    public Voxel this[Vector3 index]
    {
        get
        {
            return voxelArray[index];
        }

        set
        {
            voxelArray[index] = value;
        }
    }
}
