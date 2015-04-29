
using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour 
{
	public int activeState			=	-1;
	public int previousState		=	-1;
	public string stateMachineName = 	"StateMachine";

	public bool doDebug		= 	false;

	#if UNITY_EDITOR
		//public string	stateHistory = "";
	#endif

	//constructor
	public StateMachine(string _stateMachineName)
	{
		this.stateMachineName = _stateMachineName;
	}

	public StateMachine() {}

	public int GetActiveState()
	{
		return this.activeState;
	}


	//A dictionary of state modes
	private Dictionary<int, StateMode>	stateModes		= new Dictionary<int, StateMode>();

	public bool		AddState(int _stateID, StateMode.DelStart _startDel, StateMode.DelUpdate _updateDel, StateMode.DelEnd _endDel )
	{
		if (this.stateModes.ContainsKey(_stateID))
			return false;			//Mode already exists
	
		StateMode newStateMode = new StateMode(_stateID, _startDel, _updateDel ,_endDel, this.stateMachineName);

		if (newStateMode==null)
			return false;			//Failed to create new mode

		this.stateModes.Add(_stateID, newStateMode);
		return true;
	}

	protected virtual void Start () 
	{

	}

	protected virtual void Update () 
	{
		UpdateStateMachine();
	}


	// Update is called once per frame
	public void UpdateStateMachine () 
	{
		//check to see if there is an active state on this controller
		//and it has been added
		if (this.stateModes.ContainsKey(this.activeState))
		{
			//do the update
			this.stateModes[this.activeState].DoUpdate();
		}
	}

	public bool SetState(int newState)
	{
		//stateHistory = stateHistory + "," + newState;

		if (this.doDebug)
		{
			Debug.Log("SetState " + newState);
		}

		if (this.activeState == newState)
			return false;


		//Does the target State exist?
		if (this.stateModes.ContainsKey(newState)==false)
			return false;

		//Switching to new state
		//Call end on the active state
		if (this.activeState!=-1)
		{
			if (this.stateModes.ContainsKey(this.activeState))
			{
				this.stateModes[this.activeState].DoEnd(newState);	//pass in the stateID you are going to
			}
		}

		this.previousState = this.activeState;
		this.activeState = newState;


		//Call start on the new state if it exists
		if (this.stateModes.ContainsKey(newState))
		{
			this.stateModes[activeState].DoStart(this.previousState);	//pass in the state ID you are currently in - soon to be the previous stateID
		}


		return true;
	}

	public float ActiveStateTime()
	{
		//How long has the current state been active
		if (this.stateModes.ContainsKey(this.activeState))
			return this.stateModes[this.activeState].activeTime;
		
		return 0.0f;
	}

	public string GetActiveStateInfo()
	{
		return string.Format(">>>>>{0} ID {1} time {2:0.0}s",this.stateMachineName, this.activeState, this.ActiveStateTime());
	}

	#if UNITY_EDITOR2

	void OnGUI()
	{
		if (this.doDebug==false)
			return;

		GUI.color = Color.red;
		GUI.Label(new Rect(30,30,200,200), GetActiveStateInfo());
	}

	#endif


}

public class StateMode
{
	private int 	stateID				=	-1;
	public float	activeTime			=	0.0f;
	public string	ownerStatemachine	=	"?????";

	public delegate	void 	DelStart(int previousState);
	public delegate	void 	DelUpdate();
	public delegate	void 	DelEnd(int nextState);

	private DelStart		StartState	=	null;
	private DelUpdate		UpdateState	=	null;
	private DelEnd			EndState	=	null;

	public bool doDebug		= 	false;

	//constructors
	public StateMode() {}

	public StateMode(int _stateID, DelStart _startState, DelUpdate _updateState, DelEnd _endState, string _ownerMachine = "????")
	{
		this.stateID 			= _stateID;
		this.StartState			= _startState;
		this.UpdateState		= _updateState;
		this.EndState			= _endState;

		this.activeTime			=	0.0f;

		this.ownerStatemachine 	= _ownerMachine;
	}

	public void DoStart(int previousStateID)
	{
		if (this.doDebug)
			Debug.Log(ownerStatemachine + ": DoStart " + this.stateID + " from " + previousStateID);


		this.activeTime = 0.0f;
		if (this.StartState!=null)
		{
			this.StartState(previousStateID);
		}
	}
	
	public void DoUpdate()
	{
		this.activeTime += Time.deltaTime;
		if (this.UpdateState!=null)
		{
			this.UpdateState();
		}
	}
	
	public void DoEnd(int nextStateID)
	{
		if (this.doDebug)
			Debug.Log(ownerStatemachine + ": DoEnd " + this.stateID + " to " + nextStateID);

		if (this.EndState!=null)
		{
			this.EndState(nextStateID);
		}
	}
	
}
