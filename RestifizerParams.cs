using UnityEngine;
using System.Collections;

namespace Restifizer {
	public interface RestifizerParams {
		string GetClientId();
		string GetClientSecret();
		string GetAccessToken();
	}
}
