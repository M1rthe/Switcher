using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour, IOnPlayersSpawned
{
    bool isOpen = false;
    bool isLocked = false;

    enum DoorType { FrontDoor }
    [SerializeField] DoorType doorType;

    [SerializeField] List<Door> futureDoors = new List<Door>();

    Animation anim;

    [HideInInspector] public bool checkInput = false;

    InstructionManager instructionManager;

    AudioSource audioSource;

    public PhotonView PhotonView { get; private set; }

    public void OnPlayersSpawned()
    {
        instructionManager = FindObjectOfType<InstructionManager>();
        PhotonView.RPC("OpenDoor", RpcTarget.All, isOpen, false);
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
                    PhotonView.RPC("OpenDoor", RpcTarget.All, !isOpen, true);
                }
            }
        }
    }

    [PunRPC]
    public void OpenDoor(bool open, bool makeSound)
    {
        isOpen = open;

        if (makeSound) audioSource.Play();

        if (open) anim.Play("DoorOpen");
        else anim.Play("DoorClose");

        foreach (Door futureDoor in futureDoors)
        {
            futureDoor.PhotonView.RPC("OpenDoor", RpcTarget.All, open, false);
        }
    }

    public void TriggerEnter(bool front)
    {
        if (isLocked)
        {
            instructionManager.GiveInstruction("Door is locked");
            return;
        }

        if (doorType == DoorType.FrontDoor)
        {
            if (front)
            {
                instructionManager.GiveInstruction("Door can't be opened from front");
                return;
            }
        }

        checkInput = true;
        instructionManager.GiveInstruction("Open door by pressing 'E'");
    }

    public void TriggerExit(bool front)
    {
        checkInput = false;
        instructionManager.PullInstructionAway();
    }
}
