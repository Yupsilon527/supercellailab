using UnityEngine;

namespace VikingParty
{
    public class CheatConsole : MonoBehaviour
    {
        public void ProcessInput(string s)
        {
            Debug.Log("Handle cheat " + s);
            s = s.ToLower();
            if (s.Contains("attack"))
            {
                Debug.Log("Attack");
                NPC_Friend.main.Attack();
            }
            else if (s.Contains("help") || s.Contains("heal"))
            {
                NPC_Friend.main.Help();
                Debug.Log("Help");
            }
        }
    }
}
