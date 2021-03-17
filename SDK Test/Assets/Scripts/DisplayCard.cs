using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DisplayCard : MonoBehaviour
{
    
    public TextMeshProUGUI title;
    public TextMeshProUGUI item_id;
    public TextMeshProUGUI item_name;
    public Image item_image;
    public TextMeshProUGUI price;
    
    public void DisplayDetails(Card card, Sprite image)
    {
        title.text = card.title;
        item_id.text = card.item_id;
        item_name.text = card.item_name;
        item_image.sprite= image;
        price.text = card.currency_sign + card.price;
    }
    
}
