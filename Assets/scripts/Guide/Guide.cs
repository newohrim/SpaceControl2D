using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

public class Guide : MonoBehaviour
{
    [SerializeField]
    private Animator FadingBackground;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float radius;
    [SerializeField]
    private string Language;
    [SerializeField]
    private TextAsset guideXml;
    [SerializeField]
    private Renderer background;
    [SerializeField]
    private float backgroundScrollSpeed = 1.0f;
    [SerializeField]
    private TextMeshProUGUI StartText;
    [SerializeField]
    private Vector3[] TextPositions;
    [SerializeField]
    private List<string> guideTexts;
    [SerializeField]
    private Transform Mun;
    [SerializeField]
    private Image powerCounter;
    [SerializeField]
    private GameObject Border;

    private int state = 0;
    private float offset;
    private Animator textAnim;
    private bool isOrbited;
    private float angle = Mathf.PI;

    private void Start() {
        guideTexts = new List<string>();
        textAnim = StartText.GetComponent<Animator>();
        Debug.Log(Application.systemLanguage.ToString());
        ReadXml();
    }

    private void Update() 
    {
        offset += backgroundScrollSpeed * Time.deltaTime;
		background.material.mainTextureOffset = new Vector2(offset, 0);
        if (Input.GetKeyUp(KeyCode.Mouse0) && state < guideTexts.Count) 
        {
            state++;
            switch(state)
            {
                case 2:
                    Mun.gameObject.SetActive(true);
                    isOrbited = true;
                    break;
                case 4:
                    powerCounter.fillAmount = 0.5f;
                    break;
                case 6:
                    powerCounter.fillAmount = 1f;
                    Mun.GetComponent<Animator>().Play("simpleCounterFull");
                    break;
                case 9:
                    Border.SetActive(true);
                    break;
            }
            if (state >= guideTexts.Count)
            {
                StartText.GetComponent<Animator>().Play("TextShowDown");
                FadingBackground.Play("fadeOut");
                return;
            }
            StartText.text = guideTexts[state];    
        }
        if (isOrbited)
        {
            Mun.position = planets.OrbitalPosition(angle, radius, transform.position);
            angle += rotationSpeed * Time.deltaTime;
        }
    }

    public void SetTextVisible()
    {
        textAnim.enabled = true;
    }

    public void ReadXml()
    {
        try
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(guideXml.text);
            XmlElement xRoot = xDoc.DocumentElement;
            Debug.Log(xRoot.Name);
            XmlNode locs = xRoot.FirstChild;
            foreach(XmlElement localization in locs)
            {
                Debug.Log(localization.Name);
                if (localization.Attributes.Count > 0)
                {
                    if (localization.Attributes.GetNamedItem("lang").Value == Application.systemLanguage.ToString())
                    {
                        Language = Application.systemLanguage.ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(Language))
                Language = "English";
            foreach(XmlElement localization in locs)
            {
                if (localization.Attributes.Count > 0)
                {
                    if (localization.Attributes.GetNamedItem("lang").Value == Language)
                    {
                        XmlNode xNode = localization.FirstChild;
                        foreach(XmlNode line in xNode)
                        {
                            guideTexts.Add(line.InnerText);
                        }
                    }
                }
            }
            Debug.Log("Чтение прошло успешно.");
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}

[System.Serializable]
public class GuideContent
{
    [XmlIgnore]
    public string Language { get; set;}
    public Localization[] Localizations { get; set; }

    public GuideContent(){}

    public void ReadXml(TextAsset guideXml)
    {
        try
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(guideXml.text);
            XmlElement xRoot = xDoc.DocumentElement;
            foreach(XmlElement localization in xRoot)
            {
                if (localization.Attributes.Count > 0)
                {
                    if (localization.Attributes.GetNamedItem("lang").Value == Application.systemLanguage.ToString())
                    {
                        Language = Application.systemLanguage.ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(Language))
                Language = "English";
            foreach(XmlElement localization in xRoot)
            {
                if (localization.Attributes.Count > 0)
                {
                    if (localization.Attributes.GetNamedItem("lang").Value == Language)
                    {
                        XmlNode xNode = localization.FirstChild;
                        foreach(XmlNode line in xNode)
                        {
                            //Localizations.Add();
                        }
                    }
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    [System.Serializable]
    public class Localization
    {
        public string[] lines;
        public Localization(){}
    }
}
