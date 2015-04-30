using UnityEngine;
using System.Collections;


enum PlayerStates
{
	Idle,		//	Playing
	Hit,		//	reacting to hit		
	Punch,		//	for AI
	Knockdown,	//	on floor
	GetUp,		//	waiting to finish getting up from ragdoll
	Wait,		//	other player is down, cannot play
};

public class PlayerState
{
	public PlayerManager	mPlayer;

	public PlayerState(PlayerManager Player)
	{
		mPlayer = Player;
	}
};

public class PlayerState_Idle : PlayerState
{
	public PlayerState_Idle(PlayerManager Player) :	base (Player)
	{
	}
};

public class PlayerState_Knockdown : PlayerState
{
	public PlayerState_Knockdown(PlayerManager Player) :	base (Player)
	{
	}
};

public class PlayerManager : MonoBehaviour {

	public bool			mAiControlled = false;
	private float		mHealth = 1.0f;
	private PlayerState	mState;

	// Use this for initialization
	void Start () {
		mState = new PlayerState_Idle(this);

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void SetStateKnockdown()
	{
		//	enable ragdoll, change other player to wait until other player is up
		mState = new PlayerState_Knockdown (this);
	}



	//	returns false to reject a hit
	bool DoRecieveHit()
	{
	//	if (mState.GetType() != PlayerState_Idle)
	//		return false;

		//	take hit
		mHealth -= 0.25f;
		if ( mHealth <= 0.0f )
		{
			SetStateKnockdown();
			return false;
		}

		return true;
	}
}
