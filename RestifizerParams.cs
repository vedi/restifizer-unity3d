using UnityEngine;
using System.Collections;

public interface RestifizerParams {
	string GetClientId();
	string GetClientSecret();
	string GetAccessToken();
}
