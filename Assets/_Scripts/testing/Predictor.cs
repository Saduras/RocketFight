using UnityEngine;
using System.Collections;

public class Predictor : MonoBehaviour {
	
	private Vector3 lastPos;
	private Vector3 currPos;

	// Use this for initialization
	void Start () {
		lastPos = transform.position;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // Always send transform (depending on reliability of the network view)
        if (stream.isWriting) {
            currPos = transform.position;
			Vector3 move = currPos - lastPos;
			stream.Serialize( ref move );
			lastPos = currPos;
        }
        // When receiving, buffer the information
        else
        {
			// Receive latest state information
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            // Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
            for (int i = m_BufferedState.Length - 1; i >= 1; i--)
            {
                m_BufferedState[i] = m_BufferedState[i - 1];
            }
			
			
            // Save currect received state as 0 in the buffer, safe to overwrite after shifting
            State state;
            state.timestamp = info.timestamp;
            state.pos = pos;
            state.rot = rot;
            m_BufferedState[0] = state;

            // Increment state count but never exceed buffer size
            m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

            // Check integrity, lowest numbered state in the buffer is newest and so on
            for (int i = 0; i < m_TimestampCount - 1; i++)
            {
                if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
                    Debug.Log("State inconsistent");
            }
		}
    }
	
	[RPC]
	public void AddForce(Vector3 force, int hash) {
			
	}
}
