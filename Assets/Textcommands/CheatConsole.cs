using UnityEngine;

namespace VikingParty
{
    public class CheatConsole : MonoBehaviour
    {
        public void ProcessInput(string s)
        {
            Debug.Log("Handle cheat " + s);
            if (s.Contains("attack"))
            {
                Debug.Log("Attack");
                NPC_Friend.main.Attack();
            }
            else if (s.Contains("help"))
            {
                NPC_Friend.main.Help();
                Debug.Log("Help");
            }
        }
    }
}
