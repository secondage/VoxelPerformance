using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoxelCut : MonoBehaviour
{
    public class VoxelDesc
    {
        public int x, y, z;
        public GameObject cell;
    }
    //public float MaxSize = 30.0f;
    //public int Density = 4;
    //public float CellSize = 0.1f;
    public Vector3 sizeMax = Vector3.zero;
    public Camera maincamera;
    public GameObject cutObject;
    public Toggle objectRotToggle;
    public Toggle cutRotToggle;

    public Transform dummyRight;
    public Transform dummyLeft;
    
    public Button btnCut;
    public Button btnReset;

    public Text scoreLeft;
    public Text scoreRight;
    public Text missionText;
    public RectTransform controllerPanel;
    static public int rotateHot = 1;
    private List<VoxelDesc> voxels;
    private List<Transform> cells;
    private uint[] voxelfill;

    public GameObject billboardPanel;
    public GameObject tilePanel;
    public Image blinkingImg;

    private int currentMission = 50;
    private int currentLevel = 1;

    private int GameStage = 0; //0 : title, 1 : mainmenu, 2 : game set
    private void Awake()
    {
        DOTween.Init(false, false, LogBehaviour.ErrorsOnly);
        DOTween.useSafeMode = false;
        DOTween.useSmoothDeltaTime = false;
        DOTween.defaultUpdateType = UpdateType.Late;
    }

    // Use this for initialization
    void Start()
    {
#if OLD
        voxelfill = new byte[Density * Density * Density];
        for (int x = 0; x < Density; ++x)
        {
            for (int y = 0; y < Density; ++y)
            {
                for (int z = 0; z < Density; ++z)
                {
                    Vector3 p = new Vector3(x * CellSize - MaxSize * 0.5f, y * CellSize - MaxSize * 0.5f, z * CellSize - MaxSize * 0.5f);
                    float d = (p).magnitude;

                    if (d <= MaxSize * 0.5)
                    {
                        GameObject newone = Instantiate(Resources.Load("cell") as GameObject);
                        newone.transform.parent = this.transform;
                        newone.transform.position = new Vector3(x * CellSize - MaxSize * 0.5f, y * CellSize - MaxSize * 0.5f, z * CellSize - MaxSize * 0.5f);
                        newone.transform.localScale = new Vector3(CellSize, CellSize, CellSize);
                        cells.Add(newone.transform);
                        voxelfill[x * Density * Density + y * Density + z] = 1;
                        VoxelDesc vd = new VoxelDesc
                        {
                            x = x,
                            y = y,
                            z = z,
                            cell = newone
                        };
                        voxels.Add(vd);
                    }
                }
            }
        }

        //surface only voxel can be render
        foreach (VoxelDesc vd in voxels)
        {
            if (vd.x == 0 || vd.y == 0 || vd.z == 0)
                continue;
            if (vd.x == Density - 1 || vd.y == Density - 1 || vd.z == Density - 1)
                continue;
            if (voxelfill[(vd.x - 1) * Density * Density + vd.y * Density + vd.z] == 0)
            {
                continue;
            }
            else if (voxelfill[(vd.x + 1) * Density * Density + vd.y * Density + vd.z] == 0)
            {
                continue;
            }
            else if (voxelfill[vd.x * Density * Density + (vd.y - 1) * Density + vd.z] == 0)
            {
                continue;
            }
            else if (voxelfill[vd.x * Density * Density + (vd.y + 1) * Density + vd.z] == 0)
            {
                continue;
            }
            else if (voxelfill[vd.x * Density * Density + vd.y * Density + vd.z - 1] == 0)
            {
                continue;
            }
            else if (voxelfill[vd.x * Density * Density + vd.y * Density + vd.z + 1] == 0)
            {
                continue;
            }
            MeshRenderer mr = vd.cell.GetComponent<MeshRenderer>();
            mr.enabled = false;
        }
#endif
        if (GameStage == 0)
        {
            ReloadObject(-1, "halfit");
        }
        else
        {
            StartLevel(currentLevel);
        }




    }

    void StartLevel(int level)
    {
        scoreLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(scoreLeft.GetComponent<RectTransform>().anchoredPosition.x, 472);
        scoreRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(scoreRight.GetComponent<RectTransform>().anchoredPosition.x, 472);
        currentMission = UnityEngine.Random.Range(40, 60);
        missionText.text = "本关任务 : " + currentMission;
        ReloadObject(level);

        btnCut.interactable = true;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        cutObject.transform.localEulerAngles = new Vector3(0, 1, 0);
        GameObject objn = GameObject.Find("rootN");
        if (objn != null)
        {
            objn.transform.DOKill();
            DestroyObject(objn);
        }
        GameObject objp = GameObject.Find("rootP");
        if (objp != null)
        {
            objp.transform.DOKill();
            DestroyObject(objp);
        }
        cutObject.gameObject.SetActive(true);
        cutObject.transform.position = Vector3.zero;
        GetComponent<DragRotate>().Position = 0.0f;
      
    }

    void ReloadObject(int level, string force = "")
    {
        if (voxels != null)
        {
            voxels.Clear();
            voxels = null;
        }
        if (cells != null)
        {
            cells.Clear();
            cells = null;
        }
        if (voxelfill != null)
        {
            voxelfill = null;
        }
        VoxLoader vl = GetComponent<VoxLoader>();
        //vl.CreateVoxelGrid(rootP.gameObject, Application.dataPath + "/StreamingAssets/" + "untitled1.vox", "test", 1.0f);
        /* vl.LoadQB2(this.gameObject, Application.streamingAssetsPath + "untitled2.qb",
         out voxels,
         out cells,
         out voxelfill);*/
        if (force != "")
        {
            GameObject obj = new GameObject("halfit");
            //obj.transform.localEulerAngles = (new Vector3(0, -10, 0));
            vl.LoadQB2File(obj.transform, Application.streamingAssetsPath + "/" + force + ".qb");

           
            //obj.transform.DOShakeRotation(5.0f, 5, 5, 120.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            obj.transform.DOMoveY(2.0f, 2.0f).SetEase(Ease.InBounce).SetLoops(-1, LoopType.Yoyo);
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(obj.transform.DORotate(new Vector3(0, 10.0f, 0), 2.0f, RotateMode.FastBeyond360));
            mySequence.Append(obj.transform.DORotate(new Vector3(0, -10.0f, 0), 2.0f, RotateMode.FastBeyond360));
            mySequence.SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            vl.LoadQB2File(this.gameObject.transform, Application.streamingAssetsPath + "/level" + level + ".qb");
        }
    }
    GameObject cutEffectobj;
    public void OnLoadCompleted(Transform praent, List<VoxelCut.VoxelDesc> _voxels, List<Transform> _cells, uint[] _matrix, Vector3 _sizeMax)
    {
        voxels = _voxels;
        cells = _cells;
        voxelfill = _matrix;
        sizeMax = _sizeMax;
        cutEffectobj = Instantiate(Resources.Load("cutEffect") as GameObject);
        cutEffectobj.transform.parent = praent.transform;
        cutEffectobj.transform.position = new Vector3(sizeMax.x * 0.5f, -sizeMax.y * 0.5f, 0);
        cutEffectobj.GetComponent<TrailRenderer>().enabled = false;
        
        if (GameStage == 0)
        {
            cutObject.GetComponent<MeshRenderer>().enabled = false;
            maincamera.transform.position = new Vector3(0, 0, sizeMax.z * 2.5f);
            maincamera.transform.LookAt(new Vector3(0, sizeMax.y * -0.1f, 0));
            cutObject.transform.localScale = new Vector3(sizeMax.x / 40.0f * 7.0f, sizeMax.y / 40.0f * 7.0f, sizeMax.z / 40.0f * 7.0f);
        }
        else
        {
            cutObject.GetComponent<MeshRenderer>().enabled = true;
            maincamera.transform.position = new Vector3(sizeMax.x * 1.5f, sizeMax.y * 1.6f, sizeMax.z * 1.5f);
            maincamera.transform.LookAt(new Vector3(0, sizeMax.y * 0.1f, 0));
            cutObject.transform.localScale = new Vector3(sizeMax.x / 40.0f * 7.0f, sizeMax.y / 40.0f * 7.0f, sizeMax.z / 40.0f * 7.0f);
        }
    }

    void DelObject()
    {
        foreach(Transform cell in cells)
        {
            Destroy(cell.gameObject);
        }
        if (voxels != null)
        {
            voxels.Clear();
            voxels = null;
        }
        if (cells != null)
        {
            cells.Clear();
            cells = null;
        }
        if (voxelfill != null)
        {
            voxelfill = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Plane cutplane = new Plane(cutObject.transform.up, 0);
        /*
        foreach (Transform ct in cells)
        {
            float d = cutplane.GetDistanceToPoint(ct.position);
            if (d > 0)
            {
                //MeshRenderer mr = ct.gameObject.GetComponent<MeshRenderer>();
                //mr.material.color = new Color(1, 0.0f, 0.0f);
            }
            else if (d <= 0)
            {
                //MeshRenderer mr = ct.gameObject.GetComponent<MeshRenderer>();
                //mr.material.color = new Color(0, 1.0f, 0.0f);
            }

        }
        */
    }

    public void OnObjectRotChecked()
    {
        if (objectRotToggle.isOn)
        {
            rotateHot = 1;
        }
    }

    public void OnCutRotChecked()
    {
        if (cutRotToggle.isOn)
        {
            rotateHot = 2;
        }
    }

    public void OnResetBtnClicked()
    {
        btnCut.interactable = true;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        cutObject.transform.localEulerAngles = new Vector3(0, 1, 0);
        GameObject objn = GameObject.Find("rootN");
        if (objn != null)
        {
            objn.transform.DOKill();
            DestroyObject(objn);
        }
        GameObject objp = GameObject.Find("rootP");
        if (objp != null)
        {
            objp.transform.DOKill();
            DestroyObject(objp);
        }

        //this.gameObject.SetActive(true);
        ReloadObject(currentLevel);
        cutObject.gameObject.SetActive(true);
        cutObject.transform.position = Vector3.zero;
        GetComponent<DragRotate>().Position = 0.0f;
        scoreLeft.GetComponent<RectTransform>().DOAnchorPosY(472, 0.5f).SetEase(Ease.OutExpo);
        scoreRight.GetComponent<RectTransform>().DOAnchorPosY(472, 0.5f).SetEase(Ease.OutExpo);
    }

    public void OnToggleCutBtnClicked()
    {
        if (cutObject.activeSelf)
        {
            cutObject.SetActive(false);
        }
        else
        {
            cutObject.SetActive(true);
        }
    }


    public void OnNextLevel(bool win)
    {
        controllerPanel.DOAnchorPosX(20, 0.4f).SetEase(Ease.InExpo);
        if (win)
        {
            StartLevel(++currentLevel);
        }
        else
        {
            StartLevel(currentLevel);
        }
    }

    public void OnTileClick()
    {

        cutEffectobj.GetComponent<TrailRenderer>().enabled = true;
        DOTween.To(x =>
        {
            if (x < 0.1f)
            {
                blinkingImg.enabled = false;
            }
            else if (x < 0.2f)
            {
                blinkingImg.enabled = true;
            }
            else if (x < 0.4f)
            {
                blinkingImg.enabled = false;
            }
            else if (x < 0.6f)
            {
                blinkingImg.enabled = true;
            }
            else if (x < 0.8f)
            {
                blinkingImg.enabled = false;
                
            }
        }, 0, 1, 0.2f).OnComplete(() =>
        {
            
            cutEffectobj.transform.DOMove(new Vector3(-cutEffectobj.transform.position.x, -cutEffectobj.transform.position.y, 0), 0.1f).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                DestroyObject(cutEffectobj);
            });
            CutIt(new Vector3(1, 1, 0).normalized, 0, new Vector3(1, -1, 0), 1.0f, false, false, false);
            tilePanel.SetActive(false);
        });

        

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="dist"></param>
    /// <param name="moveDist">确定切开以后错开的距离</param>
    public void CutIt(Vector3 normal, float dist, Vector3 moveDir, float moveDist, bool combinemesh = true, bool rotAfterCut = true, bool showScore = true)
    {
        Plane cutplane = new Plane(normal, dist);
        if (combinemesh)
        {
            Mesh combineMesh = new Mesh();
            List<MeshFilter> meshN = new List<MeshFilter>();
            List<MeshFilter> meshP = new List<MeshFilter>();
            foreach (Transform ct in cells)
            {
                float d = cutplane.GetDistanceToPoint(ct.position);
                if (d > 0)
                {
                    meshN.Add(ct.gameObject.GetComponent<MeshFilter>());
                }
                else if (d < 0)
                {
                    meshP.Add(ct.gameObject.GetComponent<MeshFilter>());
                }
            }
            CombineInstance[] combineN = new CombineInstance[meshN.Count];
            CombineInstance[] combineP = new CombineInstance[meshP.Count];

            for (int i = 0; i < meshN.Count; ++i)
            {
                combineN[i].mesh = meshN[i].sharedMesh;
                combineN[i].transform = meshN[i].transform.localToWorldMatrix;
            }
            for (int i = 0; i < meshP.Count; ++i)
            {
                combineP[i].mesh = meshP[i].sharedMesh;
                combineP[i].transform = meshP[i].transform.localToWorldMatrix;
            }


            GameObject rootN = new GameObject("rootN");
            rootN.AddComponent<MeshRenderer>().sharedMaterial = cells[0].GetComponent<MeshRenderer>().sharedMaterial;
            rootN.AddComponent<MeshFilter>().sharedMesh = new Mesh();
            rootN.GetComponent<MeshFilter>().sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            rootN.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combineN);
            rootN.GetComponent<MeshRenderer>().receiveShadows = false;

            //rootN.AddComponent<Rigidbody>();
            //rootN.AddComponent<MeshCollider>().convex = true;

            GameObject rootP = new GameObject("rootP");
            rootP.AddComponent<MeshRenderer>().sharedMaterial = cells[0].GetComponent<MeshRenderer>().sharedMaterial;
            rootP.AddComponent<MeshFilter>().sharedMesh = new Mesh();
            rootP.GetComponent<MeshFilter>().sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            rootP.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combineP);
            rootP.GetComponent<MeshRenderer>().receiveShadows = false;
            //rootP.AddComponent<Rigidbody>();
            //rootP.AddComponent<MeshCollider>().convex = true;

            DelObject();// this.gameObject.SetActive(false);
            cutObject.gameObject.SetActive(false);



            Vector3 spn = Camera.main.WorldToScreenPoint(rootN.GetComponent<MeshRenderer>().bounds.center);
            Vector3 spp = Camera.main.WorldToScreenPoint(rootP.GetComponent<MeshRenderer>().bounds.center);
            Debug.Log(spn);
            Debug.Log(spp);
            Debug.Log(Screen.width + " " + Screen.height);

            Vector3 dummyLeftPosition = moveDir * moveDist;// 
            Vector3 dummyRightPosition = -moveDir * moveDist;// new Vector3(sizeMax.x * 0.8f, 0, -sizeMax.z * 0.8f);
            rootN.transform.DOMove((spn.x > Screen.width / 2) ? dummyLeftPosition : dummyRightPosition, 0.7f).SetEase(Ease.InOutExpo).OnComplete(() =>
            {
                if (rotAfterCut)
                {
                    rootN.transform.DOLocalRotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                }
            });
            rootP.transform.DOMove((spn.x > Screen.width / 2) ? dummyRightPosition : dummyLeftPosition, 0.7f).SetEase(Ease.InOutExpo).OnComplete(() =>
            {
                if (rotAfterCut)
                {
                    rootP.transform.DOLocalRotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                }
            });


            if (showScore)
            {
                if ((spn.x > Screen.width / 2))
                {
                    scoreRight.text = ((int)((float)meshN.Count * 100.0f / (meshN.Count + meshP.Count) + 0.5f)).ToString();
                    scoreLeft.text = ((int)((float)meshP.Count * 100.0f / (meshN.Count + meshP.Count) + 0.5f)).ToString();
                }
                else
                {
                    scoreLeft.text = ((int)((float)meshN.Count * 100.0f / (meshN.Count + meshP.Count) + 0.5f)).ToString();
                    scoreRight.text = ((int)((float)meshP.Count * 100.0f / (meshN.Count + meshP.Count) + 0.5f)).ToString();
                }
                scoreLeft.GetComponent<RectTransform>().DOAnchorPosY(82, 0.5f).SetEase(Ease.OutExpo);
                scoreRight.GetComponent<RectTransform>().DOAnchorPosY(82, 0.5f).SetEase(Ease.OutExpo);
                controllerPanel.DOAnchorPosX(-324, 0.4f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    billboardPanel.SetActive(true);
                    Billboard billboard = billboardPanel.GetComponent<Billboard>();

                    var min = meshN.Count < meshP.Count ? meshN.Count : meshP.Count;
                    min = ((int)((float)min * 100.0f / (meshN.Count + meshP.Count) + 0.5f));
                    var star = 0;
                    if (min < 40)
                        star = 0;
                    else if (min <= 43)
                        star = 1;
                    else if (min <= 46)
                        star = 2;
                    else if (min <= 50)
                        star = 3;

                    billboard.StartWithWin(currentMission, min, star);
                });
            }
        }
        else
        {
            List<Transform> meshN = new List<Transform>();
            List<Transform> meshP = new List<Transform>();
            foreach (Transform ct in cells)
            {
                float d = cutplane.GetDistanceToPoint(ct.position);
                if (d > 0)
                {
                    meshN.Add(ct);
                }
                else if (d < 0)
                {
                    meshP.Add(ct);
                }
            }

            GameObject rootN = new GameObject("rootN");
            GameObject rootP = new GameObject("rootP");
           
            foreach(Transform t in meshN)
            {
                t.parent = rootN.transform;
            }
            foreach (Transform t in meshP)
            {
                t.parent = rootP.transform;
            }

            Vector3 dummyLeftPosition = moveDir * moveDist;// 
            Vector3 dummyRightPosition = -moveDir * moveDist;// new Vector3(sizeMax.x * 0.8f, 0, -sizeMax.z * 0.8f);
            rootN.transform.DOMove(dummyLeftPosition, 1.0f).SetEase(Ease.InExpo).OnComplete(() =>
            {
                if (rotAfterCut)
                {
                    rootN.transform.DOLocalRotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                }
            });
            rootP.transform.DOMove(dummyRightPosition, 1.0f).SetEase(Ease.InExpo).OnComplete(() =>
            {
                if (rotAfterCut)
                {
                    rootP.transform.DOLocalRotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                }
            });
        }

    }

    public void OnCutItBtnClicked()
    {
        btnCut.interactable = false;
        CutIt(cutObject.transform.up, -GetComponent<DragRotate>().Position, new Vector3(-sizeMax.x, 0, sizeMax.z), 0.8f);

    }

}
