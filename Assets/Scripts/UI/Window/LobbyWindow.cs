using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyWindow : UIWindow
{
    [SerializeField]
    private Transform goodsThumbnailListGrid;
    

    [SerializeField]
    private Transform MiddleAnchor;


    [SerializeField]
    private GameObject goodsThumbnail;
    private List<IView> goodsThumbnailList;

    [SerializeField]
    private GameObject stageThumbnail;
    private IView stageThumbnailView;

    private void Start()
    {
        goodsThumbnailList = new List<IView>();
        for (int i = 0; i < 3; i++)
        {
            GameObject goodsThumbnailObj = Instantiate(goodsThumbnail, goodsThumbnailListGrid);
            goodsThumbnailObj.SetActive(true);
            goodsThumbnailObj.transform.SetParent(goodsThumbnailListGrid);
            goodsThumbnailList.Add(goodsThumbnailObj.GetComponent<IView>());
        }

        stageThumbnailView = Instantiate(stageThumbnail, transform).GetComponent<IView>();
    }

}
