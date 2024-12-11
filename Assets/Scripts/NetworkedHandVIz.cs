using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using Mirror;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Hands
{
    public class NetworkedHandVIz : NetworkBehaviour
    {
        [SerializeField]
        XROrigin m_Origin;

        [SerializeField]
        GameObject m_LeftWristLoc;

        [SerializeField]
        GameObject m_RightWristLoc;

        [SerializeField]
        GameObject bodyLoc;

        public List<GameObject> leftHandSphere, rightHandSphere;
        public float scale = 1;


        XRHandSubsystem m_Subsystem;
        HandGameObjects m_LeftHandGameObjects;
        HandGameObjects m_RightHandGameObjects;

        static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

        protected void Awake()
        {
        #if ENABLE_INPUT_SYSTEM
            InputSystem.InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
        #endif // ENABLE_INPUT_SYSTEM
        }

        

        protected void Update()
        {
             if (m_Subsystem != null) {
                return;
             }
            
            if(!isLocalPlayer)
                return;
            SubsystemManager.GetSubsystems(s_SubsystemsReuse);
            if (s_SubsystemsReuse.Count == 0)
                return;

            m_Subsystem = s_SubsystemsReuse[0];
            if (m_LeftHandGameObjects == null)
            {
                m_LeftHandGameObjects = new HandGameObjects(
                    Handedness.Left,
                    transform,
                    m_LeftWristLoc,
                    leftHandSphere,
                    rightHandSphere
                    );
            }
            if (m_RightHandGameObjects == null)
            {
                m_RightHandGameObjects = new HandGameObjects(
                    Handedness.Right,
                    transform,
                    m_RightWristLoc,
                    leftHandSphere,
                    rightHandSphere
                    );
            }

            
            m_Subsystem.updatedHands += OnUpdatedHands;
        }

        

        void LateUpdate()
        {
            m_LeftWristLoc.transform.localScale = new Vector3(scale, scale, scale);
            m_RightWristLoc.transform.localScale = new Vector3(scale, scale, scale);
        }


        void OnUpdatedHands(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags, XRHandSubsystem.UpdateType updateType)
        {
            // We have no game logic depending on the Transforms, so early out here
            // (add game logic before this return here, directly querying from
            // subsystem.leftHand and subsystem.rightHand using GetJoint on each hand)
            if (updateType == XRHandSubsystem.UpdateType.Dynamic)
                return;

            bool leftHandTracked = subsystem.leftHand.isTracked;
            bool rightHandTracked = subsystem.rightHand.isTracked;

            m_LeftHandGameObjects.UpdateJoints(m_Origin, subsystem.leftHand,  m_LeftWristLoc,bodyLoc, leftHandTracked);

            // if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
            //     m_LeftHandGameObjects.UpdateRootPose(subsystem.leftHand);

            m_RightHandGameObjects.UpdateJoints(m_Origin, subsystem.rightHand,  m_RightWristLoc,bodyLoc, rightHandTracked);
            
            // if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            //     m_RightHandGameObjects.UpdateRootPose(subsystem.rightHand);
        }

        class HandGameObjects
        {
            GameObject m_HandRoot;
            GameObject m_DrawJointsParent;

            Transform[] m_JointXforms = new Transform[XRHandJointID.EndMarker.ToIndex()];
            GameObject[] m_DrawJoints = new GameObject[XRHandJointID.EndMarker.ToIndex()];
            
            LineRenderer[] m_Lines = new LineRenderer[XRHandJointID.EndMarker.ToIndex()];

            GameObject[] spheres = new GameObject[XRHandJointID.EndMarker.ToIndex()];
            bool m_IsTracked;
            public List<GameObject> leftHandSphere, rightHandSphere;

            static Vector3[] s_LinePointsReuse = new Vector3[2];
            const float k_LineWidth = 0.005f;

            public HandGameObjects(Handedness handedness, Transform parent, GameObject wristLoc, List<GameObject> leftHandSpheres, List<GameObject> rightHandSpheres)
            {
                void AssignJoint(XRHandJointID jointId, Transform jointXform, Transform drawJointsParent)
                {
                    int jointIndex = jointId.ToIndex();
                    m_JointXforms[jointIndex] = jointXform;

                    //GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //s.transform.localScale = new Vector3(0.01f, 0.01f, 0.015f);

                    // s.transform.parent = wristLoc.transform;
                    // spheres[jointIndex] = s;
                    // Debug.Log("Creating sphere with jointIndex" + jointIndex.ToString());
                    

                    
                }
                leftHandSphere = leftHandSpheres;
                rightHandSphere = rightHandSpheres;
                // foreach(var x in leftHandSphere){
                //     x.GetComponent<MeshRenderer>().enabled = false;
                // }
                // foreach(var x in rightHandSphere){
                //     x.GetComponent<MeshRenderer>().enabled = false;
                // }
                

                Transform wristRootXform = wristLoc.transform;
                

                m_DrawJointsParent = new GameObject();
                m_DrawJointsParent.transform.parent = parent;
                m_DrawJointsParent.transform.localPosition = Vector3.zero;
                m_DrawJointsParent.transform.localRotation = Quaternion.identity;
                m_DrawJointsParent.name = handedness + " Hand Debug Draw Joints";

                if (wristRootXform == null)
                {
                    // Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find Wrist joint!");
                }
                else
                {
                    AssignJoint(XRHandJointID.Wrist, wristRootXform, m_DrawJointsParent.transform);
                    for (int childIndex = 0; childIndex < wristRootXform.childCount; ++childIndex)
                    {
                        var child = wristRootXform.GetChild(childIndex);

                        for (int fingerIndex = (int)XRHandFingerID.Thumb; fingerIndex <= (int)XRHandFingerID.Little; ++fingerIndex)
                        {
                            var fingerId = (XRHandFingerID)fingerIndex;

                            var jointIdFront = fingerId.GetFrontJointID();
                            if (!child.name.EndsWith(jointIdFront.ToString()))
                                continue;

                            AssignJoint(jointIdFront, child, m_DrawJointsParent.transform);
                            var lastChild = child;

                            int jointIndexBack = fingerId.GetBackJointID().ToIndex();
                            for (int jointIndex = jointIdFront.ToIndex() + 1; jointIndex <= jointIndexBack; ++jointIndex)
                            {
                                for (int nextChildIndex = 0; nextChildIndex < lastChild.childCount; ++nextChildIndex)
                                {
                                    var nextChild = lastChild.GetChild(nextChildIndex);
                                    if (nextChild.name.EndsWith(XRHandJointIDUtility.FromIndex(jointIndex).ToString()))
                                    {
                                        lastChild = nextChild;
                                        break;
                                    }
                                }

                                 if (!lastChild.name.EndsWith(XRHandJointIDUtility.FromIndex(jointIndex).ToString()))
                                     throw new InvalidOperationException("Hand transform hierarchy not set correctly - couldn't find " + XRHandJointIDUtility.FromIndex(jointIndex) + " joint!");

                                var jointId = XRHandJointIDUtility.FromIndex(jointIndex);
                                AssignJoint(jointId, lastChild, m_DrawJointsParent.transform);
                            }
                        }
                    }
                }

                for (int fingerIndex = (int)XRHandFingerID.Thumb; fingerIndex <= (int)XRHandFingerID.Little; ++fingerIndex)
                {
                    var fingerId = (XRHandFingerID)fingerIndex;
                    var jointId = fingerId.GetFrontJointID();
                    if (m_JointXforms[jointId.ToIndex()] == null)
                         Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find " + jointId + " joint!");
                }
            }

        


            public void UpdateRootPose(XRHand hand)
            {
                var xform = m_JointXforms[XRHandJointID.Wrist.ToIndex()];
                xform.localPosition = hand.rootPose.position;
                xform.localRotation = hand.rootPose.rotation;
            }

            public void UpdateJoints(
                XROrigin xrOrigin,
                XRHand hand,
                GameObject wristLoc,
                GameObject bodyLoc,
                bool areJointsTracked)
            {
                
                
                if (!areJointsTracked)
                    return;
                var originTransform = xrOrigin.Origin.transform;
                var originPose = new Pose(originTransform.position, originTransform.rotation);

                var wristPose = Pose.identity;
                //UpdateJoint(originPose, hand.GetJoint(XRHandJointID.Wrist), ref wristPose, hand, wristLoc);
                //UpdateJoint(originPose, hand.GetJoint(XRHandJointID.Palm), ref wristPose, hand, false);
                var wristJoint = hand.GetJoint(XRHandJointID.Wrist);
                // wristJoint.TryGetPose(out wristPose);
                // wristLoc.transform.position = wristPose.position;

                // var palmJoint = m_JointXforms[hand.GetJoint(XRHandJointID.Palm).id.ToIndex()];
                // print(palmJoint.name);
                // wristJoint.TryGetPose(out Pose wristPose);

                // m_JointXforms[wristJoint.id.ToIndex()].localPosition = wristPose.position;
                for (int fingerIndex = (int)XRHandFingerID.Thumb; fingerIndex <= (int)XRHandFingerID.Little; ++fingerIndex)
                {
                    var parentPose = wristPose;
                    var fingerId = (XRHandFingerID)fingerIndex;
                    
                    int jointIndexBack = fingerId.GetBackJointID().ToIndex();
                    Transform parentSphere;
                    parentSphere = hand.handedness == Handedness.Left ? leftHandSphere[jointIndexBack].transform : rightHandSphere[jointIndexBack].transform; 
                    for (int jointIndex = fingerId.GetFrontJointID().ToIndex(); jointIndex <= jointIndexBack; ++jointIndex)
                    {
                        if (m_JointXforms[jointIndex] != null)
                            UpdateJoint(originPose, hand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndex)), ref parentPose, ref parentSphere, hand, wristLoc, bodyLoc, (jointIndex!=jointIndexBack));
                    }
                }
            }

            void UpdateJoint(
                Pose originPose,
                XRHandJoint joint,
                ref Pose parentPose,
                ref Transform parentSphere,
                XRHand myHand,
                GameObject wristLoc,
                GameObject bodyLoc,
                bool hideSphere = false,
                bool cacheParentPose = true
                )
            {
               
                int jointIndex = joint.id.ToIndex();
                var xform = m_JointXforms[jointIndex];
                if (xform == null || !joint.TryGetPose(out var pose))
                    return;
                

                Quaternion jointRotation; 

                var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
                //xform.localPosition = inverseParentRotation * (pose.position - parentPose.position);
                jointRotation = inverseParentRotation * pose.rotation;
                //xform.localRotation = jointRotation;

                var wristJoint = myHand.GetJoint(XRHandJointID.Wrist);
                wristJoint.TryGetPose(out Pose wristPose);
                inverseParentRotation = Quaternion.Inverse(wristPose.rotation);
                Transform sphere;
                sphere = myHand.handedness == Handedness.Left ? leftHandSphere[jointIndex].transform : rightHandSphere[jointIndex].transform; 
                sphere.transform.localPosition =   Quaternion.Inverse(wristLoc.transform.rotation) * (bodyLoc.transform.rotation * pose.position -  bodyLoc.transform.rotation * wristPose.position);
                //Debug.Log("joint name " + joint.id + " Sphere name " + sphere.name);
                // sphere.gameObject.GetComponent<MeshRenderer>().enabled = true;
                //spheres[jointIndex].transform.localRotation = inverseParentRotation * pose.rotation;

                // Debug.Log("My xforms position: " +xform.position.ToString());
                xform.position = sphere.transform.position;
                // Quaternion s_rotation = spheres[jointIndex].transform.rotation;
                // jointRotation.Set(s_rotation.x, -s_rotation.z, s_rotation.y, s_rotation.w);
                // xform.rotation = jointRotation;
                 
                xform.up = sphere.transform.forward;
                // Debug.Log("My xforms rotation: " +xform.rotation.ToString());
                if (hideSphere)
                    sphere.GetComponent<Renderer>().enabled = false;
                else
                {
                    sphere.GetComponent<Renderer>().enabled = true;
                    sphere.transform.forward = parentSphere.forward;
                }
                
                
                if (cacheParentPose)
                {
                    parentPose = pose;
                    // Debug.Log("making parentsphere look at");
                    parentSphere.LookAt(sphere.transform.position);
                    parentSphere = sphere.transform;
                }

            }

            static void ToggleRenderers<TRenderer>(bool toggle, Transform xform)
                where TRenderer : Renderer
            {
                if (xform.TryGetComponent<TRenderer>(out var renderer))
                    renderer.enabled = toggle;

                for (int childIndex = 0; childIndex < xform.childCount; ++childIndex)
                    ToggleRenderers<TRenderer>(toggle, xform.GetChild(childIndex));
            }
        }
    }
}