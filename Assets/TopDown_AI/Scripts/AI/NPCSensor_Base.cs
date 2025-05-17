using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Sensors change a NPC_Base status condition
/// </summary>
/*[System.Serializable]
public struct NPCSensor_Condition{
	public NPC_Condition condition;
	public bool value;
}*/
public class NPCSensor_Base : MonoBehaviour {
	public NPC_Base npcBase;
//	public List<NPCSensor_Condition> appliedConditons;
	protected List<GameObject> sensedObjects=new List<GameObject>();

	void Awake () {
		if (npcBase == null)
			npcBase = gameObject.GetComponent<NPC_Base> ();
		StartSensor ();
	}

	void Update () {
		UpdateSensor ();
	}
	protected virtual void StartSensor(){}
	protected virtual void UpdateSensor(){}

	protected List<GameObject> GetSensedObjects(){
		return sensedObjects;
	}

}

