using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;

public class LobbyView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text TicketAmtText;
    [SerializeField]
    private TMP_Text MaxTicketAmtText;
    [SerializeField]
    private TMP_Text GoldAmtText;
    [SerializeField]
    private TMP_Text GoldKeyAmtText;
    // Start is called before the first frame update
    void Start()
    {
        UserData userData = Managers.Data.UserDic.Select(x => x.Value).FirstOrDefault();
        GoldAmtText.text = userData.GoldAmt.ToString();
    }

    // Update is called once per frame
    // private void UpdateUI(UserData data)
    // {
    //     // TicketAmtText.text = data.TicketAmt.ToString();
    //     // MaxTicketAmtText.text = data.MaxTicketAmt.ToString();
    //     GoldAmtText.text = data.GoldAmt.ToString();
    //     // GoldKeyAmtText.text = data.GoldKeyAmt.ToString();
    // }
    //
    // public void UpdateUI(object data)
    // {
    //     UpdateUI((UserData)data);
    // }
}
