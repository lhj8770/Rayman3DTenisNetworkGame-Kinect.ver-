using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;



public class UnitController : NetworkBehaviour {

	//Networked network = new Networked();

    // Bool that has the characters (facing the player) actions become mirrored. Default false.
    public bool mirroredMovement = false;

    // Bool that determines whether the avatar is allowed to move in vertical direction.
    public bool verticalMovement = false;

    // 수평적 움직임만 강조.
    public bool horizonalMovement = true;

    // Rate at which avatar will move through the scene. The rate multiplies the movement speed (.001f, i.e dividing by 1000, unity's framerate).
    //protected int moveRate = 1;

    // Slerp smooth factor
    //public float smoothFactor = 5f;

    // Whether the offset node must be repositioned to the user's coordinates, as reported by the sensor or not.
    //public bool offsetRelativeToSensor = false;


    // The body root node
    protected Transform bodyRoot;

    // A required variable if you want to rotate the model in space.
    protected GameObject offsetNode;

    // Variable to hold all them bones. It will initialize the same size as initialRotations.
    protected Transform[] bones;

    // Rotations of the bones when the Kinect tracking starts.
    protected Quaternion[] initialRotations;
    protected Quaternion[] initialLocalRotations;

    // Initial position and rotation of the transform
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;

    // Calibration Offset Variables for Character Position.
    protected bool offsetCalibrated = false;
    protected float xOffset, yOffset, zOffset;

    // private instance of the KinectManager
    protected KinectManager kinectManager;

    // 디버깅 변수
    public Text DebugText;

	private Transform HeadBone;
	private Transform SpineBone;
	private Transform RightHandBone;
    /*
    머리 2
	몸통 5
    왼손 12
    오른손 24
    왼발 10
    오른발 35
    */
    Hashtable ht_bones = new Hashtable();
    Hashtable ht_origin = new Hashtable();

	public Transform MoveObject;
    private Vector3 InitMovePosition;

    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;
    public Transform LeftFoot;
    public Transform RightFoot;
    public Transform Stomach;
    //public Transform RotateModifyPart;
	[SyncVar] Vector3 head;
	[SyncVar] Vector3 leftHand;
	[SyncVar] Vector3 rightHand;
	[SyncVar] Vector3 leftFoot;
	[SyncVar] Vector3 rightFoot;
	[SyncVar] Vector3 stomach;
	[SyncVar] Quaternion btRotation;


	[SyncVar]public string playerUniqueName;
	private NetworkInstanceId playerNetId;

    public BSensorController bSensorContoller;

    public float ModelScale = 2;
    public int MoveScale = 10;
	public int count =0;


	//	public void startPosition(){
	//		if (GameObject.Find ("DetectStartPosition1")!=null&&count ==0) {
	//			Debug.Log ("Enter1");
	//			count++;
	//			MoveObject = GameObject.Find("Spawnpoint 1P").transform;
	//			transform.localPosition = MoveObject.position;
	//			Destroy (GameObject.Find("DetectStartPosition1"));
	//		} else if (GameObject.Find ("DetectStartPosition2")!=null&&count ==0) {
	//			Debug.Log ("Enter2");
	//			count++;
	//			MoveObject = GameObject.Find("Spawnpoint 2P").transform;
	//			transform.localPosition = MoveObject.position;
	//			Destroy (GameObject.Find("DetectStartPosition2"));
	//		}
	//	}

	void OnTriggerEnter(Collider starting){
//		Debug.Log ("Enter");
//		Debug.Log (starting.name);
		if (starting.gameObject.name.Contains ("DetectStartPosition1")&&count ==0) {
			//Debug.Log ("Enter1");
			count++;
			MoveObject = GameObject.Find("Spawnpoint 1P").transform;
			transform.localPosition = MoveObject.position;
//			Debug.Log (gameObject.name);
			Destroy (starting.gameObject);
		} else if (starting.gameObject.name.Contains ("DetectStartPosition2")&&count ==0) {
			//Debug.Log ("Enter2");
			count++;
			MoveObject = GameObject.Find("Spawnpoint 2P").transform;
			transform.localPosition = MoveObject.position;
			Destroy (starting.gameObject);

		}
	}

	public override void OnStartLocalPlayer(){
		GetNetIdentity ();
		SetIdentity ();
	}

	[Client]
	void GetNetIdentity(){
		playerNetId = GetComponent<NetworkIdentity> ().netId;
		CmdTellServerMyIdentity (MakeUniqueIdentity ());
	}

	void SetIdentity(){
		if (!isLocalPlayer) {
			transform.name = playerUniqueName;
		} else {
			transform.name = MakeUniqueIdentity();
		}
	}

	[Command]
	void CmdTellServerMyIdentity(string name){
		playerUniqueName = name;
	}

	string MakeUniqueIdentity(){
		string uniqueName = "Player " + playerNetId.ToString ();
		return uniqueName;
	}

    public void Start()
	{
		if (!isLocalPlayer) {
			return;
		}
        setLog("Awake");
		//Connect MoveObject to SpawnPoint

		//카메라 활성화
		transform.Find ("Body").Find ("Stoma").Find ("Main Camera").gameObject.SetActive (true);

        // check for double start
        if (bones != null)
            return;

        // inits the bones array
        bones = new Transform[22];

        // Initial rotations and directions of the bones.
        initialRotations = new Quaternion[bones.Length];
        initialLocalRotations = new Quaternion[bones.Length];

        // Map bones to the points the Kinect tracks
        // MapBones();
        // Debug.Log("MapBones");

        // Get initial bone rotations
        GetInitialRotations();



        Debug.Log("GetInitialRotations");
//		MoveObject = null;


        
        ht_bones.Add((int)KinectWrapper.NuiSkeletonPositionIndex.Head, Head);
        ht_bones.Add((int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft, LeftHand);
        ht_bones.Add((int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, RightHand);
        ht_bones.Add((int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft, LeftFoot);
        ht_bones.Add((int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight, RightFoot);
        ht_bones.Add((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, Stomach);

		ht_origin.Add((int)KinectWrapper.NuiSkeletonPositionIndex.Head, Head.position);
        ht_origin.Add((int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft, LeftHand.position);
        ht_origin.Add((int)KinectWrapper.NuiSkeletonPositionIndex.HandRight, RightHand.position);
        ht_origin.Add((int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft, LeftFoot.position);
        ht_origin.Add((int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight, RightFoot.position);
        ht_origin.Add((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, Stomach.position);

//		startPosition ();
    }
    
    private Vector3 initialPosOffset = Vector3.zero;
    private uint initialPosUserID = 0;

    // `te the avatar each frame.
    public void UpdateUnit(uint UserID)
    {

		if (!isLocalPlayer) {
			
			return;
		}

        setLog("Update Unit");

        if (!transform.gameObject.activeInHierarchy)
            return;

        // Get the KinectManager instance
        if (kinectManager == null)
        {
            kinectManager = KinectManager.Instance;
        }

        // move the avatar to its Kinect position
        //MoveAvatar(UserID);


        KinectManager manager = KinectManager.Instance;

        // get 1st player
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;

        if (playerID <= 0)
        {
            // reset the pointman position and rotation
            if (transform.position != initialPosition)
            {
                transform.position = initialPosition;
            }

            if (transform.rotation != initialRotation)
            {
                transform.rotation = initialRotation;
            }


            // 위치초기화
            // initialRotation = MoveObject.rotation;

//            for (int i = 0; i < bones.Length; i++)
//            {
//                bones[i].gameObject.SetActive(true);
//
//                bones[i].transform.localPosition = Vector3.zero;
//                bones[i].transform.localRotation = Quaternion.identity;
//
//                if (SkeletonLine)
//                {
//                    lines[i].gameObject.SetActive(false);
//                }
//            }

            return;
        }

        // set the user position in space
        Vector3 posPointMan = manager.GetUserPosition(playerID);
        // [원본] posPointMan.z = !mirroredMovement ? -posPointMan.z : posPointMan.z;
        posPointMan.z = mirroredMovement ? -posPointMan.z : posPointMan.z;
        // [원본] posPointMan.x = !mirroredMovement ? -posPointMan.x : posPointMan.x;
        posPointMan.x = mirroredMovement ? -posPointMan.x : posPointMan.x;

        // 움직임 반영
        //if (MoveObject != null)
        //{
        //    if (InitMovePosition != null)
        //    {
        //        InitMovePosition = MoveObject.position;
        //    }
        //    Vector3 moveResultPos = MoveObject.position;
        //    moveResultPos.x = InitMovePosition.x + posPointMan.x * MoveScale;
        //    // MoveObject.position = moveResultPos;
        //}

        // store the initial position
        if (initialPosUserID != playerID)
        {
            initialPosUserID = playerID;
            initialPosOffset = transform.position - (verticalMovement ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));
        }

        transform.position = initialPosOffset + (verticalMovement ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));

        // Debug.Log("update the local positions of the bones");
        // update the local positions of the bones

        transform.rotation = initialRotation;
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null || true)
            {
                //Debug.Log("bones[i] != null");

                int joint = mirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i) : i;

                if (manager.IsJointTracked(playerID, joint))
                {
                    //bones[i].gameObject.SetActive(true);

                    Vector3 posJoint = manager.GetJointPosition(playerID, joint);
                    posJoint.z = !mirroredMovement ? -posJoint.z : posJoint.z;

                    // [원본] Quaternion rotJoint = manager.GetJointOrientation(playerID, joint, !mirroredMovement);
                    // 몸통의 축이 반대로 논다...
                    Quaternion rotJoint = manager.GetJointOrientation(playerID, joint, mirroredMovement);
                    rotJoint = initialRotation * rotJoint;
                    

                    // 위치 인식과 관절인식을 달리하기 위해 구분
                    posJoint -= posPointMan;

                    if (mirroredMovement)
                    {
                        posJoint.x = -posJoint.x;
                        posJoint.z = -posJoint.z;
                    }
                    

                    foreach (DictionaryEntry Item in ht_bones)
                    {
                        Transform trans = (Transform) Item.Value;
                        if (i == (int) Item.Key && trans != null)
                        {
                            float scaleFactor = 1;
                            if (scaleFactor >= 0.1) {
                                scaleFactor = ModelScale;
                            }
                            
                            Vector3 result;
                            Vector3 origin = (Vector3) ht_origin[i];
                            result.x = origin.x * 0.3f + posJoint.x * 0.7f;
                            result.y = posJoint.y * 0.7f ;
                            result.z = posJoint.z * 0.7f ;

                            result.x *= scaleFactor;
                            result.y *= scaleFactor;
                            result.z *= scaleFactor;

                            result.y += 1.0F * scaleFactor;


                            if (i == (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight || i == (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft)
                            {

                            } else if (i == (int)KinectWrapper.NuiSkeletonPositionIndex.KneeLeft || i == (int)KinectWrapper.NuiSkeletonPositionIndex.KneeRight) {
                                result.y -= 0.3f * scaleFactor;
                            }
                            else {
                                // trans.rotation = rotJoint;
                            }
							if(MoveObject!=null)
                            	trans.position = result + MoveObject.position;
                            if (horizonalMovement)
                            {
                                var tempVector = trans.position;
                                tempVector.x += posPointMan.x * MoveScale;
                                trans.position = tempVector;
                            }

                            if (i != (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight && i != (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft && i != (int)KinectWrapper.NuiSkeletonPositionIndex.Spine) { 
                                // trans.rotation = rotJoint;
                            }

                            break;
                        }
                    }

                }
                else
                {
                    //bones[i].gameObject.SetActive(false);
                }
            }
			if(MoveObject!=null)
				transform.rotation = MoveObject.rotation;
        }
        for (var boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            //string debugFormat = string.Format("boneIndex: {0}   value: {1}", boneIndex, bones[boneIndex]);
            //Debug.Log(debugFormat);

            if (!bones[boneIndex])
                continue;

            Debug.Log("진입");

            //if (boneIndex2JointMap.ContainsKey(boneIndex))
            //{
            //    KinectWrapper.NuiSkeletonPositionIndex joint = !mirroredMovement ? boneIndex2JointMap[boneIndex] : boneIndex2MirrorJointMap[boneIndex];
            //    TransformBone(UserID, joint, boneIndex, !mirroredMovement);
            //}
            //else if (specIndex2JointMap.ContainsKey(boneIndex))
            //{
            //    // special bones (clavicles)
            //    List<KinectWrapper.NuiSkeletonPositionIndex> alJoints = !mirroredMovement ? specIndex2JointMap[boneIndex] : specIndex2MirrorJointMap[boneIndex];

            //    if (alJoints.Count >= 2)
            //    {
            //        //Vector3 baseDir = alJoints[0].ToString().EndsWith("Left") ? Vector3.left : Vector3.right;
            //        //TransformSpecialBone(UserID, alJoints[0], alJoints[1], boneIndex, baseDir, !mirroredMovement);
            //    }
            //}

            

            // 디버깅이 가능해야 한다. 좌표 출력 부분
            //if (boneIndex == ((int)HumanBodyBones.Spine))
            //{
            //    Vector3 spinePos = bones[boneIndex].position;
            //    string str_debug = string.Format("Spine({0},{1},{2})", spinePos.x, spinePos.y, spinePos.z);

            //    Debug.Log(str_debug);
            //}

        }
		CmdsyncPlayer ();
    }
	[Command]
	void CmdsyncPlayer(){
		head = Head.position;
		rightHand = RightHand.position;
		leftHand = LeftHand.position;
		rightFoot = RightFoot.position;
		leftFoot = LeftFoot.position;
		stomach = Stomach.position;
		btRotation = RightHand.rotation;
	}

    // Set bones to their initial positions and rotations
    public void ResetToInitialPosition()
    {
        if (bones == null)
            return;

        if (offsetNode != null)
        {
            offsetNode.transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        // For each bone that was defined, reset to initial position.
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                bones[i].rotation = initialRotations[i];
            }
        }

        if (bodyRoot != null)
        {
            bodyRoot.localPosition = Vector3.zero;
            bodyRoot.localRotation = Quaternion.identity;
        }

        // Restore the offset's position and rotation
        if (offsetNode != null)
        {
            offsetNode.transform.position = initialPosition;
            offsetNode.transform.rotation = initialRotation;
        }
        else
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }


    // If the bones to be mapped have been declared, map that bone to the model.
    protected virtual void MapBones()
    {
        // make OffsetNode as a parent of model transform.
        
        offsetNode.transform.position = transform.position;
        offsetNode.transform.rotation = transform.rotation;
        offsetNode.transform.parent = transform.parent;

        transform.parent = offsetNode.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // take model transform as body root
        bodyRoot = transform;

        // get bone transforms from the animator component
        var animatorComponent = GetComponent<Animator>();
        
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            if (!boneIndex2MecanimMap.ContainsKey(boneIndex))
                continue;

            // bones[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
            bones[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
        }
    }

    // Capture the initial rotations of the bones
    protected void GetInitialRotations()
    {
        // save the initial rotation
        if (offsetNode != null)
        {
            initialPosition = offsetNode.transform.position;
            initialRotation = offsetNode.transform.rotation;

            offsetNode.transform.rotation = Quaternion.identity;
        }
        else if (MoveObject !=null && false) {
            initialPosition = MoveObject.position;
            initialRotation = MoveObject.rotation;
        }
        else
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            transform.rotation = Quaternion.identity;
        }

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                initialRotations[i] = bones[i].rotation; // * Quaternion.Inverse(initialRotation);
                initialLocalRotations[i] = bones[i].localRotation;
            }
        }

        // Restore the initial rotation
        if (offsetNode != null)
        {
            offsetNode.transform.rotation = initialRotation;
        }
        else
        {
            transform.rotation = initialRotation;
        }
    }



    // Update is called once per frame
    private bool flag_rotateModify = false;
    void Update()
    {
		if(transform.name=="" || transform.name =="Rayman(Clone)"){
			SetIdentity();
		}
		if (!isLocalPlayer) {
			Head.position = head;
			RightHand.position = rightHand;
			LeftHand.position = leftHand;
			RightFoot.position = rightFoot;
			LeftFoot.position = leftFoot;
			Stomach.position = stomach;
			RightHand.rotation = btRotation;
			return; 
		}
		if (bSensorContoller != null && RightHand != null) {
			RightHand.rotation =
        bSensorContoller.getQuaternion ();
		}
    }


    // 디버깅 관련 메소드
    private void setLog(string inputText) {
        if (DebugText != null) {
            DebugText.text = inputText;
        }
    }

    private void addLog(string inputText) {
        if (DebugText != null)
        {
            DebugText.text += "\n " + inputText;
        }
    }

    // dictionaries to speed up bones' processing
    // the author of the terrific idea for kinect-joints to mecanim-bones mapping
    // along with its initial implementation, including following dictionary is
    // Mikhail Korchun (korchoon@gmail.com). Big thanks to this guy!
    private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
    {
        {0, HumanBodyBones.Hips},
        {1, HumanBodyBones.Spine},
        {2, HumanBodyBones.Neck},
        {3, HumanBodyBones.Head},

        {4, HumanBodyBones.LeftShoulder},
        {5, HumanBodyBones.LeftUpperArm},
        {6, HumanBodyBones.LeftLowerArm},
        {7, HumanBodyBones.LeftHand},
        {8, HumanBodyBones.LeftIndexProximal},

        {9, HumanBodyBones.RightShoulder},
        {10, HumanBodyBones.RightUpperArm},
        {11, HumanBodyBones.RightLowerArm},
        {12, HumanBodyBones.RightHand},
        {13, HumanBodyBones.RightIndexProximal},

        {14, HumanBodyBones.LeftUpperLeg},
        {15, HumanBodyBones.LeftLowerLeg},
        {16, HumanBodyBones.LeftFoot},
        {17, HumanBodyBones.LeftToes},

        {18, HumanBodyBones.RightUpperLeg},
        {19, HumanBodyBones.RightLowerLeg},
        {20, HumanBodyBones.RightFoot},
        {21, HumanBodyBones.RightToes},
    };

    protected readonly Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex> boneIndex2JointMap = new Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex>
    {
        {0, KinectWrapper.NuiSkeletonPositionIndex.HipCenter},
        {1, KinectWrapper.NuiSkeletonPositionIndex.Spine},
        {2, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter},
        {3, KinectWrapper.NuiSkeletonPositionIndex.Head},

        {5, KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft},
        {6, KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft},
        {7, KinectWrapper.NuiSkeletonPositionIndex.WristLeft},
        {8, KinectWrapper.NuiSkeletonPositionIndex.HandLeft},

        {10, KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight},
        {11, KinectWrapper.NuiSkeletonPositionIndex.ElbowRight},
        {12, KinectWrapper.NuiSkeletonPositionIndex.WristRight},
        {13, KinectWrapper.NuiSkeletonPositionIndex.HandRight},

        {14, KinectWrapper.NuiSkeletonPositionIndex.HipLeft},
        {15, KinectWrapper.NuiSkeletonPositionIndex.KneeLeft},
        {16, KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft},
        {17, KinectWrapper.NuiSkeletonPositionIndex.FootLeft},

        {18, KinectWrapper.NuiSkeletonPositionIndex.HipRight},
        {19, KinectWrapper.NuiSkeletonPositionIndex.KneeRight},
        {20, KinectWrapper.NuiSkeletonPositionIndex.AnkleRight},
        {21, KinectWrapper.NuiSkeletonPositionIndex.FootRight},
    };

    protected readonly Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>> specIndex2JointMap = new Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>>
    {
        {4, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
        {9, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
    };

    protected readonly Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex> boneIndex2MirrorJointMap = new Dictionary<int, KinectWrapper.NuiSkeletonPositionIndex>
    {
        {0, KinectWrapper.NuiSkeletonPositionIndex.HipCenter},
        {1, KinectWrapper.NuiSkeletonPositionIndex.Spine},
        {2, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter},
        {3, KinectWrapper.NuiSkeletonPositionIndex.Head},

        {5, KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight},
        {6, KinectWrapper.NuiSkeletonPositionIndex.ElbowRight},
        {7, KinectWrapper.NuiSkeletonPositionIndex.WristRight},
        {8, KinectWrapper.NuiSkeletonPositionIndex.HandRight},

        {10, KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft},
        {11, KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft},
        {12, KinectWrapper.NuiSkeletonPositionIndex.WristLeft},
        {13, KinectWrapper.NuiSkeletonPositionIndex.HandLeft},

        {14, KinectWrapper.NuiSkeletonPositionIndex.HipRight},
        {15, KinectWrapper.NuiSkeletonPositionIndex.KneeRight},
        {16, KinectWrapper.NuiSkeletonPositionIndex.AnkleRight},
        {17, KinectWrapper.NuiSkeletonPositionIndex.FootRight},

        {18, KinectWrapper.NuiSkeletonPositionIndex.HipLeft},
        {19, KinectWrapper.NuiSkeletonPositionIndex.KneeLeft},
        {20, KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft},
        {21, KinectWrapper.NuiSkeletonPositionIndex.FootLeft},
    };

    protected readonly Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>> specIndex2MirrorJointMap = new Dictionary<int, List<KinectWrapper.NuiSkeletonPositionIndex>>
    {
        {4, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
        {9, new List<KinectWrapper.NuiSkeletonPositionIndex> {KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft, KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter} },
    };
}
