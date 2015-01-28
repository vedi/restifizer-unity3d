using UnityEngine;
using System.Collections;

namespace Restifizer {
	public interface IErrorHandler {
		/**
		 * return true if the error has been handled, and should not be propagated to Response
		 */
		bool onRestifizerError(RestifizerError restifizerError);
	}
}