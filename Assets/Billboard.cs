using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Billboard : MonoBehaviour
{
    public VoxelCut voxelCut;
    public Text winText;
    public Text loseText;
    public Image star1Img;
    public Image star2Img;
    public Image star3Img;
    public Image haloImg;
    public Image gameoverImg;
    public Image bgImg;
    public Text scoreText;
    public Text missionText;
    public Button nextBtn;
    // Use this for initialization
    void Start()
    {
        // winText.enabled = false;
        //loseText.enabled = false;
        //star1Img.enabled = false;
        //star2Img.enabled = false;
        //star3Img.enabled = false;
        //haloImg.enabled = false;
        //gameoverImg.enabled = false;

    }


    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartWithWin(int mission, int score, int star)
    {
        missionText.enabled = true;
        scoreText.enabled = true;
        missionText.transform.parent.GetComponent<Text>().enabled = true;
        scoreText.transform.parent.GetComponent<Text>().enabled = true;
        missionText.text = mission + " / " + (100 - mission).ToString();
        scoreText.text = score + " / " + (100 - score).ToString();
        winText.enabled = false;
        loseText.enabled = false;
        gameoverImg.enabled = false;
        haloImg.enabled = false;
        bgImg.DOFade(0.95f, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            DOTween.To(x =>
            {
                //wait 2 second
            }, 0, 1, 1).OnComplete(() =>
            {
                if (star > 0)
                {
                    haloImg.enabled = true;
                    haloImg.DOFade(1.0f, 2.2f).SetLoops(-1, LoopType.Yoyo);

                    RectTransform rt = haloImg.GetComponent<RectTransform>();
                    rt.DOPunchRotation(new Vector3(0, 0, 360.0f), 10.0f, 1, 0.5f/*, RotateMode.LocalAxisAdd*/).SetLoops(-1).SetEase(Ease.Unset);
                    star1Img.enabled = true;
                    star1Img.GetComponent<RectTransform>().DOAnchorPosY(128, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                    {
                        if (star >= 2)
                        {
                            star2Img.enabled = true;
                            star2Img.GetComponent<RectTransform>().DOAnchorPosY(128, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                            {
                                if (star == 3)
                                {
                                    star3Img.enabled = true;
                                    star3Img.GetComponent<RectTransform>().DOAnchorPosY(128, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                                    {
                                        ShowTopicAndNextButton(true);
                                    });
                                }
                                else
                                {
                                    ShowTopicAndNextButton(true);
                                }
                            });
                        }
                        else
                        {
                            ShowTopicAndNextButton(true);
                        }

                    });

                }
                else
                {
                    //lose
                    gameoverImg.enabled = true;
                    gameoverImg.GetComponent<RectTransform>().DOAnchorPosY(101, 0.8f).SetEase(Ease.OutBounce).OnComplete(() =>
                    {
                        ShowTopicAndNextButton(false);
                    });
                }
            });
        });
    }

    private void ShowTopicAndNextButton(bool win)
    {
        winText.enabled = win;
        loseText.enabled = !win;
        DOTween.To(x =>
        {

        }, 0, 1, 0.5f).OnComplete(() =>
        {
            if (win)
            {
                nextBtn.GetComponentInChildren<Text>().text = "下一关";
            }
            else
            {
                nextBtn.GetComponentInChildren<Text>().text = "重试";
            }
            nextBtn.interactable = true;
            nextBtn.GetComponent<RectTransform>().DOAnchorPosY(-304, 0.4f).SetEase(Ease.InExpo);
        });

    }


    public void OnNextBtnClick()
    {
        nextBtn.interactable = false;
        nextBtn.GetComponent<RectTransform>().DOAnchorPosY(-450, 0.4f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            haloImg.enabled = false;
            gameoverImg.enabled = false;
            star1Img.enabled = false;
            star2Img.enabled = false;
            star3Img.enabled = false;
            haloImg.DOKill();
            gameoverImg.DOKill();
            star1Img.GetComponent<RectTransform>().anchoredPosition = new Vector2(star1Img.GetComponent<RectTransform>().anchoredPosition.x, 480);
            star2Img.GetComponent<RectTransform>().anchoredPosition = new Vector2(star2Img.GetComponent<RectTransform>().anchoredPosition.x, 480);
            star3Img.GetComponent<RectTransform>().anchoredPosition = new Vector2(star3Img.GetComponent<RectTransform>().anchoredPosition.x, 480);
            gameoverImg.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameoverImg.GetComponent<RectTransform>().anchoredPosition.x, 652);
            winText.enabled = false;
            loseText.enabled = false;
            missionText.enabled = false;
            scoreText.enabled = false;

            missionText.transform.parent.GetComponent<Text>().enabled = false;
            scoreText.transform.parent.GetComponent<Text>().enabled = false;
            bgImg.DOFade(0.0f, 0.7f).SetEase(Ease.InCirc).OnComplete(()=>
            {
                this.gameObject.SetActive(false);
            });
            voxelCut.OnNextLevel(nextBtn.GetComponentInChildren<Text>().text == "下一关");
        });
    }

    public void StartWithLose()
    {

    }
}
