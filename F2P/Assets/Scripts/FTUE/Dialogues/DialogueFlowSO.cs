using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    [CreateAssetMenu(fileName = "new dialogue flow", menuName = "FTUE/Dialogue", order = 1)]
    public class DialogueFlowSO : ScriptableObject
    {
        [Header("Dialogue")]
        [SerializeField] private string[] _Dialogues = new string[0];

        [SerializeField] private DialogueWordPrinting.Animation _Animation = DialogueWordPrinting.Animation.NONE;
        [SerializeField] private DialogueManager.DisplayType _DialogueType = default;

        public string[] Dialogues { get { return _Dialogues; } }

        public DialogueWordPrinting.Animation Tween { get { return _Animation; } }

        public DialogueManager.DisplayType Type { get { return _DialogueType; } }
    }
}
