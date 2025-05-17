using UnityEngine;

namespace VikingParty
{
    public class CheatConsole : MonoBehaviour
    {
        bool cheatmode = false;
        private void Start()
        {
#if UNITY_EDITOR
            cheatmode = true;
#endif
        }
        public void ProcessInput(string s)
        {
            Debug.Log("Handle cheat " + s);
            if (s.Contains("attack"))
            {
                Debug.Log("Attack");
            }
            else if (s.Contains("help"))
            {
                Debug.Log("Help");
            }
        }
    }
}
