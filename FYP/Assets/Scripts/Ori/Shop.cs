using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private GameData difficultyData;

    [Header("Money")]
    [SerializeField] public TextMeshProUGUI money;
    [SerializeField] public Button resetButton;

    [Header("Draw Color")]
    [SerializeField] public Button blackButton;
    [SerializeField] public Button redButton;
    [SerializeField] public Button greenButton;
    [SerializeField] public Button blueButton;
    [SerializeField] public GameObject redPriceTMP;
    [SerializeField] public GameObject greenPriceTMP;
    [SerializeField] public GameObject bluePriceTMP;
    public int redPrice = 2000;
    public int greenPrice = 2000;
    public int bluePrice = 2000;

    [Header("Particle Effect")]
    [SerializeField] public Button particle0Button;
    [SerializeField] public Button particle1Button;
    [SerializeField] public Button particle2Button;
    [SerializeField] public Button particle3Button;
    [SerializeField] public GameObject particle1PriceTMP;
    [SerializeField] public GameObject particle2PriceTMP;
    [SerializeField] public GameObject particle3PriceTMP;
    public int particle1Price = 10000;
    public int particle2Price = 10000;
    public int particle3Price = 10000;

    [Header("Audio")]
    public string buyAudioName;
    public string cannotButyAudioName;


    // Start is called before the first frame update
    void Start()
    {
        resetButton.onClick.AddListener(ResetShop);

        blackButton.onClick.AddListener(UseBlack);
        redButton.onClick.AddListener(UseRed);
        greenButton.onClick.AddListener(UseGreen);
        blueButton.onClick.AddListener(UseBlue);
        AllColorInteractable();
        CheckColor();

        particle0Button.onClick.AddListener(UseParticle0);
        particle1Button.onClick.AddListener(UseParticle1);
        particle2Button.onClick.AddListener(UseParticle2);
        particle3Button.onClick.AddListener(UseParticle3);
        AllParticleInteractable();
        CheckParticle();

        money.text = difficultyData.money.ToString();
    }

    private void UseBlack()
    {
        AllColorInteractable();
        blackButton.interactable = false;
        difficultyData.drawColor = Color.black;
    }

    private void UseRed()
    {
        if (!difficultyData.redObtain)
        {
            if (difficultyData.money >= redPrice)
            {
                difficultyData.money -= redPrice;
                money.text = difficultyData.money.ToString();
                AudioManager.instance.Play(buyAudioName);

                difficultyData.redObtain = true;
                redPriceTMP.SetActive(false);
            }
            else
            {
                AudioManager.instance.Play(cannotButyAudioName);
            }
        }
        else if (difficultyData.redObtain)
        {
            AllColorInteractable();
            redButton.interactable = false;
            difficultyData.drawColor = Color.red;
        }
    }

    private void UseGreen()
    {
        if (!difficultyData.greenObtain)
        {
            if (difficultyData.money >= greenPrice)
            {
                difficultyData.money -= greenPrice;
                money.text = difficultyData.money.ToString();
                AudioManager.instance.Play(buyAudioName);
                difficultyData.greenObtain = true;
                greenPriceTMP.SetActive(false);
            }
            else
            {
                AudioManager.instance.Play(cannotButyAudioName);
            }
        }
        else if (difficultyData.greenObtain)
        {
            AllColorInteractable();
            greenButton.interactable = false;
            difficultyData.drawColor = Color.green;
        }
    }

    private void UseBlue()
    {
        if (!difficultyData.blueObtain)
        {
            if (difficultyData.money >= bluePrice)
            {
                difficultyData.money -= bluePrice;
                money.text = difficultyData.money.ToString();
                AudioManager.instance.Play(buyAudioName);
                bluePriceTMP.SetActive(false);
                difficultyData.blueObtain = true;
            }
            else
            {
                AudioManager.instance.Play(cannotButyAudioName);
            }
        }
        else if (difficultyData.blueObtain)
        {
            AllColorInteractable();
            blueButton.interactable = false;
            difficultyData.drawColor = Color.blue;
        }
    }

    private void UseParticle0()
    {
        AllParticleInteractable();
        particle0Button.interactable = false;
        difficultyData.particleIdx = 0;   
    }

    private void UseParticle1()
    {
        if (!difficultyData.particle1Obtain)
        {
            if (difficultyData.money >= particle1Price)
            {
                difficultyData.money -= particle1Price;
                money.text = difficultyData.money.ToString();
                AudioManager.instance.Play(buyAudioName);

                difficultyData.particle1Obtain = true;
                particle1PriceTMP.SetActive(false);
            }
            else
            {
                AudioManager.instance.Play(cannotButyAudioName);
            }
        }
        else if (difficultyData.redObtain)
        {
            AllParticleInteractable();
            particle1Button.interactable = false;
            difficultyData.particleIdx = 1;
        }
    }

    private void UseParticle2()
    {
        if (!difficultyData.particle2Obtain)
        {
            if (difficultyData.money >= particle2Price)
            {
                difficultyData.money -= particle2Price;
                money.text = difficultyData.money.ToString();
                AudioManager.instance.Play(buyAudioName);

                difficultyData.particle2Obtain = true;
                particle2PriceTMP.SetActive(false);
            }
            else
            {
                AudioManager.instance.Play(cannotButyAudioName);
            }
        }
        else if (difficultyData.redObtain)
        {
            AllParticleInteractable();
            particle2Button.interactable = false;
            difficultyData.particleIdx = 2;
        }
    }

    private void UseParticle3()
    {
        if (!difficultyData.particle3Obtain)
        {
            if (difficultyData.money >= particle3Price)
            {
                difficultyData.money -= particle3Price;
                money.text = difficultyData.money.ToString();
                AudioManager.instance.Play(buyAudioName);

                difficultyData.particle3Obtain = true;
                particle3PriceTMP.SetActive(false);
            }
            else
            {
                AudioManager.instance.Play(cannotButyAudioName);
            }
        }
        else if (difficultyData.redObtain)
        {
            AllParticleInteractable();
            particle3Button.interactable = false;
            difficultyData.particleIdx = 3;
        }
    }

    private void AllColorInteractable()
    {
        blackButton.interactable = true;   
        redButton.interactable = true;
        greenButton.interactable = true;
        blueButton.interactable = true;
    }

    private void AllParticleInteractable()
    {
        particle0Button.interactable = true;
        particle1Button.interactable = true;
        particle2Button.interactable = true;
        particle3Button.interactable = true;
    }

    private void CheckColor()
    {
        if (difficultyData.drawColor == Color.black)
        {
            blackButton.interactable = false;
        }

        if (difficultyData.drawColor == Color.red)
        {
            redButton.interactable = false;
        }

        if (difficultyData.drawColor == Color.green)
        {
            greenButton.interactable = false;
        }

        if (difficultyData.drawColor == Color.blue)
        {
            blueButton.interactable = false;
        }

        if (difficultyData.redObtain)
        {
            redPriceTMP.SetActive(false);
        }

        if (difficultyData.greenObtain)
        {
            greenPriceTMP.SetActive(false);
        }

        if (difficultyData.blueObtain)
        {
            bluePriceTMP.SetActive(false);
        }
    }

    private void CheckParticle()
    {
        if (difficultyData.particleIdx == 0)
        {
            particle0Button.interactable = false;
        }
        if (difficultyData.particleIdx == 1)
        {
            particle1Button.interactable = false;
        }
        if (difficultyData.particleIdx == 2)
        {
            particle2Button.interactable = false;
        }
        if (difficultyData.particleIdx == 3)
        {
            particle3Button.interactable = false;
        }

        if (difficultyData.particle1Obtain)
        {
            particle1PriceTMP.SetActive(false);
        }

        if (difficultyData.particle2Obtain)
        {
            particle2PriceTMP.SetActive(false);
        }

        if (difficultyData.particle3Obtain)
        {
            particle3PriceTMP.SetActive(false);
        }
    }

    public void ResetShop()
    {
        UseBlack();
        redPriceTMP.SetActive(true);
        greenPriceTMP.SetActive(true);
        bluePriceTMP.SetActive(true);
        difficultyData.redObtain = false;
        difficultyData.greenObtain = false;
        difficultyData.blueObtain = false;
        CheckColor();

        UseParticle0();
        particle1PriceTMP.SetActive(true);
        particle2PriceTMP.SetActive(true);
        particle3PriceTMP.SetActive(true);
        difficultyData.particle1Obtain = false;
        difficultyData.particle2Obtain = false;
        difficultyData.particle3Obtain = false;
        CheckParticle();

        difficultyData.money = 100000;
        money.text = difficultyData.money.ToString();
        difficultyData.Save();
        difficultyData.Load();
    }
}
