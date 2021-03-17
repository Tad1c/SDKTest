using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;



public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    public VideoPlayer videoPlayer;

    public DisplayCard displayCard;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        API();
    }

    public void API()
    {
        StartCoroutine(GetVideoAD());
        StartCoroutine(GetPruchaseItem());
    }

    private IEnumerator GetVideoAD()
    {
        //setting up the destination from where we want to get the data
        string url = "https://6u3td6zfza.execute-api.us-east-2.amazonaws.com/prod/ad/vast";
        
        //creating and sending request to the destination
        UnityWebRequest request = UnityWebRequest.Get(url);

        //waiting for response 
        yield return request.SendWebRequest();

        //Checking if there were some problems, maybe problem with connection or the server, or even the destination that we set is incorrect
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //Creating new xml document and loading the document based on the response that we get from the server
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(request.downloadHandler.text);
            
            string videoUrl = xmlDoc.DocumentElement.InnerText;
            
            //We make substring because there is also displaying a duration of the video we dont need that as url/destination
            var substring = videoUrl.Substring(8, videoUrl.Length - 8);

            //sending again request to the destination that we make a substring
            request = UnityWebRequest.Get(substring);

            yield return request.SendWebRequest();
            
            //creating a path were the video will be saved on the device
            var path = $"{Application.dataPath}/video.webm";
            
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                //Save the video to local file
                File.WriteAllBytes(path, request.downloadHandler.data);
                
                //getting the video from where we have saved
                videoPlayer.url = path;
                //playing the video
                videoPlayer.Play();
            }
        }
    }


    private IEnumerator GetPruchaseItem()
    {
        //creating just a simple json file that we can pass when we post information to the server
        string simpleJson = "{\"Hello\" : \"Hi\"}";
        string url = "https://6u3td6zfza.execute-api.us-east-2.amazonaws.com/prod/v1/gcom/ad";

        var request = new UnityWebRequest(url, "POST");
        
        //creating a byte array from the json file
        byte[] bodyRaw = Encoding.UTF8.GetBytes(simpleJson);

        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //storing the infromatin that we get from the server
            var json = request.downloadHandler.text;
            
            json = json.Replace("'", "\"");
            
            //writing information from json to our card
            Card card = JsonUtility.FromJson<Card>(json);

            //sending again request to get the image from the server
            request = UnityWebRequestTexture.GetTexture(card.item_image);

            yield return request.SendWebRequest();
            
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                //storing the image as texture
                Texture2D image = ((DownloadHandlerTexture) request.downloadHandler).texture;
                //creating a sprite from the texture
                Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.one / 2);
                //displaying card information to UI
                displayCard.DisplayDetails(card, sprite);
            }
        }
    }


    public void SubmitePurchase(string email, string cardNum, string expDate)
    {
        StartCoroutine(SubmitePurchaseEnumerator(email, cardNum, expDate));
    }

    public IEnumerator SubmitePurchaseEnumerator(string email, string cardNum, string expDate)
    {
        //setting up the data that user have enter
        PaymentDetails paymentDetails = new PaymentDetails(email, cardNum, expDate);
        
        //creating json file from the data
        string json = JsonUtility.ToJson(paymentDetails);

        string url = "https://6u3td6zfza.execute-api.us-east-2.amazonaws.com/prod/v1/gcom/action";

        var request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }
}

public class PaymentDetails
{
    public string email;
    public string creditCardNum;
    public string expirationDate;

    public PaymentDetails(string email, string creditCardNum, string expirationDate)
    {
        this.email = email;
        this.creditCardNum = creditCardNum;
        this.expirationDate = expirationDate;
    }
}