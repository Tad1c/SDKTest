using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PaymentForm : MonoBehaviour
{
    private string email;
    private string creditCardNum;
    private string expirationDate;

    public void Email(string email)
    {
        this.email = email;
    }

    public void CreditCardNumb(string number)
    {
        creditCardNum = number;
    }

    public void ExpirationDate(string date)
    {
        expirationDate = date;
    }

    public void Submite()
    {
        APIManager.instance.SubmitePurchase(email, creditCardNum, expirationDate);
    }
}