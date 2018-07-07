using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	// GUI Text to display the gesture messages.
	public GUIText GestureInfo;
	public Text GestureText;

	private bool swingForward;
	private bool swingBackward;
	public bool startPosition;
	private bool swingTopDown;

	
//	public bool IsSwipeLeft()
//	{
//		if(swipeLeft)
//		{
//			swipeLeft = false;
//			return true;
//		}
//		
//		return false;
//	}
//	
//	public bool IsSwipeRight()
//	{
//		if(swipeRight)
//		{
//			swipeRight = false;
//			return true;
//		}
//		
//		return false;
//	}

	public bool IsSwingForward()
	{
		if (swingForward) {
			swingForward = false;
			return true;
		}

		return false;
	}

	public bool IsSwingBackward()
	{
		if (swingBackward) {
			swingBackward = false;
			return true;
		}

		return false;
	}

	public bool IsSwingTopdown()
	{
		if (swingTopDown) {
			swingTopDown = false;
			return true;
		}

		return false;
	}


    public bool IsStartPosition()
    {
        if (startPosition)
        {
            startPosition = false;
            return true;
        }

        return false;
    }


    public void UserDetected(uint userId, int userIndex)
	{
		// detect these user specific gestures
		KinectManager manager = KinectManager.Instance;
		
		manager.DetectGesture(userId, KinectGestures.Gestures.SwingForward);
		manager.DetectGesture(userId, KinectGestures.Gestures.SwingBackward);
		manager.DetectGesture(userId, KinectGestures.Gestures.SwingTopdown); // 구현이 아직 안되었다.
        manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand); // 기본제스쳐 사용. 이후 수정이 있으면 수정.

        if (GestureInfo != null)
		{
			Debug.Log ("Swipe left or right to change the slides.");
			GestureInfo.GetComponent<GUIText>().text = "Swipe left or right to change the slides.";
		}
	}
	
	public void UserLost(uint userId, int userIndex)
	{
		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<GUIText>().text = string.Empty;
		}
	}

	public void GestureInProgress(uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		// don't do anything here
	}

	public bool GestureCompleted (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		string sGestureText = gesture + " detected";
		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<GUIText>().text = sGestureText;
			Debug.Log ("[제스쳐감지] : "+ sGestureText);
		}
		
//		if (gesture == KinectGestures.Gestures.SwipeLeft)
//			swipeLeft = true;
//		else if (gesture == KinectGestures.Gestures.SwipeRight)
//			swipeRight = true;

		if (gesture == KinectGestures.Gestures.SwingForward) {
			swingForward = true;
		}

		if (gesture == KinectGestures.Gestures.SwingBackward){
			swingBackward = true;
		}

		if (gesture == KinectGestures.Gestures.SwingTopdown) {
			swingTopDown = true;
		}

        if (gesture == KinectGestures.Gestures.RaiseLeftHand)
        {
			Debug.Log ("raise hand");

            startPosition = true;
        }

        setGestureText (gesture);

		return true;
	}

	public bool GestureCancelled (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint)
	{
		// don't do anything here, just reset the gesture state
		return true;
	}


	Hashtable htGesture = new Hashtable();
	private void setGestureText(KinectGestures.Gestures gestrue){
		
		if(GestureText == null){	return;	}

		if (htGesture.ContainsKey (gestrue)) {
			htGesture [gestrue] = (int)htGesture [gestrue] + 1;
		} else {
			htGesture.Add(gestrue, 1);
		}

		String dt = "";
		foreach (DictionaryEntry entry in htGesture) {
			dt += String.Format ("{0}({1}) \n",entry.Key, entry.Value);
		}
		GestureText.text = dt;
	}

}
