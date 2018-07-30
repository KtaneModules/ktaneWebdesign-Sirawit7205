using UnityEngine;
using System.Linq;

public class Webdesign : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMSelectable[] btn;
    public TextMesh text;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    private readonly string[,] selectors = new string[8, 8] {
        {"body.post","a#header","a.author","h3.post","h3#header",".post blockquote",".post #comments","#header h3"},
        {"div#msg","#msg","img#cover","#sidebar a","div#content .post","div.title","div.post","#content span.share"},
        {"div#fullview","#comments.large","img#main","img.large","#fullview div.username","div#main img","img#fullview","div#comments.large"},
        {"ol","i.avatar","#sidebar b.username","ul.avatar","ul b","#sidebar ul","#sidebar img.username","ol i"},
        {"div#main iframe","iframe#main","#comments.channel .share","#comments b.username","div b","div#comments b.channel","#comments i","#rating iframe.share"},
        {"body iframe","body.fullscreen","iframe.fullscreen","body #rating","#rating .rating",".fullscreen #comments","iframe#rating.rating","iframe#comments.rating"},
        {"#sidebar h3","div#download","#download","iframe#download","div .menu","#sidebar h3.author","img.author","img.menu"},
        {"body #content","body #sidebar","#content img.avatar","blockquote#sidebar","blockquote.reply","div.reply","div.avatar","img.reply"}
    };

    private readonly string[] sitename = { "Edison Daily", "Buddymaker", "PNGdrop", "BobIRS", "Vidhost", "Go Team Falcon online", "Stufflocker", "Steel Nexus" };
    private readonly string[] randomcolor = { "blue", "green", "purple", "yellow", "white", "magenta", "red", "orange", "gray" };
    private readonly string[] btnText = { "Accept", "Consider", "Reject" };

    private string[] colors = { "color: *;", "background: *;", "background-color: *;" };
    private string[] margin = { "margin: 0 auto;", "margin: 2px 4px;", "margin: 1em;", "margin: calc(100vw-640px);", "margin: 25%;", "margin: 80px 40px;", "margin: 4.2em 1.0em;", "margin: auto;", "padding: 4px;",
        "padding: 2px 8px;", "padding: 0;", "padding: calc(100%-64px);", "padding: 32px;" };
    private string[] border = { "border: 0px;", "border-radius: 0px;", "border-radius: 50%;", "border: 4px solid *;", "border: 1px dashed\n  *;", "border-left: 2px solid\n  *;", "border-bottom: 1px dotted\n  *;", "border-right: 1px solid\n  *;", "border-radius: 2px;", "border-radius: 4px;", "border-radius: 20px;" };
    private string[] position = { "position: absolute;", "position: relative;", "position: fixed;", "z-index: 2;", "z-index: 10;", "z-index: 1048576;", "z-index: 9999999999;" };
    private string[] fonts = { "font-family: \"Comic Sans MS\";", "font-family: \"Special Elite\",\n  monospace;", "font-family: \"Futura\",\n  monospace;", "font-family: \"BombIcons\";", "font-family: \"Helvetica Neue\",\n  \"Helvetica\", sans-serif;",
        "font-family: \"Gotham\",\n  \"Proxima Nova\", sans-serif;", "font-family: \"Century\",\n  \"Georgia\", serif;", "font-family: \"Hoefler Text\",\n  \"Times New Roman\", serif;", "font-family: \"Avenir Next\",\n  \"Avenir\", sans-serif;" };
    private string[] shadow = { "box-shadow: none;", "text-shadow: none;", "text-shadow: *;", "box-shadow: 0px 2px 4px\n  *;", "box-shadow: 2px 3px 8px\n  *;", "box-shadow: 2px 2px 0px\n  * inset;", "text-shadow: 1px 2px 6px\n  *;", "text-shadow: -1px -4px 0px\n  *;", "text-shadow: 12px 14px 1px\n  *;" };

    private readonly int[] thresR = { 0, 128, 186, 3, 96, 80, 176, 190 }, thresG = { 255, 64, 218, 230, 6, 19, 32, 166 }, thresB = { 0, 192, 85, 30, 30, 55, 229, 30 };
    private readonly int[] colorR = { 0, 0, 128, 255, 255, 255, 255, 255, 128 }, colorG = { 0, 255, 0, 255, 255, 0, 0, 165, 128 }, colorB = { 255, 0, 128, 0, 255, 255, 0, 0, 128 };

    private string screen = "";
    private string[] tempscreen;
    private readonly int[] dbg = new int[5];
    private int lineCnt, selectA, selectB, ans = 0, finalans, chk, tarR = 127, tarG = 127, tarB = 127, zidx = 0;
    private bool useColor = false, pos = false, _isSolved = false, _lightsOn = false, isColorblind = false;

    void Start ()
    {
        _moduleId = _moduleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Init;
    }

    void Awake()
    {
		btn[0].OnInteract += delegate ()
		{
			AnsChk(0);
			return false;
		};
		btn[1].OnInteract += delegate ()
		{
			AnsChk(1);
			return false;
		};
		btn[2].OnInteract += delegate ()
		{
			AnsChk(2);
			return false;
		};
    }

    void Init()
    {
        int feature, subfeature, temp = 0;

        //check for color blind mode first!
        isColorblind = GetComponent<KMColorblindMode>().ColorblindModeActive;

        //change screen color
        if (isColorblind)
        {
            Debug.LogFormat("[Web Design #{0}] Running the screen in black and white mode.", _moduleId);
            text.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        }

        selectA = Random.Range(0, 8);
        selectB = Random.Range(0, 8);
        Debug.LogFormat("[Web Design #{0}] Site {1} was selected", _moduleId, sitename[selectA]);
        Debug.LogFormat("[Web design #{0}] Threshold RGB value is #{1}{2}{3} (Or in Dec: {4} {5} {6})", _moduleId, thresR[selectA].ToString("X2"), thresG[selectA].ToString("X2"), thresB[selectA].ToString("X2"), thresR[selectA], thresG[selectA], thresB[selectA]);

        lineCnt = Random.Range(3, 7);
        tempscreen = new string[lineCnt + 2];
        tempscreen[0] = selectors[selectA, selectB] + "{";
        tempscreen[lineCnt + 1] = "}";
        ans += lineCnt;
        Debug.LogFormat("[Web Design #{0}] Line count = {1} (Also a base score)", _moduleId, lineCnt);

        //Main adjustments
        for (int i = 1; i <= lineCnt; i++)
        {
            feature = Random.Range(1,7);
            if(feature == 1)
            {
                subfeature = Random.Range(0, colors.Length);
                tempscreen[i] = colors[subfeature];
                ColorSelector(i);
            }
            else if(feature == 2)
            {
                subfeature = Random.Range(0, margin.Length);
                tempscreen[i] = margin[subfeature];
                ans += 2;
                dbg[0]++;
            }
            else if (feature == 3)
            {
                subfeature = Random.Range(0, border.Length);
                tempscreen[i] = border[subfeature];
                if (subfeature >= 3 && subfeature <= 7) ColorSelector(i);
                if (subfeature > 2)
                {
                    ans++;
                    dbg[1]++;
                }
            }
            else if (feature == 4)
            {
                subfeature = Random.Range(0, position.Length);
                tempscreen[i] = position[subfeature];
                if (subfeature < 3) pos = true; else zidx++;
            }
            else if (feature == 5)
            {
                subfeature = Random.Range(0, fonts.Length);
                tempscreen[i] = fonts[subfeature];
                if (subfeature == 0)
                {
                    ans -= 5;
                    dbg[2]++;
                }
                else
                {
                    ans++;
                    dbg[3]++;
                }
            }
            else
            {
                subfeature = Random.Range(0, shadow.Length);
                tempscreen[i] = shadow[subfeature];
                if (subfeature > 1) ColorSelector(i);
                if (subfeature > 1)
                {
                    ans += 2;
                    dbg[4]++;
                }
            }
        }

        screen = string.Join("\n ", tempscreen);
        text.text = screen;
        Debug.LogFormat("[Web design #{0}] For reference purpose, complete code is:\n{1}", _moduleId, screen);

        //Logging features
        Debug.LogFormat("[Web design #{0}] Detailed score calculation:",_moduleId);
        Debug.LogFormat("[Web design #{0}] -> Base score = {1}", _moduleId, lineCnt);
        if (dbg[0] > 0) Debug.LogFormat("[Web design #{0}] ->  {1} Margin/Padding found: score +{2}", _moduleId, dbg[0], dbg[0] * 2);
        if (dbg[1] > 0) Debug.LogFormat("[Web design #{0}] ->  {1} Acceptable Border/Border-radius found: score +{2}", _moduleId, dbg[1], dbg[1]);
        if (dbg[2] > 0) Debug.LogFormat("[Web design #{0}] ->  {1} Unacceptable font found: score -{2}", _moduleId, dbg[2], dbg[2] * 5);
        if (dbg[3] > 0) Debug.LogFormat("[Web design #{0}] ->  {1} Acceptable font found: score +{2}", _moduleId, dbg[3], dbg[3]);
        if (dbg[4] > 0) Debug.LogFormat("[Web design #{0}] ->  {1} Acceptable drop shadow found: score +{2}", _moduleId, dbg[4], dbg[4] * 2);

        //other misc adjustments
        if (zidx > 0 && !pos)
        {
            ans-=zidx;
            Debug.LogFormat("[Web design #{0}] ->  z-index without position found: score -{1}", _moduleId, zidx);
        }

        if (!useColor) Debug.LogFormat("[Web design #{0}] ->  No color found, now using default target of #7F7F7F (Or in Dec: 127 127 127)", _moduleId);
        if (tarR < thresR[selectA])
        {
            ans += 3;
            Debug.LogFormat("[Web design #{0}] ->  R target < threshold: score +3", _moduleId);
        }
        if (tarG >= thresG[selectA])
        {
            ans += 3;
            Debug.LogFormat("[Web design #{0}] ->  G target >= threshold: score +3", _moduleId);
        }
        if (tarB > thresB[selectA])
        {
            ans += 3;
            Debug.LogFormat("[Web design #{0}] ->  B target > threshold: score +3", _moduleId);
        }

        if (Random.Range(0, 2) == 1)
        {
            ans *= 2;
            SetButtonColor(true);
            Debug.LogFormat("[Web design #{0}] ->  Colored buttons: score x2", _moduleId);
        }
        else
        {
            ans -= 3;
            SetButtonColor(false);
            Debug.LogFormat("[Web design #{0}] ->  Gray buttons: score -3", _moduleId);
        }

        Debug.LogFormat("[Web design #{0}] ->  Score is now {1}", _moduleId, ans);
        while (ans < 1) ans += 16;
        Debug.LogFormat("[Web design #{0}] ->  After positive adjustment: {1}", _moduleId, ans);
        finalans = ans;
        do
        {
            temp = 0;
            while (finalans != 0)
            {
                temp += finalans % 10;
                finalans /= 10;
            }
            finalans = temp;
        }
        while (finalans > 9);
        Debug.LogFormat("[Web design #{0}] ->  After digital root: {1}", _moduleId, finalans);

        if (new[] { 2, 3, 5, 7 }.Contains(finalans)) chk = 0;
        else if (new[] { 6, 8 }.Contains(finalans)) chk = 1;
        else chk = 2;

        Debug.LogFormat("[Web design #{0}] Summary: Score is {1}, Sum is {2}, Expected answer is {3}", _moduleId, ans, finalans, btnText[chk]);

        _lightsOn = true;
    }

    void ColorSelector(int idx)
    {
        int sel = Random.Range(0, 9);
        tempscreen[idx] = tempscreen[idx].Replace("*", randomcolor[sel]);
        if(!useColor)
        {
            useColor = true;
            tarR = colorR[sel];
            tarG = colorG[sel];
            tarB = colorB[sel];
            Debug.LogFormat("[Web design #{0}] First color found (target) is {1}, with RGB value of #{2}{3}{4} (Or in Dec: {5} {6} {7})", _moduleId, randomcolor[sel], tarR.ToString("X2"), tarG.ToString("X2"), tarB.ToString("X2"), tarR, tarG, tarB);
        }
    }

    void SetButtonColor(bool hasColor)
    {
        if(isColorblind)
        {
            Debug.LogFormat("[Web design #{0}] Colorblind mode enabled, showing buttons in black and white mode.", _moduleId);

            //black on white
            if (hasColor)
            {
                for(int i=0;i<3;i++)
                {
                    btn[i].GetComponent<MeshRenderer>().material.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                    btn[i].GetComponentInChildren<TextMesh>().color = new Color32(0x00, 0x00, 0x00, 0xFF);
                }
            }

            //white on black
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    btn[i].GetComponent<MeshRenderer>().material.color = new Color32(0x00, 0x00, 0x00, 0xFF);
                    btn[i].GetComponentInChildren<TextMesh>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                }
            }
        }
        else
        {
            //only color condition, no color is already on the model
            if(hasColor)
            {
                btn[0].GetComponent<MeshRenderer>().material.color = new Color32(0x50, 0xF7, 0x36, 0xFF);
                btn[1].GetComponent<MeshRenderer>().material.color = new Color32(0xF7, 0xA4, 0x20, 0xFF);
                btn[2].GetComponent<MeshRenderer>().material.color = new Color32(0xEF, 0x2D, 0x2D, 0xFF);
            }
        }
    }

    void AnsChk(int m)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, btn[m].transform);
        btn[m].AddInteractionPunch();
        if(!_isSolved && _lightsOn)
        {
            Debug.LogFormat("[Web design #{0}] button {1} was pressed. (Expected {2})", _moduleId, btnText[m], btnText[chk]);

            if (m == chk)
            {
                _isSolved = true;
                Module.HandlePass();
                Debug.LogFormat("[Web Design #{0}] Answer correct! Module passed!", _moduleId);
            }
            else
            {
                Module.HandleStrike();
                Debug.LogFormat("[Web Design #{0}] Answer incorrect! Strike!", _moduleId);
            }
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Accept with “!{0} acc”. Consider with “!{0} con”. Reject with “!{0} rej”.";
#pragma warning restore 414

    KMSelectable[] ProcessTwitchCommand(string command)
    {
        switch(command.ToLowerInvariant().Trim())
        {
            case "accept": case "acc": return new[] { btn[0] };
            case "consider": case "con": return new[] { btn[1] };
            case "reject": case "rej": return new[] { btn[2] };
        }

        return null;
    }

}
