using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuspectBase : DialogueUser
{
    [SerializeField] private string _name;
    [SerializeField] private ScriptableObject[] initialTestimony;
    [SerializeField] private ScriptableObject[] idle;
    [SerializeField] private string[] contradictionPlayerPrefs;
    [SerializeField] private ScriptableObject[] secondTestimony;
    [SerializeField] private GameObject promptPrefab;
    private GameObject prompt;
    public bool interactable = true;
    private bool hasSpoken;
    private bool isIdle;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        name = _name;
        assets = initialTestimony;
        base.Start();
        dialogueManager.OnDialogueEnd += EndSpeaking;
    }

    private void FixedUpdate()
    {
        if(!interactable) return;

        HandlePrompt();
        HandleInteraction();
    }

    private void HandlePrompt()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 2f &&  prompt == null)
        {
            prompt = Instantiate(promptPrefab, player.transform.position + new Vector3(-1, -1, 0), Quaternion.identity);
        }
        else if (Vector3.Distance(transform.position, player.transform.position) > 2f && prompt != null)
        {
            Destroy(prompt);
            prompt = null;
        }
    }

    private void HandleInteraction()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 2f && Input.GetButton("Interact"))
        {
            StartSpeaking();
            Destroy(prompt);
            prompt = null;
            interactable = false;
        }
    }

    void StartSpeaking()
    {
        if (!hasSpoken)
        {
            if (CheckContradictions())
            {
                assets = initialTestimony.Concat(secondTestimony).ToArray();
            }
            else
            {
                assets = initialTestimony;
            }
        }
        else if (CheckContradictions())
        {
            assets = secondTestimony;
        }
        else if (isIdle)
        {
            assets = idle;
        }
        print("assets: " + assets.Length);
        OnBeginDialogue();
        player.GetComponent<Player>().SetMovement(false);
    }

    bool CheckContradictions()
    {
        return contradictionPlayerPrefs.Any(pref => PlayerPrefs.GetString(pref) == "true");
    }

    void EndSpeaking()
    {
        if (!hasSpoken)
        {
            hasSpoken = true;
            isIdle = true;
        }
        player.GetComponent<Player>().SetMovement(true);
        StartCoroutine(ResetInteractability());
    }

    IEnumerator ResetInteractability()
    {
        yield return new WaitForSeconds(3);
        interactable = true;
    }
}
