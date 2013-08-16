using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Predictor : Photon.MonoBehaviour {

	//
	// NOTE: Network interpolation is afffected by the network sendRate.
	// By default this is 10 times/second for OnSerialize. (See PhotonNetwork.sendIntervalOnSerialize)
	// Raise the sendrate if you want to lower the interpolationBackTime.
	//
    internal struct State {
        internal double timestamp;
        internal Vector3 pos;
        internal Quaternion rot;
		
		/**
		 * Create an interpolated state inbetween the given states with the given timestamp.
		 */ 
		public static State Interpolate( State curr, State last, double time ) {
			if( curr.timestamp < time || last.timestamp > time ) {
				Debug.LogError("State.Interpolate(): the given time is not inbetween the given states!");
				Debug.LogError("Curr: " + curr.timestamp);
				Debug.LogError("Time: " + time);
				Debug.LogError("Last: " + last.timestamp);
			}
			
			State interpolState;
			interpolState.timestamp = time;
			float timediff = (float) (curr.timestamp - last.timestamp);
			float relativeTime = (float) (time - last.timestamp);
			Vector3 pos = Vector3.Lerp(last.pos, curr.pos, relativeTime / timediff );
			Quaternion rot = Quaternion.Lerp(last.rot, curr.rot, relativeTime / timediff );
			interpolState.pos = pos;
			interpolState.rot = rot;
			
			return interpolState;
		}
		
		/**
		 * Calculate an extrapolated state to 2 given states with the given timestamp
		 */ 
		public static State Extrapolate( State curr, State last, double time ) {
			if( curr.timestamp > time || last.timestamp > time ) {
				Debug.LogError("State.Extrapolate: invalid time value for extrapolation");
				Debug.LogError("Time: " + time);
				Debug.LogError("Curr: " + curr.timestamp);
				Debug.LogError("Last: " + last.timestamp);
			}
			
			// calculate difference between last and curr
			Vector3 diffPos = curr.pos - last.pos;
//			Quaternion diffRot = last.rot * Quaternion.Inverse(curr.rot);
			// calculate time factor
			float timeFactor = (float) ( (time - last.timestamp) / (curr.timestamp - last.timestamp) );
			// generate new extrapolated state
			State extrapolState;
			extrapolState.timestamp = time;
			extrapolState.pos = last.pos + diffPos * timeFactor;
//			Debug.Log(last.pos + " + " + diffPos + " * " + timeFactor);
			// respresent rotation as angle + axis
//			float angle = 0;
//			Vector3 axis = Vector3.up;
//			diffRot.ToAngleAxis( out angle, out axis );
//			// multiply angle by timeFactor
//			angle *= timeFactor;
//			extrapolState.rot = Quaternion.AngleAxis(angle, axis);
			extrapolState.rot = Quaternion.identity;
			
			return extrapolState;
		}
		
		public override string ToString() {
			return "State: timestamp = " + timestamp + " pos = " + pos + " rot = " + rot;
		}
    }

    // We store twenty states with "playback" information
    State[] n_BufferedState = new State[20];
    // Keep track of what slots are used
    int n_TimestampCount;
	
	// We store twenty states of local movements
	State[] l_BufferedState = new State[20];
	
    public double interpolationBackTime = 0.15;
	private CharacterMover mover;
	private Animator anim;
	
	private Vector3 movementError = Vector3.zero;

    void Start() {
        if (photonView.isMine)
            this.enabled = false;//Only enable inter/extrapol for remote players
		
		if(GetComponent<Animator>()) {
			anim = GetComponent<Animator>(); 
			anim.speed = GetComponent<CharacterMover>().movementSpeed / 2;
		}
		
		mover = GetComponent<CharacterMover>();
		
		for( int i=0; i<l_BufferedState.Length-1; i++ ) {
			l_BufferedState[i].pos = transform.localPosition;
			l_BufferedState[i].rot = transform.localRotation;
			l_BufferedState[i].timestamp = 0;
		}
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // Always send transform (depending on reliability of the network view)
        if (stream.isWriting) {
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        // When receiving, buffer the information
        else {
			// Receive latest state information
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            // Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
            for (int i = n_BufferedState.Length - 1; i >= 1; i--) {
                n_BufferedState[i] = n_BufferedState[i - 1];
				l_BufferedState[i] = l_BufferedState[i - 1];
            }
			
			
            // Save currect received state as 0 in the buffer, safe to overwrite after shifting
            State netState;
            netState.timestamp = info.timestamp;
            netState.pos = pos;
            netState.rot = rot;
            n_BufferedState[0] = netState;
			
			// save current sitaution as 0 in the buffer for local stats, safe to overwrite after shifting
			State localState;
			localState.timestamp = PhotonNetwork.time;
			localState.pos = transform.localPosition;
			localState.rot = transform.rotation;
			l_BufferedState[0] = localState;
			
			

            // Increment state count but never exceed buffer size
            n_TimestampCount = Mathf.Min(n_TimestampCount + 1, n_BufferedState.Length);

            // Check integrity, lowest numbered state in the buffer is newest and so on
            for (int i = 0; i < n_TimestampCount - 1; i++) {
                if (n_BufferedState[i].timestamp < n_BufferedState[i + 1].timestamp
					|| l_BufferedState[i].timestamp < l_BufferedState[i + 1].timestamp)
                    Debug.LogError("State inconsistent");
            }
			
			// fix erros in local state buffer with new recieved network state
			State realNetState = n_BufferedState[0];
//			State extrapolState = realNetState;
			State interpolState = realNetState;
			int index = 0;
			// find local states wich are near to new network state
			for( int i=l_BufferedState.Length - 2; i >= 0; i-- ) {
				if( l_BufferedState[i].timestamp > realNetState.timestamp ) {
					// we found the right local states
					// extrapolate the new l_BufferedState[i] with the correction through realNetState
//					extrapolState = State.Extrapolate(realNetState, l_BufferedState[i+1], l_BufferedState[i].timestamp);
					interpolState = State.Interpolate(l_BufferedState[i],l_BufferedState[i+1], realNetState.timestamp);
					index = i;
					break;
				}
			}
			
			// calculate the error we had in our prediction
			Vector3 posError = realNetState.pos - interpolState.pos;
			movementError += posError;
			Quaternion rotError = realNetState.rot * Quaternion.Inverse(interpolState.rot);
			// replace old bufferedState with corrected extrapolation
			l_BufferedState[index].pos += posError;
			// apply error correction to all following states
			for( int i=index-1; i>=0; i-- ) {
				l_BufferedState[i].pos += posError;
				l_BufferedState[i].rot = rotError * l_BufferedState[i].rot;
			}
		}
    }

    // This only runs where the component is enabled, which is only on remote peers (server/clients)
    void Update() {
        double currentTime = PhotonNetwork.time;
        double interpolationTime = currentTime - interpolationBackTime;
        // We have a window of interpolationBackTime where we basically play 
        // By having interpolationBackTime the average ping, you will usually use interpolation.
        // And only if no more data arrives we will use extrapolation

        // Use interpolation
        // Check if latest state exceeds interpolation time, if this is the case then
        // it is too old and extrapolation should be used
        if (n_BufferedState[0].timestamp > interpolationTime) {
			for (int i = 0; i < n_TimestampCount; i++) {
                // Find the state which matches the interpolation time (time+0.1) or use last state
                if (n_BufferedState[i].timestamp <= interpolationTime || i == n_TimestampCount - 1) {
                    // The state one slot newer (<100ms) than the best playback state
                    State rhs = n_BufferedState[Mathf.Max(i - 1, 0)];
                    // The best playback state (closest to 100 ms old (default time))
                    State lhs = n_BufferedState[i];

                    // Use the time between the two slots to determine if interpolation is necessary
                    double length = rhs.timestamp - lhs.timestamp;
                    float t = 0.0F;
                    // As the time difference gets closer to 100 ms t gets closer to 1 in 
                    // which case rhs is only used
                    if (length > 0.0001)
                        t = (float)((interpolationTime - lhs.timestamp) / length);

                    // if t=0 => lhs is used directly
                    transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                    transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
					
					float speed = Mathf.Abs((lhs.pos - rhs.pos).magnitude) / (float)length;
					if(anim)
						anim.SetFloat("speed", speed );
					return;
                }
            }
        }
        // Use extrapolation. Here we do something really simple and just repeat the last
        // received state. You can do clever stuff with predicting what should happen.
        else {
            State latest = l_BufferedState[0];
			movementError = Vector3.zero;
			
			float t = (float)(latest.timestamp - PhotonNetwork.time + 0.1f)/(mover.movementSpeed*2) + 0.4f;

			transform.localPosition = Vector3.Lerp(transform.localPosition, latest.pos, t/2);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, n_BufferedState[0].rot, t/2);
			
			float speed = Mathf.Abs((transform.localPosition - latest.pos).magnitude) / Time.deltaTime;
			if(anim)
				anim.SetFloat("speed", speed );
        }
    }
	
	public void UpdateLatestState( Vector3 frameMovement ) {
		l_BufferedState[0].pos += frameMovement;
	}
}
