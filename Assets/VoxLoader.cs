using Cubizer.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VoxLoader : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    Vector3 globalScale;
    List<VoxelCut.VoxelDesc> _voxels;
    List<Transform> _cells;
    uint[] _matrix;
    public void LoadQB2File(Transform parent, string path)
    {
        StartCoroutine(wwwLoadQB2(parent, path));
    }

    IEnumerator wwwLoadQB2(Transform parent, string path)
    {
        WWW www = new WWW(path);

        yield return www;
        if (www.error == null)
        {
            MemoryStream stream = new MemoryStream(www.bytes);
            using (var reader = new BinaryReader(stream))
            {
                _voxels = new List<VoxelCut.VoxelDesc>();
                _cells = new List<Transform>();
                var version = reader.ReadUInt32(); ;
                var colorFormat = reader.ReadUInt32();
                Debug.Log("colorFormat is " + colorFormat);
                var zAxisOrientation = reader.ReadUInt32();
                Debug.Log("zAxisOrientation is " + zAxisOrientation);
                var compressed = reader.ReadUInt32();
                var visibilityMaskEncoded = reader.ReadUInt32();
                Debug.Log("visibilityMaskEncoded is " + visibilityMaskEncoded);
                var numMatrices = reader.ReadUInt32();
                //var matrixList = new List<uint[]>();

                VoxelCut vc = GetComponent<VoxelCut>();
                Vector3 sizeMax = Vector3.zero;
                for (uint i = 0; i < numMatrices; i++) // for each matrix stored in file
                {
                    // read matrix name
                    var nameLength = reader.ReadByte();
                    var name = reader.ReadChars(nameLength);

                    // read matrix size 
                    var sizeX = reader.ReadUInt32();
                    var sizeY = reader.ReadUInt32();
                    var sizeZ = reader.ReadUInt32();

                    

                    // read matrix position (in this example the position is irrelevant)
                    var posX = reader.ReadUInt32();
                    var posY = reader.ReadUInt32();
                    var posZ = reader.ReadUInt32();

                    // create matrix and add to matrix list
                    _matrix = new uint[sizeX * sizeY * sizeZ];
                    //matrixList.Add(matrix);
                    MaterialPropertyBlock props = new MaterialPropertyBlock();
                    if (compressed == 0) // if uncompressd
                    {
                        for (int z = 0; z < sizeZ; z++)
                        {
                            for (int y = 0; y < sizeY; y++)
                            {
                                for (int x = 0; x < sizeX; x++)
                                {
                                    uint color = reader.ReadUInt32();
                                    _matrix[x + y * sizeX + z * sizeX * sizeY] = color;
                                    if (color != 0)
                                    {
                                        if (sizeMax.x < x)
                                        {
                                            sizeMax.x = x;
                                        }
                                        if (sizeMax.y < y)
                                        {
                                            sizeMax.y = y;
                                        }
                                        if (sizeMax.z < z)
                                        {
                                            sizeMax.z = z;
                                        }
                                        GameObject newone = Instantiate(Resources.Load("cell") as GameObject);
                                        newone.transform.parent = parent.transform;
                                        newone.transform.position = new Vector3(x - sizeX * 0.5f,
                                            y - sizeY * 0.5f,
                                            z - sizeZ * 0.5f);
                                        //newone.transform.localScale = new Vector3(vc.CellSize, vc.CellSize, vc.CellSize);
                                        MeshRenderer mr = newone.GetComponent<MeshRenderer>();
                                        byte A = (byte)((color >> 24) & 0xFF);
                                        byte B = (byte)((color >> 16) & 0xFF);
                                        byte G = (byte)((color >> 8) & 0xFF);
                                        byte R = (byte)((color) & 0xFF);


                                        props.SetColor("_Color", new Color32(R, G, B, A));


                                        mr.SetPropertyBlock(props);
                                        //mr.sharedMaterial.SetColor("Color", new Color32(R, G, B, A));

                                        VoxelCut.VoxelDesc vd = new VoxelCut.VoxelDesc
                                        {
                                            x = x,
                                            y = y,
                                            z = z,
                                            cell = newone
                                        };
                                        _voxels.Add(vd);
                                        _cells.Add(newone.transform);
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                        foreach (VoxelCut.VoxelDesc vd in _voxels)
                        {
                            if (vd.x == 0 || vd.y == 0 || vd.z == 0)
                                continue;
                            if (vd.x == sizeX - 1 || vd.y == sizeY - 1 || vd.z == sizeZ - 1)
                                continue;
                            if (_matrix[(vd.x - 1) + vd.y * sizeX + vd.z * sizeY * sizeX] == 0)
                            {
                                continue;
                            }
                            else if (_matrix[(vd.x + 1) + vd.y * sizeX + vd.z * sizeY * sizeX] == 0)
                            {
                                continue;
                            }
                            else if (_matrix[vd.x + (vd.y - 1) * sizeX + vd.z * sizeY * sizeX] == 0)
                            {
                                continue;
                            }
                            else if (_matrix[vd.x + (vd.y + 1) * sizeX + vd.z * sizeY * sizeX] == 0)
                            {
                                continue;
                            }
                            else if (_matrix[vd.x  + vd.y * sizeZ + (vd.z - 1) * sizeY * sizeX] == 0)
                            {
                                continue;
                            }
                            else if (_matrix[vd.x + vd.y * sizeZ + (vd.z + 1) * sizeY * sizeX] == 0)
                            {
                                continue;
                            }
                             MeshRenderer mr = vd.cell.GetComponent<MeshRenderer>();
                             mr.enabled = false;
                        }
                    }
                    else // if compressed
                    {
                        /*var z = 0;

                        while (z < sizeZ)
                        {
                            z++;
                            var index = 0;

                            while (true)
                            {
                                var data = reader.ReadUInt32();

                                if (data == NEXTSLICEFLAG)
                                    break;
                                else if (data == CODEFLAG)
                                {
                                    var count = reader.ReadUInt32();
                                    data = reader.ReadUInt32();

                                    for (uint j = 0; j < count; j++)
                                    {
                                        var x = index % sizeX + 1; // mod = modulo e.g. 12 mod 8 = 4
                                        var y = index / sizeX + 1; // div = integer division e.g. 12 div 8 = 1
                                        index++;
                                        matrix[x + y * sizeX + z * sizeX * sizeY] = data;
                                    }
                                }
                                else
                                {
                                    var x = index % sizeX + 1;
                                    var y = index / sizeX + 1;
                                    index++;
                                    matrix[x + y * sizeX + z * sizeX * sizeY] = data;
                                }
                            }
                        }
                        */
                    }
                }
                vc.OnLoadCompleted(parent, _voxels, _cells, _matrix, sizeMax);
            }
        }
    }

    /*

    function LoadNode(stream)
    {
        nodeTypeID = stream.readUInt;
        dataSize = stream.readUInt;

        switch nodeTypeID
          case 0:
            loadMatrix(stream);
        case 1:
            loadModel(stream);
        case 2:
            loadCompound(stream);
    else
      stream.seek(dataSize) // skip node if unknown
    }

        function loadModel(stream)
        {
            childCount = stream.loadUInt;
            for (i = 0; i < childCount; i++)
                loadNode(stream);
        }

        function loadMatrix(stream)
        {
            nameLength = stream.readInt;
            name = stream.readString(nameLength);
            position.x = stream.readInt;
            position.y = stream.readInt;
            position.z = stream.readInt;
            localScale.x = stream.readInt;
            localScale.y = stream.readInt;
            localScale.z = stream.readInt;
            pivot.x = stream.readFloat;
            pivot.y = stream.readFloat;
            pivot.z = stream.readFloat;
            size.x = stream.readUInt;
            size.y = stream.readUInt;
            size.z = stream.readUInt;
            decompressStream = new zlibDecompressStream(stream);
            for (x = 0; x < size.x; x++)
                for (z = 0; z < size.z; z++)
                    for (y = 0; y < size.y; y++)
                        voxelGrid[x, y, z] = decompressStream.ReadBuffer(4);
        }

        function loadCompound(stream)
        {
            nameLength = stream.readInt;
            name = stream.readString(nameLength);
            position.x = stream.readInt;
            position.y = stream.readInt;
            position.z = stream.readInt;
            localScale.x = stream.readInt;
            localScale.y = stream.readInt;
            localScale.z = stream.readInt;
            pivot.x = stream.readFloat;
            pivot.y = stream.readFloat;
            pivot.z = stream.readFloat;
            size.x = stream.readUInt;
            size.y = stream.readUInt;
            size.z = stream.readUInt;

            decompressStream = new zlibDecompressStream(stream);
            for (x = 0; x < size.x; x++)
                for (z = 0; z < size.z; z++)
                    for (y = 0; y < size.y; y++)
                        voxelGrid[x, y, z] = decompressStream.ReadBuffer(4);

            childCount = stream.loadUInt;
            if (mergeCompounds)
            { // if you don't need the datatree you can skip child nodes
                for (i = 0; i < childCount; i++)
                    skipNode(stream);
            }
            else
            {
                for (i = 0; i < childCount; i++)
                    LoadNode(stream);
            }
        }

        function skipNode(stream)
        {
            stream.readInt; // node type, can be ignored
            dataSize = stream.readUInt;
            stream.seek(dataSize);
        }
        */


}
