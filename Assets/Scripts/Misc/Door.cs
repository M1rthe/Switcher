using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour, IOnPlayersSpawned, IOnSwitchedTimeline
{
    bool isOpen = false;
    bool isLocked = false;

    enum DoorType { FrontDoor }
    [SerializeField] DoorType doorType;

    [SerializeField] List<Door> futureDoors = new List<Door>();

    Animation anim;

    public bool inTrigger = false;
    public bool checkInput = false;
    bool dontExit = false;

    InstructionManager instructionManager;

    AudioSource audioSource;

    public PhotonView PhotonView { get; private set; }

    public void OnPlayersSpawned()
    {
        instructionManager = FindObjectOfType<InstructionManager>();

        dontExit = true;
        StartCoroutine(WaitForFrame(delegate { dontExit = false; inTrigger = false; checkInput = false; instructionManager.PullInstructionAway(true); }));

        PhotonView.RPC("OpenDoor", RpcTarget.All, isOpen, true, false);
    }

    void Start()
    {
        anim = GetComponentInChildren<Animation>();
        PhotonView = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (checkInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!anim.IsPlaying("DoorOpen") && !anim.IsPlaying("DoorClose"))
                {
                    PhotonView.RPC("OpenDoor", RpcTarget.All, !isOpen, false, true);
                }
            }
        }
    }

    [PunRPC]
    public void OpenDoor(bool open, bool isFirst, bool playSound)
    {
        isOpen = open;

        if (playSound) audioSource.Play();

        if (open)
        {
            if (isFirst)
            {
                anim.Rewind("DoorClose");
                anim.Play("DoorClose");
                anim.Sample();
                anim.Stop("DoorClose");

            }
            else anim.Play("DoorOpen");
        }
        else
        {
            if (isFirst)
            {
                anim.Rewind("DoorOpen");
                anim.Play("DoorOpen");
                anim.Sample();
                anim.Stop("DoorOpen");
            }
            else anim.Play("DoorClose");
        }

        foreach (Door futureDoor in futureDoors)
        {
            futureDoor.PhotonView.RPC("OpenDoor", RpcTarget.All, open, isFirst, false);
        }
    }

    public void TriggerEnter(bool front)
    {
        if (!inTrigger)
        {
            inTrigger = true;

            if (isLocked)
            {
                instructionManager.GiveInstruction("Door is locked");
                return;
            }

            if (doorType == DoorType.FrontDoor)
            {
                if (front && !isOpen)
                {
                    instructionManager.GiveInstruction("Door can't be opened from front");
                    return;
                }
            }

            checkInput = true;

            if (isOpen) instructionManager.GiveInstruction("Close door by pressing 'E'");
            else instructionManager.GiveInstruction("Open door by pressing 'E'");
        }
    }

    public void TriggerExit()
    {
        if (inTrigger)
        {
            inTrigger = false;

            if (!dontExit)
            {
                checkInput = false;
                instructionManager.PullInstructionAway();
            }
        }
    }

    public void OnPlayerSwitched()
    {

    }

    public void OnOtherPlayerSwitched(bool firstTime) { }

    IEnumerator WaitForFrame(UnityEngine.Events.UnityAction action)
    {
        yield return new WaitForFixedUpdate();
        action.Invoke();
    }
}
