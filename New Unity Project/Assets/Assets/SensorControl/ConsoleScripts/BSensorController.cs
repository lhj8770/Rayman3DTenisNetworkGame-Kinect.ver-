using UnityEngine;
using System.Collections;
using System.Text;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Xml;

public class BSensorController : MonoBehaviour
{
    private bool onceConnected = false;
    public Text DebugText;
    System.Diagnostics.Process proc;

    public Transform ParentObject;
    public Transform TargetObject;
    // public bool LookUpNow = false;
    public bool ConsoleHide = false;

    // Use this for initialization
    void Awake()
    {
        string file = System.IO.Directory.GetCurrentDirectory() + @"\Execute\BReceiverConsole.exe";
       // Debug.Log(file);


        proc = new System.Diagnostics.Process();

        string applicationName = "BReceiverConsole";
        foreach (var p in System.Diagnostics.Process.GetProcessesByName(applicationName))
        {
            if (p.ProcessName.Equals(applicationName))
            {
                p.Kill();
            }
        }

        proc.EnableRaisingEvents = true;
        proc.StartInfo.FileName = file;

        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        // proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.UseShellExecute = false;
        
        proc.OutputDataReceived += onDataReceive;
        //proc.ErrorDataReceived += onDataReceive;

        if (ConsoleHide)
        {
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.StartInfo.CreateNoWindow = true;
        }
        else {
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            proc.StartInfo.CreateNoWindow = false;
        }
        
        //proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //proc.StartInfo.CreateNoWindow = true;
        

        proc.Start();
       // Debug.Log("proc Started");
        proc.BeginOutputReadLine();
       // Debug.Log("proc BeginOutputReadLine");


    }

    private Quaternion _quaternion = new Quaternion();
    public Quaternion getQuaternion()
    {
        return transform.rotation;
    }

    private float _accelateration;
    public float getAcceleration()
    {
        return _accelateration;
    }

    XmlDocument xmlDoc;
    XmlNode aNode;
    XmlNode gNode;
    XmlNode sNode;
    String[] quatStr;
    private void onDataReceive(object sender, System.Diagnostics.DataReceivedEventArgs e)
    {
        if (e.Data == null)
        {
            return;
        }

        String xmlInput = e.Data.TrimEnd();
        if (!Regex.Match(xmlInput, "^<data>(.*?)</data>$").Success)
        {
            DisplayText(e.Data);
          //  Debug.Log(e.Data);
            return;
        }

        if (isPass1Second()) {
            DisplayText(e.Data);
        }

        xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlInput);

        aNode = xmlDoc.SelectSingleNode("/data/a");
		this._accelateration = float.Parse(aNode.InnerText);
        


		gNode = xmlDoc.SelectSingleNode("/data/q");
        quatStr = gNode.InnerText.Split(',');
		this._quaternion.w = float.Parse(quatStr[0]);
		this._quaternion.x = float.Parse(quatStr[2]);
		this._quaternion.y = float.Parse(quatStr[1]);
		this._quaternion.z = true ? float.Parse(quatStr[3]) : -float.Parse(quatStr[3]);

		sNode = xmlDoc.SelectSingleNode("/data/s");
        try
        {
            bool zeroSignalDetect = Int32.Parse(sNode.InnerText) > 0;
            if (zeroSignalDetect) {
                applyZeroing();
             //   Debug.Log("영점신호 발견");
            }
        }
		catch (Exception ex) {
        }

		//Debug.Log (this._accelateration);
        if (!onceConnected)
        {
            onceConnected = true;
        }

    }

    public bool isOnceConnected() {
        return onceConnected;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = _quaternion;
        //DebugText.text = _debugText;

        updateApplyZeroing();
    }

    void OnApplicationQuit()
    {

        if (proc != null)
        {
            proc.Kill();
        }

    }

//    String _debugText = String.Empty;
    public void DisplayText(String input) {
        if (DebugText != null && input!=null) {
           // _debugText = input;
        }
    }

    long tempThick=0;
    public bool isPass1Second() {
        var currentThick = DateTime.Now.Ticks;
        if (tempThick == 0 || Math.Abs(currentThick-tempThick)>1000*10000) {
            tempThick = DateTime.Now.Ticks;
            return true;
        }
        return false;
    }

    long zeroApplyThick = 0;
    // 1秒のタームを置く
    private void applyZeroing() {

        if (ParentObject == null || TargetObject == null) {
           // Debug.Log("영점 조준 적용 불가능 PObject 및 TObject null 값 적용");
            return;
        }
        

        bool valid = (DateTime.Now.Ticks - zeroApplyThick) > 10000 * 1000;
        if (valid) {
            updateApplyZero = true;
        }

    }

    bool updateApplyZero = false;
    private void updateApplyZeroing() {

        if (updateApplyZero)
        {
			
            float _angle = getBlueAngleBetweenVectors(transform, TargetObject);
            //Debug.Log("_angle: " + _angle);

            // var rot = ParentObject.Rotate();
            
            //Debug.Log("영점 조준 변형가함");
            var tempEuler = ParentObject.eulerAngles;
            tempEuler.y += _angle;
            ParentObject.eulerAngles = tempEuler;

            zeroApplyThick = DateTime.Now.Ticks;
            updateApplyZero = false;
            
        }
    }
	// 두 오브젝트의 파란색 축 사이각 구하기
	private float getBlueAngleBetweenVectors(Transform obj, Transform targetObj) {
		var crossVector = Vector3.Cross(obj.forward, targetObj.forward);
		double angle = Math.Asin(crossVector.magnitude) * Mathf.Rad2Deg;
		//Debug.Log(crossVector.y);
		double result = crossVector.y > 0 ? angle : -angle;
		return (float) result;
		// return Vector3.Angle(obj1.forward, obj2.forward);
	}

}
